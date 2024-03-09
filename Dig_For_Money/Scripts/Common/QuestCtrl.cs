using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCtrl : MonoBehaviour
{
    private static string[] lastInfo_infos =
    {
        "�̰��� �κ��Դϴ�. < �������� �̵� > , < ���� �̵� > , < �κ��丮 �̵� > �� �پ��� �ൿ�� ������ �� �ֽ��ϴ�.",
        "< �������� �̵� > �� ���� ��ܿ� '�� �ķ� ����' ��ư�� �����ø� �˴ϴ�.",
        "< ���� �̵� > �� ������ ��ܿ� '����' ��ư��!",
        "< �κ��丮 �̵� > �� ���� ��ư ���� �ִ� '�κ��丮' ��ư�� �����ּ���!",
        "������ ������ < ���� ����Ʈ > �� ���ؼ� ����帮�ڽ��ϴ�! �Ʒ� ����Ʈ�� ���� ���� ����Ʈ�� �������ּ��� :)",
    };

    public static QuestCtrl instance;

    [SerializeField] private Animator animator;
    [SerializeField] private Image backImage, spriteImage;
    [SerializeField] private Text infoText;
    [SerializeField] private GameObject lastInfoObject;
    [SerializeField] private Text lastInfoText;
    [SerializeField] private GameObject pageObject;
    private Order[] pages;

    public bool questIsPrint; // ���� ����Ʈ �޼� UI�� ����� ���� �ִ� ��?
    public bool isOnLastInfo; // ������ ����(�κ� ����)�� ų ���ΰ�?
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
        // ������ ��������� ����
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
            return false; // �̹� ����Ʈ �Ϸ��
        return GameFuction.HasElement(_array, _element);
    }

    public void SetMainQuestAmount(int[] _array)
    {
        if (!GameFuction.HasElement(_array, SaveScript.saveData.mainQuest_list) || SaveScript.saveData.isTutorial
            || SaveScript.saveData.mainQuest_goal == SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal)
            return;
        SaveScript.saveData.mainQuest_goal++;

        // ��� üũ
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

        // ��� üũ
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
    /// ���� ����Ʈ ���� UI�� ����ϴ� �Լ��̴�.
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

        // ��� ���� ����
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
        if (SaveScript.saveData.tier_achievement > 250) // ���� 250ȸ �̻� �� PASS
            instance.SetMainQuestAmount(new int[] { 9, 10 }, 10);
        if (GameFuction.GetPlayerGrade() > 0) // ���� ���� �̻��� ��� ����ġ ��ȭ PASS
            instance.SetMainQuestAmount(new int[] { 7, 14, 26 }, 10);
        instance.SetMainQuestAmount(new int[] { 39 }, SaveScript.saveData.facility_level);
        MineFacilityUI.CheckQuestUpgrades();
        MineFusionUI.Check_M_PetQuest();
        MineUpgradeUI.Check_PetUpgrade();
    }
}
