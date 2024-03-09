using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class GameItemShop : MonoBehaviour
{
    public static GameItemShop instance;

    public UIBox[] menuButtons;
    private List<UIBox> buySelects = new List<UIBox>();
    public Image content;
    public GameObject buyContentBox, sellContentBox;
    public GameObject buyContentPanel, sellContentPanel;
    public GameObject buySelectPanel, sellSelectPanel;
    public GameObject buySelector, sellSelector;
    public GameObject buyButton;
    public GameObject allSellButton;
    public TMP_Text plusGoldText, ringInfoText;
    public GameObject[] buyOrSellInfos;
    public Text pickInfoText, pickText01, pickText02;
    public Text accessoryInfoText01, accessoryInfoText02;
    public Image itemImage;
    public Text itemNameText;
    public Sprite growthOreSprte;
    public GameObject growthInfo;

    public InputNumBox sellNumBox;
    public Text sellGoldText;
    public GameObject sellInfoObject;
    public TMP_Text sellInfoText;
    private Text buyGoldText;

    public int menuIndex;
    static public int listIndex;

    GameObject jem;
    Button[] tempButtons;
    UIBox uiBox;
    Image[] tempImages;
    Text tempText;
    Order tempOrder;
    Order[] orders;

    private void Start()
    {
        instance = this;
        buyGoldText = buySelectPanel.GetComponentsInChildren<Text>()[1];

        allSellButton.SetActive(false);
        buySelectPanel.SetActive(false);
        sellSelectPanel.SetActive(false);
        plusGoldText.color = new Color(1f, 1f, 1f, 0f);
        ringInfoText.color = new Color(1f, 1f, 1f, 0f);
        menuIndex = 5;

        SetMenu();
    }

    private IEnumerator TouchIdle()
    {
        tempButtons = sellContentPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < tempButtons.Length; i++)
            if (tempButtons[i] != null)
                tempButtons[i].interactable = false;

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < tempButtons.Length; i++)
            if(tempButtons[i] != null)
                tempButtons[i].interactable = true;
    }

    public void ResetMenu()
    {
        menuIndex = 5;
        SetMenu();
    }

    public void OnMenuButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>() != null)
        {
            menuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
            Shop.instance.SetAudio(0);
        }

        SetMenu();
    }

    public void SetMenu()
    {
        // 버튼 Setting 및 초기화
        for (int i = 0; i < menuButtons.Length - 1; i++)
            menuButtons[i].images[0].color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
        if (menuIndex < 5)
            menuButtons[menuIndex].images[0].color = new Color(0.45f, 0.45f, 0.45f, 0.8f);
        buySelectPanel.SetActive(false);
        sellSelectPanel.SetActive(false);
        sellInfoObject.SetActive(false);

        for (int i = 0; i < buySelects.Count; i++)
            Destroy(buySelects[i].gameObject);
        buySelects.Clear();

        orders = sellContentPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < orders.Length; i++)
            Destroy(orders[i].gameObject);

        if (menuIndex == 5)
        {
            // 버튼 Setting 및 초기화
            int count = 0;
            buyContentBox.SetActive(false);
            sellContentBox.SetActive(true);
            allSellButton.SetActive(true);
            sellInfoObject.SetActive(true);
            SetSellInfoText();

            count = SaveScript.qualityNum - 1;

            // 성장하는 돌 출력
            if (SaveScript.saveData.growthOreNum > 0)
            {
                jem = Instantiate(sellSelector, sellContentPanel.transform);
                tempImages = jem.GetComponentsInChildren<Image>();
                tempText = jem.GetComponentInChildren<Text>();
                tempOrder = jem.GetComponent<Order>();

                tempImages[0].color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
                tempImages[1].sprite = growthOreSprte;
                tempText.text = "x" + GameFuction.GetNumText(SaveScript.saveData.growthOreNum);
                tempOrder.order = -1;
                tempOrder.order2 = 0;
                jem.GetComponent<Button>().onClick.AddListener(SellSelectButton);
            }

            // 광물 출력
            for (int i = 0; i < SaveScript.saveData.hasItemNums.Length; i++)
            {
                // 마이너스 버그 대비
                if (SaveScript.saveData.hasItemNums[i] < 0)
                    SaveScript.saveData.hasItemNums[i] = 0;

                if (SaveScript.saveData.hasItemNums[i] != 0 && count == SaveScript.jems[i].quality)
                {
                    jem = Instantiate(sellSelector, sellContentPanel.transform);
                    tempImages = jem.GetComponentsInChildren<Image>();
                    tempText = jem.GetComponentInChildren<Text>();
                    tempOrder = jem.GetComponent<Order>();

                    tempImages[0].color = SaveScript.qualityColors_weak[SaveScript.jems[i].quality] * 0.8f;
                    tempImages[1].sprite = SaveScript.jems[i].jemSprite;
                    tempText.text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasItemNums[i]);
                    tempOrder.order = i;
                    tempOrder.order2 = 0;
                    jem.GetComponent<Button>().onClick.AddListener(SellSelectButton);
                }

                if (i == SaveScript.saveData.hasItemNums.Length - 1)
                {
                    if (count == 0)
                        break;
                    else
                    {
                        i = -1;
                        count--;
                    }
                }
            }
        }
        else
        {
            switch (menuIndex)
            {
                case 0: // 곡괭이
                    for (int i = 0; i < SaveScript.pickNum; i++)
                    {
                        uiBox = Instantiate(buySelector, buyContentPanel.transform).GetComponent<UIBox>();
                        uiBox.images[1].sprite = SaveScript.picks[i].sprites[0];
                        uiBox.texts[0].text = SaveScript.picks[i].name;
                        uiBox.order = i;
                        uiBox.button.onClick.AddListener(BuySelectButton);
                        buySelects.Add(uiBox);
                    }
                    break;
                case 1: // 모자
                    for (int i = 0; i < SaveScript.pickNum; i++)
                    {
                        uiBox = Instantiate(buySelector, buyContentPanel.transform).GetComponent<UIBox>();
                        uiBox.images[1].sprite = SaveScript.hats[i].sprite;
                        uiBox.texts[0].text = SaveScript.hats[i].name;
                        uiBox.order = i;
                        uiBox.button.onClick.AddListener(BuySelectButton);
                        buySelects.Add(uiBox);
                    }
                    break;
                case 2: // 반지
                    for (int i = 0; i < SaveScript.pickNum; i++)
                    {
                        uiBox = Instantiate(buySelector, buyContentPanel.transform).GetComponent<UIBox>();
                        uiBox.images[1].sprite = SaveScript.rings[i].sprite;
                        uiBox.texts[0].text = SaveScript.rings[i].name;
                        uiBox.order = i;
                        uiBox.button.onClick.AddListener(BuySelectButton);
                        buySelects.Add(uiBox);
                    }
                    break;
                case 3: // 목걸이
                    for (int i = 0; i < SaveScript.pickNum; i++)
                    {
                        uiBox = Instantiate(buySelector, buyContentPanel.transform).GetComponent<UIBox>();
                        uiBox.images[1].sprite = SaveScript.pendants[i].sprite;
                        uiBox.texts[0].text = SaveScript.pendants[i].name;
                        uiBox.order = i;
                        uiBox.button.onClick.AddListener(BuySelectButton);
                        buySelects.Add(uiBox);
                    }
                    break;
                case 4: // 검
                    for (int i = 0; i < SaveScript.swordNum; i++)
                    {
                        uiBox = Instantiate(buySelector, buyContentPanel.transform).GetComponent<UIBox>();
                        uiBox.images[1].sprite = SaveScript.swords[i].sprite;
                        uiBox.texts[0].text = SaveScript.swords[i].name;
                        uiBox.order = i;
                        uiBox.button.onClick.AddListener(BuySelectButton);
                        buySelects.Add(uiBox);
                    }
                    break;
            }

            buyContentBox.SetActive(true);
            sellContentBox.SetActive(false);
            allSellButton.SetActive(false);
            BuySelectSetColor();
        }
    }

    private void BuySelectSetColor()
    {
        for (int i = 0; i < buySelects.Count; i++)
        {
            buySelects[i].images[0].color = buySelects[i].images[1].color = buySelects[i].texts[0].color = Color.white;
            if (GetBestEquipment(menuIndex) + 1 < i)
            {
                buySelects[i].images[0].color = buySelects[i].images[1].color = buySelects[i].texts[0].color = new Color(1f, 1f, 1f, 0.2f);
                buySelects[i].button.enabled = false;
            }

            bool isHave = false;
            switch (menuIndex)
            {
                case 0: isHave = SaveScript.saveData.hasPicks[i]; break;
                case 1: isHave = SaveScript.saveData.hasHats[i]; break;
                case 2: isHave = SaveScript.saveData.hasRings[i]; break;
                case 3: isHave = SaveScript.saveData.hasPenants[i]; break;
                case 4: isHave = SaveScript.saveData.hasSwords[i]; break;
            }

            if (isHave)
                buySelects[i].images[0].color = buySelects[i].images[1].color = buySelects[i].texts[0].color = Color.white * 0.8f;
        }
    }

    public void BuySelectButton()
    {
        listIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;

        Shop.instance.SetAudio(0);
        buySelectPanel.SetActive(true);
        sellSelectPanel.SetActive(false);
        for (int i = 0; i < buyOrSellInfos.Length; i++)
            buyOrSellInfos[i].SetActive(false);

        if(menuIndex == 0)
            buyOrSellInfos[0].SetActive(true);
        else
            buyOrSellInfos[1].SetActive(true);

        switch (menuIndex)
        {
            case 0: // 곡괭이
                itemImage.sprite = SaveScript.picks[listIndex].sprites[0];
                itemNameText.text = SaveScript.picks[listIndex].name;
                pickInfoText.text = "블럭 파괴 " + (listIndex + 1) + "단계";
                pickText01.text = "내구도(HP) : " + SaveScript.picks[listIndex].durability;
                pickText02.text = "효율(속도) : " + Mathf.Round(SaveScript.picks[listIndex].breakTime * 5f * 100f) / 100f + "초";
                if (listIndex > 13)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, 0, SaveScript.picks[listIndex].price);
                if (listIndex > 9)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, SaveScript.picks[listIndex].price, 0);
                else if (listIndex > 5)
                    buyGoldText.text = GameFuction.GetGoldText(0, SaveScript.picks[listIndex].price, 0, 0);
                else
                    buyGoldText.text = GameFuction.GetGoldText(SaveScript.picks[listIndex].price, 0, 0, 0);

                if (SaveScript.saveData.hasPicks[listIndex])
                    buyButton.gameObject.SetActive(false);
                else
                    buyButton.gameObject.SetActive(true);
                break;
            case 1: // 모자
                itemImage.sprite = SaveScript.hats[listIndex].sprite;
                itemNameText.text = SaveScript.hats[listIndex].name;
                accessoryInfoText01.text = "방어력 상승 " + (listIndex + 1) + "단계";
                accessoryInfoText02.text = "( 몬스터에 대한 방어력 " + SaveScript.hats[listIndex].forcePercent + " 증가 )";
                if (listIndex > 13)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, 0, SaveScript.hats[listIndex].price);
                if (listIndex > 9)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, SaveScript.hats[listIndex].price, 0);
                else if (listIndex > 5)
                    buyGoldText.text = GameFuction.GetGoldText(0, SaveScript.hats[listIndex].price, 0, 0);
                else
                    buyGoldText.text = GameFuction.GetGoldText(SaveScript.hats[listIndex].price, 0, 0, 0);

                if (SaveScript.saveData.hasHats[listIndex])
                    buyButton.gameObject.SetActive(false);
                else
                    buyButton.gameObject.SetActive(true);
                break;
            case 2: // 반지
                itemImage.sprite = SaveScript.rings[listIndex].sprite;
                itemNameText.text = SaveScript.rings[listIndex].name;
                accessoryInfoText01.text = "거래 효율 " + (listIndex + 1) + "단계";
                accessoryInfoText02.text = "( 상점 판매금 " + (SaveScript.rings[listIndex].forcePercent * 100) + "% 증가 )";
                if (listIndex > 13)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, 0, SaveScript.rings[listIndex].price);
                if (listIndex > 9)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, SaveScript.rings[listIndex].price, 0);
                else if (listIndex > 5)
                    buyGoldText.text = GameFuction.GetGoldText(0, SaveScript.rings[listIndex].price, 0, 0);
                else
                    buyGoldText.text = GameFuction.GetGoldText(SaveScript.rings[listIndex].price, 0, 0, 0);

                if (SaveScript.saveData.hasRings[listIndex])
                    buyButton.gameObject.SetActive(false);
                else
                    buyButton.gameObject.SetActive(true);
                break;
            case 3: // 목걸이
                itemImage.sprite = SaveScript.pendants[listIndex].sprite;
                itemNameText.text = SaveScript.pendants[listIndex].name;
                accessoryInfoText01.text = "광석 행운 " + (listIndex + 1) + "단계";
                accessoryInfoText02.text = "( 하나의 광물에서 광석이 최대 " + SaveScript.pendants[listIndex].forcePercent + "개 추출 )";
                if (listIndex > 13)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, 0, SaveScript.pendants[listIndex].price);
                if (listIndex > 9)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, SaveScript.pendants[listIndex].price, 0);
                else if (listIndex > 5)
                    buyGoldText.text = GameFuction.GetGoldText(0, SaveScript.pendants[listIndex].price, 0, 0);
                else
                    buyGoldText.text = GameFuction.GetGoldText(SaveScript.pendants[listIndex].price, 0, 0, 0);

                if (SaveScript.saveData.hasPenants[listIndex])
                    buyButton.gameObject.SetActive(false);
                else
                    buyButton.gameObject.SetActive(true);
                break;
            case 4: // 검
                itemImage.sprite = SaveScript.swords[listIndex].sprite;
                itemNameText.text = SaveScript.swords[listIndex].name;
                accessoryInfoText01.text = "공격 상승 " + (listIndex + 1) + "단계";
                accessoryInfoText02.text = "( 공격 한 번당 " + SaveScript.swords[listIndex].forcePercent + "의 피해를 입힘 )";
                if (listIndex > 13)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, 0, SaveScript.swords[listIndex].price);
                if (listIndex > 9)
                    buyGoldText.text = GameFuction.GetGoldText(0, 0, SaveScript.swords[listIndex].price, 0);
                else if (listIndex > 5)
                    buyGoldText.text = GameFuction.GetGoldText(0, SaveScript.swords[listIndex].price, 0, 0);
                else
                    buyGoldText.text = GameFuction.GetGoldText(SaveScript.swords[listIndex].price, 0, 0, 0);

                if (SaveScript.saveData.hasSwords[listIndex])
                    buyButton.gameObject.SetActive(false);
                else
                    buyButton.gameObject.SetActive(true);
                break;
        }
    }

    public void SellSelectButton()
    {
        listIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        Shop.instance.SetAudio(0);

        buySelectPanel.SetActive(false);
        sellSelectPanel.SetActive(true);
        for (int i = 0; i < buyOrSellInfos.Length; i++)
            buyOrSellInfos[i].SetActive(false);
        buyOrSellInfos[2].SetActive(true);
        tempImages = sellSelectPanel.GetComponentsInChildren<Image>();
        tempText = sellSelectPanel.GetComponentInChildren<Text>();

        if (listIndex == -1)
        {
            growthInfo.SetActive(true);
            tempImages[1].color = new Color(1f, 0.4f, 0.8f, 0.8f);
            tempImages[2].sprite = growthOreSprte;
            tempText.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            sellNumBox.slider.value = sellNumBox.slider.maxValue = SaveScript.saveData.growthOreNum;
            tempText.text = "[ 언노운 ] 성장하는 돌: " + (SaveScript.saveData.pickLevel + 1) + "단계 (";
            if (SaveScript.saveData.pickLevel < 5)
                tempText.text += GameFuction.GetGoldText(SaveScript.growthOre_prices[SaveScript.saveData.pickLevel], 0, 0, 0) + ")";
            else if (SaveScript.saveData.pickLevel < 10)
                tempText.text += GameFuction.GetGoldText(0, SaveScript.growthOre_prices[SaveScript.saveData.pickLevel], 0, 0) + ")";
            else if (SaveScript.saveData.pickLevel < 14)
                tempText.text += GameFuction.GetGoldText(0, 0, SaveScript.growthOre_prices[SaveScript.saveData.pickLevel], 0) + ")";
            else
                tempText.text += GameFuction.GetGoldText(0, 0, 0, SaveScript.growthOre_prices[SaveScript.saveData.pickLevel]) + ")";
        }
        else
        {
            growthInfo.SetActive(false);
            tempImages[1].color = SaveScript.qualityColors_weak[SaveScript.jems[listIndex].quality];
            tempImages[2].sprite = SaveScript.jems[listIndex].jemSprite;
            tempText.color = SaveScript.qualityColors_weak[SaveScript.jems[listIndex].quality];
            sellNumBox.slider.value = sellNumBox.slider.maxValue = SaveScript.saveData.hasItemNums[listIndex];
            if (Jem.IsPrice2(listIndex))
            {
                tempText.text = "[" + SaveScript.qualityNames_kr[SaveScript.jems[listIndex].quality] + "] " + SaveScript.jems[listIndex].name
                    + " (" + GameFuction.GetGoldText(0, SaveScript.jems[listIndex].GetRealPrice(), 0, 0) + ")";
            }
            else
            {
                tempText.text = "[" + SaveScript.qualityNames_kr[SaveScript.jems[listIndex].quality] + "] " + SaveScript.jems[listIndex].name
                    + " (" + GameFuction.GetGoldText(SaveScript.jems[listIndex].GetRealPrice(), 0, 0, 0) + ")";
            }
        }
        OnSellInputBoxChanged();
    }

    public void BuyButton()
    {
        bool isCanBuy = false;

        switch (menuIndex)
        {
            case 0: // 곡괭이
                if ((listIndex > 9 && GameFuction.CheckCanBuy(0, 0, SaveScript.picks[listIndex].price)) ||
                    (listIndex <= 9 && listIndex > 5 && GameFuction.CheckCanBuy(0, SaveScript.picks[listIndex].price, 0)) ||
                    (listIndex <= 5 && GameFuction.CheckCanBuy(SaveScript.picks[listIndex].price, 0, 0)))
                {
                    isCanBuy = true;
                    if (listIndex == 0 || !SaveScript.saveData.hasPicks[listIndex - 1])
                    {
                        if (listIndex > 9) GameFuction.Buy(0, 0, SaveScript.picks[listIndex].price);
                        else if (listIndex > 5) GameFuction.Buy(0, SaveScript.picks[listIndex].price, 0);
                        else GameFuction.Buy(SaveScript.picks[listIndex].price, 0, 0);
                        SaveScript.saveData.hasPicks[listIndex] = true;
                        SaveScript.saveData.equipPick = listIndex;
                        SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.hats[listIndex].name + "' 구매 완료!");
                    }
                    else
                        ShopTransmission.instance.SetAni();
                }
                break;
            case 1: // 모자
                if ((listIndex > 9 && GameFuction.CheckCanBuy(0, 0, SaveScript.hats[listIndex].price)) ||
                    (listIndex <= 9 && listIndex > 5 && GameFuction.CheckCanBuy(0, SaveScript.hats[listIndex].price, 0)) ||
                    (listIndex <= 5 && GameFuction.CheckCanBuy(SaveScript.hats[listIndex].price, 0, 0)))
                {
                    isCanBuy = true;
                    if (listIndex == 0 || !SaveScript.saveData.hasHats[listIndex - 1])
                    {
                        if (listIndex > 9) GameFuction.Buy(0, 0, SaveScript.hats[listIndex].price);
                        else if (listIndex > 5) GameFuction.Buy(0, SaveScript.hats[listIndex].price, 0);
                        else GameFuction.Buy(SaveScript.hats[listIndex].price, 0, 0);
                        SaveScript.saveData.hasHats[listIndex] = true;
                        SaveScript.saveData.equipHat = listIndex;

                        SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.hats[listIndex].name + "' 구매 완료!");
                    }
                    else
                        ShopTransmission.instance.SetAni();
                }
                break;
            case 2: // 반지
                if ((listIndex > 9 && GameFuction.CheckCanBuy(0, 0, SaveScript.rings[listIndex].price)) ||
                    (listIndex <= 9 && listIndex > 5 && GameFuction.CheckCanBuy(0, SaveScript.rings[listIndex].price, 0)) ||
                    (listIndex <= 5 && GameFuction.CheckCanBuy(SaveScript.rings[listIndex].price, 0, 0)))
                {
                    isCanBuy = true;
                    if (listIndex == 0 || !SaveScript.saveData.hasRings[listIndex - 1])
                    {
                        if (listIndex > 9) GameFuction.Buy(0, 0, SaveScript.rings[listIndex].price);
                        else if (listIndex > 5) GameFuction.Buy(0, SaveScript.rings[listIndex].price, 0);
                        else GameFuction.Buy(SaveScript.rings[listIndex].price, 0, 0);
                        SaveScript.saveData.hasRings[listIndex] = true;
                        SaveScript.saveData.equipRing = listIndex;

                        SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.rings[listIndex].name + "' 구매 완료!");
                    }
                    else
                        ShopTransmission.instance.SetAni();
                }
                break;
            case 3: // 목걸이
                if ((listIndex > 9 && GameFuction.CheckCanBuy(0, 0, SaveScript.pendants[listIndex].price)) ||
                    (listIndex <= 9 && listIndex > 5 && GameFuction.CheckCanBuy(0, SaveScript.pendants[listIndex].price, 0)) ||
                    (listIndex <= 5 && GameFuction.CheckCanBuy(SaveScript.pendants[listIndex].price, 0, 0)))
                {
                    isCanBuy = true;
                    if (listIndex == 0 || !SaveScript.saveData.hasPenants[listIndex - 1])
                    {
                        if (listIndex > 9) GameFuction.Buy(0, 0, SaveScript.pendants[listIndex].price);
                        else if (listIndex > 5) GameFuction.Buy(0, SaveScript.pendants[listIndex].price, 0);
                        else GameFuction.Buy(SaveScript.pendants[listIndex].price, 0, 0);
                        SaveScript.saveData.hasPenants[listIndex] = true;
                        SaveScript.saveData.equipPendant = listIndex;

                        SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.pendants[listIndex].name + "' 구매 완료!");
                    }
                    else
                        ShopTransmission.instance.SetAni();
                }
                break;
            case 4: // 검
                if ((listIndex > 9 && GameFuction.CheckCanBuy(0, 0, SaveScript.swords[listIndex].price)) ||
                    (listIndex <= 9 && listIndex > 5 && GameFuction.CheckCanBuy(0, SaveScript.swords[listIndex].price, 0)) ||
                    (listIndex <= 5 && GameFuction.CheckCanBuy(SaveScript.swords[listIndex].price, 0, 0)))
                {
                    isCanBuy = true;
                    if (listIndex == 0 || !SaveScript.saveData.hasSwords[listIndex - 1])
                    {
                        if (listIndex > 9) GameFuction.Buy(0, 0, SaveScript.swords[listIndex].price);
                        else if (listIndex > 5) GameFuction.Buy(0, SaveScript.swords[listIndex].price, 0);
                        else GameFuction.Buy(SaveScript.swords[listIndex].price, 0, 0);
                        SaveScript.saveData.hasSwords[listIndex] = true;
                        SaveScript.saveData.equipSword = listIndex;

                        SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.swords[listIndex].name + "' 구매 완료!");
                    }
                    else
                        ShopTransmission.instance.SetAni();
                }
                break;
        }

        if (isCanBuy)
        {
            // 퀘스트
            CheckEquipmentQuest();

            Shop.instance.SetBasicInfo();
            Shop.instance.SetAudio(0);
            SetMenu();
        }
        else
        {
            SystemInfoCtrl.instance.SetErrorInfo("금액이 부족합니다");
            Shop.instance.SetAudio(2);
        }
    }

    private int GetBestEquipment(int _equipment)
    {
        int index = -1;
        bool[] hasEquipments;

        switch (_equipment)
        {
            case 0: hasEquipments = SaveScript.saveData.hasPicks; break;
            case 1: hasEquipments = SaveScript.saveData.hasHats; break;
            case 2: hasEquipments = SaveScript.saveData.hasRings; break;
            case 3: hasEquipments = SaveScript.saveData.hasPenants; break;
            default: hasEquipments = SaveScript.saveData.hasSwords; break;
        }

        for (int i = 0; i < hasEquipments.Length; i++)
            if (hasEquipments[i])
                index = i;
        return index;
    }

    static public void CheckEquipmentQuest()
    {
        int mainQuest = -1;
        switch (SaveScript.saveData.mainQuest_list)
        {
            case 5: mainQuest = 1; break;
            case 12: mainQuest = 2; break;
            case 24: mainQuest = 3; break;
            case 29: mainQuest = 4; break;
            case 43: mainQuest = 5; break;
            case 49: mainQuest = 6; break;
            case 63: mainQuest = 7; break;
            case 71: mainQuest = 8; break;
            case 81: mainQuest = 9; break;
            case 91: mainQuest = 10; break;
            case 101: mainQuest = 11; break;
            case 111: mainQuest = 12; break;
            case 121: mainQuest = 13; break;
            case 131: mainQuest = 14; break;
        }

        if (mainQuest != -1)
        {
            if (mainQuest < 6) // 7층 이전 곡괭이
            {
                if (SaveScript.saveData.hasPicks[mainQuest] && SaveScript.saveData.hasHats[mainQuest] && SaveScript.saveData.hasRings[mainQuest]
                    && SaveScript.saveData.hasPenants[mainQuest] && SaveScript.saveData.hasSwords[mainQuest])
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { SaveScript.saveData.mainQuest_list }, 1);
            }
            else // 7층 이후(포함) 곡괭이
            {
                if (SaveScript.saveData.hasPicks[mainQuest])
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { SaveScript.saveData.mainQuest_list }, 1);
            }
        }
    }

    public string GetRingInfoText(int jemIndex, bool isSpecial)
    {
        string text = "<color=white>";
        bool isInit = false;
        bool[] isOnAsQuality = new bool[SaveScript.qualityNum]; // 노멀, 레어, 에픽, 유니크, 레전드리, 얼티밋, 미스틱
        if (!isSpecial || SaveScript.saveData.equipRing == -1) return text;

        if(jemIndex == -1)
        {
            // 모든 광물 팔기
            for (int i = 0; i < SaveScript.saveData.hasItemNums.Length; i++)
            {
                if (SaveScript.saveData.hasItemNums[i] > 0 && SaveScript.jems[i].quality > GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]))
                {
                    // 반지 효과 적용 X
                    if (!isInit) isInit = true;
                    if (!isOnAsQuality[SaveScript.jems[i].quality]) isOnAsQuality[SaveScript.jems[i].quality] = true;
                }
            }
        }
        else
        {
            // jemIndex 광물 팔기
            if (SaveScript.saveData.hasItemNums[jemIndex] > 0 && SaveScript.jems[jemIndex].quality > GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]))
            {
                // 반지 효과 적용 X
                isInit = true;
                isOnAsQuality[SaveScript.jems[jemIndex].quality] = true;
            }
        }

        if (isInit)
            text += "반지 등급이 부족해 '특수 효과'를 받지\n못한 광물이 있습니다!\n\n<color=yellow>* 반지 효과가 제외된 광물 등급 *\n\n";
        for (int i = 0; i < isOnAsQuality.Length; i++)
            if(isOnAsQuality[i])
                text += SaveScript.qualityColors_weekTmp[i] + "[" + SaveScript.qualityNames_kr[i] + "]\n";

        return text;
    }

    public void SellButton()
    {
        long price = 0, price2 = 0, price3 = 0, price4 = 0;
        long maxNum;
        long count;
        float plusBufPercent = 0f;
        float plusPercent = 0f;
        float plusFlag = SaveScript.stat.ring02;
        string infoText = "";
        bool isSpecial = false;
        bool isBufItem = false;
        StartCoroutine("TouchIdle");

        if (SaveScript.saveData.hasIcons[9]) plusBufPercent += SaveScript.icons[9].force;
        if (SaveScript.saveData.hasIcons[11]) plusBufPercent += SaveScript.icons[11].force;
        plusBufPercent += GameFuction.GetBufItemPercent(4) + GameFuction.GetElixirPercent(4);
        plusBufPercent += GameFuction.GetManaBufForceForData(1);

        if (plusBufPercent != 0f) isBufItem = true;

        // 광물
        if (listIndex != -1)
        {
            maxNum = GameFuction.GOLD_UNIT / SaveScript.jems[listIndex].GetRealPrice();
            count = sellNumBox.itemNum / maxNum;
            SetPrice(ref price, ref price2, ref price3, ref price4, listIndex, sellNumBox.itemNum, maxNum, count);

            Debug.Log(price + " / " + price2 + " / " + price3 + " / " + price4);

            // 반지 효과
            if (SaveScript.saveData.equipRing != -1)
            {
                int index = GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]);
                plusPercent += SaveScript.rings[SaveScript.saveData.equipRing].forcePercent + SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * SaveScript.stat.ring01;
                if (GameFuction.GetRandFlag(plusFlag))
                {
                    isSpecial = true;
                    infoText += "<color=red>(특수 판매 발동 - '" + SaveScript.qualityNames_kr[index] + "'까지 적용)\n";
                }
                else
                {
                    infoText += "<color=white>(추가 판매 발동 - '모든 광물' 적용)\n";
                }
            }

            if (isBufItem) infoText += "<color=#9696FF>(버프 효과 발동 - 모두 적용)\n";
            if (isSpecial && SaveScript.jems[listIndex].quality <= GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]))
            {
                if (plusFlag >= 1f) plusPercent *= GameFuction.GetReinforcePercent_Over2(2);
                else plusPercent *= 2;
            }
            plusPercent += 1f;

            // 데이터 설정
            long tempPrice = price, tempPrice2 = price2, tempPrice3 = price3, tempPrice4 = price4;
            price = price2 = price3 = price4 = 0;
            if (tempPrice != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    price += tempPrice * maxNum;
                    SetPriceBalance(ref price, ref price2, ref price3, ref price4);
                }
                price += (long)(tempPrice * (plusPercent % maxNum));
            }
            if (tempPrice2 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice2;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    price2 += tempPrice2 * maxNum;
                    SetPriceBalance(ref price, ref price2, ref price3, ref price4);
                }
                price2 += (long)(tempPrice2 * (plusPercent % maxNum));
            }
            if (tempPrice3 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice3;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    price3 += tempPrice3 * maxNum;
                    SetPriceBalance(ref price, ref price2, ref price3, ref price4);
                }
                price3 += (long)(tempPrice3 * (plusPercent % maxNum));
            }
            if (tempPrice4 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice4;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    price4 += tempPrice4 * maxNum;
                    SetPriceBalance(ref price, ref price2, ref price3, ref price4);
                }
                price4 += (long)(tempPrice4 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref price, ref price2, ref price3, ref price4);

            Debug.Log(price + " / " + price2 + " / " + price3 + " / " + price4);
        }
        else {
            // 성장하는 돌
            maxNum = GameFuction.GOLD_UNIT / SaveScript.growthOre_prices[SaveScript.saveData.pickLevel];
            count = sellNumBox.itemNum / maxNum;
            SetPrice(ref price, ref price2, ref price3, ref price4, listIndex, sellNumBox.itemNum, maxNum, count);

            Debug.Log(price + " / " + price2 + " / " + price3 + " / " + price4);
        }
        
        if (isSpecial || SaveScript.saveData.hasIcons[5] || SaveScript.saveData.hasIcons[9]) Shop.instance.SetAudio(5);
        else Shop.instance.SetAudio(4);

        int lastNum = (int)(price % 10);
        if(lastNum != 0) price += 10 - lastNum;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 1)
        {
            price *= 2;
            price2 *= 2;
            price3 *= 2;
            price4 *= 2;
            SetPriceBalance(ref price, ref price2, ref price3, ref price4);
        }

        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 1)
            infoText += "<color=#AFFFAF>\n <주말 이벤트> 상점 판매 2배 혜택 적용!";
        infoText += "<color=yellow>\n+ " + GameFuction.GetGoldText(price, price2, price3, price4);

        SaveScript.saveData.gold += price;
        SaveScript.saveData.gold2 += price2;
        SaveScript.saveData.gold3 += price3;
        SaveScript.saveData.gold4 += price4;
        if (listIndex == -1)
            SaveScript.saveData.growthOreNum -= sellNumBox.itemNum;
        else
            SaveScript.saveData.hasItemNums[listIndex] -= sellNumBox.itemNum;
        SetPriceBalance(ref SaveScript.saveData.gold, ref SaveScript.saveData.gold2, ref SaveScript.saveData.gold3, ref SaveScript.saveData.gold4);

        // 업적 갱신 부분
        AchievementCtrl.instance.SetAchievementAmount(20, price, price2, price3);
        if (price3 > SaveScript.saveData.achievementAmount3[1])
        {
            // 루니일 경우
            SaveScript.saveData.achievementAmount3[1] = price3;
            SaveScript.saveData.achievementAmount2[1] = price2;
            SaveScript.saveData.achievementAmounts[29] = price;
            // 동일한 경우 한 단계 아래 단위 체크
            if (price3 == SaveScript.saveData.achievementAmount3[1]
                && price2 > SaveScript.saveData.achievementAmount2[1])
                SaveScript.saveData.achievementAmount2[1] = price2;
        }
        else if (price2 > SaveScript.saveData.achievementAmount2[1])
        {
            // 경일 경우
            SaveScript.saveData.achievementAmount2[1] = price2;
            SaveScript.saveData.achievementAmounts[29] = price;
            // 동일한 경우 한 단계 아래 단위 체크
            if (price2 == SaveScript.saveData.achievementAmount2[1]
                && price > SaveScript.saveData.achievementAmounts[29])
                SaveScript.saveData.achievementAmounts[29] = price;
        }
        else if (price > SaveScript.saveData.achievementAmounts[29])
        {
            // 경 이하일 경우
            SaveScript.saveData.achievementAmounts[29] = price;
        }

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 4 }, SaveScript.saveData.gold);

        // 뒤끝챗
        if (!SaveScript.saveData.isRankerChat && SaveScript.saveData.gold2 > 1)
        {
            SaveScript.saveData.isRankerChat = true;
            Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이  '전설'이 되셨습니다!", 1);
        }
        if (!SaveScript.saveData.isGodChat && SaveScript.saveData.gold3 > 1)
        {
            SaveScript.saveData.isGodChat = true;
            Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이  '신화'가 되셨습니다!", 2);
        }

        plusGoldText.text = infoText;
        ringInfoText.text = GetRingInfoText(listIndex, isSpecial);

        plusGoldText.color = ringInfoText.color = Color.white;
        plusGoldText.GetComponent<FadeUI>().SetFadeValues(0f, 2f, 4f);
        ringInfoText.GetComponent<FadeUI>().SetFadeValues(0f, 4f, 6f);
        Shop.instance.SetBasicInfo();
        SetMenu();
    }

    public void AllSellButton()
    {
        long nonPlusPrice = 0, nonPlusPrice2 = 0, nonPlusPrice3 = 0, nonPlusPrice4 = 0;
        long plusPrice = 0, plusPrice2 = 0, plusPrice3 = 0, plusPrice4 = 0;
        long maxNum;
        long count;
        float plusBufPercent = 0f;
        float plusPercent = 0f;
        float plusFlag = SaveScript.stat.ring02;
        string infoText = "";
        bool isSpecial = false;
        StartCoroutine("TouchIdle");

        // 반지 효과
        if (SaveScript.saveData.equipRing != -1)
        {
            int index = GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]);
            plusPercent += SaveScript.rings[SaveScript.saveData.equipRing].forcePercent + SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * SaveScript.stat.ring01;
            if (GameFuction.GetRandFlag(plusFlag))
            {
                infoText += "<color=red>(특수 판매 발동 - '" + SaveScript.qualityNames_kr[index] + "'까지)\n";
                isSpecial = true;
            }
            else
                infoText += "<color=white>(추가 판매 발동 - '모든 광물' 적용)\n";
        }
        ringInfoText.text = GetRingInfoText(-1, isSpecial);

        if (SaveScript.saveData.hasIcons[9]) plusBufPercent += SaveScript.icons[9].force;
        if (SaveScript.saveData.hasIcons[11]) plusBufPercent += SaveScript.icons[11].force;
        plusBufPercent += GameFuction.GetBufItemPercent(4) + GameFuction.GetElixirPercent(4);
        plusBufPercent += GameFuction.GetManaBufForceForData(1);

        bool isBufItem = false;
        if (plusBufPercent != 0f) isBufItem = true;

        // 광물
        if (SaveScript.saveData.equipRing != -1)
        {
            for (int i = 0; i < SaveScript.saveData.hasItemNums.Length; i++)
            {
                if (SaveScript.saveData.hasItemNums[i] != 0)
                {
                    maxNum = GameFuction.GOLD_UNIT / SaveScript.jems[i].GetRealPrice();
                    count = SaveScript.saveData.hasItemNums[i] / maxNum;
                    if (SaveScript.jems[i].quality <= GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]))
                    {
                        SetPrice(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4, i, SaveScript.saveData.hasItemNums[i], maxNum, count);
                    }
                    else
                    {
                        SetPrice(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4, i, SaveScript.saveData.hasItemNums[i], maxNum, count);
                    }
                    SaveScript.saveData.hasItemNums[i] = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < SaveScript.saveData.hasItemNums.Length; i++)
            {
                if (SaveScript.saveData.hasItemNums[i] != 0)
                {
                    maxNum = GameFuction.GOLD_UNIT / SaveScript.jems[i].GetRealPrice();
                    count = SaveScript.saveData.hasItemNums[i] / maxNum;
                    SetPrice(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4, i, SaveScript.saveData.hasItemNums[i], maxNum, count);
                    SaveScript.saveData.hasItemNums[i] = 0;
                }
            }
        }

        Debug.Log(plusPrice + " / " + plusPrice2 + " / " + plusPrice3 + " / " + plusPrice4 + " / " + nonPlusPrice + " / " + nonPlusPrice2 + " / " + nonPlusPrice3 + " / " + nonPlusPrice4);

        if(nonPlusPrice != 0 || plusPrice != 0 || nonPlusPrice2 != 0 || plusPrice2 != 0 || nonPlusPrice3 != 0 || plusPrice3 != 0 || nonPlusPrice4 != 0 || plusPrice4 != 0)
        {
            if (isBufItem) infoText += "<color=#9696FF>(버프 효과 발동 - 모든 광물)\n";
            plusPercent += 1f;

            // 데이터 설정 - 일반 효과
            long tempPrice = nonPlusPrice, tempPrice2 = nonPlusPrice2, tempPrice3 = nonPlusPrice3, tempPrice4 = nonPlusPrice4;
            nonPlusPrice = nonPlusPrice2 = nonPlusPrice3 = nonPlusPrice4 = 0;
            if (tempPrice != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice += tempPrice * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice += (long)(tempPrice * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
            if (tempPrice2 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice2;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice2 += tempPrice2 * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice2 += (long)(tempPrice2 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
            if (tempPrice3 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice3;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice3 += tempPrice3 * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice3 += (long)(tempPrice3 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
            if (tempPrice4 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice4;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice4 += tempPrice4 * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice4 += (long)(tempPrice4 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);

            plusPercent -= 1f;
            if (isSpecial)
            {
                if (plusFlag >= 1f) plusPercent *= GameFuction.GetReinforcePercent_Over2(2);
                else plusPercent *= 2;
            }
            plusPercent += 1f;

            // 데이터 설정 - 특수 효과
            tempPrice = plusPrice;
            tempPrice2 = plusPrice2;
            tempPrice3 = plusPrice3;
            tempPrice4 = plusPrice4;
            plusPrice = plusPrice2 = plusPrice3 = plusPrice4 = 0;
            if (tempPrice != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice += tempPrice * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice += (long)(tempPrice * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
            if (tempPrice2 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice2;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice2 += tempPrice2 * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice2 += (long)(tempPrice2 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
            if (tempPrice3 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice3;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice3 += tempPrice3 * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice3 += (long)(tempPrice3 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
            if (tempPrice4 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice4;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice4 += tempPrice4 * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice4, ref plusPrice4);
                }
                plusPrice4 += (long)(tempPrice4 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);

            if (isSpecial || SaveScript.saveData.hasIcons[5] || SaveScript.saveData.hasIcons[9]) Shop.instance.SetAudio(5);
            else Shop.instance.SetAudio(4);

            long totalPrice = nonPlusPrice + plusPrice;
            long totalPrice2 = nonPlusPrice2 + plusPrice2;
            long totalPrice3 = nonPlusPrice3 + plusPrice3;
            long totalPrice4 = nonPlusPrice4 + plusPrice4;
            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 1)
            {
                totalPrice *= 2;
                totalPrice2 *= 2;
                totalPrice3 *= 2;
                totalPrice4 *= 2;
                SetPriceBalance(ref totalPrice, ref totalPrice2, ref totalPrice3, ref totalPrice4);
            }

            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 1)
                infoText += "<color=#AFFFAF>\n <주말 이벤트> 상점 판매 2배 혜택 적용!";
            infoText += "<color=yellow>\n+ " + GameFuction.GetGoldText(totalPrice, totalPrice2, totalPrice3, totalPrice4);

            plusGoldText.text = infoText;
            SaveScript.saveData.gold += totalPrice;
            SaveScript.saveData.gold2 += totalPrice2;
            SaveScript.saveData.gold3 += totalPrice3;
            SaveScript.saveData.gold4 += totalPrice4;
            SetPriceBalance(ref SaveScript.saveData.gold, ref SaveScript.saveData.gold2, ref SaveScript.saveData.gold3, ref SaveScript.saveData.gold4);

            Debug.Log(totalPrice + " / " + totalPrice2 + " / " + totalPrice3 + " / " + totalPrice4);

            // 업적 갱신 부분
            AchievementCtrl.instance.SetAchievementAmount(20, totalPrice, totalPrice2, totalPrice3);
            if (totalPrice3 > SaveScript.saveData.achievementAmount3[1])
            {
                // 루니일 경우
                SaveScript.saveData.achievementAmount3[1] = totalPrice3;
                SaveScript.saveData.achievementAmount2[1] = totalPrice2;
                SaveScript.saveData.achievementAmounts[29] = totalPrice;
                // 동일한 경우 한 단계 아래 단위 체크
                if (totalPrice3 == SaveScript.saveData.achievementAmount3[1] && totalPrice2 > SaveScript.saveData.achievementAmount2[1])
                    SaveScript.saveData.achievementAmount2[1] = totalPrice2;
            }
            else if (totalPrice2 > SaveScript.saveData.achievementAmount2[1])
            {
                // 경일 경우
                SaveScript.saveData.achievementAmount2[1] = totalPrice2;
                SaveScript.saveData.achievementAmounts[29] = totalPrice;
                // 동일한 경우 한 단계 아래 단위 체크
                if (totalPrice2 == SaveScript.saveData.achievementAmount2[1] && totalPrice > SaveScript.saveData.achievementAmounts[29])
                    SaveScript.saveData.achievementAmounts[29] = totalPrice;
            }
            else if (totalPrice > SaveScript.saveData.achievementAmounts[29])
            {
                // 경 이하일 경우
                SaveScript.saveData.achievementAmounts[29] = totalPrice;
            }

            // 퀘스트
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 4 }, SaveScript.saveData.gold);

            // 뒤끝챗
            if (!SaveScript.saveData.isRankerChat && SaveScript.saveData.gold2 > 1)
            {
                SaveScript.saveData.isRankerChat = true;
                Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이  '전설'이 되셨습니다!", 1);
            }
            if (!SaveScript.saveData.isGodChat && SaveScript.saveData.gold3 > 1)
            {
                SaveScript.saveData.isGodChat = true;
                Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이  '신화'가 되셨습니다!", 2);
            }

            plusGoldText.color = ringInfoText.color = Color.white;
            plusGoldText.GetComponent<FadeUI>().SetFadeValues(0f, 2f, 4f);
            ringInfoText.GetComponent<FadeUI>().SetFadeValues(0f, 4f, 6f);
            SetMenu();
            Shop.instance.SetBasicInfo();
        }
    }

    public void SetPriceBalance(ref long price, ref long price2, ref long price3, ref long price4)
    {
        if (price3 >= GameFuction.GOLD_UNIT)
        {
            price4 += price3 / GameFuction.GOLD_UNIT;
            price3 = price3 % GameFuction.GOLD_UNIT;
        }
        if (price2 >= GameFuction.GOLD_UNIT)
        {
            price3 += price2 / GameFuction.GOLD_UNIT;
            price2 = price2 % GameFuction.GOLD_UNIT;
        }
        if (price >= GameFuction.GOLD_UNIT)
        {
            price2 += price / GameFuction.GOLD_UNIT;
            price = price % GameFuction.GOLD_UNIT;
        }
    }

    public void SetPrice(ref long price, ref long price2, ref long price3, ref long price4, int jem_index, long jem_num, long maxNum, long count)
    {
        if (jem_index == -1)
        {
            if (SaveScript.saveData.pickLevel < 5)
            {
                price += SaveScript.growthOre_prices[SaveScript.saveData.pickLevel] * (jem_num % maxNum);
                price2 += count;
            }
            else if (SaveScript.saveData.pickLevel < 10)
            {
                price2 += SaveScript.growthOre_prices[SaveScript.saveData.pickLevel] * (jem_num % maxNum);
                price3 += count;
            }
            else if (SaveScript.saveData.pickLevel < 14)
            {
                price3 += SaveScript.growthOre_prices[SaveScript.saveData.pickLevel] * (jem_num % maxNum);
                price4 += count;
            }
            else
            {
                price4 += SaveScript.growthOre_prices[SaveScript.saveData.pickLevel] * (jem_num % maxNum);
                if (count > 0)
                    price4 = GameFuction.GOLD_UNIT + 1;
            }
        }
        else
        {
            if (Jem.IsPrice2(jem_index))
            {
                price2 += SaveScript.jems[jem_index].GetRealPrice() * (jem_num % maxNum);
                price3 += count;
            }
            else
            {
                price += SaveScript.jems[jem_index].GetRealPrice() * (jem_num % maxNum);
                price2 += count;
            }
        }
        SetPriceBalance(ref price, ref price2, ref price3, ref price4);
    }

    public void OnSellInputBoxChanged()
    {
        long price = 0, price2 = 0, price3 = 0, price4 = 0;
        long maxNum;
        long count;

        if (listIndex == -1)
            maxNum = GameFuction.GOLD_UNIT / SaveScript.growthOre_prices[SaveScript.saveData.pickLevel];
        else
            maxNum = GameFuction.GOLD_UNIT / SaveScript.jems[listIndex].GetRealPrice();
        count = sellNumBox.itemNum / maxNum;
        SetPrice(ref price, ref price2, ref price3, ref price4, listIndex, sellNumBox.itemNum, maxNum, count);
        sellGoldText.text = GameFuction.GetGoldText(price, price2, price3, price4);
    }

    private void SetSellInfoText()
    {
        long nonPlusPrice = 0, nonPlusPrice2 = 0, nonPlusPrice3 = 0, nonPlusPrice4 = 0;
        long plusPrice = 0, plusPrice2 = 0, plusPrice3 = 0, plusPrice4 = 0;
        long maxNum;
        long count;
        float plusBufPercent = 0f;
        float plusPercent = 0f;
        float plusFlag = SaveScript.stat.ring02;
        bool isSpecial = plusFlag >= 1f;
        string str = "";

        // 반지 효과
        if (SaveScript.saveData.equipRing != -1)
            plusPercent += SaveScript.rings[SaveScript.saveData.equipRing].forcePercent + SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * SaveScript.stat.ring01;

        if (SaveScript.saveData.hasIcons[9]) plusBufPercent += SaveScript.icons[9].force;
        if (SaveScript.saveData.hasIcons[11]) plusBufPercent += SaveScript.icons[11].force;
        plusBufPercent += GameFuction.GetBufItemPercent(4) + GameFuction.GetElixirPercent(4);
        plusBufPercent += GameFuction.GetManaBufForceForData(1);

        // 광물 가격 총합
        if (SaveScript.saveData.equipRing != -1)
        {
            for (int i = 0; i < SaveScript.saveData.hasItemNums.Length; i++)
            {
                if (SaveScript.saveData.hasItemNums[i] != 0)
                {
                    maxNum = GameFuction.GOLD_UNIT / SaveScript.jems[i].GetRealPrice();
                    count = SaveScript.saveData.hasItemNums[i] / maxNum;
                    if (SaveScript.jems[i].quality <= GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]))
                    {
                        SetPrice(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4, i, SaveScript.saveData.hasItemNums[i], maxNum, count);
                    }
                    else
                    {
                        SetPrice(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4, i, SaveScript.saveData.hasItemNums[i], maxNum, count);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < SaveScript.saveData.hasItemNums.Length; i++)
            {
                if (SaveScript.saveData.hasItemNums[i] != 0)
                {
                    maxNum = GameFuction.GOLD_UNIT / SaveScript.jems[i].GetRealPrice();
                    count = SaveScript.saveData.hasItemNums[i] / maxNum;
                    SetPrice(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4, i, SaveScript.saveData.hasItemNums[i], maxNum, count);
                }
            }
        }

        if (nonPlusPrice != 0 || plusPrice != 0 || nonPlusPrice2 != 0 || plusPrice2 != 0 || nonPlusPrice3 != 0 || plusPrice3 != 0 || nonPlusPrice4 != 0 || plusPrice4 != 0)
        {
            str += "<color=white>< 광물 순수 가격 합 >\n";
            str += "<color=yellow>" + GameFuction.GetGoldText(nonPlusPrice + plusPrice, nonPlusPrice2 + plusPrice2, nonPlusPrice3 + plusPrice3, nonPlusPrice4 + plusPrice4) + "\n\n";
            str += "<color=red>< 판매 효과를 포함한 예상 금액 >\n";
            plusPercent += 1f;

            // 데이터 설정 - 일반 효과
            long tempPrice = nonPlusPrice, tempPrice2 = nonPlusPrice2, tempPrice3 = nonPlusPrice3, tempPrice4 = nonPlusPrice4;
            nonPlusPrice = nonPlusPrice2 = nonPlusPrice3 = nonPlusPrice4 = 0;
            if (tempPrice != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice += tempPrice * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice += (long)(tempPrice * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
            if (tempPrice2 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice2;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice2 += tempPrice2 * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice2 += (long)(tempPrice2 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
            if (tempPrice3 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice3;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice3 += tempPrice3 * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice3 += (long)(tempPrice3 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
            if (tempPrice4 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice4;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    nonPlusPrice4 += tempPrice4 * maxNum;
                    SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);
                }
                nonPlusPrice4 += (long)(tempPrice4 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref nonPlusPrice, ref nonPlusPrice2, ref nonPlusPrice3, ref nonPlusPrice4);

            plusPercent -= 1f;
            if (isSpecial)
                plusPercent *= GameFuction.GetReinforcePercent_Over2(2);
            plusPercent += 1f;

            // 데이터 설정 - 특수 효과
            tempPrice = plusPrice;
            tempPrice2 = plusPrice2;
            tempPrice3 = plusPrice3;
            tempPrice4 = plusPrice4;
            plusPrice = plusPrice2 = plusPrice3 = plusPrice4 = 0;
            if (tempPrice != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice += tempPrice * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice += (long)(tempPrice * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
            if (tempPrice2 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice2;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice2 += tempPrice2 * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice2 += (long)(tempPrice2 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
            if (tempPrice3 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice3;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice3 += tempPrice3 * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice3 += (long)(tempPrice3 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
            if (tempPrice4 != 0)
            {
                maxNum = GameFuction.GOLD_UNIT * 900 / tempPrice4;
                count = (long)(plusPercent / maxNum);
                for (int i = 0; i < count; i++)
                {
                    plusPrice4 += tempPrice4 * maxNum;
                    SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);
                }
                plusPrice4 += (long)(tempPrice4 * (plusPercent % maxNum));
            }
            SetPriceBalance(ref plusPrice, ref plusPrice2, ref plusPrice3, ref plusPrice4);

            long totalPrice = nonPlusPrice + plusPrice;
            long totalPrice2 = nonPlusPrice2 + plusPrice2;
            long totalPrice3 = nonPlusPrice3 + plusPrice3;
            long totalPrice4 = nonPlusPrice4 + plusPrice4;
            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 1)
            {
                totalPrice *= 2;
                totalPrice2 *= 2;
                totalPrice3 *= 2;
                totalPrice4 *= 2;
                SetPriceBalance(ref totalPrice, ref totalPrice2, ref totalPrice3, ref totalPrice4);
            }

            str += "<color=red>" + GameFuction.GetGoldText(totalPrice, totalPrice2, totalPrice3, totalPrice4);
            sellInfoObject.SetActive(true);
        }
        else
            sellInfoObject.SetActive(false);

        sellInfoText.SetText(str);
    }
}
