using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件中心，所有注册的事件ID都将永久保存
/// </summary>
public static class EventCenter {
    private static ConcurrentDictionary<string, Action<object>> _events;

    static EventCenter() {
        _events = new ConcurrentDictionary<string, Action<object>>();
        foreach (var id in EventIds.GetEventIds()) {
            _events[id] = null;
        }
    }

    public static void On(string id, Action<object> action) {
        action.ThrowIfNull(nameof(action));

        _events.AddOrUpdate(
            id,
            action,
            (_, existing) => SafeCombine(existing, action)
        );
    }

    public static void Off(string id, Action<object> action) {
        if (action == null) return;

        _events.AddOrUpdate(
            id,
            null,
            (_, existing) => SafeRemove(existing, action)
        );
    }

    public static void Emit(string id, object args) {
        if (_events.TryGetValue(id, out var action)) {
            action?.Invoke(args);
        }
    }

    /// <summary>
    /// 安全组合委托
    /// </summary>
    private static Action<object> SafeCombine(Action<object> existing, Action<object> newAction) {
        try {
            return existing + newAction;
        } catch (Exception ex) {
            Debug.LogError($"组合委托失败: {ex.Message}");
            return existing;  // 失败时返回原委托
        }
    }

    /// <summary>
    /// 安全移除委托
    /// </summary>
    private static Action<object> SafeRemove(Action<object> existing, Action<object> action) {
        try {
            var result = existing - action;
            return result;  // 可能为null
        } catch (Exception ex) {
            Debug.LogError($"移除委托失败: {ex.Message}");
            return existing;
        }
    }

    /// <summary>
    /// 参数验证扩展
    /// </summary>
    public static T ThrowIfNull<T>(this T obj, string paramName) where T : class {
        if (obj == null)
            throw new ArgumentNullException(paramName);
        return obj;
    }
}