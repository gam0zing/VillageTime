using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// 由于保存多语言数据的类仅有GameKeys.Translations一个，索性给它专门写一个转Json的工具类
/// 这个工具类强耦合于GameKeys.Translations，但又不属于游戏玩法部分，所以把它放在Core和Game之外，开一个Tools文件夹来存放
/// 后续所有类似的“实现依赖型”工具类都放在这个文件夹
/// </summary>
public static class TranslationsJsonHelper {

    private static JsonSerializerSettings _settings;
    public static JsonSerializerSettings Settings {
        get {
            return TranslationsJsonHelper._settings ??= new JsonSerializerSettings {
                Converters = new List<JsonConverter> { new TranslationsJsonConvert() },
                Formatting = Formatting.Indented,
            };
        }
    }

    public static string Serialize(GameKeys.Translations translations) {
        return JsonConvert.SerializeObject(translations, Settings);
    }

    public static GameKeys.Translations Deserialize(string json) {
        return JsonConvert.DeserializeObject<GameKeys.Translations>(json, Settings);
    }
}