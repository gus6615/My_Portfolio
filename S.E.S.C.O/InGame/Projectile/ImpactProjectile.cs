using _Project.Scripts.System.Grid_System;
using UnityEngine;

namespace SESCO.InGame
{
    public sealed class ImpactProjectile : ProjectileBase
    {
        public BreakTileEvent breakTileEvent;
        public override void FixedManagedUpdate(float dt)
        {
            this.transform.position += Direction * (Data.MoveSpeed * dt);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var monster = InGameDataHelper.FindMonster(other.transform);
            
            if (monster && HitCount > 0)
            {
                monster.AddStoppingForce(Direction.normalized);
                monster.TakeDamage(Damage);
                HitCount--;
                
                if (!string.IsNullOrEmpty(Data.VfxID))
                    InGameVFXManager.Instance.CreateVFX(Data.VfxID, transform.position);
            }
            else
            {
                if(other.gameObject.layer == LayerMask.NameToLayer("Grid") && HitCount > 0)
                {
                    breakTileEvent.InvokeBreakTileInRadius(transform.position, Damage, GridUtils.CellSize);
                    HitCount--;
                    
                    if (!string.IsNullOrEmpty(Data.VfxID))
                        InGameVFXManager.Instance.CreateVFX(Data.VfxID, transform.position);
                }   
            }
            
            if (HitCount <= 0)
            {
                Release();
            }
        }

    }
}