
using System;
using Cysharp.Threading.Tasks;
using DevelopKit.BasicTemplate;
using UnityEngine;

namespace SESCO.InGame
{
    [UIProperty(UIType.None, "InGame/Manager/InGameManager")]
    public class InGameManager : UIBase
    {
        [SerializeField] private InGameExpUI expUI;
        
        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);
            
            InGameDataContainer.Instance.Initialize();
            PartPool.Reset();
            ProjectilePool.Reset();
            UnitPool.Reset();

            this.transform.localScale = Vector3.one;
            expUI.Initialize();
            
            GameFlowManager.Instance.PushState<InGameEnterState>();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GameFlowManager.Instance.SetUpdateSpeed(1.0f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GameFlowManager.Instance.SetUpdateSpeed(0.2f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GameFlowManager.Instance.SetUpdateSpeed(2.0f);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                ManagerHub.UI.ShowUIAsync<InGameSelectorPopup>().Forget();
            }
        }
    }
}