using UnityEngine;
using UnityEditor;
public class DataGen {

    [MenuItem("Tools/DataGen/GenTranslation")]
    public static void GenTranslation() {
        StorageHelper.SaveJson(StorageHelper.LANG_PATH + GameKeys.Translation.DEFAULT_LANG, GameKeys.TRANSLATION);
    }
}
