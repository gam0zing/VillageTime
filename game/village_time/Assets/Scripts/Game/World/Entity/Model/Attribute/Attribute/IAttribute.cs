public interface IAttribute {
    float Value { get; }
    float Base { get; set; }
    bool AddModifier<T>(T modifier, IModifyHandler<T> handler) where T : IModifier;
    bool RemoveModifier<T>(T modifier, IModifyHandler<T> handler) where T : IModifier;
}