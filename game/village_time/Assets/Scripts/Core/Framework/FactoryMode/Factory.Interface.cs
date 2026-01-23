// 工厂接口，所有接口及子类都只有一个方法，Get<T>();
// 作为属性放在配置类中用来给资产和运行时对象之间建立映射
// 在注册资产的配置中选择，以决定该元素具体要构造什么类型的实例
// 不同类型的注册配置其工厂类型的选择范围也不同

/// <summary>
/// 基本工厂接口，不要使用该接口作为多态序列化的起点类型
/// </summary>
public interface IFactory<TCfg, TProduct> where TCfg : IFactoryConfiguration where TProduct : IFactoryProduct {
    TProduct Get(TCfg cfg);
}
public interface IItemFactory<TCfg, TProduct> : IFactory<TCfg, TProduct> where TCfg : IItemConfiguration where TProduct : IItemProduct {

}
public interface IBlockFactory<TCfg, TProduct> : IFactory<TCfg, TProduct> where TCfg : IBlockConfiguration where TProduct : IBlockProduct {

}