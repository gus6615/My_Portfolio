
using System;
using Cysharp.Threading.Tasks;
using DevelopKit.BasicTemplate;
using SESCO.SO;
using UnityEngine;

namespace SESCO.InGame
{
    public abstract class PartBase : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        public PartDataSO Data;
        public int ID => Data.ID;
        public float RotateSpeed => Data.RotationSpeed;
        public bool IsActive;
        
        protected virtual void OnDestroy()
        {
            GameFlowManager.UpdateSpeedChanged -= OnUpdateSpeedChanged;
        }
        
        public virtual void Initialize(int id)
        {
            Data = ManagerHub.Data.GetPartDataSO(id);
            IsActive = true;
            
            GameFlowManager.UpdateSpeedChanged += OnUpdateSpeedChanged;
        }
        public virtual UnitBase FindTarget() => null;
        public virtual void LookSomething() { }
        public virtual async UniTask LookAtTarget(Vector3 targetPosition, Action onLookAtCallback) => await UniTask.Yield();

        public virtual void Attack(Vector3 targetPosition) { }
        
        public void OnEndAnimation()
        {
            animator.Play($"Part_{Data.ID}_Idle", 0, 0);
        }

        private void OnUpdateSpeedChanged(float speed)
        {
            animator.speed = speed;
        }
    }
}