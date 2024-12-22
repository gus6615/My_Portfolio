using UnityEngine;
using UnityEngine.UI;
#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
    [UIProperty(UIType.Overlay, "UI/LobbySelectPanel")]
    public class LobbySelectPanel : UIBase
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button closeButton;
        
        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(OnClickContinue);
            newGameButton.onClick.AddListener(OnClickNewGame);
            settingButton.onClick.AddListener(OnClickSetting);
            closeButton.onClick.AddListener(OnClickExit);
        }

        private void OnDestroy()
        {
            continueButton.onClick.RemoveListener(OnClickContinue);
            newGameButton.onClick.RemoveListener(OnClickNewGame);
            settingButton.onClick.RemoveListener(OnClickSetting);
            closeButton.onClick.RemoveListener(OnClickExit);
        }

        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);
            
            ManagerHub.Sound.PlayBGM(BGM.BGM_0);
            GameFlowManager.Instance.PushState<LobbyState>();
            
            var isUserDataExist = ManagerHub.Save.FindUserData();
            continueButton.gameObject.SetActive(isUserDataExist);
        }

        public void PlayHoverSound() => ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
        
        private void OnClickContinue()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);
            
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
#endif
        }
        
        private void OnClickNewGame()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);

            if (ManagerHub.Save.FindUserData())
            {
                CallbackTuple callbackTuple = new CallbackTuple(OnClickNewGameYes, null);
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
                ManagerHub.UI.ShowUIAsync<ConfirmAlert>(callbackTuple).Forget();
#endif
            }
            else
            {
                ManagerHub.Save.ResetData();
                
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
                var transition = SceneTransitionFX_FadeInOut.CreateInstance();
                ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
#else
                UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
#endif
            }
        }
        
        private void OnClickSetting()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
            ManagerHub.UI.ShowUIAsync<SettingPanel>().Forget();
#endif
        }
        
        private void OnClickExit()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);
#if UNITY_EDITOR
            GameLifeManager.InvokeQuitCallback();
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // 어플리케이션 종료
#endif
        }

        private void OnClickNewGameYes()
        {
            ManagerHub.Save.ResetData();
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
#endif
        }
    }
}