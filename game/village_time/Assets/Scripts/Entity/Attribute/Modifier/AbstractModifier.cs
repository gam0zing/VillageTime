public abstract class AbstractModifier : IModifier {
    public float Value { get => this._value; set => this._value = value; }
    protected float _value;
    public float Current => this._current;
    protected float _current;
    public IModifier Next => this._next;
    protected IModifier _next;
    public IModifier Last => this._last;
    protected IModifier _last;
    public IAttribute Root => this._root;
    protected IAttribute _root;

    protected AbstractModifier(IAttribute root, IModifier last, float value = 0, IModifier next = null) {
        this._root = root;
        this._last = last;
        this._value = value;
        this._next = next;
    }
    public abstract float GetModified(float value);

    public void SetLast(IModifier modifier) {
    }

    public void SetNext(IModifier modifier) {
    }

    public void Banding(IModifier first, IModifier final) {
    }
}