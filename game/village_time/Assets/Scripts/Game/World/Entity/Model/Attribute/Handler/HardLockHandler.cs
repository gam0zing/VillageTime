using static UnityEngine.UI.Image;

public class HardLockHandler : ModifyHandler<Locker> {
    public override float GetValue(float origin, out bool canModify) {
        canModify = false;
        if (this._modifiers.Count <= 0) {
            canModify = true;
            return origin;
        }
        return this._value;
    }
    public override void Refresh() {
        base.Refresh();
        this._value = 0;
        float result = this._value;
        float maxStr = float.MinValue;
        foreach (Locker locker in this._modifiers) {
            if (maxStr <= locker.GetStrength()) {
                result = locker.GetValue();
                maxStr = locker.GetStrength();
            }
        }
        this._value = result;
    }
}