using System;
/// <summary>
/// 最基本的工厂配置接口，所有的配置接口都继承它，作为形式上的统一。
/// 不要使用这个接口作为多态序列化的起点类型
/// </summary>
public interface IFactoryConfiguration {
    string GetRegistryType();
}

/// <summary>
/// 该抽象类完全服务于抽象方法 GetRegistryType()，用以在基本各个类型的工厂类中实现override sealed，阻止子类覆盖分类方法
/// </summary>
public abstract class FactoryConfiguration : IFactoryConfiguration {
    public abstract string GetRegistryType();
}