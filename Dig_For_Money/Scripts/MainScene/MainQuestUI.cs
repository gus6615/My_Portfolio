using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainQuestUI : MonoBehaviour
{
    public static MainQuestUI instance;
    private static int[] contentOpens = { 0, 4, 7, 9, 11, 16, 23, 31, 38, 47, 49, 63, 71, 81, 91, 101, 111, 121, 131 };
    private static string[] contentNames = {
        "< 퀘스트 시작 >", "< 상점(판매 및 구매) 알아보기 >", "< 인벤토리(강화) 알아보기 >", "< 업적 시스템 알아보기 >",
        "< 컬렉션 알아보기 >", "< 아이템 사용 알아보기 >", "< 이벤트 포탈 알아보기 >", "< 광산(펫) 알아보기 >",
        "< 펫 하우스 알아보기 >", "< 아이템 합성 알아보기 >", "< 7층 메인 퀘스트 >", "< 8층 메인 퀘스트 >",
        "< 9층 메인 퀘스트 >", "< 10층 메인 퀘스트 >", "< 11층 메인 퀘스트 >", "< 12층 메인 퀘스트 >",
        "< 13층 메인 퀘스트 >", "< 14층 메인 퀘스트 >", "< 15층 메인 퀘스트 >",
    };
    // 0 = Quest(완료), 1 = Quest(진행), 2 = Quest(미완료), 3 = Content
    private static Color[] ImageColors = { new Color(0.15f, 0.2f, 0.25f), new Color(0.3f, 0.4f, 0.5f), new Color(0.15f, 0.15f, 0.2f), new Color(0.25f, 0.15f, 0.15f) };
    private static Color[] TextColors = { new Color(0.4f, 0.4f, 0.4f), new Color(0.8f, 0.9f, 1f), new Color(0.3f, 0.3f, 0.4f), new Color(1f, 0.6f, 0.6f) };
    private static int[][] subRewards =
    {
        new int[] { 50, 100, 50, 100 }, // 나무
        new int[] { 100, 200, 100, 200 }, // 돌
        new int[] { 200, 400, 150, 300 }, // 철
        new int[] { 250, 500, 250, 500 }, // 금
        new int[] { 500, 1000, 500, 1000 }, // 다이아
        new int[] { 1000, 2000, 1000, 2000  }, // 흑요석
        new int[] { 2500, 5000, 2500, 5000 }, // 청록석
        new int[] { 5000, 10000, 5000, 10000 }, // 영혼석
        new int[] { 10000, 20000, 10000, 20000 }, // 흑마석
        new int[] { 50000, 100000, 50000, 100000 }, // 태초석
        new int[] { 200000, 400000, 200000, 400000,}, // 연옥석
        new int[] { 1000000, 2000000, 1000000, 2000000 }, // 육천석
        new int[] { 5000000, 10000000, 5000000, 10000000 }, // 천계석
        new int[] { 30000000, 60000000, 30000000, 60000000 }, // 공허석
        new int[] { 100000000, 200000000, 100000000, 200000000 }, // 서플라스
    };
    static public bool isTicketTimeSet;

    public GameObject questListPrefab, questRewardPrefab;
    public Canvas questObject;
    public GameObject subQuestObject;
    public GameObject questCanInfo, mainQuestCanInfo, subQuestCanInfo;
    public Transform mainQuestListPanel, mainQuestRewardPanel;
    public Image mainQuestImage;
    public Text mainQuestNameText, mainQuestInfoText, mainQuestInfo2Text;
    public Slider mainQuestGoalSlider;
    public Text mainQuestGoalText;
    public UIBox rewardButton;
    public GameObject subQuestContent;
    public GameObject subQuestReSetUI;
    public Sprite expSprite, reinforceSprite, manaSprite, cashSprite;
    public Text subQuestInfoText;
    private RectTransform mainQuestListRect;
    private Button[] subQuestButtons;
    private Text[] subQuestInfoTexts, subQuestGoalTexts, subQuestReceiveTexts;
    private Image[] subQuestReceiveImages;
    public Text ticketNumText, ticketTimeText;

    private bool isQuestUIOn, isSubQuestUIOn;

    private int ListIndex;
    public int listIndex
    {
        get { return ListIndex; }
        set 
        { 
            ListIndex = value;
            if (ListIndex >= SaveScript.mainQuestNum)
                ListIndex = SaveScript.mainQuestNum - 1;
        }
    }

    MainQuest mainQuest;
    SubQuest subQuest;
    UIBox UIBox;
    UIBox[] UIBoxes;
    SpriteRenderer[] sprites;
    Text[] texts;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainQuestListRect = mainQuestListPanel.GetComponent<RectTransform>();
        subQuestButtons = subQuestContent.GetComponentsInChildren<Button>();
        subQuestInfoTexts = new Text[subQuestButtons.Length];
        subQuestGoalTexts = new Text[subQuestButtons.Length];
        subQuestReceiveTexts = new Text[subQuestButtons.Length];
        subQuestReceiveImages = new Image[subQuestButtons.Length];

        texts = subQuestContent.GetComponentsInChildren<Text>();
        sprites = subQuestContent.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < subQuestButtons.Length; i++)
        {
            subQuestInfoTexts[i] = texts[3 * i];
            subQuestGoalTexts[i] = texts[3 * i + 1];
            subQuestReceiveTexts[i] = texts[3 * i + 2];
            subQuestReceiveImages[i] = sprites[i].GetComponent<Image>();
        }

        questObject.gameObject.SetActive(false);
        subQuestObject.SetActive(false);
        SetCanInfoActive();
        SetTicketInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTicketTimeSet) 
            SetTicketInfo();

        // 광고 관련
        if (GoogleAd.isReward)
        {
            switch (GoogleAd.ADType)
            {
                case 2:
                    ReSetSubQuest();
                    GoogleAd.isReward = false;
                    break;
            }
        }
        else
        {
            if (GoogleAd.ADType == -1)
            {
                subQuestReSetUI.SetActive(false);
                GoogleAd.ADType = -2;
            }
        }
    }

    public bool CheckMainQuest()
    {
        return SaveScript.saveData.mainQuest_list < SaveScript.mainQuestNum && SaveScript.saveData.mainQuest_goal >= SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal;
    }

    public bool CheckSubQuest()
    {
        // 예외성 처리
        for (int i = 0; i < SaveScript.hasQuestNum; i++)
        {
            if (SaveScript.saveData.quastLists[i] >= SaveScript.quests[SaveScript.saveData.quastLevels[i]].Length)
            {
                SaveScript.Setquest();
                break;
            }
        }

        // 퀘스트 처리
        for (int i = 0; i < SaveScript.hasQuestNum; i++)
            if (!SaveScript.saveData.quastSuccesses[i])
                if (SaveScript.saveData.quastGoals[i] >= SaveScript.quests[SaveScript.saveData.quastLevels[i]][SaveScript.saveData.quastLists[i]].goal)
                    return true;
        return false;
    }

    public void SetCanInfoActive()
    {
        bool isMainDone = CheckMainQuest(), isSubDone = CheckSubQuest();
        questCanInfo.SetActive(isMainDone || isSubDone);
        mainQuestCanInfo.SetActive(isMainDone);
        subQuestCanInfo.SetActive(isSubDone);
    }

    public void OnOffQuestUI()
    {
        listIndex = SaveScript.saveData.mainQuest_list;
        isQuestUIOn = !isQuestUIOn;
        questObject.gameObject.SetActive(isQuestUIOn);
        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        SetTicketInfo();
        SetQuestListUI();
        SetQuestContent(listIndex);
    }

    public void SetQuestListUI()
    {
        UIBoxes = mainQuestListPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < UIBoxes.Length; i++)
            Destroy(UIBoxes[i].gameObject);

        for (int i = 0; i < SaveScript.mainQuestNum; i++)
        {
            int type;
            if ((type = CheckContent(i)) != -1)
            {
                // Content 추가 부분
                UIBox = Instantiate(questListPrefab, mainQuestListPanel).GetComponent<UIBox>();
                UIBox.images[0].color = ImageColors[3];
                UIBox.images[1].gameObject.SetActive(false);
                UIBox.texts[0].color = TextColors[3];
                UIBox.texts[0].text = contentNames[type];
                UIBox.button.enabled = false;
            }

            if (i < SaveScript.saveData.mainQuest_list)
                type = 0; // 완료
            else if (i > SaveScript.saveData.mainQuest_list)
                type = 2; // 미완료
            else
                type = 1; // 진행중
            UIBox = Instantiate(questListPrefab, mainQuestListPanel).GetComponent<UIBox>();
            UIBox.images[0].color = ImageColors[type];
            UIBox.texts[0].color = TextColors[type];
            UIBox.texts[0].text = SaveScript.mainQuests[i].name;
            UIBox.order = i;
            UIBox.button.onClick.AddListener(OnMainQuestList);
            if (type == 0)
                UIBox.images[1].gameObject.SetActive(true);
            else
                UIBox.images[1].gameObject.SetActive(false);
        }

        int listNum = SaveScript.saveData.mainQuest_list;
        for (int i = 0; i < contentOpens.Length; i++)
            if (contentOpens[i] < SaveScript.saveData.mainQuest_list)
                listNum++;
        mainQuestListRect.anchoredPosition = new Vector2(0, 80f * listNum);
    }

    private void SetQuestContent(int _list)
    {
        mainQuest = SaveScript.mainQuests[_list];
        mainQuestImage.sprite = mainQuest.sprite;
        mainQuestNameText.text = "[ " + mainQuest.name + " ]";
        mainQuestInfoText.text = mainQuest.info;
        mainQuestInfo2Text.text = mainQuest.info2;
        mainQuestGoalSlider.maxValue = mainQuest.goal;
        mainQuestGoalSlider.value = SaveScript.saveData.mainQuest_goal;
        SetCanInfoActive();

        if (_list < SaveScript.saveData.mainQuest_list)
        {
            // 완료된 상태
            mainQuestNameText.text = "[ " + mainQuest.name + " (완료) ]";
            mainQuestGoalSlider.value = mainQuestGoalSlider.maxValue;
            rewardButton.button.enabled = false;
            rewardButton.images[0].color = rewardButton.texts[0].color = new Color(1f, 1f, 1f, 0.4f);
            rewardButton.texts[0].text = "보상 완료";
            mainQuestCanInfo.SetActive(false);
        }
        else if (_list > SaveScript.saveData.mainQuest_list)
        {
            // 이후 퀘스트들
            mainQuestNameText.text = "[ " + mainQuest.name + " (진행 불가) ]";
            mainQuestGoalSlider.value = 0;
            rewardButton.button.enabled = false;
            rewardButton.images[0].color = rewardButton.texts[0].color = new Color(1f, 1f, 1f, 0.4f);
            rewardButton.texts[0].text = "보상 불가";
            mainQuestCanInfo.SetActive(false);
        }
        else
        {
            // 진행형 퀘스트
            rewardButton.button.enabled = CheckMainQuest();
            if (CheckMainQuest())
            {
                rewardButton.images[0].color = rewardButton.texts[0].color = Color.white;
                rewardButton.texts[0].text = "보상 받기";
            }
            else
            {
                rewardButton.images[0].color = rewardButton.texts[0].color = new Color(1f, 1f, 1f, 0.4f);
                rewardButton.texts[0].text = "진행중";
            }
        }

        mainQuestGoalText.text = "( " + GameFuction.GetNumText((long)mainQuestGoalSlider.value) + " / " + GameFuction.GetNumText((long)mainQuestGoalSlider.maxValue) + " )";

        // 보상 관련
        UIBoxes = mainQuestRewardPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < UIBoxes.Length; i++)
            Destroy(UIBoxes[i].gameObject);

        for (int i = 0; i < mainQuest.reward.Length; i++)
        {
            if (mainQuest.reward[i] != 0)
            {
                UIBox = Instantiate(questRewardPrefab, mainQuestRewardPanel).GetComponent<UIBox>();
                UIBox.texts[0].text = "x " + GameFuction.GetNumText(mainQuest.reward[i]);
                switch (i)
                {
                    case 0: UIBox.images[0].sprite = expSprite; break;
                    case 1: UIBox.images[0].sprite = reinforceSprite; break;
                    case 2: UIBox.images[0].sprite = manaSprite; break;
                    case 3: UIBox.images[0].sprite = cashSprite; break;
                }
            }
        }
    }

    private void OnMainQuestList()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>() != null)
        {
            listIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
            if (MainScript.instance != null) MainScript.instance.SetAudio(0);
            else PrintUI.instance.AudioPlay(0);
        }
        SetQuestContent(listIndex);
    }

    private int CheckContent(int _list)
    {
        for (int i = 0; i < contentOpens.Length; i++)
            if (contentOpens[i] == _list)
                return i;
        return -1;
    }

    public void OnMainQuestReward()
    {
        if (!CheckMainQuest())
            return;

        mainQuest = SaveScript.mainQuests[SaveScript.saveData.mainQuest_list];
        for (int i = 0; i < mainQuest.reward.Length; i++)
        {
            if (mainQuest.reward[i] > 0)
            {
                switch (i)
                {
                    case 0: 
                        SaveScript.saveData.exp += mainQuest.reward[i];
                        AchievementCtrl.instance.SetAchievementAmount(21, mainQuest.reward[i]);
                        break;
                    case 1: 
                        SaveScript.saveData.hasReinforceOre += mainQuest.reward[i];
                        AchievementCtrl.instance.SetAchievementAmount(22, mainQuest.reward[i]);
                        break;
                    case 2: 
                        SaveScript.saveData.manaOre += mainQuest.reward[i];
                        AchievementCtrl.instance.SetAchievementAmount(23, mainQuest.reward[i]);
                        break;
                    case 3: 
                        SaveScript.saveData.cash += mainQuest.reward[i];
                        AchievementCtrl.instance.SetAchievementAmount(24, mainQuest.reward[i]);
                        break;
                }
            }
        }

        SaveScript.saveData.mainQuest_goal = 0;
        listIndex = ++SaveScript.saveData.mainQuest_list;

        // 미리 해결될 수 있는 퀘스트들 체크
        QuestCtrl.CheckAllQuest();

        SetQuestListUI();
        SetQuestContent(listIndex);
        QuestCtrl.instance.questIsPrint = false;
        if (MainScript.instance != null)
        {
            MainScript.instance.SetBasicInfo();
            MainScript.instance.SetAudio(4);
        }
        else
        {
            PrintUI.instance.AudioPlay(4);
        }
    }

    public void OnOffSubQuest()
    {
        isSubQuestUIOn = !isSubQuestUIOn;
        subQuestObject.SetActive(isSubQuestUIOn);
        subQuestReSetUI.SetActive(false);
        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        SetSubQuestUI();
    }

    public void OnSubQuestReward()
    {
        int index = 0;
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            index = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        subQuest = SaveScript.quests[SaveScript.saveData.quastLevels[index]][SaveScript.saveData.quastLists[index]];

        if (!SaveScript.saveData.quastSuccesses[index] && SaveScript.saveData.quastGoals[index] >= subQuest.goal)
        {
            SaveScript.saveData.quastSuccesses[index] = true;
            if (index == 0 || index == 1)
            {
                SaveScript.saveData.exp += subRewards[SaveScript.saveData.pickLevel][index];
                AchievementCtrl.instance.SetAchievementAmount(21, subRewards[SaveScript.saveData.pickLevel][index]);
            }
            else
            {
                SaveScript.saveData.hasReinforceOre += subRewards[SaveScript.saveData.pickLevel][index];
                AchievementCtrl.instance.SetAchievementAmount(22, subRewards[SaveScript.saveData.pickLevel][index]);
            }

            QuestCtrl.instance.SetMainQuestAmount(new int[] { 28 });
            SetQuestContent(listIndex);
            SetSubQuestUI();

            if (MainScript.instance != null)
            {
                MainScript.instance.SetAudio(4);
                MainScript.instance.SetGoldAndEXPText();
                MainAchievementUI.instance.SetReceiveCanInfo();
            }
            else PrintUI.instance.AudioPlay(4);
        }
    }

    public void SetSubQuestUI()
    {
        int count = 0;

        for (int i = 0; i < subQuestButtons.Length; i++)
        {
            if (!SaveScript.saveData.quastSuccesses[i])
            {
                subQuest = SaveScript.quests[SaveScript.saveData.quastLevels[i]][SaveScript.saveData.quastLists[i]];
                subQuestButtons[count].GetComponent<Order>().order = i;
                subQuestInfoTexts[count].text = subQuest.info;
                subQuestGoalTexts[count].text = "[ " + GameFuction.GetNumText(SaveScript.saveData.quastGoals[i]) + " / " + GameFuction.GetNumText(subQuest.goal) + " ]";
                subQuestReceiveTexts[count].text = GameFuction.GetNumText(subRewards[SaveScript.saveData.pickLevel][i]);
                subQuestReceiveImages[count].gameObject.SetActive(true);
                if (i == 0 || i == 1)
                    subQuestReceiveImages[count].sprite = expSprite;
                else
                    subQuestReceiveImages[count].sprite = reinforceSprite;

                if (SaveScript.saveData.quastGoals[i] >= subQuest.goal)
                    subQuestButtons[count].GetComponent<Image>().color = new Color(1f, 1f, 0.5f, 0.9f);
                else
                    subQuestButtons[count].GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.9f);
                count++;
            }
        }

        for (int i = count; i < subQuestButtons.Length; i++)
        {
            subQuestInfoTexts[i].text = "";
            subQuestGoalTexts[i].text = "";
            subQuestReceiveTexts[i].text = "";
            subQuestReceiveImages[i].gameObject.SetActive(false);
            subQuestButtons[i].GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.4f);
        }

        SetCanInfoActive();
    }

    public void OnOpenReSetSubQuest()
    {
        if (SaveScript.saveData.ticketNum <= 0)
        {
            if (MainScript.instance != null)
                MainScript.instance.SetAudio(2);
            else
                PrintUI.instance.AudioPlay(2);
            SystemInfoCtrl.instance.SetErrorInfo("퀘스트 갱신 횟수를 모두 소모하였습니다!");
            return;
        }

        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        subQuestReSetUI.SetActive(true);
        if (SaveScript.saveData.isRemoveAD)
            subQuestInfoText.text = "퀘스트를 초기화하시겠습니까?";
        else
            subQuestInfoText.text = "퀘스트를 초기화하시겠습니까?\n[광고 시청]";
    }

    public void OnReSetSubQuest()
    {
        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        GoogleAd.instance.ADShow(2);
    }

    public void ReSetSubQuest()
    {
        if (SaveScript.saveData.ticketNum == SaveScript.ticketNum)
            SaveScript.saveData.ticketTime = SaveScript.ticketTime;
        SaveScript.saveData.ticketNum--;
        
        SaveScript.Setquest();
        SetTicketInfo();
        SetSubQuestUI();
        subQuestReSetUI.SetActive(false);
    }

    public void OnCloseReSetSubQuest()
    {
        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        subQuestReSetUI.SetActive(false);
    }

    public void SetTicketInfo()
    {
        isTicketTimeSet = false;
        ticketNumText.text = "퀘스트 갱신 가능 횟수 : " + SaveScript.saveData.ticketNum + " 회";
        ticketTimeText.text = "추가 1 회 까지 < " + GameFuction.GetTimeText(SaveScript.saveData.ticketTime) + " > 남음";
    }
}
