#if NET5_0_OR_GREATER
#else
namespace System.Runtime.CompilerServices {
    /// <summary>
    /// Unity2022环境下无法使用记录类的形参属性功能，需要添加这个类来通过编译器检查
    /// </summary>
    internal static class IsExternalInit {
        
    }
}
#endif