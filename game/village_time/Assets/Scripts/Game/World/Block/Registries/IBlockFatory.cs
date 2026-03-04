public interface IBlockFactory<TCfg, TProduct>: IFactory<TCfg, TProduct> 
    where TCfg : BaseBlockConfiguration 
    where TProduct : FactoryProduct<TCfg> {

}