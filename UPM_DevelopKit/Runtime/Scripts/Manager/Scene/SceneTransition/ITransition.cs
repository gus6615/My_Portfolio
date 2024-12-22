using UnityEngine;
#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
    public interface ITransition
    {
#if UNITASK_INSTALLED
        UniTask FadeInAsync();
        UniTask FadeOutAsync(bool withDelete);
#endif
    }
    
    public abstract class SceneTransitionFX_Base : MonoBehaviour, ITransition
    {
        public abstract Canvas Canvas { get; }
#if UNITASK_INSTALLED
        public abstract UniTask FadeInAsync();
        public abstract UniTask FadeOutAsync(bool withDelete);
#endif
    }
    
    public sealed class SceneTransitionFX_Default : SceneTransitionFX_Base
    {
        public override Canvas Canvas => null;
#if UNITASK_INSTALLED
        public override async UniTask FadeInAsync() => await UniTask.Yield();
        public override async UniTask FadeOutAsync(bool withDelete) => await UniTask.Yield();
#endif
    }
}