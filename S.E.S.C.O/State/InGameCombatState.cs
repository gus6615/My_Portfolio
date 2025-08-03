using Cysharp.Threading.Tasks;
using SESCO.InGame;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class InGameCombatState : StateBase
    {
        private bool OnHandleInput;
        
        public override void OnEnter()
        {
            Debug.Log("InGameCombatState OnEnter");

            CreateMonster().Forget();
        }

        public override void OnUpdate(float dt)
        {
            if (CheckGameEnd())
            {
                GameFlowManager.Instance.PushState<InGameEndState>();
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (!OnHandleInput)
                {
                    OnHandleInput = true;
                    DisableHandleInput().Forget();
                }
                
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var slots = InGameDataContainer.Instance.PartContainer;

                foreach (var slot in slots)
                {
                    var force = (Vector2)(mousePosition - slot.transform.position).normalized;
                    var angle = Vector2.SignedAngle(Vector2.up, force);
                    
                    slot.Rigid.velocity = Vector2.zero;
                    slot.Rigid.angularVelocity = 0.0f;
                
                    slot.Rigid.AddForce(force.normalized * 10.0f, ForceMode2D.Impulse);
                    slot.Rigid.AddTorque(angle * 0.015f, ForceMode2D.Impulse);
                }
            }

		    InGameCombatHelper.ExecuteMonsterCombat(dt);
            
            if (!OnHandleInput)
            {
                InGameCombatHelper.ExecutePartsCombat(dt);
            }
            else
            {
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                InGameCombatHelper.ExecutePartsInput(mousePosition);
            }
        }
        
        public override void OnExit()
        {
            Debug.Log("InGameCombatState OnExit");
        }

        private bool CheckGameEnd()
        {
            bool isEnd = true;
            var partContainer = InGameDataContainer.Instance.PartContainer;
            foreach (var container in partContainer)
            {
                if (!container.IsDead)
                {
                    isEnd = false;
                    break;
                }
            }

            return isEnd;
        }

        private async UniTask CreateMonster()
        {
            var randomPos = MathUtil.GetRandomPosition(Vector3.zero, 10f);
            InGameDataHelper.CreateMonster(100, randomPos);
            await UniTask.Delay((int)(1000 * Random.Range(1f, 3f)));
            CreateMonster().Forget();
        }
        
        private async UniTask DisableHandleInput()
        {
            await UniTask.Delay(2000);
            OnHandleInput = false;
        }
    }
}
