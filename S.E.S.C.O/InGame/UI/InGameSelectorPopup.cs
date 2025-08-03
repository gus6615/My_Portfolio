
using System.Collections.Generic;
using System.Linq;
using DevelopKit.BasicTemplate;
using UnityEngine;

namespace SESCO.InGame
{
    [UIProperty(UIType.Popup, "UI/InGameSelectorPopup")]
    public class InGameSelectorPopup : UIBase
    {
        private const float CELL_SIZE = 200.0f;
        
        [SerializeField] private GameObject cardSelector;
        [SerializeField] private GameObject equipSelector;
        [SerializeField] private List<InGameSelectCard> cardList;
        [SerializeField] private RectTransform equipContainer;
        
        private List<GameObject> equipSlots;
        
        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);
            
            cardSelector.SetActive(true);
            equipSelector.SetActive(false);
            this.transform.localScale = Vector3.one;
            equipSlots = new List<GameObject>();
            
            var randomIds = GetRandomIds();
            for (var i = 0; i < cardList.Count; i++)
            {
                cardList[i].Initialize(randomIds[i], OnCardSelect);
            }
            
            GameFlowManager.Instance.SetUpdateSpeed(0.0f);
        }

        public override void OnExit()
        {
            base.OnExit();

            foreach (var go in equipSlots)
            {
                AddressableUtil.Release(go);
            }
            
            var slots = equipContainer.GetComponentsInChildren<InGameEmptyModuleSlot>();
            foreach (var slot in slots)
            {
                AddressableUtil.Release(slot.gameObject);
            }
            
            GameFlowManager.Instance.SetUpdateSpeed(1.0f);
        }

        private void OnCardSelect(int selectedID)
        {
            cardSelector.SetActive(false);
            equipSelector.SetActive(true);

            var partContainers = InGameDataContainer.Instance.PartContainer;
            foreach (var partContainer in partContainers)
            {
                var equipSlot = AddressableUtil.Instantiate<InGameEquipSlot>("UI/InGameEquipSlot", equipContainer);
                equipSlot.Initialize(partContainer, selectedID, equipContainer);
                equipSlot.RectTr.anchoredPosition = new Vector2(CELL_SIZE * partContainer.Position.x, CELL_SIZE * partContainer.Position.y);
                equipSlots.Add(equipSlot.gameObject);
            }
        }

        private IReadOnlyList<int> GetRandomIds()
        {
            var ret = new List<int>();

            var isFull = true;
            var partContainers = InGameDataContainer.Instance.PartContainer;
            foreach (var container in partContainers)
            {
                if (!container.IsFullSlot)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull) return new[] { 0, 0, 0 };

            var cnt = 3;
            do
            {
                var randomId = 0;
                if (Random.value <= 0.8f)
                    randomId = ManagerHub.Data.PartDataSOList[Random.Range(0, ManagerHub.Data.PartDataSOList.Count)].ID;
                
                if (ret.Contains(randomId))
                    continue;
                
                ret.Add(randomId);
                cnt--;
            } while (cnt > 0);
            
            return ret;
        }
    }
}