using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAttribute : MonoBehaviour, IAttribute {
    public abstract float Current { get; set; }
    public abstract float Base { get; set; }
    public abstract float Max { get; set; }
    public abstract float Min { get; set; }

    private IModifier _first;
    private IModifier _final;
    private HashSet<IModifier> _modifiers = new();

    public virtual void AddModifier(IModifier modifier) {
        if (this._modifiers.Contains(modifier)) return;
        // 如果没有装饰器
        if (this._modifiers.Count == 0) {
            this._first = modifier;
            this._final = modifier;
        }
        // 如果可以修饰装饰器
        else {
            modifier.Banding(this._first, this._final);
        }
        this._modifiers.Add(modifier);
        this.Refresh();
    }
    public void RemoveModifier(IModifier modifier) {
        this.Refresh();
    }
    protected abstract void Refresh();

    protected float GetModified() {
        return this._first.GetModified(this.Base);
    }
}