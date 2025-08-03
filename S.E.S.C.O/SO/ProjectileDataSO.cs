using UnityEngine;

namespace SESCO.SO
{
    [CreateAssetMenu(fileName = "ProjectileDataSO", menuName = "SESCO/SO/ProjectileDataSO", order = 0)]
    public class ProjectileDataSO : ScriptableObject
    {
        [SerializeField] private int id;
        public int ID => id;
        
        [SerializeField] private string vfxID;
        public string VfxID => vfxID;

        [SerializeField] private float moveSpeed;
        public float MoveSpeed => moveSpeed;
        
        [SerializeField] private float destroyTime;
        public float DestroyTime => destroyTime;
        
        [SerializeField] private float explosionRange;
        public float ExplosionRange => explosionRange;
        
        [SerializeField] private int hitCount;
        public int HitCount => hitCount;
    }
}