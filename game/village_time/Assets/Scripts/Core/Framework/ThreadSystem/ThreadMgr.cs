using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class ThreadMgr {
    // 创建所有WorkQueue，提供Id
    // 提供安全的调度接口
    // 提供需求绑定的WorkQueue Id查询服务
    #region 单例
    private static ThreadMgr _instance = new ThreadMgr();
    public static ThreadMgr GetInstance() {
        return _instance;
    }
    #endregion

    public const ushort THREAD_CREATE_MS = 25; // 线程创建信号量最大等待时间

    private readonly Dictionary<string, WorkQueue> _queues;

    public readonly int maxThreads;
    public readonly SemaphoreSlim restThreads;

    private ThreadMgr() {
        this.maxThreads = Math.Max(Environment.ProcessorCount, 4);
        this.restThreads = new SemaphoreSlim(this.maxThreads);
    }

    private readonly object _requestLock = new object();
    /// <summary>
    /// 申请新的线程池，加锁方法<br/>
    /// 需要带着Id来申请，成功后这个Id将对应一个池对象<br/>
    /// 每个池创建后默认开辟一个初始线程<br/>
    /// </summary>
    /// <param name="queueId">新池的Id</param>
    /// <param name="agrs"> 池参数，新建成功后该参数将直接被池对象引用，路由API运行时有权修改自己申请的池参数</param>
    /// <returns></returns>
    public bool RequestQueue(string queueId, QueueCfg cfg) {

        if (!this.restThreads.Wait(0)) {
            UnityEngine.Debug.LogWarning("线程数量达到上限，无法再申请新的线程池");
            return false;
        }
        if (this._queues.ContainsKey(queueId)) {
            UnityEngine.Debug.LogWarning("正在用重复的Id申请线程池！已驳回");
            this.restThreads.Release();
            return false;
        }
        this._queues[queueId] = new WorkQueue(cfg);
        return true;
    }


    private readonly object _giveBackLock = new object();
    /// <summary>
    /// 释放线程池，归还线程
    /// </summary>
    public void GiveBackQueue(string queueId) {
        lock (this._giveBackLock) {
            if (!this._queues.ContainsKey(queueId)) {
                UnityEngine.Debug.LogWarning("要释放的线程池不存在！");
                return;
            }
            this._queues[queueId].Dispose();
            this._queues.Remove(queueId);
        }
    }

    /// <summary>
    /// 仅应该被线程调用
    /// </summary>
    public void OnThreadCreate() {
        this.restThreads.WaitOrThrow(ms: THREAD_CREATE_MS, null);
        // 进行下一次出队检查
    }

    /// <summary>
    /// 仅应该被线程调用
    /// </summary>
    public void OnThreadDestroy() {
        this.restThreads.Release();
        // 进行出队检查
    }
}