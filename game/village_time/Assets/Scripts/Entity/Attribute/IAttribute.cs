public interface IAttribute {
    float Current { get; set; }
    float Base {  get; set; }
    float Max { get; set; }
    float Min { get; set; }
    IModifier First { get; }
    IModifier Final { get; }
    /// <summary>
    /// 增加一个修饰器
    /// </summary>
    /// <returns>更新后的属性值</returns>
    void AddModifier(IModifier modifier);
}