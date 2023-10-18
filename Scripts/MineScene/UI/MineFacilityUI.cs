using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MineFacilityUI : MonoBehaviour
{
    public static MineFacilityUI instance;
    public static bool isRewardTimeSet;

    public Text manaOreText;
    public Text effectNameText, rewardNameText;
    public Text[] effectInfoTexts, rewardInfoTexts;
    public Image[] rewardInfoImages, rewardLockImages;
    public Text upgradeText;
    public Text rewardText, rewardTimeText;
    public Slider rewardSlider;
    public UIBox rewardUI;
    public Transform shopContentPanel;
    public GameObject manaShopSlotPrefab;
    public GameObject rewardAni;
    public Image manaSprite;
    public GameObject canRecieve;
    public GameObject bufEventUI;
    public Image[] menuButtonImages;

    public int manaOre_min, manaOre_max;
    public int elixirNum, reinforceItemNum;
    public float amountForce, qualityForce, bufForce;
    public int buyIndex;
    public int menuIndex;

    private Color canColor, canNotColor;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        canColor = new Color(0.4f, 1f, 0.4f);
        canNotColor = new Color(1f, 0.2f, 0.2f);
        rewardSlider.maxValue = SaveScript.facility_rewardTime;
        canRecieve.SetActive(false);
        bufEventUI.SetActive(false);
        CheckCanReward();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRewardTimeSet) SetRewardInfo();
    }

    public void SetDefaultVariable()
    {
        menuIndex = 0;
        buyIndex = 0;
    }

    public void SetUI()
    {
        SetData();
        SetManaText();
        CheckCanReward();
        SetFacilityInfo();
        SetRewardInfo();
        SetManaShop(true);
    }

    public void SetData()
    {
        // bufForce 수정시 GameFuction-GetManaBuf,AdventureSlime-TimeAsLevel도 수정
        manaOre_min = 10 + SaveScript.saveData.facility_level * (SaveScript.saveData.facility_level.ToString().Length + 4);
        manaOre_max = 20 + SaveScript.saveData.facility_level * (SaveScript.saveData.facility_level.ToString().Length + 5);
        elixirNum = 1 + (SaveScript.saveData.facility_level - 10) / 5;
        reinforceItemNum = 1 + (SaveScript.saveData.facility_level - 20) / 10;
        amountForce = SaveScript.saveData.facility_level * 0.03f;
        qualityForce = SaveScript.saveData.facility_level * 1f;
        bufForce = SaveScript.saveData.facility_level * 0.03f;
    }

    public void CheckCanReward()
    {
        if(SaveScript.saveData.facility_rewardTime == 0)
            canRecieve.SetActive(true);
        else
            canRecieve.SetActive(false);
    }

    public void SetManaText()
    {
        manaOreText.text = GameFuction.GetNumText(SaveScript.saveData.manaOre);
    }

    public void SetFacilityInfo()
    {
        effectNameText.text = "펫 하우스 효과 [ Lv. " + SaveScript.saveData.facility_level + " ]";
        effectInfoTexts[0].text = "채광 펫 보상 획득량 " + Mathf.RoundToInt(amountForce * 100 * 100) / 100f + "% 증가";
        effectInfoTexts[1].text = "모험가 펫 보상 시간 " + qualityForce + "분 단축";
        effectInfoTexts[2].text = "마나석 버프 효과 " + Mathf.RoundToInt(bufForce * 100 * 100) / 100f + "% 증가 (경험치 및 강화 제외)";

        if (SaveScript.saveData.facility_level >= 20)
            rewardNameText.text = "펫 하우스 보상 [ 3단계 ]";
        else
            rewardNameText.text = "펫 하우스 보상 [" + (SaveScript.saveData.facility_level / 10 + 1) + "단계 ]";
        rewardInfoTexts[0].text = "1) 마나석 " + manaOre_min + "개 ~ " + manaOre_max + "개";
        rewardInfoTexts[0].color = Color.white;
        rewardInfoImages[0].color = new Color(0.4f, 1f, 0.6f);
        rewardLockImages[0].gameObject.SetActive(false);
        if (SaveScript.saveData.facility_level / 10 > 0)
        {
            rewardInfoTexts[1].text = "2) 랜덤 영약 " + elixirNum + "개";
            rewardInfoTexts[1].color = Color.white;
            rewardInfoImages[1].color = new Color(0.4f, 1f, 0.6f);
            rewardLockImages[1].gameObject.SetActive(false);
        }
        if (SaveScript.saveData.facility_level / 10 > 1)
        {
            rewardInfoTexts[2].text = "3) 랜덤 고급 강화 아이템 " + reinforceItemNum + "개";
            rewardInfoTexts[2].color = Color.white;
            rewardInfoImages[2].color = new Color(0.4f, 1f, 0.6f);
            rewardLockImages[2].gameObject.SetActive(false);
        }

        upgradeText.text = GameFuction.GetNumText(GetUpgradeCost());
        if (SaveScript.saveData.manaOre >= GetUpgradeCost())
            upgradeText.color = canColor;
        else
            upgradeText.color = canNotColor;
    }

    public void SetRewardInfo()
    {
        isRewardTimeSet = false;
        rewardTimeText.text = GameFuction.GetTimeText(SaveScript.saveData.facility_rewardTime);
        if (SaveScript.saveData.facility_level >= 20)
            rewardText.text = "펫 하우스 보상 [ 3단계 ]";
        else
            rewardText.text = "펫 하우스 보상 [ " + (SaveScript.saveData.facility_level / 10 + 1) + "단계 ]";
        rewardSlider.value = SaveScript.facility_rewardTime - SaveScript.saveData.facility_rewardTime;

        if (SaveScript.saveData.facility_rewardTime == 0)
        {
            rewardUI.images[0].color = Color.white;
            rewardUI.texts[0].color = new Color(0.7f, 0.4f, 0.1f, 1f);
            rewardUI.button.enabled = true;
        }
        else
        {
            rewardUI.images[0].color = new Color(1f, 1f, 1f, 0.2f);
            rewardUI.texts[0].color = new Color(0.7f, 0.4f, 0.1f, 0.3f);
            rewardUI.button.enabled = false;
        }
    }

    public void MenuButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
        {
            menuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
            Mine.instance.SetAudio(0);
        }

        SetManaShop(false);
    }

    public void SetManaShop(bool focus)
    {
        ManaShopSlot[] datas = shopContentPanel.GetComponentsInChildren<ManaShopSlot>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);
        for (int i = 0; i < menuButtonImages.Length; i++)
            menuButtonImages[i].color = Color.white;
        menuButtonImages[menuIndex].color = new Color(0.6f, 0.6f, 0.6f);
        if (!focus)
            shopContentPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        switch (menuIndex)
        {
            case 0:
                for (int i = 0; i < SaveScript.manaBufNum; i++)
                {
                    ManaShopSlot data = Instantiate(manaShopSlotPrefab, shopContentPanel).GetComponent<ManaShopSlot>();
                    data.bufImage.sprite = SaveScript.manaBufs[i].sprite;
                    data.manaText.text = GameFuction.GetNumText(SaveScript.manaBufs[i].price);
                    data.order.order = i;
                    data.button.onClick.AddListener(BuyButton);
                    if (SaveScript.saveData.manaOre >= SaveScript.manaBufs[i].price)
                        data.manaText.color = canColor;
                    else
                        data.manaText.color = canNotColor;

                    data.nameText.text = SaveScript.manaBufs[i].name;
                    // 버프 수치 출력
                    if (i / 3 == 0) // 시간
                        data.infoText.text = SaveScript.manaBufs[i].info + GameFuction.GetTimeText(GameFuction.GetManaBufForceForText(i)) + SaveScript.manaBufs[i].info2;
                    else
                        data.infoText.text = SaveScript.manaBufs[i].info + GameFuction.GetNumText(GameFuction.GetManaBufForceForText(i)) + SaveScript.manaBufs[i].info2;
                }
                break;
            case 1:
                for (int i = 0; i < SaveScript.manaItemNum; i++)
                {
                    ManaShopSlot data = Instantiate(manaShopSlotPrefab, shopContentPanel).GetComponent<ManaShopSlot>();
                    data.bufImage.sprite = SaveScript.manaItems[i].sprite;
                    data.manaText.text = GameFuction.GetNumText(SaveScript.manaItems[i].price);
                    data.order.order = i;
                    data.button.onClick.AddListener(BuyButton);
                    if (SaveScript.saveData.manaOre >= SaveScript.manaItems[i].price)
                        data.manaText.color = canColor;
                    else
                        data.manaText.color = canNotColor;

                    data.nameText.text = SaveScript.manaItems[i].name;
                    data.infoText.text = SaveScript.manaItems[i].info;
                }
                break;
            case 2:
                for (int i = 0; i < SaveScript.manaUpgradeNum; i++)
                {
                    ManaShopSlot data = Instantiate(manaShopSlotPrefab, shopContentPanel).GetComponent<ManaShopSlot>();
                    data.bufImage.sprite = SaveScript.manaUpgrades[i].sprite;
                    data.manaText.text = GameFuction.GetNumText(ManaUpgrade.GetRealPrice(i));
                    data.order.order = i;
                    data.button.onClick.AddListener(BuyButton);
                    if (SaveScript.saveData.manaOre >= ManaUpgrade.GetRealPrice(i))
                        data.manaText.color = canColor;
                    else
                        data.manaText.color = canNotColor;

                    data.nameText.text = "[ Lv." + SaveScript.saveData.manaUpgrades[i] + " ] " + SaveScript.manaUpgrades[i].name;
                    if (i == 2 || i == 5)
                        data.infoText.text = SaveScript.manaUpgrades[i].info + "\n[ 현재 증가 개수 : " + Mathf.RoundToInt(SaveScript.saveData.manaUpgrades[i] * SaveScript.manaUpgrades[i].force) + "개 ]";
                    else if (i == 12)
                        data.infoText.text = SaveScript.manaUpgrades[i].info + "\n[ 현재 증가 시간 : " + Mathf.RoundToInt(SaveScript.saveData.manaUpgrades[i] * SaveScript.manaUpgrades[i].force * 100f) / 100f + "초 ]";
                    else
                        data.infoText.text = SaveScript.manaUpgrades[i].info + "\n[ 현재 증가율 : "  + Mathf.RoundToInt(SaveScript.saveData.manaUpgrades[i] * SaveScript.manaUpgrades[i].force * 100f * 10f) / 10f + "% ]";
                }
                break;
        }
    }

    public int GetUpgradeCost()
    {
        return (SaveScript.saveData.facility_level + 1) * 10;
    }

    public void UpgradeButton()
    {
        if(SaveScript.saveData.manaOre >= GetUpgradeCost())
        {
            Mine.instance.SetAudio(4);
            SaveScript.saveData.manaOre -= GetUpgradeCost();
            SaveScript.saveData.facility_level++;
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 39 }, SaveScript.saveData.facility_level);
            SetUI();
        }
        else
        {
            Mine.instance.SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("마나석이 부족합니다.");
        }
    }

    public void GetReward()
    {
        SaveScript.saveData.facility_rewardTime = SaveScript.facility_rewardTime;
        SetRewardInfo();

        QuestCtrl.instance.SetMainQuestAmount(new int[] { 38 });
        MineRewardInfo.instance2.passClickPanel.SetActive(true);
        MineRewardInfo.instance2.rewardAnimation.SetBool("isGetReward", true);
        MineRewardInfo.instance2.rewardAnimation.Play("RewardInfo_Reward", -1, 0f);
    }

    public void BuyButton()
    {
        buyIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        switch (menuIndex)
        {
            case 0:
                if (SaveScript.saveData.manaOre >= SaveScript.manaBufs[buyIndex].price)
                {
                    // 버프 적용
                    if (SaveScript.saveData.isManaBuffOns[buyIndex / 3 * 3] || SaveScript.saveData.isManaBuffOns[buyIndex / 3 * 3 + 1]
                        || SaveScript.saveData.isManaBuffOns[buyIndex / 3 * 3 + 2])
                    {
                        Mine.instance.SetAudio(0);
                        bufEventUI.SetActive(true);
                    }
                    else
                    {
                        YesBuy();
                    }
                }
                else
                {
                    Mine.instance.SetAudio(2);
                    SystemInfoCtrl.instance.SetErrorInfo("마나석이 부족합니다.");
                }
                break;
            case 1:
                if (SaveScript.saveData.manaOre >= SaveScript.manaItems[buyIndex].price)
                {
                    // 아이템 구매
                    SaveScript.saveData.manaOre -= SaveScript.manaItems[buyIndex].price;
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { 41 });
                    Mine.instance.SetAudio(4);
                    SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.manaItems[buyIndex].name + "' 구매 완료!");
                    switch (buyIndex)
                    {
                        case 0:
                        case 1:
                        case 2: SaveScript.saveData.hasReinforceOre += SaveScript.manaItems[buyIndex].force; break;
                        case 3: SaveScript.saveData.hasReinforceItems[2]++; break;
                        case 4: SaveScript.saveData.hasReinforceItems[5]++; break;
                        case 5: SaveScript.saveData.hasReinforceItems[6]++; break;
                        case 6: SaveScript.saveData.hasReinforceItems[7]++; break;
                        case 7:
                            int index = MinerSlime.FindEmptyPetInven();
                            if (index != -1)
                            {
                                SaveScript.saveData.hasMiners[index] = 0;
                                SaveScript.saveData.hasMinerExps[index] = 0;
                                SaveScript.saveData.hasMinerLevels[index] = 1;
                            }
                            else
                            {

                                Mine.instance.SetAudio(2);
                                SystemInfoCtrl.instance.SetErrorInfo("펫 인벤토리가 꽉 찼습니다!");
                                SaveScript.saveData.manaOre += SaveScript.manaItems[buyIndex].price;
                            }
                            break;
                        case 8:
                            int index2 = AdventurerSlime.FindEmptyPetInven();
                            if (index2 != -1)
                            {
                                SaveScript.saveData.hasAdventurers[index2] = 0;
                                SaveScript.saveData.hasAdventurerExps[index2] = 0;
                                SaveScript.saveData.hasAdventurerLevels[index2] = 1;
                            }
                            else
                            {

                                Mine.instance.SetAudio(2);
                                SystemInfoCtrl.instance.SetErrorInfo("펫 인벤토리가 꽉 찼습니다!");
                                SaveScript.saveData.manaOre += SaveScript.manaItems[buyIndex].price;
                            }
                            break;
                    }
                }
                else
                {
                    Mine.instance.SetAudio(2);
                    SystemInfoCtrl.instance.SetErrorInfo("마나석이 부족합니다.");
                }
                break;
            case 2:
                if (SaveScript.saveData.manaOre >= ManaUpgrade.GetRealPrice(buyIndex))
                {
                    // 강화 구매
                    SaveScript.saveData.manaOre -= ManaUpgrade.GetRealPrice(buyIndex);
                    SaveScript.saveData.manaUpgrades[buyIndex]++;
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { 42 });
                    // 퀘스트
                    CheckQuestUpgrades();

                    if (buyIndex == 2)
                        SaveScript.maxPetReward = 100 + (int)GameFuction.GetManaUpgradeForce(2);
                    Mine.instance.SetAudio(4);
                    SystemInfoCtrl.instance.SetShowInfo("<color=#FF9696>[ Lv. " + SaveScript.saveData.manaUpgrades[buyIndex] + " ] <color=white>'" + SaveScript.manaUpgrades[buyIndex].name + "' 업그레이드 완료!");
                }
                else
                {
                    Mine.instance.SetAudio(2);
                    SystemInfoCtrl.instance.SetErrorInfo("마나석이 부족합니다.");
                }
                break;
        }

        SetUI();
    }

    public void YesBuy()
    {
        // 퀘스트 (버프)
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 40 });

        if (buyIndex / 3 == 0)
        {
            // 시간 단축
            SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.manaBufs[buyIndex].name + "' 구매 성공");
            TimeShorten();
        }
        else
        {
            // 그 외
            SystemInfoCtrl.instance.SetShowInfo("'" + SaveScript.manaBufs[buyIndex].name + "' 버프 구매 성공");
            for (int i = 0; i < 3; i++)
            {
                SaveScript.saveData.isManaBuffOns[buyIndex / 3 * 3 + i] = false;
                SaveScript.saveData.manaBufTimes[buyIndex / 3 * 3 + i] = 0;
            }
            SaveScript.saveData.isManaBuffOns[buyIndex] = true;
            SaveScript.saveData.manaBufTimes[buyIndex] = SaveScript.manaBufTime;
        }
        Mine.instance.SetAudio(4);
        SaveScript.saveData.manaOre -= SaveScript.manaBufs[buyIndex].price;
        bufEventUI.SetActive(false);
    }

    public void CloseBuy()
    {
        Mine.instance.SetAudio(0);
        bufEventUI.SetActive(false);
    }

    public void TimeShorten()
    {
        MineMap.isSetMineTime = true;
        MainScript.isSetMineTime = true;
        for (int i = 0; i < SaveScript.saveData.hasOnMiners.Length; i++)
        {
            if (SaveScript.saveData.hasOnMiners[i] != -1)
            {
                SaveScript.saveData.hasMinerTimes[i] -= GameFuction.GetManaBufForceForText(buyIndex);
                while (SaveScript.saveData.hasMinerTimes[i] <= 0)
                {
                    SaveScript.saveData.hasMinerTimes[i] += MinerSlime.GetTimeAsLevel(SaveScript.saveData.hasOnMiners[i], SaveScript.saveData.hasOnMinerLevels[i]);
                    if (SaveScript.saveData.hasMinerRewards[i] < SaveScript.maxPetReward)
                    {
                        SaveScript.saveData.hasMinerRewards[i]++;
                        MineMap.rewardMinerIndexs.Add(i);
                    }
                }
            }
        }

        for (int i = 0; i < SaveScript.saveData.hasOnAdventurers.Length; i++)
        {
            if (SaveScript.saveData.hasOnAdventurers[i] != -1)
            {
                SaveScript.saveData.hasAdventurerTimes[i] -= GameFuction.GetManaBufForceForText(buyIndex);
                while (SaveScript.saveData.hasAdventurerTimes[i] <= 0)
                {
                    SaveScript.saveData.hasAdventurerTimes[i] += AdventurerSlime.GetTimeAsLevel(SaveScript.saveData.hasOnAdventurers[i], SaveScript.saveData.hasOnAdventurerLevels[i]);
                    if (SaveScript.saveData.hasAdventurerRewards[i] < SaveScript.maxPetReward)
                    {
                        SaveScript.saveData.hasAdventurerRewards[i]++;
                        MineMap.rewardAdventurerIndexs.Add(i);
                    }
;                }
            }
        }
    }

    static public void CheckQuestUpgrades()
    {
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 67 }, SaveScript.saveData.manaUpgrades[5]);
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 75 }, SaveScript.saveData.manaUpgrades[9]);
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 85 }, SaveScript.saveData.manaUpgrades[10]);
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 95, 115 }, SaveScript.saveData.manaUpgrades[14]);
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 105 }, SaveScript.saveData.manaUpgrades[13]);
    }
}
