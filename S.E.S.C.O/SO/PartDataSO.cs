using UnityEngine;

namespace SESCO.SO
{
    public enum RotateDirection
    {
        Left,
        Right
    }
    
    [CreateAssetMenu(fileName = "PartDataSO", menuName = "SESCO/SO/PartDataSO", order = 0)]
    public class PartDataSO : ScriptableObject
    {
        [SerializeField] private int id;
        public int ID => id;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        
        [SerializeField] private new string name;
        public string Name => name;

        [SerializeField] private string desc;
        public string Desc => desc;
        
        [SerializeField] private int projectileId;
        public int ProjectileId => projectileId;
        
        [SerializeField] private int capacity;
        public int Capacity => capacity;
        
        [SerializeField] private int damage;
        public int Damage => damage;
        
        [SerializeField] private RotateDirection rotateDirection;
        public RotateDirection RotateDirection => rotateDirection;
        
        [SerializeField] private int rotationSpeed;
        public int RotationSpeed => rotationSpeed;
        
        [SerializeField] private int bulletCount;
        public int BulletCount => bulletCount;
        
        [SerializeField] private float bulletAngle;
        public float BulletAngle => bulletAngle;
        
        [SerializeField] private float bulletInterval;
        public float BulletInterval => bulletInterval;
        
        [SerializeField] private float attackSpeed;
        public float AttackSpeed => attackSpeed;
        
        [SerializeField] private float attackRange;
        public float AttackRange => attackRange;
    }
}