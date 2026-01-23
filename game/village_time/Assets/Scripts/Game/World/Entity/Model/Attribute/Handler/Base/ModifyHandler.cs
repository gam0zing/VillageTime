using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifyHandler<T> : IModifyHandler<T> where T : IModifier {
    protected List<T> _modifiers;
    protected Action onChange;

    protected float _value = 0;
    public ModifyHandler() {
        this._modifiers = new();
    }

    public bool Add(T modifier) {
        if (modifier == null) {
            Debug.Log("数值系统：修饰器添加失败(传入为空)");
            return false;
        }
        if (this._modifiers.Contains(modifier)) {
            Debug.Log("数值系统：修饰器添加失败(对象已存在)");
            return false;
        }
        this._modifiers.Add(modifier);
        modifier.SetOnChangeCallback(this.Refresh);
        this.Refresh();
        return true;
    }
    public bool Remove(T modifier) {
        if (modifier == null) {
            Debug.Log("数值系统：修饰器移除失败(传入为空)");
            return false;
        }
        if (!this._modifiers.Contains(modifier)) {
            Debug.Log("数值系统：修饰器移除失败(目标不存在)");
            return false;
        }
        this._modifiers.Remove(modifier);
        modifier.SetOnChangeCallback(null);
        this.Refresh();
        return true;
    }
    public bool SetOnChangeCallback(Action action) {
        if (this.onChange != null && action != null) {
            if (this.onChange != action) Debug.LogWarning("属性系统：错误的修饰处理器回调赋值流程(原对象被覆盖)");
            else Debug.LogWarning("属性系统：错误的修饰处理器回调赋值流程(对象重复)");
        }
        this.onChange = action;
        return this.onChange != null;
    }
    public virtual void Refresh() {
        this.onChange?.Invoke();
    }
    public abstract float GetValue(float origin, out bool canModify);
}