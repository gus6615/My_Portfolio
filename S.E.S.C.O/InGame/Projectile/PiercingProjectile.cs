using System.Collections.Generic;
using _Project.Scripts.System.Grid_System;
using UnityEngine;

namespace SESCO.InGame
{
    public sealed class PiercingProjectile : ProjectileBase
    {
        public BreakTileEvent breakTileEvent;
        private readonly List<UnitBase> monsters = new();

        public override void Initialize(int id, int damage, Vector3 direction)
        {
            base.Initialize(id, damage, direction);
            
            monsters.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var monster = InGameDataHelper.FindMonster(other.transform);
            monsters.Add(monster);
            
            if (monster)
            {
                monster.AddStoppingForce(Direction.normalized);
                monster.TakeDamage(Damage);
                if (!string.IsNullOrEmpty(Data.VfxID))
                    InGameVFXManager.Instance.CreateVFX(Data.VfxID, transform.position);
            }
            else
            {
                if(other.gameObject.layer == LayerMask.NameToLayer("Grid"))
                {
                    breakTileEvent.InvokeBreakTileInRadius(transform.position, Damage, GridUtils.CellSize);
                    if (!string.IsNullOrEmpty(Data.VfxID))
                        InGameVFXManager.Instance.CreateVFX(Data.VfxID, transform.position);
                }   
            }
        }

        public void TakeDamageToTargets()
        {
            foreach (var monster in monsters)
            {
                if (monster && monster.IsDead)
                    continue;
                
                monster?.TakeDamage(Damage);
            }
        }
    }
}