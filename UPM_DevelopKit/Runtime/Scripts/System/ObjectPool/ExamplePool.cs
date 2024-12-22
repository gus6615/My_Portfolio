#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
    public class ExamplePool : Singleton<ExamplePool>, IStartUpTask
    {
        private readonly string _prefabKey = "ExampleGO";
        private GameObjectPool<ExampleGameObject> _pool;
        
        public bool IsDone { get; set; }
        public StartUpPriority Priority => StartUpPriority.Default;
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
        public UniTask StartUp()
        {
            Instance._pool = new GameObjectPool<ExampleGameObject>();
            Instance._pool.Bind(_prefabKey, ManagerHub.UI.MainCanvas.transform);
            SceneManager.OnSceneLoadedDone += ReBind;
            return UniTask.CompletedTask;
        }
#endif
        
        public static ExampleGameObject Get()
        {
            return Instance._pool.Get();
        }
        
        public static void Release(ExampleGameObject obj)
        {
            Instance._pool.Release(obj);
        }

#if UNITASK_INSTALLED
        private async UniTask ReBind(object nextSceneData)
        {
            await UniTask.Yield();
 
            Instance._pool.Bind(_prefabKey, ManagerHub.UI.MainCanvas.transform);
        }
#endif
    }
}