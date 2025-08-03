
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DevelopKit.BasicTemplate;
using UnityEngine;

namespace SESCO.InGame
{
    public class InGameVFXManager : Singleton<InGameVFXManager>
    {
        public ParticleSystem CreateVFX(string vfxID, Vector2 position)
        {
            if (!InGameDataContainer.Instance.VFXPoolDic.ContainsKey(vfxID))
            {
                InGameDataContainer.Instance.VFXPoolDic[vfxID] = new List<ParticleSystem>();
            }

            ParticleSystem particle = GetVFXInPool(vfxID);
            if (particle == null)
            {
                particle = CreateNewVFX(vfxID, position);
            }
            
            particle.transform.position = position;
            particle.gameObject.SetActive(true);
            SetPoolAsDuration(vfxID, particle).Forget();
            return particle;
        }

        public void DestroyAllVFXs()
        {
            var vfxList = InGameDataContainer.Instance.VFXList;
            for (int i = vfxList.Count - 1; i >= 0; i--)
            {
                AddressableUtil.Release(vfxList[i].gameObject);
            }
            vfxList.Clear();
            InGameDataContainer.Instance.VFXPoolDic.Clear();
        }
        
        private async UniTask SetPoolAsDuration(string vfxID, ParticleSystem particle)
        {
            await UniTask.Delay((int)(1000 * particle.main.duration));
            if (particle == null) return;
            InGameDataContainer.Instance.VFXPoolDic[vfxID].Add(particle);
            particle.gameObject.SetActive(false);
        }

        private ParticleSystem GetVFXInPool(string vfxID)
        {
            var vfxLists = InGameDataContainer.Instance.VFXPoolDic[vfxID];
            for (int i = 0; i < vfxLists.Count; i++)
            {
                if (vfxLists[i].isStopped)
                {
                    return vfxLists[i];
                }
            }

            return null;
        }

        private ParticleSystem CreateNewVFX(string vfxID, Vector2 position)
        {
            ParticleSystem particle = AddressableUtil.Instantiate<ParticleSystem>($"VFX/{vfxID}.prefab", null);
            particle.transform.position = position;
            InGameDataContainer.Instance.VFXPoolDic[vfxID].Add(particle);
            return particle;
        }
    }
}