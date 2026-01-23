#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(TranslationAttribute))]
public class TranslationDrawer : PropertyDrawer {
    // 缓存
    private static List<Lang> cachedTranslations;
    private static string[] cachedDisplayOptions;
    private static bool needRefresh = true;

    // 当前选中的索引
    private int selectedIndex = -1;
    private string lastPropertyPath = "";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        try {
            // 检查是否是Lang类型
            SerializedProperty keyProperty = property.FindPropertyRelative("_key");
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            if (keyProperty == null || valueProperty == null) {
                EditorGUI.LabelField(position, "Translation属性只能用于Lang类型字段");
                EditorGUI.EndProperty();
                return;
            }

            // 刷新缓存
            if (needRefresh) {
                CacheTranslations();
                needRefresh = false;
            }

            // 如果属性路径改变，重置选中索引
            if (lastPropertyPath != property.propertyPath) {
                selectedIndex = -1;
                lastPropertyPath = property.propertyPath;
            }

            // 获取当前key
            string currentKey = keyProperty.stringValue ?? "";

            // 如果尚未设置选中索引，查找匹配项
            if (selectedIndex == -1 && !string.IsNullOrEmpty(currentKey)) {
                selectedIndex = FindTranslationIndex(currentKey);
            }

            // 计算布局高度
            float totalHeight = GetPropertyHeight(property, label);

            // 检查是否折叠
            bool isExpanded = property.isExpanded;

            if (isExpanded) {
                // 绘制折叠箭头和Label
                Rect foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

                // 绘制折叠内容
                if (property.isExpanded) {
                    float y = position.y + EditorGUIUtility.singleLineHeight;

                    // 绘制_key字段（保持ReadOnly）
                    Rect keyRect = new Rect(position.x + 15, y, position.width - 15, EditorGUIUtility.singleLineHeight);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(keyRect, keyProperty, new GUIContent("Key"), true);
                    EditorGUI.EndDisabledGroup();
                    y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    // 绘制_value字段（保持ReadOnly）
                    Rect valueRect = new Rect(position.x + 15, y, position.width - 15, EditorGUIUtility.singleLineHeight);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent("Value"), true);
                    EditorGUI.EndDisabledGroup();
                    y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    // 绘制下拉菜单
                    Rect popupRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                    DrawTranslationPopup(popupRect, property, currentKey);
                }
            } else {
                // 未折叠时，只显示Label和下拉菜单在同一行
                float labelWidth = EditorGUIUtility.labelWidth;

                // 绘制折叠箭头和Label
                Rect foldoutRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
                property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

                // 绘制下拉菜单（在Label右侧）
                Rect popupRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, EditorGUIUtility.singleLineHeight);
                DrawTranslationPopup(popupRect, property, currentKey);
            }
        } catch (System.Exception ex) {
            EditorGUI.LabelField(position, $"错误: {ex.Message}");
            Debug.LogError(ex);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            // 未折叠：只有一行
            return EditorGUIUtility.singleLineHeight;
        } else {
            // 折叠时：Label + Key + Value + 下拉菜单 + 间距
            return EditorGUIUtility.singleLineHeight * 4 +
                   EditorGUIUtility.standardVerticalSpacing * 3;
        }
    }

    private void DrawTranslationPopup(Rect position, SerializedProperty property, string currentKey) {
        // 创建下拉选项
        string[] options = CreatePopupOptions();

        // 使用自定义按钮样式，让按钮文本居中
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.popup);

        // 绘制下拉菜单
        EditorGUI.BeginChangeCheck();

        // 显示当前选中的翻译作为按钮文本，如果没有选中则显示"选择翻译"
        string buttonText = selectedIndex >= 0 && selectedIndex < cachedTranslations.Count
            ? cachedTranslations[selectedIndex].GetKey()
            : "选择翻译";

        // 使用没有Label的Popup
        if (GUI.Button(position, buttonText, buttonStyle)) {
            // 显示下拉菜单
            ShowTranslationDropdown(position, property);
        }
    }

    private void ShowTranslationDropdown(Rect position, SerializedProperty property) {
        GenericMenu menu = new GenericMenu();

        // 添加"未选择"选项
        menu.AddItem(new GUIContent("(未选择)"), selectedIndex == -1, () => {
            selectedIndex = -1;
            // 清空当前翻译
            ClearTranslation(property);
        });

        // 添加翻译选项
        for (int i = 0; i < cachedTranslations.Count; i++) {
            int index = i; // 闭包需要局部变量
            var translation = cachedTranslations[index];
            menu.AddItem(
                new GUIContent($"{translation.GetKey()} : {translation.GetValue()}"),
                selectedIndex == index,
                () => {
                    selectedIndex = index;
                    ApplyTranslation(property, translation);
                }
            );
        }

        menu.DropDown(position);
    }

    private void ClearTranslation(SerializedProperty property) {
        try {
            Undo.RecordObject(property.serializedObject.targetObject, "清空翻译");

            SerializedProperty keyProperty = property.FindPropertyRelative("_key");
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            if (keyProperty != null && valueProperty != null) {
                keyProperty.stringValue = "";
                valueProperty.stringValue = "";
                property.serializedObject.ApplyModifiedProperties();
            }
        } catch (System.Exception ex) {
            Debug.LogError($"清空翻译失败: {ex.Message}");
        }
    }

    private string[] CreatePopupOptions() {
        List<string> options = new List<string> { "(未选择)" };

        if (cachedDisplayOptions != null && cachedDisplayOptions.Length > 0) {
            options.AddRange(cachedDisplayOptions);
        } else {
            options.Add("(无可用翻译)");
        }

        return options.ToArray();
    }

    private int FindTranslationIndex(string key) {
        if (cachedTranslations == null || string.IsNullOrEmpty(key))
            return -1;

        for (int i = 0; i < cachedTranslations.Count; i++) {
            var translation = cachedTranslations[i];
            if (translation != null && translation.GetKey() == key)
                return i;
        }
        return -1;
    }

    private void CacheTranslations() {
        cachedTranslations = new List<Lang>();

        if (GameKeys.TRANSLATION == null) {
            Debug.LogWarning("GameKeys.TRANSLATION 未初始化");
            return;
        }

        try {
            System.Type type = typeof(GameKeys.Translations);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields) {
                if (field.FieldType == typeof(Lang)) {
                    Lang lang = field.GetValue(GameKeys.TRANSLATION) as Lang;
                    if (lang != null && !string.IsNullOrEmpty(lang.GetKey())) {
                        cachedTranslations.Add(lang);
                    }
                }
            }

            // 排序
            cachedTranslations = cachedTranslations
                .OrderBy(l => l.GetKey())
                .ToList();

            // 生成显示文本
            cachedDisplayOptions = cachedTranslations
                .Select(l => $"{l.GetKey()} : {l.GetValue()}")
                .ToArray();

            // Debug.Log($"缓存了 {cachedTranslations.Count} 个翻译项");
        } catch (System.Exception ex) {
            Debug.LogError($"缓存翻译失败: {ex.Message}");
        }
    }

    private void ApplyTranslation(SerializedProperty property, Lang selectedTranslation) {
        if (property == null || selectedTranslation == null) return;

        try {
            UnityEngine.Object targetObject = property.serializedObject.targetObject;

            // 记录Undo
            Undo.RecordObject(targetObject, "应用翻译");

            // 直接更新序列化属性
            SerializedProperty keyProperty = property.FindPropertyRelative("_key");
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            if (keyProperty == null || valueProperty == null) {
                Debug.LogError("找不到_key或_value属性");
                return;
            }

            string oldKey = keyProperty.stringValue;

            // 更新值
            keyProperty.stringValue = selectedTranslation.GetKey();
            valueProperty.stringValue = selectedTranslation.GetValue();

            // 应用修改
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(targetObject);

            // 处理Prefab
            HandlePrefabModifications(targetObject);

            // Debug.Log($"已应用翻译: {oldKey} -> {selectedTranslation.GetKey()}");
        } catch (System.Exception ex) {
            Debug.LogError($"应用翻译失败: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void HandlePrefabModifications(UnityEngine.Object targetObject) {
        try {
            if (PrefabUtility.IsPartOfPrefabInstance(targetObject)) {
                PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
            }
        } catch (System.Exception ex) {
            Debug.LogWarning($"处理Prefab修改失败: {ex.Message}");
        }
    }
}
#endif