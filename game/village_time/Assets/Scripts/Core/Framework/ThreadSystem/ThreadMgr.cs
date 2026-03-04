using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ThreadMgr {

    #region 单例
    private static ThreadMgr _instance = new ThreadMgr();
    public static ThreadMgr Instance => _instance;
    #endregion

    private readonly ConcurrentDictionary<string, WorkQueue> _queues;

    private ThreadMgr() {
        this._queues = new ConcurrentDictionary<string, WorkQueue>();
    }

    private object _requestLock = new object();
    /// <summary>
    /// 申请一个新的线程池，只管线程池名字有没有被占用，别的不管
    /// </summary>
    /// <param name="name">指定线程池名称，全局唯一</param>
    /// <returns>true-申请成功，false-申请失败</returns>
    public bool Request(QueueCfg cfg, ushort initCount) {
        if (this._queues.ContainsKey(cfg.id)) return false;
        lock (this._requestLock) {
            if (this._queues.ContainsKey(cfg.id)) return false;

            this._queues[cfg.id] = new WorkQueue(cfg, initCount);
            return true;
        }
    }

    /// <summary>
    /// 业务层调用的跨线程通信入口，模拟事件循环模式，实现本地线程的异步调用
    /// </summary>
    /// <typeparam name="TResult">回调需要的返回值</typeparam>
    /// <param name="targeQueue">目标队列Id</param>
    /// <param name="task">提交的任务</param>
    /// <param name="then">任务完成后的回调</param>
    public void AwaitPromise<TResult>(string targeQueue, Func<TResult> task, Action<TResult> then) {
        if (!this._queues.TryGetValue(targeQueue, out var queue)) {
            Console.WriteLine("指定的线程池Id不存在");
            return;
        }
        string thisQueue = ThreadAPI.data.queueId;
        Action action = () => {
            TResult result = task.Invoke();
            ThreadAPI.data.Submit(thisQueue, () => then.Invoke(result));
        };
        ThreadAPI.data.Submit(targeQueue, action);
    }

    /// <summary>
    /// 内部跨线程提交入口，业务不要用
    /// </summary>
    /// <param name="queueId">队列Id</param>
    /// <param name="task">任务</param>
    public void Enqueue(string queueId, Action task) {
        if (this._queues.TryGetValue((queueId), out var queue)) {
            queue.Enqueue(task);
        }
    }
}
