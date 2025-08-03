
using DevelopKit.BasicTemplate;
using DevelopKit.BasicTemplate.GPU_Particle_System;
using SESCO.SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace SESCO.InGame
{
    public abstract class UnitBase : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected Rigidbody2D rigid;
        [SerializeField] protected new Collider2D collider;
        [FormerlySerializedAs("emitData")] [SerializeField] private GpuParticleEmitDataSo emitDataSo;
        [SerializeField] private int gpuEmitIndex = 1;
        protected UnitDataSO Data;
        protected int CurHp;
        
        public int ID => Data.ID;
        public float MoveSpeed => Data.MoveSpeed;
        public int MaxHp => Data.HP;
        public bool IsDead => CurHp <= 0;
        
        public virtual void Initialize(int id)
        {
            Data = ManagerHub.Data.GetUnitDataSO(id);
            CurHp = MaxHp;
            collider.enabled = true;
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }

        public virtual UnitBase FindTarget() => null;

        public virtual void Move(Vector3 moveVec)
        {
            this.transform.rotation = MathUtil.GetQuaternion(moveVec);
            rigid.MovePosition(this.transform.position + moveVec);
        }
        public virtual void Attack(UnitBase target) { }

        public virtual void TakeDamage(int damage)
        {
            CurHp -= damage;
            if (IsDead)
            {
                Die();
            }
        }
        
        public virtual void AddStoppingForce(Vector3 force)
        {
            rigid.AddForce(force, ForceMode2D.Impulse);
        }

        public virtual void Die()
        {
            collider.enabled = false;
            rigid.bodyType = RigidbodyType2D.Static;
            emitDataSo.AddEmitParticleInfo(transform.position, gpuEmitIndex);
            InGameDataHelper.DestroyUnit(this);
        }
    }
}