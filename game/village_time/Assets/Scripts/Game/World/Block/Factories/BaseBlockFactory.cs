public class BaseBlockFactory : IBlockFactory<BaseBlockConfiguration, BaseBlock> {
    public BaseBlock Get(BaseBlockConfiguration cfg) {
        return new BaseBlock(cfg);
    }
}