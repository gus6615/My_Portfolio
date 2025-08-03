
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DevelopKit.BasicTemplate;
using UnityEngine;

namespace SESCO.InGame
{
    public sealed class AimedProjectilePart : PartBase
    {
        [SerializeField] private Transform rotateTr;
        [SerializeField] private GameObject attackPoint;
        
        private CancellationTokenSource rotateCTS;
        private bool isAttackDelay;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            rotateCTS?.Cancel();
        }

        public override void Initialize(int id)
        {
            base.Initialize(id);

            isAttackDelay = false;
        }

        public override UnitBase FindTarget()
        {
            var monsters = InGameDataContainer.Instance.Monsters;
            var nearestDistance = float.MaxValue;
            UnitBase nearestMonster = null;
            
            foreach (var monster in monsters)
            {
                if (monster.IsDead)
                    continue;
                
                if (nearestMonster == null)
                    nearestMonster = monster;
                
                var distance = Vector3.Distance(this.transform.position, monster.transform.position);
                if (distance < nearestDistance && distance <= Data.AttackRange)
                {
                    nearestDistance = distance;
                    nearestMonster = monster;
                }
            }

            return nearestMonster;
        }

        public override void LookSomething()
        {
            var target = FindTarget();
            if (target)
            {
                LookAtTarget(target.transform.position).Forget();
            }
        }

        public override void Attack(Vector3 targetPosition)
        {
            base.Attack(targetPosition);
            
            if (isAttackDelay)
                return;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName($"Part_{Data.ID}_Attack"))
                return;
            
            animator.Play($"Part_{Data.ID}_Attack", 0, 0);
            
            isAttackDelay = true;
            AttackAsync(targetPosition).Forget();
            AttackDelayAsync().Forget();
        }

        public override async UniTask LookAtTarget(Vector3 targetPosition, Action onLookAtCallback)
        {
            await LookAtTarget(targetPosition);
            onLookAtCallback?.Invoke();
        }

        private async UniTask LookAtTarget(Vector2 targetPosition)
        {
            var currentDirection = MathUtil.GetDirectionFromQuaternion(rotateTr.rotation).normalized;
            var goalDirection = (targetPosition - (Vector2)attackPoint.transform.position).normalized;
            rotateCTS?.Cancel();
            rotateCTS = new CancellationTokenSource();
            
            while (Vector2.Distance(currentDirection, goalDirection) > 0.1f)
            {
                var dt = GameFlowManager.Instance.GetDeltaTime();
                currentDirection = Vector2.Lerp(currentDirection, goalDirection, RotateSpeed * dt);
                rotateTr.rotation = MathUtil.GetQuaternion(currentDirection);
                await UniTask.Yield(rotateCTS.Token);
            }
            
            rotateTr.rotation = MathUtil.GetQuaternion(goalDirection);
        }
        
        private async UniTask AttackAsync(Vector3 targetPosition)
        {
            var bulletCount = Data.BulletCount;
            while (bulletCount > 0)
            {
                bulletCount--;
                
                var defaultDirection = MathUtil.GetDirectionFromQuaternion(rotateTr.rotation).normalized;
                var randomDirection = MathUtil.GetRandomDirection(defaultDirection, Data.BulletAngle).normalized;
                var quaternion = MathUtil.GetQuaternion(randomDirection);
                var projectile = InGameDataHelper.CreateProjectile(Data.ProjectileId, Data.Damage, randomDirection, attackPoint.transform.position, quaternion);
                
                // 예외 처리
                if (ID == 15)
                {
                    projectile.transform.SetParent(attackPoint.transform);
                }
                
                await UniTask.Delay((int)(1000 * Data.BulletInterval));
            }
        }
        
        private async UniTask AttackDelayAsync()
        {
            await UniTask.Delay((int)(1000 * 1f / Data.AttackSpeed));
            isAttackDelay = false;
        }
    }
}   