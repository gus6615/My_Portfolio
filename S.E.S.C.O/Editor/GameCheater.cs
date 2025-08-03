using SESCO.InGame;
using UnityEditor;
using UnityEngine;

namespace SESCO
{
    [InitializeOnLoad]
    public class GameCheater : EditorWindow
    {
        private static GameCheater _window;
        
        [MenuItem("Window/Game Cheater")]
        public static void ShowWindow()
        {
            _window = GetWindow<GameCheater>("Game Cheater");
            _window.Show();
        }

        static GameCheater()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    ShowWindow();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    _window.Close();
                    break;
            }
        }
        
        private static int partId;

        private void OnGUI()
        {
            GUILayout.Label("[ Game Cheater ]", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("모든 파츠 삭제"))
            {
                var partcontainers = InGameDataContainer.Instance.PartContainer;
                foreach (var container in partcontainers)
                {
                    var parts = container.Parts;
                    for (int i = parts.Count - 1; i >= 0; i--)
                    {
                       InGameDataHelper.DestroyPart(parts[i]);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginVertical();
            partId = EditorGUILayout.IntField("파츠 ID 생성:", partId);
            if (GUILayout.Button("생성"))
            {
                var partcontainers = InGameDataContainer.Instance.PartContainer;
                foreach (var container in partcontainers)
                {
                    if (container.IsFullSlot)
                        continue;
                    container.AddPart(InGameDataHelper.CreatePart(partId), container.Parts.Count);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}