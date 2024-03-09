using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MineDecompositionUI : MonoBehaviour
{
    public static MineDecompositionUI instance;
    public static int[] manaNum_min = { 10, 20, 40, 100, 250, 600, 1500, 4000, 10000, 30000 };
    public static int[] manaNum_max = { 20, 40, 100, 250, 600, 1500, 4000, 10000, 30000, 50000 };
    private static Sprite[] manaSprites;

    public UIBox[] decompositionMenus;
    public Text manaText;
    public Image selectPlusImage, selectedPetImage;
    public Text selectedPetText;
    public GameObject decompositionButton, decompositionAllButton;
    public GameObject decompositionAllObject;
    public Text decompositionAllInfoText;
    public GameObject decompositionFormPrefab;
    public Transform decompositionFormPanel;
    public GameObject selectPetObject;
    public GameObject selectPetPanel;
    public Animator decomposition_animator;
    public Image ani_petImage;
    public GameObject teamInvenPageObject;
    private UIBox[] invenPages;
    public static List<int> decomposition_forms;

    static public int menuIndex;
    static public int selectPetIndex = -1;
    static public int selectPetCode, selectPetLevel;
    private int invenPage;

    // 임시 데이터
    Sprite[] sprites;
    Mine_TeamSlot[] slots;
    UIBox[] uIBoxes;
    UIBox uIBox;

    private void Start()
    {
        instance = this;
        manaSprites = Resources.LoadAll<Sprite>("Images/Item/ManaOres");

        invenPages = teamInvenPageObject.GetComponentsInChildren<UIBox>();
        decomposition_forms = new List<int>();
    }

    // UI 변수 초기화
    public void SetDefaultVariable()
    {
        menuIndex = 0;
        invenPage = 0;
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
        uIBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uIBox != null)
        {
            invenPage = uIBox.order;
            Mine.instance.SetAudio(0);
        }

        DecompositionUI_SetSelectPet();
    }

    // UI 변수 데이터 정리
    static public void SetDecompositionVariable()
    {
        if (selectPetIndex != -1)
        {
            switch (menuIndex)
            {
                case 0: SaveScript.saveData.hasMiners[selectPetIndex] = selectPetCode; break;
                case 1: SaveScript.saveData.hasAdventurers[selectPetIndex] = selectPetCode; break;
            }
        }

        selectPetIndex = selectPetCode = selectPetLevel = - 1;
    }


    // 펫 종류 선택
    public void DecompositionUI_Menu()
    {
        SetDecompositionVariable();
        uIBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uIBox != null)
        {
            menuIndex = uIBox.order;
            Mine.instance.SetAudio(0);
        }

        DecompositionUI_SetDefault();
    }

    // UI 초기화
    public void DecompositionUI_SetDefault()
    {
        SetDecompositionList();

        for (int i = 0; i < decompositionMenus.Length; i++)
        {
            decompositionMenus[i].images[0].color = Color.white;
            decompositionMenus[i].texts[0].color = new Color(0.8f, 0.8f, 0.8f);
        }
        decompositionMenus[menuIndex].images[0].color = new Color(0.6f, 0.6f, 0.6f);
        decompositionMenus[menuIndex].texts[0].color = new Color(0.5f, 0.5f, 0.5f);

        selectPetObject.SetActive(false);
        decompositionButton.SetActive(false);
        decompositionAllObject.SetActive(false);
        decomposition_animator.gameObject.SetActive(false);
        selectPlusImage.gameObject.SetActive(true);
        selectedPetImage.gameObject.SetActive(false);
        if (decomposition_forms.Count > 0)
            decompositionAllButton.SetActive(true);
        else
            decompositionAllButton.SetActive(false);

        manaText.text = GameFuction.GetNumText(SaveScript.saveData.manaOre);
        selectedPetText.text = "";
    }

    // SelectedPet 돌려놓기
    public void DecompositionUI_CancleSelectPet()
    {
        SetDecompositionVariable();
        selectPlusImage.gameObject.SetActive(true);
        selectedPetImage.gameObject.SetActive(false);
        decompositionButton.SetActive(false);
        decompositionAllButton.SetActive(true);
        selectedPetText.text = "";
        invenPage = 0;
        Mine.instance.SetAudio(0);
    }

    // SelectPet UI 설정
    public void DecompositionUI_SetSelectPet()
    {
        selectPetObject.SetActive(true);
        slots = selectPetPanel.GetComponentsInChildren<Mine_TeamSlot>();
        SetInvenPageUI(invenPage);
        int code;
        int index;

        switch (menuIndex)
        {
            case 0:
                for (int i = 0; i < slots.Length; i++)
                {
                    index = invenPage * 10 + i;
                    code = SaveScript.saveData.hasMiners[index];
                    slots[i].index = index;
                    if (index < SaveScript.mineInvenMinNum + SaveScript.saveData.minerUpgrades[3] * SaveScript.mineUpgradePercents[3])
                        slots[i].existOb.SetActive(true);
                    else
                        slots[i].existOb.SetActive(false);

                    if (code != -1)
                    {
                        slots[i].SetMenu(false);
                        slots[i].existOb.GetComponent<Button>().enabled = true;
                        slots[i].exist_spriteImage.sprite = MineSlime.miner_defaultSprites[code];
                        slots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasMinerLevels[index];
                    }
                    else
                    {
                        slots[i].SetMenu(true);
                        slots[i].existOb.GetComponent<Button>().enabled = false;
                        slots[i].exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
                        slots[i].exist_nameText.text = "";
                    }
                }
                break;
            case 1:
                for (int i = 0; i < slots.Length; i++)
                {
                    index = invenPage * 10 + i;
                    code = SaveScript.saveData.hasAdventurers[index];
                    slots[i].index = index;

                    if (index < SaveScript.mineInvenMinNum + SaveScript.saveData.adventurerUpgrades[3] * SaveScript.mineUpgradePercents[3])
                        slots[i].existOb.SetActive(true);
                    else
                        slots[i].existOb.SetActive(false);
                    if (code != -1)
                    {
                        slots[i].SetMenu(false);
                        slots[i].existOb.GetComponent<Button>().enabled = true;
                        slots[i].exist_spriteImage.sprite = MineSlime.adventurer_defaultSprites[code];
                        slots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasAdventurerLevels[index];
                    }
                    else
                    {
                        slots[i].SetMenu(true);
                        slots[i].existOb.GetComponent<Button>().enabled = false;
                        slots[i].exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
                        slots[i].exist_nameText.text = "";
                    }
                }
                break;
        }
    }

    // SelectPet UI 닫기
    public void DecompositionUI_CloseSelectPet()
    {
        invenPage = 0;
        selectPetObject.SetActive(false);
        Mine.instance.SetAudio(0);
    }

    // SelectPet 선택
    public void DecompositionUI_ClickSelectPet()
    {
        int petIndex = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Mine_TeamSlot>().index;
        Mine.instance.SetAudio(0);

        switch (menuIndex)
        {
            case 0:
                selectPetIndex = petIndex;
                selectPetCode = SaveScript.saveData.hasMiners[selectPetIndex];
                selectPetLevel = SaveScript.saveData.hasMinerLevels[selectPetIndex];
                SaveScript.saveData.hasMiners[selectPetIndex] = -1;

                selectedPetImage.sprite = MineSlime.miner_defaultSprites[selectPetCode];
                selectedPetText.text = "[" + MineSlime.qualityNames[selectPetCode] + "등급] Lv." + selectPetLevel;
                break;
            case 1:
                selectPetIndex = petIndex;
                selectPetCode = SaveScript.saveData.hasAdventurers[selectPetIndex];
                selectPetLevel = SaveScript.saveData.hasAdventurerLevels[selectPetIndex];
                SaveScript.saveData.hasAdventurers[selectPetIndex] = -1;

                selectedPetImage.sprite = MineSlime.adventurer_defaultSprites[selectPetCode];
                selectedPetText.text = "[" + MineSlime.qualityNames[selectPetCode] + "등급] Lv." + selectPetLevel;
                break;
        }

        invenPage = 0;
        selectPetObject.SetActive(false);
        selectPlusImage.gameObject.SetActive(false);
        selectedPetImage.gameObject.SetActive(true);
        decompositionButton.SetActive(true);
        decompositionAllButton.SetActive(false);
    }

    private void SetDecompositionList()
    {
        decomposition_forms.Clear();

        switch (menuIndex)
        {
            case 0:
                for (int i = 0; i < SaveScript.saveData.hasMiners.Length; i++)
                    if (SaveScript.saveData.hasMiners[i] != -1)
                        decomposition_forms.Add(i);
                break;
            case 1:
                for (int i = 0; i < SaveScript.saveData.hasAdventurers.Length; i++)
                    if (SaveScript.saveData.hasAdventurers[i] != -1)
                        decomposition_forms.Add(i);
                break;
        }
    }

    public void OnDecompositionAllOpen()
    {
        decompositionAllObject.SetActive(true);
        decompositionAllInfoText.text = "분해 총 건수 : " + decomposition_forms.Count + "건";
        Mine.instance.SetAudio(0);

        uIBoxes = decompositionFormPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uIBoxes.Length; i++)
            Destroy(uIBoxes[i].gameObject);
        decompositionFormPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        switch (menuIndex)
        {
            case 0: sprites = MineSlime.miner_defaultSprites; break;
            case 1: sprites = MineSlime.adventurer_defaultSprites; break;
        }

        for (int i = 0; i < decomposition_forms.Count; i++)
        {
            int code = -1;
            int level = -1;

            switch (menuIndex)
            {
                case 0:
                    code = SaveScript.saveData.hasMiners[decomposition_forms[i]];
                    level = SaveScript.saveData.hasMinerLevels[decomposition_forms[i]];
                    break;
                case 1:
                    code = SaveScript.saveData.hasAdventurers[decomposition_forms[i]];
                    level = SaveScript.saveData.hasAdventurerLevels[decomposition_forms[i]];
                    break;
            }

            uIBox = Instantiate(decompositionFormPrefab, decompositionFormPanel).GetComponent<UIBox>();
            uIBox.images[0].sprite = sprites[code];
            uIBox.images[1].sprite = manaSprites[GetManaType(code)];
            uIBox.texts[0].text = "[" + MineSlime.qualityNames[code] + "] (Lv." + level + ")";
            uIBox.texts[1].text = GetManaStr(code);
        }
    }

    public void OnDecompositionAllYes()
    {
        decompositionAllObject.SetActive(false);
        decomposition_animator.gameObject.SetActive(true);
        decomposition_animator.SetBool("isDecompositionAll", true);
        decomposition_animator.Play("DecompositionUI_DecompositionAll", -1, 0f);
        AchievementCtrl.instance.SetAchievementAmount(39, decomposition_forms.Count);
        Mine.instance.SetAudio(0);
        MineDecomposition.instance.passClickPanel.gameObject.SetActive(true);
        MineDecomposition.instance.passClickPanel.onClick.RemoveAllListeners();
        MineDecomposition.instance.passClickPanel.onClick.AddListener(MineDecomposition.instance.PassAllAni);
    }

    public void OnDecompositionAllNo()
    {
        decompositionAllObject.SetActive(false);
        Mine.instance.SetAudio(0);
    }

    // 분해 시작
    public void DecompositionUI_StartDecomposition()
    {
        decomposition_animator.gameObject.SetActive(true);
        decomposition_animator.SetBool("isDecomposition", true);
        decomposition_animator.Play("DecompositionUI_Decomposition", -1, 0f);
        AchievementCtrl.instance.SetAchievementAmount(39, 1);
        Mine.instance.SetAudio(0);
        MineDecomposition.instance.passClickPanel.gameObject.SetActive(true);
        MineDecomposition.instance.passClickPanel.onClick.RemoveAllListeners();
        MineDecomposition.instance.passClickPanel.onClick.AddListener(MineDecomposition.instance.PassAni);

        if (selectPetCode == -1) selectPetCode = 0;
        switch (menuIndex)
        {
            case 0: ani_petImage.sprite = MineSlime.miner_defaultSprites[selectPetCode]; break;
            case 1: ani_petImage.sprite = MineSlime.adventurer_defaultSprites[selectPetCode]; break;
        }
    }

    // DecompositionUI 분해 완료 창 닫기
    public void DecompositionUI_CloseDecomposition()
    {
        Mine.instance.SetAudio(0);
        decomposition_animator.gameObject.SetActive(false);
        MineDecomposition.instance.animator_result.SetActive(false);
        DecompositionUI_SetDefault();
    }

    private int GetManaType(int _code)
    {
        if (_code > 7) return 3;
        else if (_code > 5) return 2;
        else if (_code > 2) return 1;
        else return 0;
    }

    private string GetManaStr(int _code)
    {
        return GameFuction.GetNumText(manaNum_min[_code]) + "개 ~ " + GameFuction.GetNumText(manaNum_max[_code]) + "개";
    }

    static public int GetManaOreNum(int _code)
    {
        return Random.Range(manaNum_min[_code], manaNum_max[_code]);
    }
}
