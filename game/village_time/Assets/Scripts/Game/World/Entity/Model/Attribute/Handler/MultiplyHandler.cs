using UnityEngine;

public class MultiplyHandler : ModifyHandler<Modifier> {

    protected new float _value = 1;
    public override float GetValue(float origin, out bool canModify) {
        canModify = true;
        return origin *= this._value;
    }
    public override void Refresh() {
        base.Refresh();
        this._value = 1;
        foreach (var modifier in this._modifiers) {
            this._value *= modifier.GetValue();
        }
    }
}