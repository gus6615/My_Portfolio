using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SESCO.InGame;

namespace DevelopKit.BasicTemplate
{
    public class UnitPool : Singleton<UnitPool>, IStartUpTask
    {
        private Dictionary<int, GameObjectPool<UnitBase>> _pool;
        
        public bool IsDone { get; set; }
        public StartUpPriority Priority => StartUpPriority.Default;
        public UniTask StartUp()
        {
            Instance._pool = new();
            return UniTask.CompletedTask;
        }
        
        public static UnitBase Get(int unitID)
        {
            if (!Instance._pool.TryGetValue(unitID, out var pool))
            {
                Instance._pool[unitID] = new();
                Instance._pool[unitID].Bind($"Prefabs/Unit/{unitID}.prefab");
                pool = Instance._pool[unitID];
            }
            return pool.Get();
        }
        
        public static void Release(UnitBase obj)
        {
            Instance._pool[obj.ID].Release(obj);
        }
        public static void Reset()
        {
            Instance._pool.Clear();
        }
    }
}