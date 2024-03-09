using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReinforceItemUse : MonoBehaviour
{
    public static ReinforceItemUse instance;
    public static int[] slotCodes; // 적용된 강화 보조 아이템, -1이면 빈 슬롯 (0 ~ 7 = 주문서, 8 ~ 10 = 제련석)
    public static bool isOpenThis;

    public UIBox invenOnOffButton;
    public GameObject invenSelector;
    public GameObject invenInfoText;
    public GameObject invenItemBox;
    public GameObject invenPriceBox;
    public GameObject invenItemPrefab;
    public Transform invenItemPanel;
    public Transform slotPanel;
    public Text reinforceOreText, manaOreText;
    private UIBox[] slots;
    private int reinforceInvenItemIndex;
    private int slotIndex;
    private bool isOnInven;

    Order[] datas;
    Order data;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isOpenThis = true;

        slots = slotPanel.GetComponentsInChildren<UIBox>();
        slotCodes = new int[10];
        for (int i = 0; i < slotCodes.Length; i++)
            slotCodes[i] = -1;
        invenOnOffButton.gameObject.SetActive(false);
        invenSelector.SetActive(false);
        invenPriceBox.SetActive(false);
        invenItemBox.SetActive(false);
        for (int i = 0; i < invenOnOffButton.images.Length; i++)
            invenOnOffButton.images[i].color = Color.white * 0.4f;
        invenOnOffButton.texts[0].text = "장착한 강화 아이템 : 0 / " + slots.Length;
    }

    static public void SetReinforceVariable()
    {
        if (!isOpenThis)
            return;

        for (int i = 0; i < slotCodes.Length; i++)
        {
            if (slotCodes[i] != -1)
            {
                if (slotCodes[i] < SaveScript.reinforceItemNum)
                    SaveScript.saveData.hasReinforceItems[slotCodes[i]]++;
                else
                    SaveScript.saveData.hasReinforceItems2[slotCodes[i] - SaveScript.reinforceItemNum]++;
                slotCodes[i] = -1;
            }
        }
    }

    public void OnOffInven()
    {
        isOnInven = !isOnInven;
        slotIndex = -1;
        invenItemBox.SetActive(false);
        if (isOnInven)
        {
            invenSelector.SetActive(true);
            SetInvenSlots();
            SetInvenPrices();
        }
        else
        {
            invenSelector.SetActive(false);
            invenPriceBox.SetActive(false);
        }
    }

    public void SetInvenSlots()
    {
        int enabledSlotNum = 0;
        bool[] isOnItems = { false, false, false };

        for (int i = 0; i < slots.Length; i++)
        {
            if (slotCodes[i] != -1)
            {
                slots[i].images[1].gameObject.SetActive(false);
                slots[i].images[2].gameObject.SetActive(true);
                enabledSlotNum++;

                if (slotCodes[i] < SaveScript.reinforceItemNum)
                {
                    slots[i].images[2].sprite = SaveScript.reinforceItems[slotCodes[i]].sprite;
                    isOnItems[0] = true;
                }
                else
                {
                    slots[i].images[2].sprite = SaveScript.reinforceItems2[slotCodes[i] - SaveScript.reinforceItemNum].sprite;
                    if (slotCodes[i] < SaveScript.reinforceItemNum + 3)
                        isOnItems[1] = true;
                    else
                        isOnItems[2] = true;
                }
            }
            else
            {
                slots[i].images[1].gameObject.SetActive(true);
                slots[i].images[2].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < isOnItems.Length; i++)
        {
            if (isOnItems[i])
                invenOnOffButton.images[i].color = Color.white;
            else
                invenOnOffButton.images[i].color = Color.white * 0.4f;
        }

        invenOnOffButton.texts[0].text = "장착한 강화 아이템 : " + enabledSlotNum + " / " + slots.Length;
    }

    private void SetInvenItems()
    {
        bool isEmpty = true;
        invenItemBox.SetActive(true);
        invenInfoText.SetActive(false);

        datas = invenItemPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);

        for (int i = 0; i < SaveScript.reinforceItem2Num; i++)
        {
            if (SaveScript.saveData.hasReinforceItems2[i] != 0)
            {
                data = Instantiate(invenItemPrefab, invenItemPanel.transform).GetComponent<Order>();
                data.GetComponent<Button>().onClick.AddListener(SelectReinforceItem);
                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems2[i].image;
                data.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems2[i]);
                data.order = i;
                data.order2 = 1;

                if (isEmpty)
                    isEmpty = false;
            }
        }

        for (int i = 0; i < SaveScript.reinforceItemNum; i++)
        {
            if (SaveScript.saveData.hasReinforceItems[i] != 0)
            {
                data = Instantiate(invenItemPrefab, invenItemPanel.transform).GetComponent<Order>();
                data.GetComponent<Button>().onClick.AddListener(SelectReinforceItem);
                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems[i].image;
                data.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems[i]);
                data.order = i;
                data.order2 = 0;

                if (isEmpty)
                    isEmpty = false;
            }
        }

        invenInfoText.SetActive(isEmpty);
    }

    private void SetInvenPrices()
    {
        invenPriceBox.SetActive(true);
        reinforceOreText.text = GameFuction.GetNumText(ReinforceUpgradeUI.instance.reinforceOrePrice);
        if (SaveScript.saveData.hasReinforceOre >= ReinforceUpgradeUI.instance.reinforceOrePrice)
            reinforceOreText.color = Color.green;
        else
            reinforceOreText.color = Color.red;

        manaOreText.text = GameFuction.GetNumText(ReinforceUpgradeUI.instance.manaOrePrice);
        if (SaveScript.saveData.manaOre >= ReinforceUpgradeUI.instance.manaOrePrice)
            manaOreText.color = Color.green;
        else
            manaOreText.color = Color.red;
    }

    // 강화 아이템 슬롯을 눌렀을 경우
    public void ReinforceItemSlotButton()
    {
        slotIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
        if (slotCodes[slotIndex] == -1)
        {
            SetInvenItems();
        }
        else
        {
            if (slotCodes[slotIndex] < SaveScript.reinforceItemNum)
                SaveScript.saveData.hasReinforceItems[slotCodes[slotIndex]]++;
            else
                SaveScript.saveData.hasReinforceItems2[slotCodes[slotIndex] - SaveScript.reinforceItemNum]++;
            slotCodes[slotIndex] = -1;
            SetInvenSlots();
            ReinforceUpgradeUI.instance.SetReinforceInfo();
            SetInvenPrices();
        }
    }

    // 강화 아이템을 눌렀을 경우 (장착)
    public void SelectReinforceItem()
    {
        reinforceInvenItemIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order2 == 1)
            reinforceInvenItemIndex += SaveScript.reinforceItemNum;

        if (reinforceInvenItemIndex < SaveScript.reinforceItemNum)
            SaveScript.saveData.hasReinforceItems[reinforceInvenItemIndex]--;
        else
            SaveScript.saveData.hasReinforceItems2[reinforceInvenItemIndex - SaveScript.reinforceItemNum]--;
        slotCodes[slotIndex] = reinforceInvenItemIndex;
        reinforceInvenItemIndex = -1;

        invenItemBox.SetActive(false);
        SetInvenSlots();
        ReinforceUpgradeUI.instance.SetReinforceInfo();
        SetInvenPrices();
    }
}
