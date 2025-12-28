using System.Collections.Generic;

public interface IModifyHandler<T> where T : IModifier {
    bool Add(T modifier);
    bool Remove(T modifier);
    float GetValue(float origin, out bool beContinue);
}