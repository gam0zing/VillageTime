using System.Collections.Generic;
using UnityEngine;

public abstract class ModifyHandler<T> : IModifyHandler<T> where T : IModifier {
    protected List<T> _modifiers;

    public ModifyHandler() {
        this._modifiers = new();
    }

    public bool Add(T modifier) {
        this._modifiers.Add(modifier);
        return true;
    }
    public bool Remove(T modifier) {
        this._modifiers.Remove(modifier);
        return true;
    }
    public abstract float GetValue(float origin, out bool canModify);
}