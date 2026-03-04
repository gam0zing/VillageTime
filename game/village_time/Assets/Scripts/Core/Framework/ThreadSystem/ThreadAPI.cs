using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ThreadAPI {
    [ThreadStatic]
    private static ThreadData _data;
    public static ThreadData data => _data ??= new ThreadData();

    public class ThreadData {
        public string queueId;
        public readonly ConcurrentDictionary<string, List<Action>> submits;
        public ThreadData() {
            this.queueId = "undefined";
            this.submits = new ConcurrentDictionary<string, List<Action>>();
            // 监听池任务完成事件，调用攒批Enqueue方法
            EventCenter.On(EventIds.CoreEvents.WorkQueueDone, this.StartEnqueue);
        }

        public void OnThreadExit() {
            // 线程退出时取消注册
            EventCenter.Off(EventIds.CoreEvents.WorkQueueDone, this.StartEnqueue);
        }
         
        /// <summary>
        /// 内部本地提交入口，业务用ThreadMgr.AwaitPromise()
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="task"></param>
        public void Submit(string queueId, Action task) {
            if (this.submits.TryGetValue(queueId, out var list)) {
                if (task == null) list = new List<Action>();
                list.Add(task);
            } else {
                this.submits[queueId] = new List<Action>() { task };
            }
        }

        /// <summary>
        /// 当线程池的“宏任务结束事件”发送后，所有线程将“微任务”打包提交到该线程
        /// </summary>
        /// <param name="args">此处为string</param>
        private void StartEnqueue(object args) {
            if (args is not string queueId) return;
            if (this.submits.TryRemove(queueId, out var actions) && actions?.Count > 0) {
                var validActions = actions.Where(a => a != null).ToArray();
                if (validActions.Length > 0) {
                    Action combined = validActions.Length == 1 ? validActions[0] : (Action)Delegate.Combine(validActions);
                    ThreadMgr.Instance.Enqueue(queueId, combined);
                }
                actions.Clear();
            }
        }
    }

    /// <summary>
    /// 线程初始化时调用，用来传递queueId
    /// </summary>
    /// <param name="id"></param>
    public static void InitQueueId(string id) {
        data.queueId = id;
    }
}
