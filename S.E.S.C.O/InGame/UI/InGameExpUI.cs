
using TMPro;
using UnityEngine;

namespace SESCO.InGame 
{
    public class InGameExpUI : MonoBehaviour
    {
        [SerializeField] private RectTransform frameRect;
        [SerializeField] private RectTransform fillRect;
        [SerializeField] private TextMeshProUGUI levelText;
        private float frameSizeDeltaX;
        private float fillSizeDeltaY;

        private void OnDestroy()
        {
            InGameDataContainer.Instance.Level.OnValueChanged -= RefreshLevel;
            InGameDataContainer.Instance.Exp.OnValueChanged -= RefreshExp;
        }

        public void Initialize()
        {
            frameSizeDeltaX = frameRect.sizeDelta.x;
            fillSizeDeltaY = fillRect.sizeDelta.y;
            RefreshExp(0);
            
            InGameDataContainer.Instance.Level.OnValueChanged += RefreshLevel;
            InGameDataContainer.Instance.Exp.OnValueChanged += RefreshExp;
        }

        private void RefreshExp(int exp)
        {       
            var needExp = InGameDataContainer.Instance.NeedExp;
            var t = (float)exp / needExp;
            var sizeDeltaX = Mathf.Lerp(0, frameSizeDeltaX, t);
            fillRect.sizeDelta = new Vector2(sizeDeltaX, fillSizeDeltaY);
        }
        
        private void RefreshLevel(int level)
        {
            levelText.text = $"Lv.{level}";
        }
    }
}