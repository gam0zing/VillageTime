public interface IResource {
    float Get();
    void Increase(float value);
    void Reduce(float value);
    void Change(float value);
    float GetMax();
    float GetMin();
    IAttribute GetMaxAttribute();
    IAttribute GetMinAttribute();
}