using System;
using DevelopKit.BasicTemplate;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SESCO.InGame
{
    public class InGameSelectCard : MonoBehaviour
    {
        [SerializeField] private Button selectBtn;
        
        [SerializeField] private Image iconImg;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        
        private Action<int> onSelectCallback;
        private int objectId;
        
        public void Initialize(int id, Action<int> onSelect)
        {
            objectId = id;
            onSelectCallback = onSelect;
            selectBtn.onClick.AddListener(OnSelect);

            if (id == 0)
            {
                // 모듈
                var module = ManagerHub.Data.GetUnitDataSO(0);
                iconImg.sprite = module.Icon;
                nameText.text = module.Name;
                descText.text = module.Desc;

                var spriteSize = module.Icon.bounds.size;
                var maxSize = 240f;
                var ratio = spriteSize.x / spriteSize.y;
                iconImg.GetComponent<RectTransform>().sizeDelta = spriteSize.x > spriteSize.y ? new Vector2(maxSize, maxSize / ratio) : new Vector2(maxSize * ratio, maxSize);
            }
            else
            {
                // 파츠
                var part = ManagerHub.Data.GetPartDataSO(id);
                iconImg.sprite = part.Icon;
                nameText.text = part.Name;
                descText.text = part.Desc;
                
                var spriteSize = part.Icon.bounds.size;
                var maxSize = 240f;
                var ratio = spriteSize.x / spriteSize.y;
                iconImg.GetComponent<RectTransform>().sizeDelta = spriteSize.x > spriteSize.y ? new Vector2(maxSize, maxSize / ratio) : new Vector2(maxSize * ratio, maxSize);
            }
        }
        
        private void OnSelect()
        {
            onSelectCallback?.Invoke(objectId);
            onSelectCallback = null;
            selectBtn.onClick.RemoveAllListeners();
        }

        public void PlaySoundHover(PointerEventData pointerEventData) => ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
    }
}