using System;
using UnityEngine;

public abstract class AbstractAttribute : MonoBehaviour, IAttribute {
    public abstract float Current { get; set; }
    public abstract float Base { get; set; }
    public abstract float Max { get; set; }
    public abstract float Min { get; set; }
    public IModifier First => this._first;
    protected IModifier _first;
    public IModifier Final => this._final;
    protected IModifier _final;
    public virtual void AddModifier(IModifier modifier) {
        if (!this.HasFirst()) return;
        this._first = modifier;
        this._final = modifier;
        this._refresh();
    }
    protected bool HasFirst() {
        if (this._first == null ^ this._final == null) Debug.LogWarning("意料之外的装饰器丢失！属性装饰器链其中一个末端为null！");
        return this._first != null;
    }

    private void _refresh() {
        if (!this.HasFirst()) return;
        this.Refresh();
    }
    protected abstract void Refresh();
}