
using System.Collections.Generic;
using System.Linq;
using DevelopKit.BasicTemplate;
using UnityEngine;

namespace SESCO.InGame
{
    public static class InGameDataHelper
    {
        public static PartContainer InitPartContainer(IReadOnlyList<int> partIds)
        {
            var partContainer = AddressableUtil.Instantiate<PartContainer>("PartContainer");
            partContainer.Initialize(0);
            partContainer.Position = new Vector2Int(0, 0);
            for (int i = 0; i < partIds.Count; i++)
            {
                var id = partIds[i];
                var part = CreatePart(id);
                partContainer.AddPart(part, i); 
            }

            InGameDataContainer.Instance.PartContainer.Add(partContainer);
            InGameDataContainer.Instance.PartContainerMap.Add(partContainer.Position, partContainer);
            return partContainer;
        }
        
        public static PartContainer CreatePartContainer(PartContainer parent, PeerType peerType)
        {
            var partContainer = AddressableUtil.Instantiate<PartContainer>("PartContainer");
            partContainer.Initialize(0);
            partContainer.Position = parent.Position + parent.PeerOffsets[(int)peerType];
            parent.AddPeer(partContainer, peerType);
            
            InGameDataContainer.Instance.PartContainer.Add(partContainer);
            InGameDataContainer.Instance.PartContainerMap.Add(partContainer.Position, partContainer);
            return partContainer;
        }
        
        public static UnitBase CreateMonster(int unitId, Vector3 position = default, Quaternion quaternion = default)
        {
            var monster = UnitPool.Get(unitId);
            monster.Initialize(unitId);
            monster.transform.position = position;
            monster.transform.rotation = quaternion;
            InGameDataContainer.Instance.Monsters.Add(monster);
            return monster;
        }
        
        public static void DestroyUnit(UnitBase unit)
        {
            InGameDataContainer.Instance.Monsters.Remove(unit);
            UnitPool.Release(unit);
        }
        
        public static ProjectileBase CreateProjectile(int projectileId, int damage, Vector3 direction, Vector3 position = default, Quaternion quaternion = default)
        {
            var projectile = ProjectilePool.Get(projectileId);
            projectile.Initialize(projectileId, damage, direction);
            projectile.transform.position = position;
            projectile.transform.rotation = quaternion;
            InGameDataContainer.Instance.Projectiles.Add(projectile);
            return projectile;
        }
        
        public static void DestroyProjectile(ProjectileBase projectile)
        {
            InGameDataContainer.Instance.Projectiles.Remove(projectile);
            ProjectilePool.Release(projectile);
        }
        
        public static PartBase CreatePart(int partId, Vector3 position = default, Quaternion quaternion = default)
        {
            var part = PartPool.Get(partId);
            part.Initialize(partId);
            part.transform.position = position;
            part.transform.rotation = quaternion;
            InGameDataContainer.Instance.Parts.Add(part);
            return part;
        }
        
        public static void DestroyPart(PartBase part)
        {
            var allContainers = InGameDataContainer.Instance.PartContainer;
            foreach (var container in allContainers)
            {
                if (container.Parts.Contains(part))
                {
                    container.Parts.Remove(part);
                }
            }
            
            InGameDataContainer.Instance.Parts.Remove(part);
            PartPool.Release(part);
        }

        public static UnitBase FindMonster(Transform baseTransform)
        {
            foreach (var monster in InGameDataContainer.Instance.Monsters)
            {
                if (monster.transform == baseTransform)
                {
                    return monster;
                }
            }

            return null;
        }
    }
}