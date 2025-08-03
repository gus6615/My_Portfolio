using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SESCO.InGame;

namespace DevelopKit.BasicTemplate
{
    public class PartPool : Singleton<PartPool>, IStartUpTask
    {
        private Dictionary<int, GameObjectPool<PartBase>> _pool;
        
        public bool IsDone { get; set; }
        public StartUpPriority Priority => StartUpPriority.Default;
        public UniTask StartUp()
        {
            Instance._pool = new();
            return UniTask.CompletedTask;
        }
        
        public static PartBase Get(int partID)
        {
            if (!Instance._pool.TryGetValue(partID, out var pool))
            {
                Instance._pool[partID] = new();
                Instance._pool[partID].Bind($"Prefabs/Part/{partID}.prefab");
                pool = Instance._pool[partID];
            }
            return pool.Get();
        }
        
        public static void Release(PartBase obj)
        {
            Instance._pool[obj.ID].Release(obj);
        }

        public static void Reset()
        {
            Instance._pool.Clear();
        }
    }
}