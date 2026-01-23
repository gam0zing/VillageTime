using UnityEngine;

[CreateAssetMenu(menuName = "自定义/注册表资产")]
public class RegistryAsset : ScriptableObject {
    [SerializeReference, SerializePolymorphism]
    public IFactoryCfg configuration;
}
