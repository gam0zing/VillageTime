using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute { }
public class SerializePolymorphismAttribute : PropertyAttribute {
    public bool ShowTypeSelector { get; set; } = true;
    public bool AllowDragAndDrop { get; set; } = true;
    public bool ShowCreateButton { get; set; } = true;
    public bool ShowTypeLabel { get; set; } = true;
}