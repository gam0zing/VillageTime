using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
public class DataGen {
    private static string DEFAULT_LANG = "zh_cn";

    private static string BASE_GEN_PATH = Path.GetDirectoryName(Application.dataPath);
    private static string LANG_PATH = DataGen.FullPath("Assets\\AutoAssets\\Lang");

    [MenuItem("Tools/DataGen/GenTranslation")]
    public static void GenTranslation() {
        FieldInfo[] fields = typeof(GameKeys.Translation).GetFields(BindingFlags.Static | BindingFlags.Public);
        Dictionary<string, string> map = new();
        foreach (FieldInfo field in fields) {
            map.Add(field.Name, field.GetValue(null) as string);
        }
        // JsonIO.SaveJson(map, DataGen.LANG_PATH, DataGen.DEFAULT_LANG);
    }

    private static string FullPath(string path) {
        return Path.Combine(DataGen.BASE_GEN_PATH, path);
    }
}
