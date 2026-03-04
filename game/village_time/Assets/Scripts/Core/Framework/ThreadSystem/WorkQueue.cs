using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 第2个线程池，特化为WorkQueue，集群专用<br/>
/// 属性：<br/>
/// 1、（更改）不再使用数组存放线程，改用信号量记录剩余线程插槽数量<br/>
/// 2、（新增）一个整数，用来规定初始创建的线程数<br/>
/// 3、（新增）一个整数，用来规定最大线程数<br/>
/// 线程静态对象：<br/>
/// 1、（新增）一个线程静态对象，存放线程自己的一些本地专用参数和方法<br/>
/// 功能：<br/>
/// 1、（新增）提交任务现在返回布尔值：如果为false，说明请求被拒 <br/>
/// </summary>
internal class WorkQueue {
    // 不可暂停，不可强制销毁，无需等待线程退出
    // 没有任何方式可以直接关停该线程池，仅在 _isShutDown == true 且 _restTasks == 0 时自动关闭
    #region 状态量
    private volatile bool _isShutDown;
    private volatile int _currentThreads;  // 当前线程数
    private readonly SemaphoreSlim _restTasks;  // 剩余任务数
    #endregion
    private readonly ConcurrentQueue<Action> _taskQueue;

    #region 业务层需求注入
    private readonly QueueCfg _cfg;
    #endregion

    public WorkQueue(QueueCfg cfg, ushort initCount) {
        this._cfg = cfg;

        this._isShutDown = false;
        this._restTasks = new SemaphoreSlim(0);
        this._taskQueue = new ConcurrentQueue<Action>();

        for (int i = 0; i < initCount; i++) {
            new Thread(this.ThreadLoop).Start();
        }
    }

    private void ThreadLoop() {
        try { // 这个try是为了处理task?.Invoke中没处理的异常，防止炸掉线程后再破坏线程数量统计
            Interlocked.Increment(ref this._currentThreads);
            ThreadAPI.InitQueueId(this._cfg.id); // ThreadData初始化
            while (true) {
                // 每次循环开始检查一次线程池状态，如果已经关闭，则结束循环
                if (this._isShutDown && this._taskQueue.IsEmpty) {
                    break;
                }
                if (this._restTasks.Wait(5000)) {
                    if (this._taskQueue.TryDequeue(out Action task)) {
                        task?.Invoke();
                        // 如果任务为空，发送线程完成事件，用以激活其他线程的合并提交
                        if (this._taskQueue.IsEmpty) {
                            EventCenter.Emit(EventIds.CoreEvents.WorkQueueDone, this._cfg.id as object);
                        }
                    }
                } else if (this._cfg._shrinkChecker?.Invoke(this._currentThreads) ?? false) {
                    break;
                }
            }
        } finally {
            ThreadAPI.data.OnThreadExit();
            Interlocked.Decrement(ref this._currentThreads);
        }
    }

    private readonly object _addLock = new object();
    public bool Enqueue(Action task) {
        lock (this._addLock) {
            try {
                if (this._isShutDown) return false;
                this._taskQueue.Enqueue(task);
                this._restTasks.Release();
                if (this._cfg._expandChecker?.Invoke(this._currentThreads, this._taskQueue.Count) ?? false) {
                    new Thread(this.ThreadLoop).Start();
                }
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                // 此处只可能是扩容逻辑抛异常，任务已经成功添加
                return true;
            }
        }
    }

    public void ShutDown() {
        this._isShutDown = true;
    }
}
