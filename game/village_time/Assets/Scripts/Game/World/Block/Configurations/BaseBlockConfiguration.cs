using System;
using UnityEngine;

[Serializable]
public class BaseBlockConfiguration : IBlockConfiguration {
    [Header("---------- 基本 ----------")]
    [SerializeReference, SerializePolymorphism]
    public BaseBlockFactory factory;

    [Header("---------- 显示 ----------")]
    [Translation]
    public Lang nameLang;
    [Translation]
    public Lang descriptionLang;

    [Header("---------- 数据 ----------")]
    [Range(1, 16), Tooltip("该方块的宽度，单位：格")]
    public ushort height = 1;
    [Range(1, 16), Tooltip("该方块的宽度，单位：格")]
    public ushort width = 1;

    [Header("---------- 交互 ----------")]
    [Tooltip("用来判断该方块是否启用碰撞体")]
    public bool passable = false;


}