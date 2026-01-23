using UnityEngine;

public abstract class RegistryAsset<T> : ScriptableObject where T : IFactoryConfiguration {
    [SerializeReference, SerializePolymorphism]
    public T configuration;
}
