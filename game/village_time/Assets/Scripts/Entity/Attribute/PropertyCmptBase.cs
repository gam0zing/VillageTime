using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 常规属性：
/// 上下限较为稳定，且当前值存在修饰需求的属性；
/// 如攻击力、防御力、声望、勇气、经验倍率。
/// </summary>
public abstract class PropertyCmptBase : AbstractAttribute {
    public override float Current {
        get => this._current;
        set => Debug.LogWarning("修改常规属性的当前值，请使用IAttribute.AddModifier()");
    }
    protected float _current;
    /// <summary>
    /// 对于常规属性而言，基础值为当前值服务
    /// </summary>
    public override float Base {
        get => this._base;
        set {
            float origin = this._base;
            this._base = Mathf.Clamp(value, this._min, this._max);
            this.OnBaseChange(origin, this._base);
        }
    }
    protected  float _base = 0F;
    public override float Max {
        get => this._max;
        set {
            float origin = this._max;
            this._max = Mathf.Max(value, this._min);
            this.OnMaxChange(origin, this._max);
        }
    }
    protected float _max = float.MaxValue;
    public override float Min {
        get => this._min;
        set {
            float origin = this._min;
            this._min = Mathf.Min(value, this._max);
            this.OnMinChange(origin, this._min);
        }
    }
    protected float _min = 0F;
    
    public override void AddModifier(IModifier modifier) {
        base.AddModifier(modifier);
    }
    protected override void Refresh() {
        this._current = this.GetModified();
    }
    protected virtual void OnBaseChange(float origin, float now) { }
    protected virtual void OnMaxChange(float origin, float now) { }
    protected virtual void OnMinChange(float origin, float now) { }
}