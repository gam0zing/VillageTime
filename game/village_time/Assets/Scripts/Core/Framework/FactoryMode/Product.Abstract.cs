/// <summary>
/// 工厂产品抽象类，所有工厂产品必须继承这个抽象类
/// </summary>
/// <typeparam name="TCfg"></typeparam>
public abstract class FactoryProduct<TCfg> where TCfg : IFactoryConfiguration {

    public readonly TCfg configuration;
    public FactoryProduct(TCfg configuration) {
        this.configuration = configuration;
    }
}