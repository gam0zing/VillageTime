using System;

public interface IModifier {
    float GetValue();
    void SetValue(float value);
    bool SetOnChangeCallback(Action action);
}