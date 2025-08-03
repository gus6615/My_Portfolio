using Cysharp.Threading.Tasks;
using SESCO.InGame;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class InGameEndState : StateBase
    {
        public override void OnEnter()
        {
            Debug.Log("InGameEndState OnEnter");

            ManagerHub.UI.ShowUIAsync<InGameEndPopup>().Forget();
        }

        public override void OnUpdate(float dt)
        {
            
        }
        
        public override void OnExit()
        {
            Debug.Log("InGameEndState OnExit");
        }
    }
}
