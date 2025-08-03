using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    [UIProperty(UIType.Overlay, "UI/LobbySelectPanel")]
    public class LobbySelectPanel : UIBase
    {
        [SerializeField] private Button dim;
        protected override void Awake()
        {
            base.Awake();
            
            dim.onClick.AddListener(GoToGame);
        }

        private void OnDestroy()
        {
            dim.onClick.RemoveListener(GoToGame);
        }

        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);
            
            ManagerHub.Sound.PlayBGM(BGM.BGM_0);
            GameFlowManager.Instance.PushState<LobbyState>();
        }
        
        private void GoToGame()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);

            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
        }
    }
}