using System.Collections.Generic;
using UnityEngine;
#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
#if UNITASK_INSTALLED
    public class SceneLoader : MonoBehaviour
    {
        public static string CurrentSceneName;
        public static string NextSceneName;
        public static object NextSceneData;
        public static SceneTransitionFX_Base Transition;
        public static SceneProgress_Base Progress;
        
        public delegate UniTask SceneLoadedStartTask(string prevScene, string nextScene);

        private static readonly List<SceneLoadedStartTask> LoadSceneStartTasks = new ();

        public static event SceneLoadedStartTask OnStartChangeScene
        {
            add => LoadSceneStartTasks.Add(value);
            remove => LoadSceneStartTasks.Remove(value);
        }
        
        private async void Start()
        {
            await UniTask.Yield();
            await HandleLoadSceneStartTasks();
#if ADDRESSABLES_INSTALLED
            var operationWrapper = ManagerHub.Scene.ChangeScene(NextSceneName, NextSceneData, Transition);
            operationWrapper.Completed += OnSceneLoaded;
            Progress.Initialize(operationWrapper);
#endif
        }
        
        public static async UniTask HandleLoadSceneStartTasks() 
            => await UniTask.WhenAll(LoadSceneStartTasks.Select(x => x.Invoke(CurrentSceneName, NextSceneName))); 
        
        private void OnSceneLoaded()
        {
            ClearData();
        }
        
        private void ClearData()
        {
            CurrentSceneName = null;
            NextSceneName = null;
            NextSceneData = null;
            Transition = null;
        }
    }
#endif
}