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
        this.onChange?.Invoke();
    }
    public bool SetOnChangeCallback(Action action) {
        this.onChange = action;
        return this.onChange != null;
    }
}