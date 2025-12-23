public interface IAttribute {
    float Value { get; }
    float Base { get; set; }
    bool AddModifier(IModifier modifier, IModifyHandler<IModifier> handler);
    bool RemoveModifier(IModifier modifier, IModifyHandler<IModifier> handler);
}