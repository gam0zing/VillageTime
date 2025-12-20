using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 资源属性：
/// 拥有能被消耗、补充的当前值，下限一般是0，且上限可能出现修饰需求的属性；
/// 如血量、蓝量、体力、金钱。
/// 注：在本抽象类的实现中，不强制要求当前值跟随最大值的下降而下降，如有相关需求，应在子类中自行实现
/// </summary>
public abstract class ResourceCmptBase : AbstractAttribute {
    public override float Current {
        get => this._current;
        set {
            float origin = this._current;
            this._current = Mathf.Clamp(value, this._min, this._max);
            this.OnCurrentChange(origin, this._current);
        }
    }
    protected float _current = 0F;
    /// <summary>
    /// 对于资源属性而言，基础值为最大值服务
    /// </summary>
    public override float Base {
        get => this._base;
        set {
            float origin = this._base;
            this._base = Mathf.Max(value, this._min);
            this.OnBaseChange(origin, this._base);
        }
    }
    protected float _base = float.MaxValue;
    public override float Max {
        get => this._max;
        set => Debug.LogWarning("修改资源属性的最大值，请使用IAttribute.AddModifier()");
    }
    protected float _max;
    public override float Min {
        get => this._min;
        set {
            float origin = this._min;
            this._min = Mathf.Min(this._max, value);
            this.Current = Mathf.Max(this._current, this._min); //此处需要用set方法来触发修改当前值的回调
            this.OnMinChange(origin, this._min);
        }
    }
    protected float _min = 0F;

    public override void AddModifier(IModifier modifier) {
        base.AddModifier(modifier);
    }
    protected override void Refresh() {
        this._max = this.GetModified();
    }
    protected virtual void OnBaseChange(float origin, float now) { }
    protected virtual void OnCurrentChange(float origin, float now) { }
    protected virtual void OnMinChange(float origin, float now) { }

}