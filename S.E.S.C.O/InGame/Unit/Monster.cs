
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SESCO.InGame
{
    public sealed class Monster : UnitBase
    {
        private bool isAttackDelay;

        public override void Initialize(int id)
        {
            base.Initialize(id);

            isAttackDelay = false;
        }

        public override UnitBase FindTarget()
        {
            var playerSlots = InGameDataContainer.Instance.PartContainer;
            var nearestDistance = float.MaxValue;
            UnitBase nearestPlayerSlot = null;
            
            foreach (var slot in playerSlots)
            {
                if (slot.IsDead) continue;
                
                if (nearestPlayerSlot == null)
                    nearestPlayerSlot = slot;
                
                var distance = Vector3.Distance(this.transform.position, slot.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayerSlot = slot;
                }
            }

            return nearestPlayerSlot;
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            
            animator.Play($"Monster_{Data.ID}_Hit", 0, 0);
        }

        public override void Attack(UnitBase target)
        {
            if (isAttackDelay)
                return;
            
            var distance = Vector3.Distance(target.transform.position, this.transform.position);
            if (distance <= Data.AttackRange)
            {
                target.TakeDamage(Data.Damage);
                isAttackDelay = true;
                AttackDelayAsync().Forget();
            }
        }

        public override void Die()
        {
            base.Die();
            
            InGameDataContainer.Instance.Exp.Value += Data.Exp;
        }
        
        private async UniTask AttackDelayAsync()
        {
            await UniTask.Delay((int)(1000 * 1f / Data.AttackSpeed));
            isAttackDelay = false;
        }
    }
}