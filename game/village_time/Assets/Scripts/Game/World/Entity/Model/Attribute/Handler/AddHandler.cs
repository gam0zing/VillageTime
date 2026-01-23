using UnityEngine;

public class AddHandler : ModifyHandler<Modifier> {
    public override float GetValue(float origin, out bool canModify) {
        canModify = true;
        return origin + this._value;
    }
    public override void Refresh() {
        base.Refresh();
        this._value = 0;
        foreach (var modifier in this._modifiers) {
            this._value += modifier.GetValue();
        }
    }
}