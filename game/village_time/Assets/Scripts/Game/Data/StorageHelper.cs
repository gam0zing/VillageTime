using Newtonsoft.Json;
using System.Data;
using System.IO;
using UnityEngine;

/// <summary>
/// IO封装
/// </summary>
public static class StorageHelper {
    public static readonly string BASE_PATH = Path.GetDirectoryName(Application.dataPath);
    public static readonly string LANG_PATH = FullPath("Assets\\AutoAssets\\Lang\\");
    private static string FullPath(string path) {
        return Path.Combine(BASE_PATH, path);
    }
    public static void SaveJson(string path, object file) {
        string json = JsonConvert.SerializeObject(file, Formatting.Indented);
        File.WriteAllText(path + ".json", json);
    }

    public static T LoadJson<T>(string path) {
        return JsonConvert.DeserializeObject<T>(path);
    }
}