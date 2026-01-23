public abstract class AbstractItem<T> where T : IItemFactoryCfg {
    protected T _cfg;

    public AbstractItem(T cfg) {
        this._cfg = cfg;
    }

    public abstract void OnTick();
}