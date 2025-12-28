using System.Collections.Generic;
using UnityEngine;

public class ModelCtrlCmpt : MonoBehaviour {

    private HashSet<IAttribute> _attributes = new();
    private HashSet<IResource> _resources = new();

    private void Awake() {
        this.Init();
    }

    private void Init() {
        Component[] components = this.GetComponents<Component>();
        foreach (Component comp in components) {
            if (comp != null && typeof(IAttribute).IsAssignableFrom(comp.GetType())) {
                this._attributes.Add(comp as IAttribute);
            }
        }
    }

    public IAttribute[] GetAttribute<T>() where T : Component, IAttribute {
        return this.GetComponents<T>();
    }

    public IResource[] GetResource<T>() where T : Component, IResource {
        return this.GetComponents<T>();
    }

}