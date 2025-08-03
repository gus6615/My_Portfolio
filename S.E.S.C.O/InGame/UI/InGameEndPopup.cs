
using Cysharp.Threading.Tasks;
using DevelopKit.BasicTemplate;
using UnityEngine;
using UnityEngine.UI;

namespace SESCO.InGame
{
    [UIProperty(UIType.Popup, "UI/InGameEndPopup")]
    public class InGameEndPopup : UIBase
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        
        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);

            this.transform.localScale = Vector3.one;
            
            restartButton.onClick.AddListener(OnRestartButton);
            exitButton.onClick.AddListener(OnExitButton);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            restartButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        private void OnRestartButton()
        {
            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            var progress = SceneProgress_RotateCircle.CreateInstance();
            ManagerHub.Scene.GoToNextSceneWithLoading("InGameScene", null, transition, progress).Forget();
        }
        
        private void OnExitButton()
        {
            Application.Quit();
        }
    }
}