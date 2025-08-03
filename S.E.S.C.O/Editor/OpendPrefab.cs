using UnityEditor;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    [FilePath("DevelopKit/SceneHelper.asset", FilePathAttribute.Location.PreferencesFolder)]
    internal class SceneHelperSetting : ScriptableSingleton<SceneHelperSetting>
    {
        internal string OpenedScenePath;
        internal string OpenedPrefabPath;
        internal GameObject Prefab;
    }
}
