public interface IModifier {
    float Value { get; set; }

    /// <summary>
    /// 获取当前装饰结果
    /// </summary>
    /// <param name="value">应为上一个装饰器中该方法的返回值，或属性的基础值</param>
    /// <returns>到该装饰器为止被修饰过的属性值</returns>
    float GetModified(float value);

    void SetLast(IModifier modifier);
    void SetNext(IModifier modifier);
    void Banding(IModifier first, IModifier final);
}