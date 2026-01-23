using UnityEngine;

/// <summary>
/// 检查器只读属性
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }

/// <summary>
/// 检查器多态序列化
/// </summary>
public class SerializePolymorphismAttribute : PropertyAttribute {
    public bool ShowTypeSelector { get; set; } = true;
    public bool AllowDragAndDrop { get; set; } = true;
    public bool ShowCreateButton { get; set; } = true;
    public bool ShowTypeLabel { get; set; } = true;
}

/// <summary>
/// 检查器多语言翻译
/// </summary>
public class TranslationAttribute : PropertyAttribute { }
