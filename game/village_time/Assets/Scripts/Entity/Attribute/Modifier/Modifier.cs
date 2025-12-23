using System;
using UnityEngine;

public class Modifier : IModifier {

    protected float _value = 0;
    protected Action onChange;

    public float GetValue() {
        return this._value;
    }
    public void SetValue(float value) {
        this._value = value;
        if (this.onChange == null) {
            Debug.LogWarning("属性系统：无法触发修改回调，回调对象为null");
            return;
        }
        this.onChange.Invoke();
    }
    public bool SetOnChangeCallback(Action action) {
        if (this.onChange != null && action != null) {
            if (this.onChange != action) Debug.LogWarning("属性系统：错误的修饰器回调赋值流程(原对象被覆盖)");
            else Debug.LogWarning("属性系统：错误的修饰器回调赋值流程(对象重复)");
        }
        this.onChange = action;
        return this.onChange != null;
    }
}