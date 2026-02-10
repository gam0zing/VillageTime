using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public sealed class RegistryMgr {

    #region 单例
    private static RegistryMgr _instance = new();
    public static RegistryMgr getInstance() {
        return _instance;
    }
    private RegistryMgr() {
        this._registries = new();
    }
    #endregion

    private Dictionary<string, Dictionary<string, object>> _registries;

    /// <summary>
    /// 自动调用，将解析好的注册资产放入表中
    /// </summary>
    /// <typeparam name="TCfg"></typeparam>
    /// <param name="type">注册类型</param>
    /// <param name="id">注册Id</param>
    /// <param name="configuration">原型</param>
    public void Register<TCfg>(string id, TCfg configuration) where TCfg : IFactoryConfiguration {
        string type = configuration.GetRegistryType();
        this._registries.TryAdd(type, new Dictionary<string, object>());
        this._registries[type][id] = configuration;
    }

    /// <summary>
    /// 获取原型
    /// </summary>
    /// <typeparam name="TCfg">要获取的原型的准确类型</typeparam>
    /// <param name="type">注册分类</param>
    /// <param name="id">注册Id</param>
    /// <returns>返回对应类型的原型，如果类型不匹配或找不到目标，则返回null</returns>
    public TCfg Get<TCfg>(string type, string id) where TCfg : class, IFactoryConfiguration {
        return this._registries.TryGetValue(type, out var typeDict) && 
            typeDict.TryGetValue(id, out object value)
               ? value as TCfg
               : null;
    }
}