using UnityEngine;
using UnityEngine.UI;
#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif
#if ADDRESSABLES_INSTALLED
using UnityEngine.AddressableAssets;
#endif

namespace DevelopKit
{
    public sealed class SceneTransitionFX_FadeInOut : SceneTransitionFX_Base
    {
        [SerializeField] private Image dim;
        private readonly float _fadeInDuration = 0.25f;
        private readonly float _fadeOutDuration = 0.5f;
        
        private Canvas _canvas;
        public override Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<Canvas>();
                }
                return _canvas;
            }
        }

#if ADDRESSABLES_INSTALLED
        public static SceneTransitionFX_FadeInOut CreateInstance()
        {
            var prefab = Addressables.InstantiateAsync("UI/SceneTransitionFX_FadeInOut").WaitForCompletion();
            ManagerHub.UI.AddCanvas(prefab);
            DontDestroyOnLoad(prefab);
            return prefab.GetComponent<SceneTransitionFX_FadeInOut>();
        }
#endif

#if UNITASK_INSTALLED
        public override async UniTask FadeInAsync()
        {
            var color = dim.color;
            while (color.a < 1.0f)
            {
                color.a += Time.deltaTime / _fadeOutDuration;
                dim.color = color;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            
            color.a = 1.0f;
            dim.color = color;
        }

        public override async UniTask FadeOutAsync(bool withDelete)
        {
            var color = dim.color;
            while (color.a > 0.0f)
            {
                color.a -= Time.deltaTime / _fadeInDuration;
                dim.color = color;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            
            color.a = 0.0f;
            dim.color = color;
            
            if (withDelete)
            {
#if ADDRESSABLES_INSTALLED
                Addressables.ReleaseInstance(gameObject);
#endif
            }
        }
#endif
    }
}