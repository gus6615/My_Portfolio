using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace DevelopKit.BasicTemplate
{
    struct SceneInfo
    {
        internal string Name;
        internal string Path;
        internal int Index;
    }
    
    internal class SceneHelper
    {
        private static readonly List<string> Scenes = new();
        private static readonly List<SceneInfo> SceneInfos = new();
        private static readonly GUIContent GUIContentPopup = new ();
        
        private static int sceneCount;
        private static int selectSceneIndex;
        private static string scenePath;

        [InitializeOnLoadMethod]
        private static void InitializeSceneGUI()
        {
            ToolbarExtender.OnLeftToolbarGUI.Clear();
            ToolbarExtender.OnLeftToolbarGUI.Add(OnToolbarGUIScene);

            sceneCount = EditorBuildSettings.scenes.Length;

            EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;
            SceneInfos.Clear();
            Scenes.Clear();
            for (int i = 0; i < sceneCount; i++)
            {
                SceneInfos.Add(new SceneInfo()
                {
                    Path = buildSettingsScenes[i].path,
                    Name = Path.GetFileNameWithoutExtension(buildSettingsScenes[i].path),
                    Index = i,
                });

                Scenes.Add(SceneInfos[i].Name);
            }

            SetSelectIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;

            EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;

            EditorBuildSettings.sceneListChanged -= InitializeSceneGUI;
            EditorBuildSettings.sceneListChanged += InitializeSceneGUI;
        }

        private static void OnSceneChanged(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
        {
            SetSelectIndex(next.name);

            if (string.IsNullOrEmpty(current.name))
            {
                return;
            }

            SelectPopup(next.name);
        }

        private static void SetSelectIndex(string activeSceneName)
        {
            for (int i = 0; i < sceneCount; i++)
            {
                if (SceneInfos[i].Name.Equals(activeSceneName))
                {
                    selectSceneIndex = SceneInfos[i].Index;
                    return;
                }
            }

            selectSceneIndex = -1;
        }

        private static void OnToolbarGUIScene()
        {
            if (SceneInfos.Count == 0 || EditorApplication.isPlaying)
            {
                return;
            }

            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();

            SelectPopup(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            if (EditorGUI.EndChangeCheck())
            {
                var sceneInfo = SceneInfos[selectSceneIndex];
                if (File.Exists(Path.Combine(Path.GetFullPath("."), sceneInfo.Path)) == false)
                {
                    EditorUtility.DisplayDialog("에러", $"{sceneInfo.Name}은 없는 Scene입니다. Build Settings에서 해당 Scene을 지워주세요.", "확인");
                    SetSelectIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                    return;
                }

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(sceneInfo.Path, OpenSceneMode.Single);
                }
                else
                {
                    SetSelectIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
            }

            if(GUILayout.Button(new GUIContent($"▶ {SceneInfos[0].Name}", "게임의 첫 씬을 플레이합니다"), EditorStyles.toolbarButton))
            {
                StartScene(SceneInfos[0].Path);
            }
        }

        private static void SelectPopup(string sceneName)
        {
            GUIContentPopup.text = sceneName;
            
            float activeWidth = GUI.skin.label.CalcSize(GUIContentPopup).x + 20;
            selectSceneIndex = EditorGUILayout.Popup(selectSceneIndex, Scenes.ToArray(), GUILayout.Width(activeWidth));
        }

        private static void StartScene(string path)
        {
            if(EditorApplication.isPlaying)
            {
                Debug.Log("플레이모드중 사용 불가");
                return;
            }

            scenePath = path;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (EditorApplication.isPlaying || 
                EditorApplication.isPaused || 
                EditorApplication.isCompiling || 
                EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                SceneHelperSetting.instance.OpenedScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                //프리팹 모드였다면 프리팹경로 저장
                PrefabStage openedPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (openedPrefabStage != null)
                {
                    SceneHelperSetting.instance.OpenedPrefabPath = openedPrefabStage.assetPath;
                }

                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange mode)
        {
            if (mode == PlayModeStateChange.EnteredEditMode)
            {
                if (string.IsNullOrEmpty(SceneHelperSetting.instance.OpenedScenePath) == false)
                {
                    EditorSceneManager.OpenScene(SceneHelperSetting.instance.OpenedScenePath);
                    SceneHelperSetting.instance.OpenedScenePath = string.Empty;
                }

                //프리팹 모드였다면 프리팹 open
                if (string.IsNullOrEmpty(SceneHelperSetting.instance.OpenedPrefabPath) == false)
                {
                    PrefabStageUtility.OpenPrefab(SceneHelperSetting.instance.OpenedPrefabPath);
                    SceneHelperSetting.instance.OpenedPrefabPath = string.Empty;
                }
            }
        }
    }
}
