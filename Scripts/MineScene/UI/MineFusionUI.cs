using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct FusionForm
{
    public int leftIndex, rightIndex, resultIndex;
    public int leftCode, rightCode, resultCode;
    public int leftLevel, rightLevel, resultLevel;
    public float resultPercent;

    public FusionForm(int _leftIndex, int _leftCode, int _leftLevel, int _rightIndex, int _rightCode, int _rightLevel)
    {
        leftIndex = _leftIndex;
        leftCode = _leftCode;
        leftLevel = _leftLevel;
        rightIndex = _rightIndex;
        rightCode = _rightCode;
        rightLevel = _rightLevel;
        resultIndex = resultCode = resultLevel = 0;
        resultPercent = 0;
    }

    public void SetResult(int _resultIndex, int _resultCode, int _resultLevel, float _resultPercent)
    {
        resultIndex = _resultIndex;
        resultCode = _resultCode;
        resultLevel = _resultLevel;
        resultPercent = _resultPercent;
    }
}

public class MineFusionUI : MonoBehaviour
{
    public static MineFusionUI instance;

    public Animator fusionUI_animator;
    public GameObject fusionSelectPet;
    public UIBox[] fusionMenus;
    public TMP_Text m_pet_infoText;
    public GameObject fusionSelectPanel;
    public UIBox fusionLeftSlot, fusionRightSlot;
    public GameObject fusionResultInfo;
    public Text fusionResultText;
    public UIBox[] fusionResultSlots;
    public GameObject fusionResultAni;
    public Text fusionMenuText;
    public Image fusionResultLeftImage, fusionResultRightImage;
    public Button fusionButton, fusionAllButton;
    public Text fusionAllInfoText;
    public GameObject teamInvenPageObject;
    public GameObject fusionAllObject;
    public GameObject fusionUIBoxPrefab;
    public Transform fusionUIBoxPanel;
    private UIBox[] invenPages;
    private int fusionPetClick; // 현재 누른 합성 버튼의 종류 (0 = 왼쪽, 1 = 오른쪽)
    public static int fusionLeftPetIndex = -1, fusionRightPetIndex = -1;
    public static int fusionLeftPetCode, fusionRightPetCode;
    public static int fusionLeftPetLevel, fusionRightPetLevel;
    public static List<FusionForm> fusionForms;

    static public int menuIndex;
    private int invenPage;

    // 임시 데이터
    MineFusionUIBox[] mineFusionUIBoxes;
    MineFusionUIBox MineFusionUIBox;
    Sprite[] sprites;
    Sprite sprite;
    UIBox[] uiboxs;
    UIBox uibox;

    private void Start()
    {
        instance = this;
        invenPages = teamInvenPageObject.GetComponentsInChildren<UIBox>();
        fusionForms = new List<FusionForm>();
    }

    // 공통 UI 변수 초기화
    public void SetDefaultVariable()
    {
        menuIndex = 0;
        invenPage = 0;
    }

    static public void Check_M_PetQuest()
    {
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 126, 127, 128 }, Total_M_Pet());
    }

    static private int Total_M_Pet()
    {
        int num = 0;

        for (int i = 0; i < SaveScript.mineInvenMaxNum; i++)
        {
            if (SaveScript.saveData.hasMiners[i] == 10) num++;
            if (SaveScript.saveData.hasOnMiners[i] == 10) num++;
            if (SaveScript.saveData.hasAdventurers[i] == 10) num++;
            if (SaveScript.saveData.hasOnAdventurers[i] == 10) num++;
        }

        return num;
    }

    // FusionUI 데이터 정리
    static public void SetFusionVariable()
    {
        if (fusionLeftPetIndex != -1)
        {
            switch (menuIndex)
            {
                case 0: SaveScript.saveData.hasMiners[fusionLeftPetIndex] = fusionLeftPetCode; break;
                case 1: SaveScript.saveData.hasAdventurers[fusionLeftPetIndex] = fusionLeftPetCode; break;
            }
        }
        if (fusionRightPetIndex != -1)
        {
            switch (menuIndex)
            {
                case 0: SaveScript.saveData.hasMiners[fusionRightPetIndex] = fusionRightPetCode; break;
                case 1: SaveScript.saveData.hasAdventurers[fusionRightPetIndex] = fusionRightPetCode; break;
            }
        }

        fusionLeftPetIndex = fusionLeftPetCode = fusionLeftPetLevel = -1;
        fusionRightPetIndex = fusionRightPetCode = fusionRightPetLevel = -1;
    }

    public void SetInvenPageUI(int _activePage)
    {
        int upgrade = 0;

        switch (menuIndex)
        {
            case 0: upgrade = SaveScript.saveData.minerUpgrades[3]; break;
            case 1: upgrade = SaveScript.saveData.adventurerUpgrades[3]; break;
        }

        for (int i = 0; i < invenPages.Length; i++)
        {
            if (i < 1 + upgrade)
            {
                invenPages[i].button.enabled = true;
                invenPages[i].images[0].color = new Color(1f, 1f, 1f, 1f);
                invenPages[i].texts[0].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                invenPages[i].button.enabled = false;
                invenPages[i].images[0].color = new Color(1f, 1f, 1f, 0.1f);
                invenPages[i].texts[0].color = new Color(1f, 1f, 1f, 0.1f);
            }
        }
        invenPages[_activePage].button.enabled = true;
        invenPages[_activePage].images[0].color = new Color(0.6f, 0.6f, 0.6f, 1f);
        invenPages[_activePage].texts[0].color = new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    public void ClickInvenPage()
    {
        uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uibox != null)
        {
            invenPage = uibox.order;
            Mine.instance.SetAudio(0);
        }

        FusionUI_SetSelectPet();
    }

    // FusionUI 초기화
    public void FusionUI_Menu()
    {
        invenPage = 0;
        fusionButton.enabled = true;
        FusionUI_SetDefault();
    }

    // FusionUI UI 초기화
    public void FusionUI_SetDefault()
    {
        SetFusionVariable();

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
            if (uibox != null)
            {
                menuIndex = uibox.order;
                Mine.instance.SetAudio(0);
            }
        }

        for (int i = 0; i < fusionMenus.Length; i++)
        {
            fusionMenus[i].images[0].color = Color.white;
            fusionMenus[i].texts[0].color = new Color(0.7f, 0.8f, 1f);
        }
        fusionMenus[menuIndex].images[0].color = new Color(0.6f, 0.6f, 0.6f);
        fusionMenus[menuIndex].texts[0].color = new Color(0.3f, 0.4f, 0.5f);

        switch (menuIndex)
        {
            case 0: fusionMenuText.text = "광부 슬라임 합성"; break;
            case 1: fusionMenuText.text = "모험가 슬라임 합성"; break;
        }
        m_pet_infoText.gameObject.SetActive(false);
        fusionResultInfo.SetActive(false);
        fusionSelectPet.SetActive(false);
        fusionResultAni.SetActive(false);
        fusionAllObject.SetActive(false);
        FusionUI_SetFusionSlot(0);
        FusionUI_SetFusionSlot(1);
        SetFusionList();
    }

    // FusionUI 합성 슬롯 설정
    private void FusionUI_SetFusionSlot(int _type)
    {
        int code = 0;
        int level = 0;
        sprite = null;

        if (_type == 0 && fusionLeftPetIndex != -1)
        {
            switch (menuIndex)
            {
                case 0: code = fusionLeftPetCode; level = fusionLeftPetLevel; sprite = MineSlime.miner_defaultSprites[code]; break;
                case 1: code = fusionLeftPetCode; level = fusionLeftPetLevel; sprite = MineSlime.adventurer_defaultSprites[code]; break;
            }
        }
        else if (_type == 1 && fusionRightPetIndex != -1)
        {
            switch (menuIndex)
            {
                case 0: code = fusionRightPetCode; level = fusionRightPetLevel; sprite = MineSlime.miner_defaultSprites[code]; break;
                case 1: code = fusionRightPetCode; level = fusionRightPetLevel; sprite = MineSlime.adventurer_defaultSprites[code]; break;
            }
        }

        switch (_type)
        {
            case 0: // 왼쪽
                if (fusionLeftPetIndex != -1)
                {
                    fusionLeftSlot.images[0].color = new Color(1f, 1f, 1f, 1f);
                    fusionLeftSlot.images[0].sprite = sprite;
                    fusionLeftSlot.images[1].color = new Color(1f, 1f, 1f, 0f);
                    fusionLeftSlot.texts[0].text = "[" + MineSlime.qualityNames[code] + "] Lv." + level;
                }
                else
                {
                    fusionLeftSlot.images[0].color = new Color(1f, 1f, 1f, 0f);
                    fusionLeftSlot.images[1].color = new Color(1f, 1f, 1f, 1f);
                    fusionLeftSlot.texts[0].text = "";
                }
                break;
            case 1: // 오른쪽
                if (fusionRightPetIndex != -1)
                {
                    fusionRightSlot.images[0].color = new Color(1f, 1f, 1f, 1f);
                    fusionRightSlot.images[0].sprite = sprite;
                    fusionRightSlot.images[1].color = new Color(1f, 1f, 1f, 0f);
                    fusionRightSlot.texts[0].text = "[" + MineSlime.qualityNames[code] + "] Lv." + level;
                }
                else
                {
                    fusionRightSlot.images[0].color = new Color(1f, 1f, 1f, 0f);
                    fusionRightSlot.images[1].color = new Color(1f, 1f, 1f, 1f);
                    fusionRightSlot.texts[0].text = "";
                }
                break;
        }
    }

    // FusionUI 합성 예상 결과 설정 
    private void FusionUI_SetResultUI()
    {
        m_pet_infoText.gameObject.SetActive(false);
        if (fusionLeftPetIndex != -1 && fusionRightPetIndex != -1)
        {
            float[] percents = GetSuccessPercents(fusionLeftPetCode, fusionRightPetCode);
            int fusionResultCode = (fusionLeftPetCode + fusionRightPetCode) / 2;
            int fusionResultLevel = Mathf.Max(fusionLeftPetLevel, fusionRightPetLevel);
            fusionResultInfo.SetActive(true);
            if (fusionResultCode >= SaveScript.mineSlimeQualityNum - 2)
                m_pet_infoText.gameObject.SetActive(true);

            switch (menuIndex)
            {
                case 0: sprites = MineSlime.miner_defaultSprites; break;
                case 1: sprites = MineSlime.adventurer_defaultSprites; break;
            }

            if (fusionResultCode != SaveScript.mineSlimeQualityNum - 1)
            {
                fusionResultText.text = "[" + MineSlime.qualityNames[fusionResultCode] + "+] (Lv." + fusionResultLevel + ")";
                fusionButton.gameObject.SetActive(true);
            }
            else
            {
                fusionResultText.text = "[" + MineSlime.qualityNames[fusionResultCode] + "] (Lv." + fusionResultLevel + ")";
                fusionButton.gameObject.SetActive(false);
            }

            for (int i = fusionResultSlots.Length - 1; i >= 0; i--)
            {
                if (percents[i] != 0f && fusionResultCode + i < SaveScript.mineSlimeQualityNum)
                {
                    fusionResultSlots[i].images[1].color = new Color(1f, 1f, 1f, 1f);
                    fusionResultSlots[i].images[1].sprite = sprites[fusionResultCode + i];
                    fusionResultSlots[i].texts[0].text = "[" + MineSlime.qualityNames[fusionResultCode + i] + "] (Lv." + fusionResultLevel + ")\n = "
                        + Mathf.Round(percents[i] * 100f * 100f) / 100f + " %";
                }
                else
                {
                    fusionResultSlots[i].images[1].color = new Color(1f, 1f, 1f, 0f);
                    fusionResultSlots[i].texts[0].text = "";
                }
            }
        }
        else
        {
            fusionResultInfo.SetActive(false);
        }
    }

    // FusionUI 합성 슬롯 클릭
    public void FusionUI_FusionSlotClick()
    {
        fusionPetClick = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<UIBox>().order; // 0 왼쪽, 1 오른쪽
        int type = -1;
        Mine.instance.SetAudio(0);

        switch (fusionPetClick)
        {
            case 0: type = fusionLeftPetIndex; break;
            case 1: type = fusionRightPetIndex; break;
        }

        switch (type)
        {
            case -1: // 꺼진 상태
                fusionSelectPet.SetActive(true);
                FusionUI_SetSelectPet();
                break;
            default: // 켜진 상태
                fusionSelectPet.SetActive(false);
                switch (fusionPetClick)
                {
                    case 0:
                        switch (menuIndex)
                        {
                            case 0: SaveScript.saveData.hasMiners[fusionLeftPetIndex] = fusionLeftPetCode; break;
                            case 1: SaveScript.saveData.hasAdventurers[fusionLeftPetIndex] = fusionLeftPetCode; break;
                        }
                        fusionLeftPetIndex = fusionLeftPetCode = fusionLeftPetLevel = -1;
                        break;
                    case 1:
                        switch (menuIndex)
                        {
                            case 0: SaveScript.saveData.hasMiners[fusionRightPetIndex] = fusionRightPetCode; break;
                            case 1: SaveScript.saveData.hasAdventurers[fusionRightPetIndex] = fusionRightPetCode; break;
                        }
                        fusionRightPetIndex = fusionRightPetCode = fusionRightPetLevel = -1;
                        break;
                }
                break;
        }

        FusionUI_SetFusionSlot(fusionPetClick);
        FusionUI_SetResultUI();
    }

    // FusionUI 펫 선택 설정
    public void FusionUI_SetSelectPet()
    {
        int upgrade = 0;
        int[] codes = null;
        int[] levels = null;
        uiboxs = fusionSelectPanel.GetComponentsInChildren<UIBox>();
        SetInvenPageUI(invenPage);

        switch (menuIndex)
        {
            case 0:
                upgrade = SaveScript.saveData.minerUpgrades[3];
                codes = SaveScript.saveData.hasMiners;
                levels = SaveScript.saveData.hasMinerLevels;
                sprites = MineSlime.miner_defaultSprites;
                break;
            case 1:
                upgrade = SaveScript.saveData.adventurerUpgrades[3];
                codes = SaveScript.saveData.hasAdventurers;
                levels = SaveScript.saveData.hasAdventurerLevels;
                sprites = MineSlime.adventurer_defaultSprites;
                break;
        }

        for (int i = 0; i < uiboxs.Length; i++)
        {
            int index = invenPage * 10 + i;
            if (index >= SaveScript.mineInvenMinNum + upgrade * SaveScript.mineUpgradePercents[3])
            {
                uiboxs[i].images[0].color = uiboxs[i].images[1].color = uiboxs[i].images[2].color = uiboxs[i].texts[0].color = uiboxs[i].texts[1].color = new Color(1f, 1f, 1f, 0f);
                uiboxs[i].button.enabled = false;
            }
            else
            {
                if (codes[index] != -1)
                {
                    uiboxs[i].images[0].color = uiboxs[i].images[1].color = new Color(1f, 1f, 1f, 1f);
                    uiboxs[i].images[2].color = new Color(0.1f, 0.2f, 0.4f, 0.8f);
                    uiboxs[i].texts[0].color = uiboxs[i].texts[1].color = new Color(0.7f, 0.7f, 1f);
                    uiboxs[i].images[1].sprite = sprites[codes[index]];
                    uiboxs[i].texts[0].text = "Lv" + levels[index];
                    uiboxs[i].texts[1].text = MineSlime.qualityNames[codes[index]];
                    uiboxs[i].order = index;
                    uiboxs[i].button.enabled = true;
                }
                else
                {
                    uiboxs[i].images[0].color = new Color(1f, 1f, 1f, 0.4f);
                    uiboxs[i].images[1].color = uiboxs[i].images[2].color = new Color(1f, 1f, 1f, 0f);
                    uiboxs[i].texts[0].text = uiboxs[i].texts[1].text = "";
                    uiboxs[i].button.enabled = false;
                }
            }
        }
    }

    // FusionUI 펫 선택
    public void FusionUI_SelectPet()
    {
        int petIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
        Mine.instance.SetAudio(0);
        invenPage = 0;

        switch (fusionPetClick)
        {
            case 0:
                switch (menuIndex)
                {
                    case 0:
                        fusionLeftPetIndex = petIndex;
                        fusionLeftPetCode = SaveScript.saveData.hasMiners[fusionLeftPetIndex];
                        fusionLeftPetLevel = SaveScript.saveData.hasMinerLevels[fusionLeftPetIndex];
                        SaveScript.saveData.hasMiners[fusionLeftPetIndex] = -1;
                        break;
                    case 1:
                        fusionLeftPetIndex = petIndex;
                        fusionLeftPetCode = SaveScript.saveData.hasAdventurers[fusionLeftPetIndex];
                        fusionLeftPetLevel = SaveScript.saveData.hasAdventurerLevels[fusionLeftPetIndex];
                        SaveScript.saveData.hasAdventurers[fusionLeftPetIndex] = -1;
                        break;
                }
                break;
            case 1:
                switch (menuIndex)
                {
                    case 0:
                        fusionRightPetIndex = petIndex;
                        fusionRightPetCode = SaveScript.saveData.hasMiners[fusionRightPetIndex];
                        fusionRightPetLevel = SaveScript.saveData.hasMinerLevels[fusionRightPetIndex];
                        SaveScript.saveData.hasMiners[fusionRightPetIndex] = -1;
                        break;
                    case 1:
                        fusionRightPetIndex = petIndex;
                        fusionRightPetCode = SaveScript.saveData.hasAdventurers[fusionRightPetIndex];
                        fusionRightPetLevel = SaveScript.saveData.hasAdventurerLevels[fusionRightPetIndex];
                        SaveScript.saveData.hasAdventurers[fusionRightPetIndex] = -1;
                        break;
                }
                break;
        }

        FusionUI_SetFusionSlot(fusionPetClick);
        FusionUI_SetResultUI();
        fusionSelectPet.SetActive(false);
    }

    // FusionUI 합성 버튼
    public void FusionUI_FusionButton()
    {
        if (fusionRightPetCode == -1 || fusionLeftPetCode == -1)
        {
            SystemInfoCtrl.instance.SetErrorInfo("Error! 알 수 없는 이유로 인해 에러가 발생했습니다. 다시 펫을 설정하여 합성해주세요.");
            return;
        }

        fusionResultAni.SetActive(true);
        fusionUI_animator.SetBool("isFusion", true);
        fusionUI_animator.Play("FusionUI_Idle", -1, 0f);
        MineFusion.instance.passClickPanel.gameObject.SetActive(true);
        MineFusion.instance.passClickPanel.onClick.RemoveAllListeners();
        MineFusion.instance.passClickPanel.onClick.AddListener(MineFusion.instance.PassAni);
        AchievementCtrl.instance.SetAchievementAmount(37, 1);
        Mine.instance.SetAudio(0);

        switch (menuIndex)
        {
            case 0:
                fusionResultLeftImage.sprite = MineSlime.miner_defaultSprites[fusionLeftPetCode];
                fusionResultRightImage.sprite = MineSlime.miner_defaultSprites[fusionRightPetCode];
                break;
            case 1:
                fusionResultLeftImage.sprite = MineSlime.adventurer_defaultSprites[fusionLeftPetCode];
                fusionResultRightImage.sprite = MineSlime.adventurer_defaultSprites[fusionRightPetCode];
                break;
        }
    }

    private void SetFusionList()
    {
        int current_index;

        fusionForms.Clear();
        switch (menuIndex)
        {
            case 0:
                current_index = SaveScript.saveData.hasMiners.Length - 1;
                for (int i = SaveScript.saveData.hasMiners.Length - 2; i >= 0; i--)
                {
                    if (SaveScript.saveData.hasMiners[current_index] != -1 && SaveScript.saveData.hasMiners[current_index] < 9
                        && SaveScript.saveData.hasMiners[i] == SaveScript.saveData.hasMiners[current_index])
                    {
                        // 합성 가능
                        fusionForms.Add(new FusionForm(i, SaveScript.saveData.hasMiners[i], SaveScript.saveData.hasMinerLevels[i],
                            current_index, SaveScript.saveData.hasMiners[current_index], SaveScript.saveData.hasMinerLevels[current_index]));
                        i--;
                    }
                    current_index = i;
                }
                break;
            case 1:
                current_index = SaveScript.saveData.hasAdventurers.Length - 1;
                for (int i = SaveScript.saveData.hasAdventurers.Length - 2; i >= 0; i--)
                {
                    if (SaveScript.saveData.hasAdventurers[current_index] != -1 && SaveScript.saveData.hasAdventurers[current_index] < 9
                        && SaveScript.saveData.hasAdventurers[i] == SaveScript.saveData.hasAdventurers[current_index])
                    {
                        // 합성 가능
                        fusionForms.Add(new FusionForm(i, SaveScript.saveData.hasAdventurers[i], SaveScript.saveData.hasAdventurerLevels[i],
                            current_index, SaveScript.saveData.hasAdventurers[current_index], SaveScript.saveData.hasAdventurerLevels[current_index]));
                        i--;
                    }
                    current_index = i;
                }
                break;
        }

        Debug.Log("합성 총 건수: " + fusionForms.Count);
        foreach (var item in fusionForms)
            Debug.Log(item.leftIndex + " / " + item.rightIndex);

        if (fusionForms.Count != 0)
            fusionAllButton.gameObject.SetActive(true);
        else 
            fusionAllButton.gameObject.SetActive(false);
    }

    // FusionUI 모두 합성 버튼
    public void FusionAllButton()
    {
        FusionUI_SetDefault();
        fusionAllObject.SetActive(true);
        fusionAllInfoText.text = "합성 예상 결과 : " + fusionForms.Count + "건";
        Mine.instance.SetAudio(0);

        // Form 결과 Setting 하기
        for (int i = 0; i < fusionForms.Count; i++)
        {
            FusionForm form = fusionForms[i];
            int resultCode = (form.leftCode + form.rightCode) / 2 + 1;
            int resultLevel = Mathf.Max(fusionLeftPetLevel, fusionRightPetLevel);
            float[] percents = GetSuccessPercents(form.leftCode, form.rightCode);
            float resultPercent = percents[1] + percents[2];
            if (resultLevel < 1) resultLevel = 1;

            form.SetResult(-1, resultCode, resultLevel, resultPercent);
            fusionForms[i] = form;
        }

        // UI Setting
        mineFusionUIBoxes = fusionUIBoxPanel.GetComponentsInChildren<MineFusionUIBox>();
        for (int i = 0; i < mineFusionUIBoxes.Length; i++)
            Destroy(mineFusionUIBoxes[i].gameObject);
        fusionUIBoxPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        switch (menuIndex)
        {
            case 0: sprites = MineSlime.miner_defaultSprites; break;
            case 1: sprites = MineSlime.adventurer_defaultSprites; break;
        }

        for (int i = fusionForms.Count - 1; i >= 0; i--)
        {
            FusionForm form = fusionForms[i];
            MineFusionUIBox = Instantiate(fusionUIBoxPrefab, fusionUIBoxPanel).GetComponent<MineFusionUIBox>();
            MineFusionUIBox.leftPet.images[1].sprite = sprites[form.leftCode];
            MineFusionUIBox.leftPet.texts[0].text = "[" + MineSlime.qualityNames[form.leftCode] + "] (Lv." + form.leftLevel + ")";
            MineFusionUIBox.rightPet.images[1].sprite = sprites[form.rightCode];
            MineFusionUIBox.rightPet.texts[0].text = "[" + MineSlime.qualityNames[form.rightCode] + "] (Lv." + form.rightLevel + ")";
            MineFusionUIBox.resultPet.images[1].sprite = sprites[form.resultCode];
            MineFusionUIBox.resultPet.texts[0].text = "[" + MineSlime.qualityNames[form.resultCode] + "] (Lv." + form.resultLevel + ")";
            MineFusionUIBox.percentImage.color = new Color(0.3f, 0.1f, 0.1f, 0.9f);
            MineFusionUIBox.percentText.text = "확률 : " + Mathf.Round(form.resultPercent * 100f * 100f) / 100f + " %";
            MineFusionUIBox.percentText.color = new Color(1f, 0.7f, 0.7f);
        }
    }

    // FusionUI 모두 합성 Yes 버튼
    public void FusionAllYes()
    {
        fusionAllObject.SetActive(false);
        fusionResultAni.SetActive(true);
        fusionUI_animator.SetBool("isFusionAll", true);
        fusionUI_animator.Play("FusionUI_All", -1, 0f);
        MineFusion.instance.passClickPanel.gameObject.SetActive(true);
        MineFusion.instance.passClickPanel.onClick.RemoveAllListeners();
        MineFusion.instance.passClickPanel.onClick.AddListener(MineFusion.instance.PassAllAni);
        AchievementCtrl.instance.SetAchievementAmount(37, fusionForms.Count);
        Mine.instance.SetAudio(0);
    }

    // FusionUI 모두 합성 No 버튼
    public void FusionAllNo()
    {
        fusionAllObject.SetActive(false);
        Mine.instance.SetAudio(0);
    }

    // FusionUI 합성 완료 창 닫기
    public void FusionUI_CloseFusion()
    {
        Mine.instance.SetAudio(0);
        fusionResultAni.SetActive(false);
        MineFusion.instance.fusionResult.SetActive(false);
        StopCoroutine("DelayFusionButton");
        StartCoroutine("DelayFusionButton");
        FusionUI_SetDefault();
    }

    // FusionUI 모두 합성 완료 창 닫기
    public void FusionUI_CloseFusionAll()
    {
        Mine.instance.SetAudio(0);
        fusionResultAni.SetActive(false);
        MineFusion.instance.fusionResultAll.SetActive(false);
        StopCoroutine("DelayFusionButton");
        StartCoroutine("DelayFusionButton");
        FusionUI_SetDefault();
    }

    IEnumerator DelayFusionButton()
    {
        fusionButton.enabled = false;
        yield return new WaitForSeconds(0.5f);
        fusionButton.enabled = true;
    }
    
    public float[] GetSuccessPercents(int _leftPetCode, int _rightPetCode)
    {
        int currentCode = (_leftPetCode + _rightPetCode) / 2;
        float[] percents = (float[])MineSlime.fusionPercentAsCode[currentCode].Clone();

        if (currentCode < 9)
        {
            if (currentCode != SaveScript.mineSlimeQualityNum - 1)
                percents[0] -= SaveScript.stat.petFusion;
            percents[1] += SaveScript.stat.petFusion;
        }
        // 데이터 조정
        if (percents[0] < 0f)
            percents[0] = 0f;
        if (percents[0] + percents[1] + percents[2] > 1f)
            percents[1] = 1f - (percents[0] + percents[2]);

        return percents;
    }
}
