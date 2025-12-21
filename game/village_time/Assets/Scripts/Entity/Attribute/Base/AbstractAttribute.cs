
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAttribute : MonoBehaviour, IAttribute {
    private List<IModifyHandler<IModifier>> _handlers;
    public float Base { get; set; } = 0F;

    public float Value => this._value;
    private float _value = 0;

    public IModifyHandler<IModifier> HardLockHandler = new HardLockHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> BaseLockHandler = new SoftLockHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> BaseAddHandler = new AddHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> BaseMultiplyHandler = new MultiplyHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> NormalAddHandler = new AddHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> NormalMultiplyHandler = new MultiplyHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> ExtraAddHandler = new AddHandler() as IModifyHandler<IModifier>;
    public IModifyHandler<IModifier> FinalLockHandler = new SoftLockHandler() as IModifyHandler<IModifier>;

    private void Awake() {
        this.Init();
    }

    private void Init() {
        this._handlers = new() {
            this.HardLockHandler,
            this.BaseLockHandler,
            this.BaseAddHandler,
            this.BaseMultiplyHandler,
            this.NormalAddHandler,
            this.NormalMultiplyHandler,
            this.ExtraAddHandler,
            this.FinalLockHandler,
        };
    }

    public bool AddModifier(IModifier modifier, IModifyHandler<IModifier> handler) {
        if (modifier == null) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（传入的修饰器为空）");
            return false;
        }
        if (handler == null) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（传入的修饰处理器为空）");
            return false;
        }
        if (!handler.GetType().GetGenericArguments()[0].IsAssignableFrom(modifier.GetType())) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（修饰器和修饰处理器类型不兼容）");
            return false;
        }
        if (!this._handlers.Contains(handler)) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（指定的修饰处理器不存在）");
            return false;
        }

        modifier.SetOnChangeCallback(this.Refresh);
        bool ret = handler.Add(modifier);
        if (ret) {
            this.Refresh();
        }

        return ret;
    }
    public bool RemoveModifier(IModifier modifier, IModifyHandler<IModifier> handler) {
        if (modifier == null) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（传入的修饰器为空）");
            return false;
        }
        if (handler == null) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（传入的修饰处理器为空）");
            return false;
        }
        if (!handler.GetType().GetGenericArguments()[0].IsAssignableFrom(modifier.GetType())) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（修饰器和修饰处理器类型不兼容）");
            return false;
        }
        if (!this._handlers.Contains(handler)) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（指定的修饰处理器不存在）");
            return false;
        }

        modifier.SetOnChangeCallback(null);
        bool ret = handler.Remove(modifier);
        if (ret) {
            this.Refresh();
        }

        return ret;
    }

    /// <summary>
    /// 刷新属性值
    /// —— 重新计算所有修饰器对属性的影响
    /// </summary>
    public void Refresh() {
        float newValue = this.Base;
        foreach (IModifyHandler<IModifier> modifier in this._handlers) {
            newValue = modifier.GetValue(newValue, out bool canModify);
            if (!canModify) break;
        }
        this._value = newValue;
    }
}