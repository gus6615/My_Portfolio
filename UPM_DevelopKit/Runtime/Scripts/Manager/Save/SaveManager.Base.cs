using System.IO;
using UnityEngine;
#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
    public partial class SaveManager : Manager
    {
        private static string _userDataPath;
        public override ManagerPriority Priority => ManagerPriority.Save;
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
        public override UniTask StartUp()
        {
            _userDataPath = $"{Application.persistentDataPath}/userdata.json";

            bool isSuccessLoad = LoadUserData();
            if (isSuccessLoad == false)
            {
                _userData = CreateUserData();
                SaveUserData();
            }
            
            GameLifeManager.OnQuitedCallback += SaveUserData;
            return UniTask.CompletedTask;
        }
#endif

        public bool FindUserData() => File.Exists(_userDataPath);
        
        private bool DeleteUserData()
        {
            if (FindUserData())
            {
                File.Delete(_userDataPath);
                return true;
            }
            return false;
        }
        
        private UserData CreateUserData()
        {
            UserData userData = new UserData("New User", 0, 0, 0);
            return userData;
        }
    }
}