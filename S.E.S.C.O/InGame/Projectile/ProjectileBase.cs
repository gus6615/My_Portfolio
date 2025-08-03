
using System;
using DevelopKit.BasicTemplate;
using SESCO.SO;
using UnityEngine;

namespace SESCO.InGame
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        protected ProjectileDataSO Data;
        protected Vector3 Direction;
        protected int Damage;
        protected bool IsEnable;
        protected int HitCount;
        public int ID => Data.ID;

        public virtual void Initialize(int id, int damage, Vector3 direction)
        {
            Data = ManagerHub.Data.GetProjectileDataSO(id);
            Direction = direction;
            Damage = damage;
            IsEnable = true;
            HitCount = Data.HitCount;
            
            Invoke(nameof(Release), Data.DestroyTime);
            
            GameFlowManager.Instance.AddUpdateHandler(ManagedUpdate);
            GameFlowManager.Instance.AddFixedUpdateHandler(FixedManagedUpdate);
        }
        
        public virtual void ManagedUpdate(float dt) { }
        public virtual void FixedManagedUpdate(float dt) { }

        public virtual void Release()
        {
            IsEnable = false;
            
            GameFlowManager.Instance?.RemoveUpdateHandler(ManagedUpdate);
            GameFlowManager.Instance?.RemoveFixedUpdateHandler(FixedManagedUpdate);
            InGameDataHelper.DestroyProjectile(this);
        }
    }
}