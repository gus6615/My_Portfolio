using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainAchievementUI : MonoBehaviour
{
    public static MainAchievementUI instance;
    public Canvas achievementCanvas;
    public Animator animator;
    public GameObject infoObject;
    public GameObject[] infoRewards;
    public Text infoText;
    public Image tierImage, tierReward;
    public Text tierName, tierInfo, tierSliderText, tierRewardText;
    public Slider tierSlider;
    public GameObject tierCanInfo, tierRewardCanInfo;
    public GameObject receiveAllButton;
    public Transform contentPanel;
    public GameObject slotPrefab;

    Achievement_slot[] datas; // 임시 데이터
    Achievement_slot slot;
    Order rewardOrder;
    Button rewardButton;
    private bool isAchievementUIOn;
    private int menuIndex, AchievementIndex;
    private int currentAchievement = 0;
    private int[] rewards = { 0, 0, 0, 0 };
    private int[] cashes = { 30, 50, 70, 100, 150, 200, 250, 400 };

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        achievementCanvas.gameObject.SetActive(false);
        infoObject.SetActive(false);
        rewardOrder = tierReward.GetComponent<Order>();
        rewardButton = tierReward.GetComponent<Button>();
    }

    /// <summary>
    /// 업적 UI 끄고 닫기
    /// </summary>
    public void OnOffAchievement()
    {
        isAchievementUIOn = !isAchievementUIOn;
        achievementCanvas.gameObject.SetActive(isAchievementUIOn);
        MainScript.instance.SetAudio(0);
        if (isAchievementUIOn)
        {
            menuIndex = 0;
            SetReceiveAllButton();
            SetTierInfo();
            MenuButton();
        }
    }

    private void SetInfo(string info)
    {
        animator.SetBool("isInfo", true);
        animator.Play("Achievement_Info", -1, 0f);
        infoObject.SetActive(true);
        infoText.text = info;
        for (int i = 0; i < rewards.Length; i++)
        {
            if (rewards[i] == 0)
                infoRewards[i].SetActive(false);
            else
            {
                infoRewards[i].SetActive(true);
                infoRewards[i].GetComponentInChildren<Text>().text = GameFuction.GetNumText(rewards[i]);
            }
;
        }
        ClearInfoValue();
    }

    public void ClearInfoValue()
    {
        currentAchievement = 0;
        for (int i = 0; i < rewards.Length; i++)
            rewards[i] = 0;
    }

    /// <summary>
    /// 업적 티어와 관련된 내용 설정
    /// </summary>
    public void SetTierInfo()
    {
        if (SaveScript.saveData.tier_level < Achievement.TIER_MAX)
        {
            int leftAchievement = Achievement.TIER_ACHIEVEMENT * (SaveScript.saveData.tier_level + 1) - SaveScript.saveData.tier_achievement;
            if (leftAchievement < 0) leftAchievement = 0;

            tierImage.sprite = AchievementCtrl.instance.tierSprites[SaveScript.saveData.tier_level];
            tierName.text = AchievementCtrl.tierNames[SaveScript.saveData.tier_level];
            tierInfo.text = "[" + AchievementCtrl.tierNames[SaveScript.saveData.tier_level + 1] + "] 티어까지 앞으로 '" + leftAchievement + "'개 업적이 남았습니다.";
            tierSlider.maxValue = (SaveScript.saveData.tier_level + 1) * Achievement.TIER_ACHIEVEMENT;
            tierSlider.value = SaveScript.saveData.tier_achievement;
            tierSliderText.text = "( " + tierSlider.value + " / " + tierSlider.maxValue + " )";
            tierRewardText.text = "x " + cashes[SaveScript.saveData.tier_level / 5];

            if (tierSlider.value >= tierSlider.maxValue)
            {
                tierRewardCanInfo.SetActive(true);
                rewardButton.enabled = true;
                rewardOrder.order = SaveScript.saveData.tier_level / 5;
            }
            else
            {
                tierRewardCanInfo.SetActive(false);
                rewardButton.enabled = false;
            }
        }
        else
        {
            tierImage.sprite = AchievementCtrl.instance.tierSprites[Achievement.TIER_MAX];
            tierName.text = AchievementCtrl.tierNames[SaveScript.saveData.tier_level];
            tierInfo.text = "최고 업적 단계에 도달하셨습니다!";
            tierSlider.value = tierSlider.maxValue = 1;
            tierSliderText.text = "( " + SaveScript.saveData.tier_achievement + " )";
            tierRewardText.text = "";
            tierReward.color = new Color(0f, 0f, 0f, 0f);
            tierRewardCanInfo.SetActive(false);
            rewardButton.enabled = false;
        }
    }

    /// <summary>
    /// 메인 - CanInfo 설정
    /// </summary>
    public void SetReceiveCanInfo()
    {
        tierCanInfo.SetActive(false);
        for (int i = 0; i < SaveScript.achievementNum; i++)
        {
            if (SaveScript.saveData.achievementLevels[i] == Achievement.TIER_ACHIEVEMENT) continue;

            if (i == 20 || i == 29)
            {
                if (SaveScript.saveData.achievementLevels[i] >= 80)
                {
                    if (GameFuction.CheckCanBuy(0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                    {
                        tierCanInfo.SetActive(true);
                        break;
                    }
                }
                else if (SaveScript.saveData.achievementLevels[i] >= 40)
                {
                    if (GameFuction.CheckCanBuy(0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                    {
                        tierCanInfo.SetActive(true);
                        break;
                    }
                }
                else
                {
                    if (GameFuction.CheckCanBuy(AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, 0, SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                    {
                        tierCanInfo.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                if (SaveScript.saveData.achievementAmounts[i] >= AchievementCtrl.instance.achievementGoalsAsLevel[i])
                {
                    tierCanInfo.SetActive(true);
                    break;
                }

            }
        }

        if (SaveScript.saveData.tier_level != Achievement.TIER_MAX && SaveScript.saveData.tier_achievement >= (SaveScript.saveData.tier_level + 1) * Achievement.TIER_ACHIEVEMENT)
            tierCanInfo.SetActive(true);
    }

    /// <summary>
    /// 모두 수령 버튼 체크
    /// </summary>
    public void SetReceiveAllButton()
    {
        receiveAllButton.SetActive(false);
        for (int i = 0; i < SaveScript.achievementNum; i++)
        {
            if (SaveScript.saveData.achievementLevels[i] == Achievement.TIER_ACHIEVEMENT) continue;

            if (i == 20 || i == 29)
            {
                if (SaveScript.saveData.achievementLevels[i] >= 80)
                {
                    if (GameFuction.CheckCanBuy(0, 0, AchievementCtrl.instance.achievementGoalsAsLevel[i], SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                    {
                        receiveAllButton.SetActive(true);
                        break;
                    }
                }
                else if (SaveScript.saveData.achievementLevels[i] >= 40)
                {
                    if (GameFuction.CheckCanBuy(0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                    {
                        receiveAllButton.SetActive(true);
                        break;
                    }
                }
                else
                {
                    if (GameFuction.CheckCanBuy(AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, 0, SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                    {
                        receiveAllButton.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                if (SaveScript.saveData.achievementAmounts[i] >= AchievementCtrl.instance.achievementGoalsAsLevel[i])
                {
                    receiveAllButton.SetActive(true);
                    break;
                }

            }
        }
    }

    /// <summary>
    /// 메뉴 버튼
    /// </summary>
    public void MenuButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
        {
            menuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
            MainScript.instance.SetAudio(0);
        }

        contentPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        SetMenu();
    }

    public void SetMenu()
    {
        // 초기화
        datas = contentPanel.GetComponentsInChildren<Achievement_slot>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);

        int minIndex = 0;
        for (int i = 0; i < menuIndex; i++)
            minIndex += Achievement.indexAsMenu[i];

        for (int i = minIndex; i < minIndex + Achievement.indexAsMenu[menuIndex]; i++)
        {
            slot = Instantiate(slotPrefab, contentPanel).GetComponent<Achievement_slot>();
            slot.image.sprite = Achievement.sprites[i];
            slot.infoText.text = Achievement.infos[i];

            if (SaveScript.saveData.achievementLevels[i] >= Achievement.TIER_ACHIEVEMENT)
            {
                slot.nameText.text = "[MASTER] " + Achievement.names[i];
                if (i == 20 || i == 29)
                    slot.sliderText.text = "( " + GameFuction.GetGoldText(SaveScript.saveData.achievementAmounts[i]
                        ,SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)], 0) + " / 0 )";
                else
                    slot.sliderText.text = "( " + GameFuction.GetNumText(SaveScript.saveData.achievementAmounts[i]) + " / 0 )";
                slot.slider.maxValue = 1;
                slot.slider.value = 1;
                for (int j = 0; j < 3; j++)
                    slot.rewardObjects[j].SetActive(false);
                slot.rewardButton.gameObject.SetActive(false);
            }
            else
            {
                slot.nameText.text = "[Lv." + SaveScript.saveData.achievementLevels[i] + "] " + Achievement.names[i];
                slot.slider.maxValue = AchievementCtrl.instance.achievementGoalsAsLevel[i];
                slot.slider.value = SaveScript.saveData.achievementAmounts[i];
                slot.sliderText.text = "( " + GameFuction.GetNumText(SaveScript.saveData.achievementAmounts[i]) + " / " 
                    + GameFuction.GetNumText(AchievementCtrl.instance.achievementGoalsAsLevel[i]) + " )";
                slot.rewardButton.order = i;
                slot.rewardButton.GetComponent<Button>().onClick.AddListener(ReceiveButton);

                for (int j = 0; j < 3; j++)
                    slot.rewardObjects[j].GetComponentInChildren<Text>().text = GameFuction.GetNumText(GetRewardValue(i, j));
                if (SaveScript.saveData.achievementLevels[i] / 10 <= 1 || SaveScript.saveData.achievementLevels[i] == 20)
                    slot.rewardObjects[2].SetActive(false);
                if (SaveScript.saveData.achievementLevels[i] / 10 <= 0 || SaveScript.saveData.achievementLevels[i] == 10)
                    slot.rewardObjects[1].SetActive(false);

                if (SaveScript.saveData.achievementAmounts[i] < AchievementCtrl.instance.achievementGoalsAsLevel[i])
                {
                    slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f);
                    slot.rewardButton.GetComponent<Button>().enabled = false;
                }

                // 예외 상황
                if (i == 20 || i == 29)
                {
                    if (SaveScript.saveData.achievementLevels[i] >= 80)
                    {
                        slot.slider.value = SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)];
                        slot.sliderText.text = "( " + GameFuction.GetGoldText(SaveScript.saveData.achievementAmounts[i]
                            , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                            , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)], 0) + " / "
                            + GameFuction.GetGoldText(0, 0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0) + " )";
                        if (GameFuction.CheckCanBuy(0, 0, AchievementCtrl.instance.achievementGoalsAsLevel[i], SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                        {
                            slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            slot.rewardButton.GetComponent<Button>().enabled = true;
                        }
                        else
                        {
                            slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f);
                            slot.rewardButton.GetComponent<Button>().enabled = false;
                        }
                    }
                    else if (SaveScript.saveData.achievementLevels[i] >= 40)
                    {
                        slot.slider.value = SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)];
                        slot.sliderText.text = "( " + GameFuction.GetGoldText(SaveScript.saveData.achievementAmounts[i]
                            , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                            , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)], 0) + " / "
                            + GameFuction.GetGoldText(0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, 0) + " )";
                        if (GameFuction.CheckCanBuy(0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, SaveScript.saveData.achievementAmounts[i]
                        , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                        , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                        {
                            slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            slot.rewardButton.GetComponent<Button>().enabled = true;
                        }
                        else
                        {
                            slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f);
                            slot.rewardButton.GetComponent<Button>().enabled = false;
                        }
                    }
                    else
                    {
                        slot.sliderText.text = "( " + GameFuction.GetGoldText(SaveScript.saveData.achievementAmounts[i]
                            , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                            , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)], 0) + " / "
                            + GameFuction.GetGoldText(AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, 0, 0) + " )";
                        if (GameFuction.CheckCanBuy(AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, 0, SaveScript.saveData.achievementAmounts[i]
                            , SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)]
                            , SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                        {
                            slot.slider.value = slot.slider.maxValue;
                            slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            slot.rewardButton.GetComponent<Button>().enabled = true;
                        }
                        else
                        {
                            slot.rewardButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f);
                            slot.rewardButton.GetComponent<Button>().enabled = false;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 모든 업적 보상 받기
    /// </summary>
    public void ReceiveAllButton()
    {
        MainScript.instance.SetAudio(4);

        for (int i = 0; i < SaveScript.achievementNum; i++)
        {
            if (i == 20 || i == 29)
            {
                while (SaveScript.saveData.achievementLevels[i] < 40 && GameFuction.CheckCanBuy(AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, 0, SaveScript.saveData.achievementAmounts[i],
                    SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)], SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                {
                    ReceiveReward(i);
                }
                while (SaveScript.saveData.achievementLevels[i] < 80 && GameFuction.CheckCanBuy(0, AchievementCtrl.instance.achievementGoalsAsLevel[i], 0, SaveScript.saveData.achievementAmounts[i],
                    SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)], SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                {
                    ReceiveReward(i);
                }
                while (SaveScript.saveData.achievementLevels[i] >= 80 && GameFuction.CheckCanBuy(0, 0, AchievementCtrl.instance.achievementGoalsAsLevel[i], SaveScript.saveData.achievementAmounts[i],
                    SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(i)], SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(i)]))
                {
                    if (SaveScript.saveData.achievementLevels[i] == Achievement.TIER_ACHIEVEMENT) break;
                    ReceiveReward(i);
                }
            }
            else
            {
                while (SaveScript.saveData.achievementAmounts[i] >= AchievementCtrl.instance.achievementGoalsAsLevel[i])
                {
                    if (SaveScript.saveData.achievementLevels[i] == Achievement.TIER_ACHIEVEMENT) break;
                    ReceiveReward(i);
                }
            }
        }

        SetReceiveAllButton();
        SetReceiveCanInfo();
        SetTierInfo();
        SetMenu();
        SetInfo("< " + currentAchievement + " > 개 업적을 달성하였습니다.");
        MainScript.instance.SetGoldAndEXPText();
    }

    /// <summary>
    /// 특정 업적 보상 받기 버튼
    /// </summary>
    public void ReceiveButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            AchievementIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        MainScript.instance.SetAudio(4);

        ReceiveReward(AchievementIndex);
        SetReceiveAllButton();
        SetReceiveCanInfo();
        SetTierInfo();
        SetMenu();
        SetInfo("< " + currentAchievement + " > 개 업적을 달성하였습니다.");
        MainScript.instance.SetGoldAndEXPText();
    }

    /// <summary>
    /// 업적 보상 얻기
    /// </summary>
    /// <param name="type">업적의 종류</param>
    private void ReceiveReward(int type)
    {
        currentAchievement++;
        for (int i = 0; i < rewards.Length; i++)
            rewards[i] += GetRewardValue(type, i);

        if (type != 29)
        {
            if (type == 20)
            {
                if (SaveScript.saveData.achievementLevels[type] >= 80) // 루니
                    GameFuction.Buy(0, 0, AchievementCtrl.instance.achievementGoalsAsLevel[type], 
                        ref SaveScript.saveData.achievementAmounts[type],
                        ref SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(type)],
                        ref SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(type)]);
                else if (SaveScript.saveData.achievementLevels[type] >= 40) // 경
                    GameFuction.Buy(0, AchievementCtrl.instance.achievementGoalsAsLevel[type], 0,
                        ref SaveScript.saveData.achievementAmounts[type],
                        ref SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(type)],
                        ref SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(type)]);
                else
                    GameFuction.Buy(AchievementCtrl.instance.achievementGoalsAsLevel[type], 0, 0,
                        ref SaveScript.saveData.achievementAmounts[type],
                        ref SaveScript.saveData.achievementAmount2[AchievementCtrl.instance.GetSecondAmountIndex(type)],
                        ref SaveScript.saveData.achievementAmount3[AchievementCtrl.instance.GetSecondAmountIndex(type)]);
            }
            else
                SaveScript.saveData.achievementAmounts[type] -= AchievementCtrl.instance.achievementGoalsAsLevel[type];
        }
        SaveScript.saveData.exp += GetRewardValue(type, 0);
        SaveScript.saveData.hasReinforceOre += GetRewardValue(type, 1);
        SaveScript.saveData.manaOre += GetRewardValue(type, 2);
        SaveScript.saveData.achievementLevels[type]++;
        SaveScript.saveData.tier_achievement++;
        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 9 });

        AchievementCtrl.instance.SetAchievementGoal(type);
        AchievementCtrl.instance.achievementIsPrint[type] = false;
    }

    /// <summary>
    /// 보상 수치를 반환하는 함수
    /// </summary>
    /// <param name="achievement_type">업적 종류</param>
    /// <param name="reward_type">리턴할 보상 종류</param>
    /// <returns></returns>
    private int GetRewardValue(int achievement_type, int reward_type)
    {
        int rval = 0;
        switch (reward_type)
        {
            case 0:
            case 1:
                for (int i = 0; i < SaveScript.saveData.achievementLevels[achievement_type]; i++)
                    rval += Achievement.rewards[reward_type][i / 10];
                if (reward_type == 0 && SaveScript.saveData.achievementLevels[achievement_type] == 0)
                    rval = 5;
                break;
            case 2:
                for (int i = 0; i < SaveScript.saveData.achievementLevels[achievement_type]; i++)
                    rval += Achievement.rewards[reward_type][i / 10];
                if (SaveScript.saveData.achievementLevels[achievement_type] == 20)
                    rval = Achievement.rewards[reward_type][SaveScript.saveData.achievementLevels[achievement_type] / 10];
                break;
            case 3: // 사용하지 않음
                break;
        }

        return rval;
    }

    /// <summary>
    /// 티어 보상 받기 버튼
    /// </summary>
    public void TierRewardButton()
    {
        if (tierSlider.value == tierSlider.maxValue)
        {
            MainScript.instance.SetAudio(4);
            rewards[3] += cashes[SaveScript.saveData.tier_level / 5];
            SetInfo("< " + AchievementCtrl.tierNames[SaveScript.saveData.tier_level + 1] + " > 로 승급하셨습니다!");
            SaveScript.saveData.cash += cashes[SaveScript.saveData.tier_level / 5];
            SaveScript.saveData.tier_level++;
            // 퀘스트
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 10 });

            SetTierInfo();
            SetReceiveCanInfo();
        }
    }
}
