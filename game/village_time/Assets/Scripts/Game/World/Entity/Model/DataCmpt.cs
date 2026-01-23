using System.Collections.Generic;
using UnityEngine;

public class DataCmpt : MonoBehaviour {

    private HashSet<IAttribute> _attributes = new();

    private HashSet<IResource> _resources = new();

    private void Awake() {
        this.Init();
    }

    private void Init() {

    }
    public bool AddResource(IResource resource) {
        return this._resources.Add(resource);
    }
    public bool RemoveResource(IResource resource) {
        return this._resources.Remove(resource);
    }
}