
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SESCO.InGame
{
    public static class InGameCombatHelper
    {
        public static void ExecutePartsCombat(float dt)
        {
            var parts = InGameDataContainer.Instance.Parts;
            foreach (var part in parts)
            {
                if (!part.IsActive)
                    continue;
                
                part.LookSomething();
                
                var target = part.FindTarget();
                if (!target) continue;
                
                part.Attack(target.transform.position);
            }
        }

        public static void ExecuteMonsterCombat(float dt)
        {
            var monsters = InGameDataContainer.Instance.Monsters;
            foreach (var monster in monsters)
            {
                if (monster.IsDead)
                    continue;
                
                var target = monster.FindTarget();
                if (!target) continue;
                
                var moveVec = target.transform.position - monster.transform.position;
                Debug.DrawLine(monster.transform.position, target.transform.position, Color.red);
                monster.Move(moveVec.normalized * (monster.MoveSpeed * dt));
                monster.Attack(target);
            }
        }
        
        public static void ExecutePartsInput(Vector2 inputPosition)
        {
            var parts = InGameDataContainer.Instance.Parts;
            foreach (var part in parts)
            {
                if (!part.IsActive)
                    continue;
                
                part.LookAtTarget(inputPosition, () =>
                {
                    part.Attack(inputPosition);
                }).Forget();
            }
        }
    }
}