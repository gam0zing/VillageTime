
/// <summary>
/// 基本工厂接口，不要使用该接口作为多态序列化的起点类型
/// 使用时创建一个新接口来继承这个接口，比如IBlockFactory，并进一步限制泛型参数的类型范围，以免类型混用
/// 多态序列化时使用子接口或子类作为参数类型
/// 新建注册物类型的方式详见Game/World/Block或Game/World/Item
/// </summary>
public interface IFactory<TCfg, TProduct> where TCfg : IFactoryConfiguration where TProduct : FactoryProduct<TCfg> {
    TProduct Get(TCfg cfg);
}
