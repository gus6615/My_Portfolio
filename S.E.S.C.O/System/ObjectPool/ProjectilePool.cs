using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SESCO.InGame;

namespace DevelopKit.BasicTemplate
{
    public class ProjectilePool : Singleton<ProjectilePool>, IStartUpTask
    {
        private Dictionary<int, GameObjectPool<ProjectileBase>> _pool;
        
        public bool IsDone { get; set; }
        public StartUpPriority Priority => StartUpPriority.Default;
        public UniTask StartUp()
        {
            Instance._pool = new();
            return UniTask.CompletedTask;
        }
        
        public static ProjectileBase Get(int projectileID)
        {
            if (!Instance._pool.TryGetValue(projectileID, out var pool))
            {
                Instance._pool[projectileID] = new();
                Instance._pool[projectileID].Bind($"Prefabs/Projectile/{projectileID}.prefab");
                pool = Instance._pool[projectileID];
            }
            return pool.Get();
        }
        
        public static void Release(ProjectileBase obj)
        {
            Instance._pool[obj.ID].Release(obj);
        }
        
        public static void Reset()
        {
            Instance._pool.Clear();
        }
    }
}