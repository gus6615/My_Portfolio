using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCtrl : MonoBehaviour
{
    private static string[] lastInfo_infos =
    {
        "이곳이 로비입니다. < 광산으로 이동 > , < 상점 이동 > , < 인벤토리 이동 > 등 다양한 행동을 선택할 수 있습니다.",
        "< 광산으로 이동 > 은 왼쪽 상단에 '땅 파러 가기' 버튼을 누르시면 됩니다.",
        "< 상점 이동 > 은 오른쪽 상단에 '상점' 버튼을!",
        "< 인벤토리 이동 > 은 상점 버튼 옆에 있는 '인벤토리' 버튼을 눌러주세요!",
        "나머지 설명은 < 메인 퀘스트 > 를 통해서 설명드리겠습니다! 아래 퀘스트를 눌러 메인 퀘스트를 수행해주세요 :)",
    };

    public static QuestCtrl instance;

    [SerializeField] private Animator animator;
    [SerializeField] private Image backImage, spriteImage;
    [SerializeField] private Text infoText;
    [SerializeField] private GameObject lastInfoObject;
    [SerializeField] private Text lastInfoText;
    [SerializeField] private GameObject pageObject;
    private Order[] pages;

    public bool questIsPrint; // 메인 퀘스트 달성 UI를 출력한 적이 있는 가?
    public bool isOnLastInfo; // 마지막 설명(로비 설명)을 킬 것인가?
    private int infoIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            pages = pageObject.GetComponentsInChildren<Order>();
            infoIndex = 0;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetUI(bool _isOn)
    {
        lastInfoObject.SetActive(_isOn);
        pageObject.SetActive(_isOn);
        if (_isOn)
            SetPage();
    }

    public void SetPage()
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].gameObject.SetActive(false);
        pages[infoIndex].gameObject.SetActive(true);
        lastInfoText.text = lastInfo_infos[infoIndex];
    }

    public void OnPageButton()
    {
        // 마지막 페이지라면 종료
        if (infoIndex == pages.Length - 1)
        {
            MainQuestUI.instance.OnOffQuestUI();
            SetUI(false);
            return;
        }

        infoIndex++;
        SetPage();
    }

    static public bool CheckFadeUI(int[] _array, int _element)
    {
        if (SaveScript.saveData.mainQuest_list > SaveScript.mainQuestNum - 1
            || SaveScript.saveData.mainQuest_goal >= SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal)
            return false; // 이미 퀘스트 완료됨
        return GameFuction.HasElement(_array, _element);
    }

    public void SetMainQuestAmount(int[] _array)
    {
        if (!GameFuction.HasElement(_array, SaveScript.saveData.mainQuest_list) || SaveScript.saveData.isTutorial
            || SaveScript.saveData.mainQuest_goal == SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal)
            return;
        SaveScript.saveData.mainQuest_goal++;

        // 출력 체크
        if (!questIsPrint && SaveScript.saveData.mainQuest_goal >= SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal)
            PrintQuest();
    }

    public void SetMainQuestAmount(int[] _array, long amount)
    {
        if (!GameFuction.HasElement(_array, SaveScript.saveData.mainQuest_list) || SaveScript.saveData.isTutorial)
            return;
        SaveScript.saveData.mainQuest_goal = amount;
        if (SaveScript.saveData.mainQuest_goal > SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal)
            SaveScript.saveData.mainQuest_goal = SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal;

        // 출력 체크
        if (!questIsPrint && SaveScript.saveData.mainQuest_goal >= SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal)
            PrintQuest();
    }

    public void SetSubQuestAmount(int _list)
    {
        SetSubQuestAmount(_list, 1);
    }

    public void SetSubQuestAmount(int _list, int _num)
    {
        int index = -1;
        for (int i = 0; i < SaveScript.saveData.quastLists.Length; i++)
        {
            if (SaveScript.saveData.quastLists[i] == _list)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return;

        if (SaveScript.saveData.quastGoals[index] < SaveScript.quests[SaveScript.saveData.quastLevels[index]][SaveScript.saveData.quastLists[index]].goal)
            SaveScript.saveData.quastGoals[index] += _num;
        if (MainQuestUI.instance != null)
            MainQuestUI.instance.SetCanInfoActive();
    }


    /// <summary>
    /// 메인 퀘스트 성공 UI을 출력하는 함수이다.
    /// </summary>
    public void PrintQuest()
    {
        animator.SetBool("isPrint", true);
        animator.Play("Achievement_ctrl_print", -1, 0f);
        spriteImage.sprite = SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].sprite;
        infoText.text = SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].name;
        questIsPrint = true;
        if (MainQuestUI.instance != null)
            MainQuestUI.instance.SetCanInfoActive();

        // 배경 색상 설정
        int stageType = MainQuest.GetStageType(SaveScript.saveData.mainQuest_list);
        if (stageType != -1)
            backImage.color = SaveScript.stageColors[stageType];
        else
            backImage.color = new Color(0.7f, 0.5f, 0f);
        backImage.color = new Color(backImage.color.r, backImage.color.g, backImage.color.b, 0.9f);
    }

    public void EndAnimation()
    {
        animator.SetBool("isPrint", false);
    }

    static public void CheckAllQuest()
    {
        GameItemShop.CheckEquipmentQuest();
        ReinforceUpgradeUI.CheckReinforceQuest();
        ExpUpgradeUI.CheckExpQuest();
        if (SaveScript.saveData.tier_achievement > 250) // 업적 250회 이상 시 PASS
            instance.SetMainQuestAmount(new int[] { 9, 10 }, 10);
        if (GameFuction.GetPlayerGrade() > 0) // 전설 유저 이상의 경우 경험치 강화 PASS
            instance.SetMainQuestAmount(new int[] { 7, 14, 26 }, 10);
        instance.SetMainQuestAmount(new int[] { 39 }, SaveScript.saveData.facility_level);
        MineFacilityUI.CheckQuestUpgrades();
        MineFusionUI.Check_M_PetQuest();
        MineUpgradeUI.Check_PetUpgrade();
    }
}
