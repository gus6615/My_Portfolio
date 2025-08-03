
using _Project.Scripts.System.Grid_System;
using DevelopKit.BasicTemplate;
using Development.Nakwon.Juice_System.Shake_System.Runtime;
using UnityEngine;

namespace SESCO.InGame
{
    public sealed class ExplosiveProjectile : ProjectileBase
    {
        public BreakTileEvent breakTileEvent;
        public ShakePreset shakePreset;
        public override void FixedManagedUpdate(float dt)
        {
            if (!IsEnable)
                return;
                
            this.transform.position += Direction * (Data.MoveSpeed * dt);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var monster = InGameDataHelper.FindMonster(other.transform);
            if (monster)
            {
                if (!string.IsNullOrEmpty(Data.VfxID))
                    InGameVFXManager.Instance.CreateVFX(Data.VfxID, transform.position);
                var hits = Physics2D.CircleCastAll(transform.position, Data.ExplosionRange, Vector2.zero);
                foreach (var hit in hits)
                {
                    var target = InGameDataHelper.FindMonster(hit.transform);
                    if (target)
                    {
                        target.AddStoppingForce(Direction.normalized);
                        target.TakeDamage(Damage);
                    }
                }
                shakePreset?.generate_shake_from_source(transform);
                Release();
            }
            else
            {
                if(other.gameObject.layer == LayerMask.NameToLayer("Grid"))
                {
                    if (!string.IsNullOrEmpty(Data.VfxID))
                        InGameVFXManager.Instance.CreateVFX(Data.VfxID, transform.position);
                    breakTileEvent.InvokeBreakTileInRadius(transform.position, Damage, Data.ExplosionRange);
                    shakePreset?.generate_shake_from_source(transform);
                    Release();
                }
            }
        }
    }
}