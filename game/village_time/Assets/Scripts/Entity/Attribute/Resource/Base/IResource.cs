public interface IResource {
    float Get();
    void Increase(float value);
    void Reduce(float value);
    void Change(float value);
}