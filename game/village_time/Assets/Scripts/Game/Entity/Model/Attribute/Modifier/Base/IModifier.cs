using System;

public interface IModifier {
    float GetValue();
    bool SetOnChangeCallback(Action action);
}