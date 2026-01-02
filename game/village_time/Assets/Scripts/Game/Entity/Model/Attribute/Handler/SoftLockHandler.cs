using UnityEngine;

public class SoftLockHandler : ModifyHandler<Locker> {
    public override float GetValue(float origin, out bool canModify) {
        canModify = false;
        // 暂时没想到好办法，取最后一个生效
        if (this._modifiers.Count <= 0) {
            canModify = true;
            return origin;
        }
        Locker locker = this._modifiers[this._modifiers.Count - 1];
        float strength = locker.GetStrength();
        float target = locker.GetValue();
        float maxMove = Mathf.Abs(strength);
        float length = target - origin;
        float move;
        if (locker.IsTwoWay()) {
            move = Mathf.Clamp(length, -maxMove, maxMove);
            
            return origin + move;
        } else {
            if (strength > 0) move = Mathf.Clamp(length, 0, maxMove);
            else move = Mathf.Clamp(length, -maxMove, 0);
            return origin + move;
        }
    }
}