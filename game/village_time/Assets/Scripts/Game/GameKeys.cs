/// <summary>
/// 游戏文本常量，如翻译、默认名称
/// </summary>
public static class GameKeys {

    public static readonly Translations TRANSLATION;
    static GameKeys() {
        TRANSLATION = new Translations();
    }

    public class Translations {
        public static readonly string DEFAULT_LANG = "zh_cn";

        public Lang entity_resource_health_name = new(nameof(entity_resource_health_name), "生命值");

        public Lang item_empty_name = new(nameof(item_empty_name), "空物品");
        public Lang item_empty_description = new(nameof(item_empty_description), "Debug专用，你不会在正常游戏中看到它");

        public Lang block_empty_name = new(nameof(block_empty_name), "空方块");
        public Lang block_empty_description = new(nameof(block_empty_description), "Debug专用，你不会在正常游戏中看到它");
    }
}