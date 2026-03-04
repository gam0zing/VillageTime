using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class QueueCfg {
    public readonly string id;
    public readonly ushort size;
    /// <summary>
    /// 扩容检查<br/>
    /// <para>参数1: 当前线程数量</para>
    /// <para>参数2: 剩余任务数量</para>
    /// <para>返回: true-需要扩容，false-无需扩容</para>
    /// </summary>
    public Func<int, int, bool> _expandChecker;
    /// <summary>
    /// 缩容检查，等待任务超时才触发，判断线程是否可以自杀<br/>
    /// <para>参数1: 当前线程数量</para>
    /// <para>返回: true-可以缩容，false-不能缩容</para>
    /// </summary>
    public Func<int, bool> _shrinkChecker;

    public QueueCfg(string name, ushort size, Func<int, int, bool> expandChecker, Func<int, bool> shrinkChecker) {
        this.id = name;
        this.size = size;
        this._expandChecker = expandChecker;
        this._shrinkChecker = shrinkChecker;
    }
}
