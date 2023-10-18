using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    static public MainScript instance;
    static public bool isGotoGameScene, isGotoShopScene, isGotoUpgradeScene, isGotoMineScene;
    static public bool isSetMineTime;
    static public bool isChangeScene; // 현재 씬이 변경중인가?

    private const float CREATE_CLOUD_TIME = 30f;
    private const int CLICK_BACKCOUNT = 5;

    public Animator gameStartAnimator, plusMenuAnimator;
    public Animator playerAnimator;
    public FadeEffect gameStartButton, questButton, shopButton, invenButton, achievementButton, collectionButton, mineButton;
    public Text goldText, expText;
    public Text GotoGameText;
    public GameObject mineCanInfo;
    public new AudioSource audio;
    public Image[] modeImages;
    public GameObject[] modeRocks;

    public Transform mapObject;
    public GameObject cloudPrefab;
    public GameObject skyObject;
    public Image skyBlackImage;

    public GameObject modeUIObject;
    private bool isPlusMenuOn;
    private bool isModeUIOn;

    private bool isCreateCloud;
    private bool isSetSky;
    private int currentMin;
    private int clickBackCount;

    private Cloud cloud;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isChangeScene = false;
        isSetSky = true;

        modeUIObject.SetActive(false);

        SetBasicInfo();
        SetMineInfo();

        // 구름 2개 랜덤 생성
        cloud = Instantiate(cloudPrefab, mapObject).GetComponent<Cloud>();
        cloud.SetPosition(new Vector3(Random.Range(-6.75f, -6.25f), Random.Range(2.75f, 3.25f), 0f));

        cloud = Instantiate(cloudPrefab, mapObject).GetComponent<Cloud>();
        cloud.SetPosition(new Vector3(Random.Range(3f, 3.5f), Random.Range(2f, 2.5f), 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (isSetMineTime) 
            SetMineInfo();
        if (!isCreateCloud)
            StartCoroutine(CreateCloud());
        if (!isSetSky)
            StartCoroutine(SetSky());
    }

    IEnumerator SetSky()
    {
        // 24 * 60, 즉 24시간을 기준으로 한 바퀴를 돈다
        skyObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(0f, 360f, currentMin / 1440f)));
        if (currentMin > 720f)
            skyBlackImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.6f, (currentMin - 720f) / 720f));
        else
            skyBlackImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0.6f, 0f, currentMin / 720f));
        isSetSky = true;

        yield return new WaitForSeconds(60f);

        currentMin++;
        isSetSky = false;
    }

    IEnumerator CreateCloud()
    {
        isCreateCloud = true;
        Instantiate(cloudPrefab, mapObject);

        yield return new WaitForSeconds(CREATE_CLOUD_TIME);

        isCreateCloud = false;
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    public void SetGoldAndEXPText()
    {
        goldText.text = GameFuction.GetGoldText(SaveScript.saveData.gold, SaveScript.saveData.gold2, SaveScript.saveData.gold3, SaveScript.saveData.gold4);
        expText.text = GameFuction.GetNumText(SaveScript.saveData.exp);
    }

    public void SetBasicInfo()
    {
        audio.mute = !SaveScript.saveData.isSEOn;
        currentMin = SaveScript.dateTime.Hour * 60 + SaveScript.dateTime.Minute;
        isSetSky = false;
        clickBackCount = 0;

        SetTutorial();
        SetGoldAndEXPText();
        SetMineInfo();
        SetModeUI();
        IconInfoUI.instance.InitIconInfo();
        MainRankUI.instance.SetMyMainRank();
        MainRulletUI.instance.SetRulletInfo();
        MainQuestUI.instance.SetCanInfoActive();
        MainAttendance.instance.SetCanAttendanceInfo();
        MainAchievementUI.instance.SetReceiveCanInfo();
        MainInfoBoard.instance.SetBoardInfo();
        MainVendingMachine.instance.SetVendingMachineInfo();
        MainMailboxUI.instance.SetMailboxButton();
        MainComboUI.instance.SetMainUI();
        if (SaveScript.saveData.hasPicks[3] && !SaveScript.saveData.isReviewOn)
            MainReview.instance.reviewButton.SetActive(true);
        else
            MainReview.instance.reviewButton.SetActive(false);
    }

    public void SetTutorial()
    {
        gameStartButton.enabled = gameStartButton.isReSize =
        questButton.enabled = questButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 0 }, SaveScript.saveData.mainQuest_list);
        shopButton.enabled = shopButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 4, 5, 12 }, SaveScript.saveData.mainQuest_list);
        invenButton.enabled = invenButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 7, 8, 13, 14, 16, 17, 47, 48 }, SaveScript.saveData.mainQuest_list);
        achievementButton.enabled = achievementButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 9, 10 }, SaveScript.saveData.mainQuest_list);
        collectionButton.enabled = collectionButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 11 }, SaveScript.saveData.mainQuest_list);
        mineButton.enabled = mineButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42 }, SaveScript.saveData.mainQuest_list);
    }

    private void SetModeUI()
    {
        for (int i = 0; i < modeImages.Length; i++)
        {
            modeImages[i].color = Color.white;
            modeRocks[i].SetActive(false);
        }

        if (!SaveScript.saveData.hasPicks[6])
        {
            // 2번째 슬롯 잠금
            modeImages[1].color = new Color(0.4f, 0.4f, 0.4f);
            modeRocks[1].SetActive(true);
        }
        if (!SaveScript.saveData.hasPicks[10])
        {
            // 3번째 슬롯 잠금
            modeImages[2].color = new Color(0.4f, 0.4f, 0.4f);
            modeRocks[2].SetActive(true);
        }
    }

    public void GotoGameScene()
    {
        if (!SaveScript.isGameLock)
        {
            BlindScript.instance.spawnType = 0;
            if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
                BlindScript.instance.spawnType = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

            // 자격 조건 체크
            if (BlindScript.instance.spawnType == 1)
            {
                if (!SaveScript.saveData.hasPicks[6])
                {
                    SetAudio(2);
                    SystemInfoCtrl.instance.SetErrorInfo("현재 '청록석' 곡괭이가 없어 입장할 수 없습니다.");
                    return;
                }
            }
            else if (BlindScript.instance.spawnType == 2)
            {
                if (!SaveScript.saveData.hasPicks[10])
                {
                    SetAudio(2);
                    SystemInfoCtrl.instance.SetErrorInfo("현재 '연옥석' 곡괭이가 없어 입장할 수 없습니다.");
                    return;
                }
            }

            gameStartAnimator.SetBool("isGameStart", true);
            playerAnimator.SetInteger("GotoType", 1);
            isChangeScene = true;
            SetAudio(1);
        }
        else
        {
            // 기능 비활성화 상태
            SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("현재 심각한 오류로 인해 게임 버튼이 비활성화 되었습니다. 빠른 시간 내에 고치도록 하겠습니다.");

            if (BackEndLoginManager.isGM)
            {
                // 기능 활성화 상태
                BlindScript.instance.spawnType = 0;
                if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
                    BlindScript.instance.spawnType = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

                SetAudio(1);
                gameStartAnimator.SetBool("isGameStart", true);
                playerAnimator.SetInteger("GotoType", 1);
                isChangeScene = true;
            }
        }
    }

    public void OnOffModeUI()
    {
        isModeUIOn = !isModeUIOn;
        modeUIObject.SetActive(isModeUIOn);
        SetAudio(0);
    }

    public void GotoShopScene()
    {
        if (!SaveScript.isShopLock)
        {
            // 기능 활성화 상태
            SetAudio(0);
            playerAnimator.SetInteger("GotoType", 2);
            isChangeScene = true;
        }
        else
        {
            if (BackEndLoginManager.isGM)
            {
                // 기능 활성화 상태
                SetAudio(0);
                playerAnimator.SetInteger("GotoType", 2);
                isChangeScene = true;
            }
            else
            {
                // 기능 비활성화 상태
                SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("현재 심각한 오류로 인해 상점 버튼이 비활성화 되었습니다. 빠른 시간 내에 고치도록 하겠습니다.");
            }            
        }
    }

    public void GotoUpgradeScene()
    {
        if (!SaveScript.isInvenLock && !BackEndLoginManager.isGM)
        {
            // 기능 활성화 상태
            SetAudio(0);
            playerAnimator.SetInteger("GotoType", 3);
            isChangeScene = true;
        }
        else
        {
            if (BackEndLoginManager.isGM)
            {
                // 기능 활성화 상태
                SetAudio(0);
                playerAnimator.SetInteger("GotoType", 3);
                isChangeScene = true;
            }
            else
            {
                // 기능 비활성화 상태
                SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("현재 심각한 오류로 인해 인벤토리 버튼이 비활성화 되었습니다. 빠른 시간 내에 고치도록 하겠습니다.");
            }
        }
    }

    public void GotoMineScene()
    {
        if (!SaveScript.isMineLock && !BackEndLoginManager.isGM)
        {
            // 기능 활성화 상태
            SetAudio(0);
            isChangeScene = true;
            isGotoMineScene = true;
        }
        else
        {
            if (BackEndLoginManager.isGM)
            {
                // 기능 활성화 상태
                SetAudio(0);
                isChangeScene = true;
                isGotoMineScene = true;
            }
            else
            {
                // 기능 비활성화 상태
                SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("현재 심각한 오류로 인해 광산 버튼이 비활성화 되었습니다. 빠른 시간 내에 고치도록 하겠습니다.");
            }
        }
    }

    public void SetMineInfo()
    {
        bool isCanGetReward = false;
        isSetMineTime = false;

        if (SaveScript.saveData.facility_rewardTime <= 0)
            isCanGetReward = true;
        for (int i = 0; i < SaveScript.saveData.hasMinerRewards.Length; i++)
        {
            if (SaveScript.saveData.hasMinerRewards[i] != 0)
            {
                isCanGetReward = true;
                break;
            }
        }
        for (int i = 0; i < SaveScript.saveData.hasAdventurerRewards.Length; i++)
        {
            if (SaveScript.saveData.hasAdventurerRewards[i] != 0)
            {
                isCanGetReward = true;
                break;
            }
        }

        if (isCanGetReward)
        {
            mineCanInfo.SetActive(true);
        }
        else
        {
            mineCanInfo.SetActive(false);
        }
    }
    public void OnOffPlusMenu()
    {
        SetAudio(0);
        isPlusMenuOn = !isPlusMenuOn;
        plusMenuAnimator.SetBool("isOn", isPlusMenuOn);
        if (isPlusMenuOn)
            EventSystem.current.currentSelectedGameObject.transform.localScale = new Vector3(-1, 1, 1);
        else
            EventSystem.current.currentSelectedGameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ClickBackImage()
    {
        if (SaveScript.saveData.pickLevel > 1)
            return;

        clickBackCount++;
        if (clickBackCount > CLICK_BACKCOUNT)
        {
            SystemInfoCtrl.instance.SetErrorInfo("<color=black>혹시 게임을 시작하고 싶으신가요?\n왼쪽 상단에 <color=#FF9696>'땅 파러 가기' <color=black>버튼을 누르시면 됩니다!");
            clickBackCount = 0;
        }
    }
}
