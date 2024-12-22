using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

[InitializeOnLoad]
public class AddressableAssetAdder
{
    private static string folderPath = AddressableAssetSettingsDefaultObject.kDefaultConfigFolder;
    private static string settingsName = AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName;

    static AddressableAssetAdder()
    {
        EditorApplication.delayCall -= AddressableAssetAdderHandler;
        EditorApplication.delayCall += AddressableAssetAdderHandler;
    }

    private static void AddressableAssetAdderHandler()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.Log($"New AddressableAssetSettings created.");
            settings = CreateAddressableAssetSettings();
            AddressableAssetSettingsDefaultObject.Settings = settings;
        }

        if (settings.FindGroup("DevelopKit_Basic_Template") == null)
        {
            string packageVersion = FindPackageVersion();
            string assetPath = $"Assets/Samples/DevelopKit Basic Template/{packageVersion}/Basic Template Reference/AddressableAssetsData/AssetGroups/DevelopKit_Basic_Template.asset";
            string groupName = "DevelopKit_Basic_Template";
            
            AddressableAssetGroup group = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(assetPath);
            settings.groups.Add(group);

            Debug.Log($"Asset '{assetPath}' added to Addressable Group '{groupName}'.");
        }
    }
    
    private static AddressableAssetSettings CreateAddressableAssetSettings()
    {
        AddressableAssetSettings settings = AddressableAssetSettings.Create(folderPath, settingsName, true, true);

        // 기본 그룹 생성
        if (settings.DefaultGroup == null)
        {
            var defaultGroup = settings.CreateGroup(
                "Default Local Group",
                true,
                false,
                false,
                null,
                typeof(BundledAssetGroupSchema),
                typeof(ContentUpdateGroupSchema)
            );
            settings.DefaultGroup = defaultGroup;

            // 기본 경로 설정
            BundledAssetGroupSchema bundleSchema = defaultGroup.GetSchema<BundledAssetGroupSchema>();
            if (bundleSchema != null)
            {
                bundleSchema.BuildPath.SetVariableByName(settings, "LocalBuildPath");
                bundleSchema.LoadPath.SetVariableByName(settings, "LocalLoadPath");
            }

            Debug.Log("Default Addressable Group created");
        }

        return settings;
    }

    private static string FindPackageVersion()
    {
        string packagePath = Application.dataPath.Replace("Assets", string.Empty) +
                             "/Library/PackageCache/com.developkit.basictemplate/package.json";
        string packageText = System.IO.File.ReadAllText(packagePath);
        string version = string.Empty;
        string[] lines = packageText.Split('\n');
        foreach (string line in lines)
        {
            if (line.Contains("\"version\""))
            {
                version = line.Split(':')[1].Trim().Replace("\"", string.Empty).Replace(",", string.Empty);
                break;
            }
        }
        return version;
    }
}