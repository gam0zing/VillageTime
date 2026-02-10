public interface IItemFactory<TCfg, TProduct> : IFactory<TCfg, TProduct> 
    where TCfg : BaseItemConfiguration 
    where TProduct : FactoryProduct<TCfg> {

}