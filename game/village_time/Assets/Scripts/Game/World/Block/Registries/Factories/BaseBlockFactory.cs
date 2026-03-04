using System;

[Serializable]
public class BaseBlockFactory : IBlockFactory<BaseBlockConfiguration, BaseBlockPrototype> {
    public BaseBlockPrototype Get(BaseBlockConfiguration cfg) {
        return new BaseBlockPrototype(cfg);
    }
}