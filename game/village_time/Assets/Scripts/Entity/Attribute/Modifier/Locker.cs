using System;
using UnityEngine;

public class Locker : Modifier {

    private float _strength = 0;
    private bool _isTwoWay = true;
    public float GetStrength() {
        return this._strength;
    }
    public void SetStrength(float value) {
        this._strength = value;
        if (this.onChange == null) {
            Debug.LogWarning("属性系统：无法触发修改回调，回调对象为null");
            return;
        }
        this.onChange.Invoke();
    }
    public bool IsTwoWay() {
        return this._isTwoWay;
    }
    public void SetTwoWay(bool value) {
        this._isTwoWay = value;
        if (this.onChange == null) {
            Debug.LogWarning("属性系统：无法触发修改回调，回调对象为null");
            return;
        }
        this.onChange.Invoke();
    }
}