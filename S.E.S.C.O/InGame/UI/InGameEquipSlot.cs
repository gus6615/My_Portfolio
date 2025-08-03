using System;
using System.Collections.Generic;
using DevelopKit.BasicTemplate;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SESCO.InGame
{
    public class InGameEquipSlot : MonoBehaviour
    {
        [SerializeField] private Button selectBtn;
        [SerializeField] private Image frameImg;
        [SerializeField] private List<Image> partIcons;

        [SerializeField] private Sprite brokenSlotSprite;
        
        private RectTransform rectTr;
        public RectTransform RectTr => rectTr;
        
        private PartContainer partContainer;
        private int objectId;

        public void Initialize(PartContainer container, int id, Transform equipContainer)
        {
            rectTr = GetComponent<RectTransform>();
            partContainer = container;
            objectId = id;
            selectBtn.onClick.AddListener(OnSelect);
            
            if (container.IsDead)
            {
                frameImg.sprite = brokenSlotSprite;
                selectBtn.enabled = false;
            }

            var parts = container.Parts;
            for (int i = 0; i < partIcons.Count; i++)
            {
                if (i >= parts.Count)
                {
                    partIcons[i].enabled = false;
                    continue;
                }
                
                partIcons[i].enabled = true;
                partIcons[i].sprite = parts[i].Data.Icon;
                partIcons[i].transform.rotation = Quaternion.Euler(0, 0, 90 * i);
                partIcons[i].GetComponent<RectTransform>().sizeDelta = parts[i].Data.Icon.bounds.size * 35f;
            }

            if (id == 0)
            {
                selectBtn.enabled = false;
                
                // 4개 빈 슬롯 추가 생성
                var enums = Enum.GetValues(typeof(PeerType));
                for (var i = 0; i < enums.Length; i++)
                {
                    var position = container.Position + container.PeerOffsets[i];
                    if (InGameDataContainer.Instance.PartContainerMap.TryGetValue(position, out var _))
                        continue;
                    
                    var peerType = (PeerType)enums.GetValue(i);
                    var createPos = new Vector2Int(position.x * 200, position.y * 200);
                    var emptySlot = AddressableUtil.Instantiate<InGameEmptyModuleSlot>("UI/InGameEmptyModuleSlot", equipContainer);
                    emptySlot.Initialize(partContainer, peerType);
                    emptySlot.GetComponent<RectTransform>().anchoredPosition = createPos;
                }
            }
            else
            {
                frameImg.color = Color.white;
                selectBtn.enabled = !container.IsFullSlot;
            }
        }
        
        private void OnSelect()
        {
            var part = InGameDataHelper.CreatePart(objectId);
            part.transform.SetParent(partContainer.transform);
            partContainer.AddPart(part, partContainer.Parts.Count);
            
            selectBtn.onClick.RemoveAllListeners();
            ManagerHub.UI.HideUI(FindObjectOfType<InGameSelectorPopup>());
        }

        public void PlaySoundHover(PointerEventData pointerEventData) => ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
    }
}