using System;

/// <summary>
/// 为了防止开发中在注册表中加入错误的注册类型，将可用的注册类型都罗列在这里
/// </summary>
public class RegisterHelper {
    public const string BLOCK = nameof(BLOCK);
    public const string ITEM = nameof(ITEM);

}