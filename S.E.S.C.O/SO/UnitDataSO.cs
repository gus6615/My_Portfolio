using UnityEngine;

namespace SESCO.SO
{
    [CreateAssetMenu(fileName = "UnitDataSO", menuName = "SESCO/SO/UnitDataSO", order = 0)]
    public class UnitDataSO : ScriptableObject
    {
        [SerializeField] private int id;
        public int ID => id;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private new string name;
        public string Name => name;

        [SerializeField] private string desc;
        public string Desc => desc;

        [SerializeField] private int exp;
        public int Exp => exp;

        [SerializeField] private int hp;
        public int HP => hp;
        
        [SerializeField] private int damage;
        public int Damage => damage;
        
        [SerializeField] private float moveSpeed;
        public float MoveSpeed => moveSpeed;
        
        [SerializeField] private float attackSpeed;
        public float AttackSpeed => attackSpeed;
        
        [SerializeField] private float attackRange;
        public float AttackRange => attackRange;
    }
}