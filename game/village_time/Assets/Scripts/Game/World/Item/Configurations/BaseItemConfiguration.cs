using System;
using UnityEngine;

[Serializable]
public class BaseItemConfiguration : FactoryConfiguration {
    [Header("---------- 基本 ----------")]
    [SerializeReference, SerializePolymorphism]
    public BaseItemFactory factory;

    [Header("显示")]
    [Translation]
    public Lang nameLang;
    [Translation]
    public Lang descriptionLang;

    [Header("数据")]
    [Range(0, 1024), Tooltip("物品的最大堆叠数量")] 
    public uint maxStack = 128;

    public override sealed string GetRegistryType() {
        return RegisterHelper.ITEM;
    }
}