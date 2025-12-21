public class HardLockHandler : ModifyHandler<Locker> {
    public override float GetValue(float origin, out bool canModify) {
        canModify = false;
        if (this._modifiers.Count <= 0) {
            canModify = true;
            return origin;
        }
        float ret = origin;
        float maxStr = float.MinValue;
        foreach (Locker locker in this._modifiers) {
            if (maxStr <= locker.GetStrength()) {
                ret = locker.GetValue();
                maxStr = locker.GetStrength();
            }
        }
        return ret;
    }
}