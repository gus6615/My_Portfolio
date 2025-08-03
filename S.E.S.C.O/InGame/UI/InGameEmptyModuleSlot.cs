using System;
using DevelopKit.BasicTemplate;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SESCO.InGame
{
    public class InGameEmptyModuleSlot : MonoBehaviour
    {
        [SerializeField] private Button selectBtn;
        
        private PartContainer partContainer;
        private PeerType peerType;

        public void Initialize(PartContainer container, PeerType type)
        {
            partContainer = container;
            peerType = type;
            
            selectBtn.onClick.AddListener(OnSelect);
        }
        
        private void OnSelect()
        {
            selectBtn.onClick.RemoveAllListeners();
            InGameDataHelper.CreatePartContainer(partContainer, peerType);
            ManagerHub.UI.HideUI(FindObjectOfType<InGameSelectorPopup>());
        }

        public void PlaySoundHover(PointerEventData pointerEventData) => ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
    }
}