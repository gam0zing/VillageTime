using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
///  本项目的线程池，跨线程任务提交的窗口，目的是封装线程池集群之间的规范化提交。
/// </summary>
public sealed class WorkQueue : IDisposable {
    private readonly ConcurrentQueue<Action> _tasks;
    private readonly SemaphoreSlim _restThreads;
    private readonly SemaphoreSlim _restTasks;
    private readonly ManualResetEventSlim _pauseEvent;
    private volatile bool _disposed;

    private readonly object _disposeLock;

    private readonly QueueCfg _cfg;

    public WorkQueue(QueueCfg cfg) {
        this._tasks = new();
        this._restThreads = new(cfg.maxThreads);
        this._restTasks = new(0);
        this._pauseEvent = new(false);
        this._disposed = false;

        this._disposeLock = new();

        this._cfg = cfg;

        this.InitThreads();
    }

    private void InitThreads() {
        new Thread(this.ThreadLoop).Start();
    }

    private void ThreadLoop() {
        this.OnThreadCreate();
        // 拦截并释放刚执行完任务的线程
        while (!this._disposed) {
            // 此处暂停
            this._pauseEvent.Wait();

            if (this._disposed) break; // 拦截并释放被解除暂停的线程

            try {
                this._restTasks.Wait();
            } catch (ObjectDisposedException) {
                break;
            }

            if (this._disposed) break; // 拦截并释放等待中的线程

            // 从此处开始新任务被加入，对应数量的Thread被放行，开始接取并执行任务
            if (this._tasks.TryDequeue(out var task)) task.Invoke();
        }
        this.OnThreadDestroy();
    }

    public void AddTask(Action task) {
        this.CheckDispose(
            () => {
                this._tasks.Enqueue(task);
                this._restTasks.Release();
            },
            null
        );
    }

    public void Pause() {
        this.CheckDispose(
            () => {
                this._pauseEvent.Reset();
            },
            null
        );
    }

    public void Resume() {
        this.CheckDispose(
            () => {
                this._pauseEvent.Set();
            },
            null
        );
    }

    /// <summary>
    /// 使用这个方法来安全停止线程池，将立即停止任务接取并等待任务队列处理完毕
    /// 此时必须将该线程池移出管理器，它将在任务队列清空后自动释放并销毁
    /// </summary>
    public void Dispose() {
        lock (this._disposeLock) {
            if (this._disposed) return;
            this._disposed = true;

            this._pauseEvent.Set();
            this._restTasks.Release(this._cfg.maxThreads);

            this._pauseEvent.Dispose();
            this._restTasks.Dispose();
        }
    }

    private readonly object _getThreadCountLock = new object();

    // 调用者：
    public int GetThreadCount() {
        lock (this._getThreadCountLock) {
            return this._cfg.maxThreads - this._restThreads.CurrentCount;
        }
    }

    public int GetTaskCount() {
        return this._tasks.Count;
    }

    public void CheckDispose(Action success, Action final) {
        try {
            if (this._disposed) throw new ObjectDisposedException(this.GetType().Name);
            success?.Invoke();
        } finally {
            final?.Invoke();
        }
    }

    // 跨线程操作
    // 子线程 --> 主线程
    // 并发：信号量，ThreadMgr尝试出队逻辑
    private void OnThreadCreate() {
        this._restThreads.WaitOrThrow(ThreadMgr.THREAD_CREATE_MS, null);
        ThreadMgr.GetInstance().OnThreadCreate();
    }

    // 跨线程操作 
    // 子线程 --> 主线程
    // 并发：信号量，ThreadMgr尝试出队逻辑
    private void OnThreadDestroy() {
        this._restThreads.Release();
        ThreadMgr.GetInstance().OnThreadDestroy();
    }
}