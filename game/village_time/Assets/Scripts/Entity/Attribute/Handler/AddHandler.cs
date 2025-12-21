public class AddHandler : ModifyHandler<Modifier> {
    public override float GetValue(float origin, out bool canModify) {
        canModify = true;
        foreach (IModifier modifier in this._modifiers) {
            origin += modifier.GetValue();
        }
        return origin;
    }
}