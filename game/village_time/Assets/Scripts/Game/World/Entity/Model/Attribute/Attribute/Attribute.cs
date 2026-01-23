
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attribute : IAttribute {
    private List<IModifyHandler<IModifier>> _handlers;
    public float Base { get; set; } = 0F;
    public float Value => this._value;
    private float _value = 0;

    public readonly IModifyHandler<IModifier> HardLockHandler = new HardLockHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> BaseLockHandler = new SoftLockHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> BaseAddHandler = new AddHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> BaseMultiplyHandler = new MultiplyHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> NormalAddHandler = new AddHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> NormalMultiplyHandler = new MultiplyHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> ExtraAddHandler = new AddHandler() as IModifyHandler<IModifier>;
    public readonly IModifyHandler<IModifier> FinalLockHandler = new SoftLockHandler() as IModifyHandler<IModifier>;

    public Attribute() {
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
        this._handlers.ForEach(e => e.SetOnChangeCallback(this.Refresh));
    }

    public bool AddModifier<T>(T modifier, IModifyHandler<T> handler) where T : IModifier {
        if (modifier == null) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（传入的修饰器为空）");
            return false;
        }
        if (handler == null) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（传入的修饰处理器为空）");
            return false;
        }
        if (!this._handlers.Contains(handler as IModifyHandler<IModifier>)) {
            Debug.LogWarning("属性系统：添加属性修饰器失败（指定的修饰处理器不存在）");
            return false;
        }

        bool ret = handler.Add(modifier);

        return ret;
    }
    public bool RemoveModifier<T>(T modifier, IModifyHandler<T> handler) where T : IModifier {
        if (modifier == null) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（传入的修饰器为空）");
            return false;
        }
        if (handler == null) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（传入的修饰处理器为空）");
            return false;
        }
        if (!this._handlers.Contains(handler as IModifyHandler<IModifier>)) {
            Debug.LogWarning("属性系统：移除属性修饰器失败（指定的修饰处理器不存在）");
            return false;
        }

        bool ret = handler.Remove(modifier);

        return ret;
    }

    /// <summary>
    /// 刷新属性值
    /// —— 重新计算所有修饰器对属性的影响
    /// </summary>
    public void Refresh() {
        float newValue = this.Base;
        foreach (IModifyHandler<IModifier> handler in this._handlers) {
            newValue = handler.GetValue(newValue, out bool canModify);
            if (!canModify) break;
        }
        this._value = newValue;
    }
}