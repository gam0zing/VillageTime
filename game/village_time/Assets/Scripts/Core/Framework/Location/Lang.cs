using UnityEngine;
using System;

[Serializable]
public class Lang {
    [SerializeField, ReadOnly] private string _key;
    [SerializeField, ReadOnly] private string _value;

    public Lang() {
        this._key = "";
        this._value = "";
    }

    public Lang(string key, string value) {
        this._key = key;
        this._value = value;
    }

    public string GetKey() => this._key;
    public string GetValue() => this._value;

    public void SetValue(string value) {
        value ??= "";
        this._value = value;
    }
}