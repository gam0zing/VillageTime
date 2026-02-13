using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.VersionControl;

/// <summary>
/// 线程通信API，每个线程拥有自己的通信身份
/// </summary>
public static class ThreadAPI {
    [ThreadStatic]
    private static ThreadInfo _threadInfo;

    private class ThreadInfo {
        private readonly Dictionary<int, Action> _submitBuffer;
        public ThreadInfo() {
            this._submitBuffer = new Dictionary<int, Action>();
        }
        public void Submit(int queueId, Action task) {
            this._submitBuffer.Add(queueId, task);
        }
    }

    /// <summary>
    /// 任务提交API
    /// </summary>
    /// <param name="queueId"></param>
    /// <param name="task"></param>
    public static void Submit(int queueId, Action task) {
        ThreadAPI._threadInfo.Submit(queueId, task);
    }
}
