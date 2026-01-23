/// <summary>
/// 游戏文本常量，如翻译、默认名称
/// </summary>
public static class GameKeys {

    public static readonly Translation TRANSLATION;
    static GameKeys() {
        TRANSLATION = new Translation();
    }

    public class Translation {
        public static readonly string DEFAULT_LANG = "zh_cn";
        public readonly string ENTITY_RESOURCE_HEALTH = "生命值";
    }
}