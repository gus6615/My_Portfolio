
using System.Collections.Generic;
using SESCO.InGame;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class InGameEnterState : StateBase
    {
        public override void OnEnter()
        {
            Debug.Log("InGameEnterState OnEnter");

            var parts = new List<int>() { 10 };
            InGameDataHelper.InitPartContainer(parts);
            GameFlowManager.Instance.PushState<InGameCombatState>();
        }

        public override void OnUpdate(float dt)
        {

        }
        
        public override void OnExit()
        {
            Debug.Log("InGameEnterState OnExit");
        }
    }
}
