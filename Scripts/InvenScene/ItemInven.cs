using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInven : MonoBehaviour
{
    public static ItemInven instance;

    public new AudioSource audio;
    public GameObject invenObject;
    public GameObject itemPrefab;
    public GameObject invenInfoBox;
    public GameObject invenPanel;
    public GameObject invenEventBox;
    public GameObject invenInfoText;
    public GameObject autoUseObject;
    public GameObject cashEquipmentObject;
    private Image invenInfoImage;
    private Text[] invenInfoTexts;
    private Button invenUseButton;
    private Text[] autoUseTexts;
    private Toggle[] autoUseToggles;
    private Text[] cashEquipmentTexts;
    private Toggle[] cashEquipmentToggles;
    private int invenItemIndex, invenItemType;
    private bool isOnOffInven;

    // Temp 변수들
    Order[] orders;
    Order order;
    BufItem bufItem;
    ReinforceItem reinforceItem;
    Elixir elixir;
    ReinforceItem2 reinforceItem2;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        autoUseTexts = autoUseObject.GetComponentsInChildren<Text>();
        autoUseToggles = autoUseObject.GetComponentsInChildren<Toggle>();
        cashEquipmentTexts = cashEquipmentObject.GetComponentsInChildren<Text>();
        cashEquipmentToggles = cashEquipmentObject.GetComponentsInChildren<Toggle>();

        invenInfoImage = invenInfoBox.GetComponentsInChildren<Image>()[3];
        invenInfoTexts = invenInfoBox.GetComponentsInChildren<Text>(); // 0 ~ 2 사용
        invenUseButton = invenInfoBox.GetComponentInChildren<Button>();

        invenObject.SetActive(false);
        invenInfoBox.SetActive(false);
        invenEventBox.SetActive(false);
        invenInfoText.SetActive(false);
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    // 인벤토리 열기
    public void OnOffInven()
    {
        isOnOffInven = !isOnOffInven;
        invenObject.SetActive(isOnOffInven);
        SetAudio(0);

        if (isOnOffInven)
        {
            SettingInven();
            SetAutoUse();
            SetCashEquipment();
            if (Inventory.instance != null)
                Inventory.instance.SlotClear();
        }
        else
        {
            SaveScript.stat.SetStat();
        }
    }

    // 아이템 창 정보 갱신
    public void SettingInven()
    {
        bool isEmpty = true;

        IconInfoUI.instance.InitIconInfo();
        invenInfoBox.SetActive(false);
        invenEventBox.SetActive(false);

        orders = invenPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < orders.Length; i++)
            Destroy(orders[i].gameObject);

        for (int i = 0; i < SaveScript.bufItemCodeNum; i++)
        {
            if (SaveScript.saveData.hasElixirs[i] != 0)
            {
                order = Instantiate(itemPrefab, invenPanel.transform).GetComponent<Order>();
                order.GetComponent<Button>().onClick.AddListener(SelectItem);
                order.GetComponentsInChildren<Image>()[1].sprite = SaveScript.elixirs[i].sprite;
                order.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasElixirs[i]);
                order.order = i;
                order.order2 = 2;

                if (isEmpty)
                    isEmpty = false;
            }
        }

        for (int i = 0; i < SaveScript.bufItemNum; i++)
        {
            if (SaveScript.saveData.hasBufItems[i] != 0)
            {
                order = Instantiate(itemPrefab, invenPanel.transform).GetComponent<Order>();
                order.GetComponent<Button>().onClick.AddListener(SelectItem);
                order.GetComponentsInChildren<Image>()[1].sprite = SaveScript.bufItems[i].sprite;
                order.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasBufItems[i]);
                order.order = i;

                if (isEmpty)
                    isEmpty = false;
            }
        }

        for (int i = 0; i < SaveScript.reinforceItem2Num; i++)
        {
            if (SaveScript.saveData.hasReinforceItems2[i] != 0)
            {
                order = Instantiate(itemPrefab, invenPanel.transform).GetComponent<Order>();
                order.GetComponent<Button>().onClick.AddListener(SelectItem);
                order.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems2[i].sprite;
                order.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems2[i]);
                order.order = i;
                order.order2 = 3;

                if (isEmpty)
                    isEmpty = false;
            }
        }

        for (int i = 0; i < SaveScript.reinforceItemNum; i++)
        {
            if (SaveScript.saveData.hasReinforceItems[i] != 0)
            {
                order = Instantiate(itemPrefab, invenPanel.transform).GetComponent<Order>();
                order.GetComponent<Button>().onClick.AddListener(SelectItem);
                order.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems[i].sprite;
                order.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems[i]);
                order.order = i;
                order.order2 = 1;

                if (isEmpty)
                    isEmpty = false;
            }
        }

        invenInfoText.SetActive(isEmpty);
    }

    // 아이템 선택
    public void SelectItem()
    {
        order = EventSystem.current.currentSelectedGameObject.GetComponent<Order>();
        invenInfoBox.SetActive(true);
        SetAudio(0);

        if (order.order2 == 0)
        {
            invenItemIndex = order.order;
            invenItemType = order.order2;
            bufItem = SaveScript.bufItems[invenItemIndex];

            invenUseButton.gameObject.SetActive(true);
            invenInfoImage.sprite = bufItem.sprite;
            invenInfoTexts[0].text = bufItem.name;
            invenInfoTexts[1].text = bufItem.info;
            invenInfoTexts[2].text = "현재 보유 개수 : " + GameFuction.GetNumText(SaveScript.saveData.hasBufItems[invenItemIndex]) + "개";
        }
        else if (order.order2 == 1)
        {
            reinforceItem = SaveScript.reinforceItems[order.order];

            invenUseButton.gameObject.SetActive(false);
            invenInfoImage.sprite = reinforceItem.sprite;
            invenInfoTexts[0].text = reinforceItem.name;
            invenInfoTexts[1].text = reinforceItem.info;
            invenInfoTexts[2].text = "현재 보유 개수 : " + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems[order.order]) + "개";
        }
        else if (order.order2 == 2)
        {
            invenItemIndex = order.order;
            invenItemType = order.order2;
            elixir = SaveScript.elixirs[invenItemIndex];

            invenUseButton.gameObject.SetActive(true);
            invenInfoImage.sprite = elixir.sprite;
            invenInfoTexts[0].text = elixir.name;
            invenInfoTexts[1].text = elixir.info;
            invenInfoTexts[2].text = "현재 보유 개수 : " + GameFuction.GetNumText(SaveScript.saveData.hasElixirs[invenItemIndex]) + "개";
        }
        else
        {
            reinforceItem2 = SaveScript.reinforceItems2[order.order];

            invenUseButton.gameObject.SetActive(false);
            invenInfoImage.sprite = reinforceItem2.sprite;
            invenInfoTexts[0].text = reinforceItem2.name;
            invenInfoTexts[1].text = reinforceItem2.info;
            invenInfoTexts[2].text = "현재 보유 개수 : " + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems2[order.order]) + "개";
        }
    }

    // 아이템 사용
    public void UseItem()
    {
        SetAudio(0);

        if (invenItemType == 0)
        {
            // 일반 물약
            int type = invenItemIndex / 3;

            if (SaveScript.saveData.isBufItemOns[3 * type] || SaveScript.saveData.isBufItemOns[3 * type + 1] 
                || SaveScript.saveData.isBufItemOns[3 * type + 2] || SaveScript.saveData.isElixirOns[type])
                invenEventBox.SetActive(true);
            else
                UseItemYes();
        }
        else if (invenItemType == 2)
        {
            // 영약
            if (SaveScript.saveData.isBufItemOns[3 * invenItemIndex] || SaveScript.saveData.isBufItemOns[3 * invenItemIndex + 1] 
                || SaveScript.saveData.isBufItemOns[3 * invenItemIndex + 2] || SaveScript.saveData.isElixirOns[invenItemIndex])
                invenEventBox.SetActive(true);
            else
                UseItemYes();
        }
    }

    // 아이템 사용 동의
    public void UseItemYes()
    {
        if (invenItemType == 0)
        {
            // 일반 물약
            int type = invenItemIndex / 3;

            SaveScript.saveData.bufItemTimes[3 * type] = SaveScript.saveData.bufItemTimes[3 * type + 1] = SaveScript.saveData.bufItemTimes[3 * type + 2] = 0;
            SaveScript.saveData.isBufItemOns[3 * type] = SaveScript.saveData.isBufItemOns[3 * type + 1] = SaveScript.saveData.isBufItemOns[3 * type + 2] = false;
            SaveScript.saveData.elixirTimes[type] = 0;
            SaveScript.saveData.isElixirOns[type] = false;

            SaveScript.saveData.hasBufItems[invenItemIndex]--;
            SaveScript.saveData.bufItemTimes[invenItemIndex] = SaveScript.bufItemTime;
            SaveScript.saveData.isBufItemOns[invenItemIndex] = true;
        }
        else if (invenItemType == 2)
        {
            SaveScript.saveData.bufItemTimes[3 * invenItemIndex] = SaveScript.saveData.bufItemTimes[3 * invenItemIndex + 1] = SaveScript.saveData.bufItemTimes[3 * invenItemIndex + 2] = 0;
            SaveScript.saveData.isBufItemOns[3 * invenItemIndex] = SaveScript.saveData.isBufItemOns[3 * invenItemIndex + 1] = SaveScript.saveData.isBufItemOns[3 * invenItemIndex + 2] = false;
            SaveScript.saveData.elixirTimes[invenItemIndex] = 0;
            SaveScript.saveData.isElixirOns[invenItemIndex] = false;

            SaveScript.saveData.hasElixirs[invenItemIndex]--;
            SaveScript.saveData.elixirTimes[invenItemIndex] = SaveScript.elixirTime;
            SaveScript.saveData.isElixirOns[invenItemIndex] = true;
        }

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 16 });
        AchievementCtrl.instance.SetAchievementAmount(25, 1);
        GameFuction.SetPlayerStat();
        SettingInven();
        SetAudio(23);
        if (Inventory.instance != null)
            Inventory.instance.SetBasicInfo();
    }

    // 아이템 사용 안함
    public void UseItemNo()
    {
        invenItemIndex = 0;
        SettingInven();
        SetAudio(0);
    }

    // 물약 자동 사용 문구 설정
    public void SetAutoUse()
    {
        autoUseToggles[0].isOn = SaveScript.saveData.isAutoUse_1;
        autoUseToggles[1].isOn = SaveScript.saveData.isAutoUse_2;
        if (SaveScript.saveData.isAutoUse_1)
            autoUseTexts[0].color = Color.white;
        else
            autoUseTexts[0].color = new Color(1f, 1f, 1f, 0.5f);
        if (SaveScript.saveData.isAutoUse_2)
            autoUseTexts[1].color = Color.white;
        else
            autoUseTexts[1].color = new Color(1f, 1f, 1f, 0.5f);
    }

    public void SetCashEquipment()
    {
        for (int i = 0; i < SaveScript.cashEquipmentNum; i++)
        {
            cashEquipmentToggles[i].isOn = SaveScript.saveData.isCashEquipmentOn[i];
            if (SaveScript.saveData.isCashEquipmentOn[i])
                cashEquipmentTexts[i].color = Color.white;
            else
                cashEquipmentTexts[i].color = new Color(1f, 1f, 1f, 0.5f);

            if (!SaveScript.saveData.hasCashEquipment[i])
            {
                cashEquipmentTexts[i].color = new Color(1f, 1f, 1f, 0.15f);
                cashEquipmentToggles[i].interactable = false;
            }
        }    
    }

    public void OnSetAutoUse_1()
    {
        SaveScript.saveData.isAutoUse_1 = autoUseToggles[0].isOn;
        SetAutoUse();
    }

    public void OnSetAutoUse_2()
    {
        SaveScript.saveData.isAutoUse_2 = autoUseToggles[1].isOn;
        SetAutoUse();
    }

    public void OnSetCashEquipment()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;

        order = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Order>();
        if (order == null)
            return;

        int index = order.order;
        SaveScript.saveData.isCashEquipmentOn[index] = cashEquipmentToggles[index].isOn;
        SetCashEquipment();
        SettingInven();
    }
}
