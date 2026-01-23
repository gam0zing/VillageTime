using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceCmpt), true)]
[CanEditMultipleObjects]
public class AbstractResourceEditor : Editor {
    private float inputValue = 0f;

    public override void OnInspectorGUI() {
        var resource = (ResourceCmpt)this.target;

        string displayName = resource.GetName();

        float current = resource.Get();
        float max = resource.GetMax();
        float min = resource.GetMin();

        const float INF_THRESHOLD = float.MaxValue * 0.9f;
        bool useCodeMax = max >= INF_THRESHOLD;
        bool useCodeMin = min <= -INF_THRESHOLD;

        string minStr = this.FormatBound(min);
        string maxStr = this.FormatBound(max);
        string label = $"[{minStr}, {maxStr}]";

        SerializedProperty valueProp = this.serializedObject.FindProperty("_value");
        if (valueProp != null) {
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent customLabel = new GUIContent(displayName);
                EditorGUILayout.PropertyField(valueProp, customLabel, true);

                GUIStyle rangeStyle = EditorStyles.miniLabel;
                rangeStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                GUILayout.Label(label, rangeStyle, GUILayout.Width(80));
            }
            EditorGUILayout.EndHorizontal();
        } else {
            EditorGUILayout.LabelField("资源值", $"{current:F1} {label}");
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("修改");
        this.inputValue = EditorGUILayout.FloatField(this.inputValue);

        if (GUILayout.Button("-", GUILayout.Width(25))) {
            Undo.RecordObject(this.target, "减少资源值");
            ((ResourceCmpt)this.target).Reduce(this.inputValue);
            EditorUtility.SetDirty(this.target);
        }

        if (GUILayout.Button("+", GUILayout.Width(25))) {
            Undo.RecordObject(this.target, "增加资源值");
            ((ResourceCmpt)this.target).Increase(this.inputValue);
            EditorUtility.SetDirty(this.target);
        }

        if (GUILayout.Button("=", GUILayout.Width(25))) {
            Undo.RecordObject(this.target, "设置资源值");
            ((ResourceCmpt)this.target).Change(this.inputValue);
            EditorUtility.SetDirty(this.target);
        }

        EditorGUILayout.EndHorizontal();

        this.serializedObject.Update();
        this.serializedObject.ApplyModifiedProperties();
    }

    private string FormatBound(float value) {
        const float INF_THRESHOLD = float.MaxValue * 0.9f;
        const float ZERO_TOLERANCE = 1e-5f;

        if (value >= INF_THRESHOLD) return "∞";
        if (value <= -INF_THRESHOLD) return "-∞";

        if (Mathf.Abs(value) < ZERO_TOLERANCE)
            return "0";

        return $"{value:F1}";
    }
}