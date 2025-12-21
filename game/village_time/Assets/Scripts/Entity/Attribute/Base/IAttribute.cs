public interface IAttribute {
    bool AddModifier(IModifier modifier, IModifyHandler<IModifier> handler);
    bool RemoveModifier(IModifier modifier, IModifyHandler<IModifier> handler);
}