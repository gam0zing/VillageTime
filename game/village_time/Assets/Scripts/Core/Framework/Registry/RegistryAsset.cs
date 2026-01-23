using UnityEngine;

public abstract class RegistryAsset<T> : ScriptableObject where T : IFactoryCfg {
    [SerializeReference, SerializePolymorphism]
    public T configuration;
}
