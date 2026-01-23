using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializePolymorphismAttribute))]
public class SerializePolymorphismDrawer : PropertyDrawer {
    private const float SELECT_BUTTON_WIDTH = 70f;    // "选择类型"按钮宽度
    private const float RESET_BUTTON_WIDTH = 70f;    // "恢复默认"按钮宽度
    private const float BUTTON_SPACING = 2f;         // 按钮间距
    private const float TYPE_LABEL_HEIGHT = 18f;
    private const float FOLDOUT_WIDTH = 15f;

    private static Dictionary<string, Type> _fieldTypeCache = new Dictionary<string, Type>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.managedReferenceValue != null) {
            height += TYPE_LABEL_HEIGHT + 2f;
        }

        if (property.isExpanded && property.managedReferenceValue != null) {
            float childHeight = CalculateChildPropertiesHeight(property);
            height += childHeight;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        // 主控制行
        Rect mainRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        DrawMainControl(mainRect, property, label);

        float currentY = position.y + EditorGUIUtility.singleLineHeight + 2f;

        // 类型标签
        if (property.managedReferenceValue != null) {
            Rect typeRect = new Rect(position.x + 2, currentY, position.width - 4, TYPE_LABEL_HEIGHT);
            DrawTypeLabel(typeRect, property);
            currentY += TYPE_LABEL_HEIGHT + 2f;
        }

        // 子属性
        if (property.isExpanded && property.managedReferenceValue != null) {
            Rect childRect = new Rect(position.x, currentY, position.width, position.height - currentY + position.y);
            DrawChildProperties(childRect, property);
        }

        // 拖拽区域
        HandleDragAndDrop(position, property);

        EditorGUI.EndProperty();
    }

    private void DrawMainControl(Rect rect, SerializedProperty property, GUIContent label) {
        // 计算按钮总宽度（包含间距）
        float totalButtonsWidth = SELECT_BUTTON_WIDTH + RESET_BUTTON_WIDTH + BUTTON_SPACING;

        // 确保标签有最小宽度（至少80像素）
        float minLabelWidth = 80f;
        float availableWidth = rect.width;

        // 如果空间不足，按比例缩小按钮
        if (availableWidth < minLabelWidth + totalButtonsWidth) {
            // 计算可用按钮空间
            float availableForButtons = availableWidth - minLabelWidth - BUTTON_SPACING;

            // 按比例分配按钮宽度
            float selectWidth = availableForButtons * 0.5f;
            float resetWidth = availableForButtons * 0.5f;

            // 绘制压缩布局
            DrawCompressedLayout(rect, property, label, minLabelWidth, selectWidth, resetWidth);
        } else {
            // 正常布局
            float labelWidth = rect.width - totalButtonsWidth;
            DrawNormalLayout(rect, property, label, labelWidth);
        }
    }

    private void DrawNormalLayout(Rect rect, SerializedProperty property, GUIContent label, float labelWidth) {
        // 标签区域
        Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
        DrawLabelAndFoldout(labelRect, property, label);

        // 选择类型按钮
        Rect selectRect = new Rect(
            rect.x + labelWidth,
            rect.y,
            SELECT_BUTTON_WIDTH,
            rect.height);

        // 恢复默认按钮
        Rect resetRect = new Rect(
            rect.x + labelWidth + SELECT_BUTTON_WIDTH + BUTTON_SPACING,
            rect.y,
            RESET_BUTTON_WIDTH,
            rect.height);

        // 绘制选择类型按钮
        if (GUI.Button(selectRect, "选择类型", EditorStyles.miniButtonLeft)) {
            ShowTypeSelector(property);
        }

        // 绘制恢复默认按钮
        GUI.enabled = property.managedReferenceValue != null;
        if (GUI.Button(resetRect, "恢复默认", EditorStyles.miniButtonRight)) {
            ResetToDefaultInstance(property);
        }
        GUI.enabled = true;
    }

    private void DrawCompressedLayout(Rect rect, SerializedProperty property, GUIContent label,
                                     float labelWidth, float selectWidth, float resetWidth) {
        // 标签区域
        Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
        DrawLabelAndFoldout(labelRect, property, label);

        // 选择类型按钮（压缩文本）
        Rect selectRect = new Rect(
            rect.x + labelWidth,
            rect.y,
            selectWidth,
            rect.height);

        // 恢复默认按钮（压缩文本）
        Rect resetRect = new Rect(
            rect.x + labelWidth + selectWidth + BUTTON_SPACING,
            rect.y,
            resetWidth,
            rect.height);

        // 使用更短的按钮文本
        if (GUI.Button(selectRect, "选择", EditorStyles.miniButtonLeft)) {
            ShowTypeSelector(property);
        }

        GUI.enabled = property.managedReferenceValue != null;
        if (GUI.Button(resetRect, "重置", EditorStyles.miniButtonRight)) {
            ResetToDefaultInstance(property);
        }
        GUI.enabled = true;
    }

    private void DrawLabelAndFoldout(Rect rect, SerializedProperty property, GUIContent label) {
        Rect foldoutRect = new Rect(rect.x, rect.y, FOLDOUT_WIDTH, rect.height);

        if (property.managedReferenceValue != null) {
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "", true);
        } else {
            // 绘制占位符保持对齐
            GUI.Label(new Rect(rect.x, rect.y, FOLDOUT_WIDTH, rect.height), "");
        }

        Rect labelRect = new Rect(
            rect.x + FOLDOUT_WIDTH,
            rect.y,
            rect.width - FOLDOUT_WIDTH,
            rect.height);

        string displayLabel = label.text;
        if (property.managedReferenceValue != null) {
            Type type = property.managedReferenceValue.GetType();
            displayLabel += $" ({GetShortTypeName(type)})";
        }

        EditorGUI.LabelField(labelRect, displayLabel);
    }

    private void DrawTypeLabel(Rect rect, SerializedProperty property) {
        if (property.managedReferenceValue == null)
            return;

        Type type = property.managedReferenceValue.GetType();
        string displayText = $"类型: {type.FullName}";

        GUIStyle style = new GUIStyle(EditorStyles.miniLabel) {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(4, 4, 0, 0),
            fontSize = 10,
            normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 1f) }
        };

        EditorGUI.LabelField(rect, displayText, style);
    }

    private void DrawChildProperties(Rect rect, SerializedProperty property) {
        if (property.managedReferenceValue == null)
            return;

        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();

        bool enterChildren = true;
        float currentY = rect.y;

        iterator.NextVisible(enterChildren);

        while (!SerializedProperty.EqualContents(iterator, endProperty)) {
            float height = EditorGUI.GetPropertyHeight(iterator, true);
            Rect childRect = new Rect(rect.x, currentY, rect.width, height);

            EditorGUI.PropertyField(childRect, iterator, true);

            currentY += height + EditorGUIUtility.standardVerticalSpacing;
            enterChildren = false;
            iterator.NextVisible(enterChildren);
        }
    }

    private float CalculateChildPropertiesHeight(SerializedProperty property) {
        float height = 0f;

        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();

        bool enterChildren = true;

        iterator.NextVisible(enterChildren);

        while (!SerializedProperty.EqualContents(iterator, endProperty)) {
            height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            enterChildren = false;
            iterator.NextVisible(enterChildren);
        }

        return height;
    }

    private void HandleDragAndDrop(Rect rect, SerializedProperty property) {
        Event evt = Event.current;

        if (!rect.Contains(evt.mousePosition))
            return;

        switch (evt.type) {
            case EventType.DragUpdated:
                bool isValid = false;
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences) {
                    if (CanAssignObjectToProperty(obj, property)) {
                        isValid = true;
                        break;
                    }
                }
                DragAndDrop.visualMode = isValid ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                evt.Use();
                break;

            case EventType.DragPerform:
                DragAndDrop.AcceptDrag();
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences) {
                    if (TryAssignObjectToProperty(obj, property)) {
                        property.serializedObject.ApplyModifiedProperties();
                        evt.Use();
                        return;
                    }
                }
                break;
        }
    }

    private bool CanAssignObjectToProperty(UnityEngine.Object obj, SerializedProperty property) {
        Type fieldType = GetFieldType(property);
        if (fieldType == null) return false;

        if (obj is GameObject gameObject) {
            Component component = gameObject.GetComponent(fieldType);
            return component != null;
        }

        return fieldType.IsAssignableFrom(obj.GetType());
    }

    private bool TryAssignObjectToProperty(UnityEngine.Object obj, SerializedProperty property) {
        Type fieldType = GetFieldType(property);
        if (fieldType == null) return false;

        if (obj is GameObject gameObject) {
            Component component = gameObject.GetComponent(fieldType);
            if (component != null) {
                property.managedReferenceValue = component;
                return true;
            }
        } else if (fieldType.IsAssignableFrom(obj.GetType())) {
            property.managedReferenceValue = obj;
            return true;
        }

        return false;
    }

    private void ShowTypeSelector(SerializedProperty property) {
        GenericMenu menu = new GenericMenu();

        Type fieldType = GetFieldType(property);
        if (fieldType == null) {
            menu.AddDisabledItem(new GUIContent("无法确定字段类型"));
            menu.ShowAsContext();
            return;
        }

        menu.AddDisabledItem(new GUIContent($"选择 {fieldType.Name} 类型"));
        menu.AddSeparator("");

        List<Type> availableTypes = GetAssignableTypes(fieldType);

        if (availableTypes.Count == 0) {
            menu.AddDisabledItem(new GUIContent("未找到可用类型"));
        } else {
            // 按命名空间分组
            var groupedTypes = availableTypes
                .GroupBy(t => t.Namespace)
                .OrderBy(g => g.Key);

            foreach (var group in groupedTypes) {
                if (!string.IsNullOrEmpty(group.Key)) {
                    menu.AddSeparator(group.Key + "/");
                }

                foreach (Type type in group.OrderBy(t => t.Name)) {
                    string displayName = GetTypeDisplayName(type);
                    bool isCurrent = property.managedReferenceValue?.GetType() == type;

                    menu.AddItem(new GUIContent(displayName), isCurrent, () => {
                        if (!isCurrent) {
                            CreateAndAssignInstance(property, type);
                        }
                    });
                }
            }
        }

        menu.ShowAsContext();
    }

    private void ResetToDefaultInstance(SerializedProperty property) {
        if (property.managedReferenceValue == null) {
            Debug.LogWarning("无法恢复默认：当前值为空");
            return;
        }

        Type currentType = property.managedReferenceValue.GetType();

        try {
            // 检查是否有默认构造函数
            ConstructorInfo constructor = currentType.GetConstructor(Type.EmptyTypes);
            if (constructor == null) {
                Debug.LogError($"类型 {currentType.Name} 没有无参构造函数，无法创建默认实例");
                return;
            }

            // 创建新的默认实例
            object newInstance = constructor.Invoke(null);
            property.managedReferenceValue = newInstance;
            property.serializedObject.ApplyModifiedProperties();

            // 展开显示新实例的属性
            property.isExpanded = true;

            Debug.Log($"已将 {property.name} 恢复为 {currentType.Name} 的默认实例");
        } catch (Exception ex) {
            Debug.LogError($"恢复默认实例失败: {ex.Message}");
        }
    }

    private void CreateAndAssignInstance(SerializedProperty property, Type type) {
        try {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null) {
                Debug.LogError($"类型 {type.Name} 没有无参构造函数");
                return;
            }

            object instance = constructor.Invoke(null);
            property.managedReferenceValue = instance;
            property.serializedObject.ApplyModifiedProperties();
            property.isExpanded = true;
        } catch (Exception ex) {
            Debug.LogError($"创建 {type.Name} 实例失败: {ex.Message}");
        }
    }

    private string GetShortTypeName(Type type) {
        if (type == null) return "null";

        string name = type.Name;

        // 如果名字太长，截断显示
        if (name.Length > 15) {
            name = name.Substring(0, 12) + "...";
        }

        return name;
    }

    private string GetTypeDisplayName(Type type) {
        if (type == null) return "null";

        string displayName = type.Name;

        // 如果有命名空间，显示最后一部分
        if (!string.IsNullOrEmpty(type.Namespace)) {
            string[] namespaceParts = type.Namespace.Split('.');
            if (namespaceParts.Length > 0) {
                string lastNamespace = namespaceParts[namespaceParts.Length - 1];
                displayName = $"{lastNamespace}.{type.Name}";
            }
        }

        return displayName;
    }

    private Type GetFieldType(SerializedProperty property) {
        string cacheKey = $"{property.serializedObject.targetObject.GetInstanceID()}.{property.propertyPath}";

        if (!_fieldTypeCache.TryGetValue(cacheKey, out Type fieldType)) {
            fieldType = GetFieldTypeFromProperty(property);
            _fieldTypeCache[cacheKey] = fieldType;
        }

        return fieldType;
    }

    private Type GetFieldTypeFromProperty(SerializedProperty property) {
        Type targetType = property.serializedObject.targetObject.GetType();
        string[] pathParts = property.propertyPath.Split('.');

        FieldInfo fieldInfo = null;
        Type currentType = targetType;

        foreach (string part in pathParts) {
            fieldInfo = currentType.GetField(part,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (fieldInfo == null) {
                // 处理数组/列表
                if (part == "Array" || part.EndsWith("]")) {
                    if (currentType.IsArray) {
                        currentType = currentType.GetElementType();
                        continue;
                    }

                    if (currentType.IsGenericType &&
                        typeof(System.Collections.IList).IsAssignableFrom(currentType)) {
                        currentType = currentType.GetGenericArguments()[0];
                        continue;
                    }
                }

                return null;
            }

            currentType = fieldInfo.FieldType;
        }

        return fieldInfo?.FieldType;
    }

    private List<Type> GetAssignableTypes(Type baseType) {
        List<Type> types = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            try {
                foreach (Type type in assembly.GetTypes()) {
                    if (!type.IsAbstract && !type.IsInterface &&
                        type.IsClass && baseType.IsAssignableFrom(type) &&
                        (type.IsSerializable || type.GetCustomAttribute<SerializableAttribute>() != null)) {
                        types.Add(type);
                    }
                }
            } catch (ReflectionTypeLoadException) {
                // 忽略无法加载的类型
            }
        }

        return types;
    }
}
#endif