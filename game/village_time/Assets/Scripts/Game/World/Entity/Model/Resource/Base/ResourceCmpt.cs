using UnityEngine;

public abstract class ResourceCmpt : MonoBehaviour, IResource {

    public const float DEFAULT_MAX = float.MaxValue;
    public const float DEFAULT_MIN = 0F;

    [SerializeField, ReadOnly] private float _value = 0F;
    private IAttribute _max;
    private IAttribute _min;

    public float Get() {
        return this._value;
    }
    public float GetMax() {
        return this._max != null ? this._max.Value : ResourceCmpt.DEFAULT_MAX;
    }
    public float GetMin() {
        return this._min != null ? this._min.Value : ResourceCmpt.DEFAULT_MIN;
    }
    public void Change(float value) {
        this._value = Mathf.Clamp(value, this.GetMin(), this.GetMax());
    }
    public void Increase(float value) {
        this._value = Mathf.Clamp(this._value + value, this.GetMin(), this.GetMax());
    }
    public void Reduce(float value) {
        this._value = Mathf.Clamp(this._value - value, this.GetMin(), this.GetMax());
    }
    public IAttribute GetMaxAttribute() {
        return this._max;
    }
    public IAttribute GetMinAttribute() {
        return this._min;
    }
    public abstract string GetName();
}