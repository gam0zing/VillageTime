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
        this.onChange?.Invoke();
    }
    public bool IsTwoWay() {
        return this._isTwoWay;
    }
    public void SetTwoWay(bool value) {
        this._isTwoWay = value;
        this.onChange?.Invoke();
    }
}