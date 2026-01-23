using System;
// 项目内所有抽象工厂配置的接口
// 序列化最好使用抽象类或者IFactoryCfg的子接口，以便快速找到类型匹配的子类，同时保证运行时的类型安全性
// 使用配置嵌套可以实现非常多的功能
// 工厂得到的实例都以泛型对象的形式存在，不同注册对象的泛型类型可能一样，所以游戏中使用这些泛型对象需要通过id查找注册表
// 建议添加Helper在游戏初始化阶段获取所有工厂并保存为字段，以便提升性能

// 这些接口的意义————构建树状配置分类，规定必须暴露的值，以及某些逻辑的通用入口，比如方块、物品的生命周期入口

/// <summary>
/// 最基本的工厂配置接口，所有的配置接口都继承它，作为形式上的统一。
/// 不要用这个接口作为Assets的配置字段类型，这样做没有任何意义。
/// </summary>
public interface IFactoryCfg { }

public interface IBlockFactoryCfg : IFactoryCfg {
    
}
public interface IItemFactoryCfg : IFactoryCfg {

}