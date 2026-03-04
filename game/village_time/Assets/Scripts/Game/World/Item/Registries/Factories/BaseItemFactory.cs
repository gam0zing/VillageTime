using System;

[Serializable]
public class BaseItemFactory : IItemFactory<BaseItemConfiguration, BaseItem> {
    public BaseItem Get(BaseItemConfiguration cfg) {
        return new BaseItem(cfg);
    }
}