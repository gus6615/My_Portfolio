using UnityEngine;
#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
    public interface IProgress
    {
        void Initialize(object data = null);
#if UNITASK_INSTALLED
        UniTask OnProgressAsync();
#endif
        void OnComplete();
    }

    public abstract class SceneProgress_Base : MonoBehaviour, IProgress
    {
        public virtual void Initialize(object data = null) => gameObject.SetActive(true);

#if UNITASK_INSTALLED
        public virtual async UniTask OnProgressAsync() => await UniTask.Yield();
#endif
        public abstract void OnComplete();
    }
    
    public sealed class SceneProgress_Default : SceneProgress_Base
    {
        public override void OnComplete() { }
    }
}