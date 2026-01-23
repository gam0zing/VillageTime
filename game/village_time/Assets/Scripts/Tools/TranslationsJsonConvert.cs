
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

public class TranslationsJsonConvert : JsonConverter<GameKeys.Translations> {

    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override GameKeys.Translations ReadJson(JsonReader reader, Type objectType, GameKeys.Translations existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (reader.TokenType == JsonToken.Null) return null;

        JObject json = JObject.Load(reader);
        GameKeys.Translations translations = new();

        FieldInfo[] fields = typeof(GameKeys.Translations).GetFields(
            BindingFlags.Public
            | BindingFlags.Instance
            | BindingFlags.DeclaredOnly
            );

        Dictionary<string, FieldInfo> fieldDic = new();
        foreach (FieldInfo field in fields) {
            if (field.FieldType == typeof(Lang)) {
                fieldDic[field.Name] = field;
            }
        }

        foreach (JProperty property in json.Properties()) {
            string key = property.Name;
            string value = property.Value?.Value<string>();

            if (fieldDic.TryGetValue(key, out FieldInfo field)) {
                field.SetValue(translations, new Lang(key, value ?? ""));
            }
        }

        return translations;
    }

    public override void WriteJson(JsonWriter writer, GameKeys.Translations translations, JsonSerializer serializer) {

        if (translations == null) {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        FieldInfo[] fields = typeof(GameKeys.Translations).GetFields(
            BindingFlags.Public
            | BindingFlags.Instance
            | BindingFlags.DeclaredOnly
            );

        foreach (FieldInfo field in fields) {
            Lang lang = (Lang)field.GetValue(translations);
            if (lang != null) {
                writer.WritePropertyName(lang.GetKey());
                writer.WriteValue(lang.GetKey());
            }
        }

        writer.WriteEndObject();
    }
}