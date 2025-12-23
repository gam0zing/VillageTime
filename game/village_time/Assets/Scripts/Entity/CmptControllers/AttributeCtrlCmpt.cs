using UnityEngine;
public class AttributeCtrlCmpt : MonoBehaviour
{
    public IAttribute GetAttribute(Type type) {
        if (!typeof(MonoBehaviour).IsAssignableFrom(type)) {
            Debug.LogWarning($"属性系统：尝试获取属性组件失败（传入的Type不是组件类型），Type：{type}");
            return null;
        }
        return this.GetComponent(type);
    }
}