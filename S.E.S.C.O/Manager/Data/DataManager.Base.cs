
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using SESCO.SO;

namespace DevelopKit.BasicTemplate
{
    public partial class DataManager : Manager
    {
        public override ManagerPriority Priority => ManagerPriority.Data;

        public override async UniTask StartUp()
        {
            var handle = Addressables.LoadAssetsAsync<UnitDataSO>(unitDataKey, (x) =>
            {
                unitDataSoDic[x.ID] = x;   
            });
            await handle;
            
            var handle2 = Addressables.LoadAssetsAsync<PartDataSO>(partDataKey, (x) =>
            {
                partDataSoDic[x.ID] = x;
            });
            await handle2;
            
            var handle3 = Addressables.LoadAssetsAsync<ProjectileDataSO>(projectileDataKey, (x) =>
            {
                projectileDataSoDic[x.ID] = x;
            });
            await handle3;
        }
    }
}