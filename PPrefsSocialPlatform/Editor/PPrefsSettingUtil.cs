using UnityEngine;
using UnityEditor;
using System.IO;

public static class PPrefsSettingUtil
{
    public static string defaultAssetPath = Application.dataPath + "/Resources/";
    public static string defaultAssetName = "LocalSocialSetting.asset";

    [MenuItem ("Assets/Create/make LocalSocialSetting.asset")]
    static void CreatePPrefsSetting()
    {
	    string path = defaultAssetPath;
		if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
		}

        path += defaultAssetName;

        if (File.Exists(path)){
			EditorUtility.DisplayDialog("Fatal error", path.Substring(Application.dataPath.Length - 6)+ " already exists", "Ok");
			return;
        }

        path = path.Substring(Application.dataPath.Length - 6);
		PPrefsSetting setting = ScriptableObject.CreateInstance("PPrefsSetting") as PPrefsSetting;
        setting.achievements = new PPrefsSetting.SeAchivementDescription[0];
		AssetDatabase.CreateAsset(setting, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

        //select to new one
		Object[] selection = new Object[1];
		selection[0] = setting;
		Selection.objects = selection;
    }
}
