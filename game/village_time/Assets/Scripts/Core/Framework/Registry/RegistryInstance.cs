public class RegistryInstance<T> {
    public string Id { get; }
    public T configuration;

    public RegistryInstance(string id, T configuration) {
        this.Id = id;
        this.configuration = configuration;
    }
}
