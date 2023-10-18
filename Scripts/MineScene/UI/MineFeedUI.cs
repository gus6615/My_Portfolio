using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MineFeedUI : MonoBehaviour
{
    private const int ITEM_MAX = 1000000;
    public static MineFeedUI instance;

    public UIBox[] feedMenus;
    public Text feedInfoText;
    public GameObject selectPetPanel;
    public GameObject feedSelectPageObject;
    public GameObject selectPetInfo;
    public Animator feedUI_animator;
    public GameObject selectItemPrefab;
    public Transform selectItemPanel;
    public GameObject selectItemInfo;
    public InputNumBox itemNumBox;
    public Text B_itemNameText, B_itemExpText, A_itemNameText, A_itemExpText;
    public Slider B_itemExpSlider, A_itemExpSlider;
    private Image petImage;
    private Text petNameText, petTimeText, petBeforeText, petAfterText, petExpText;
    private Slider petExpSlider;
    private Image itemImage;
    private Text itemNameText, itemCanHaveExpText;
    private UIBox[] selectPages;

    private int menuIndex, selectPetIndex;
    private int selectItemType, selectItemCode;
    private int selectPage;

    // 임시 데이터
    Text[] texts;
    Mine_TeamSlot[] slots;
    Order[] datas;
    Order data;
    UIBox uibox;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        texts = selectPetInfo.GetComponentsInChildren<Text>();
        selectPages = feedSelectPageObject.GetComponentsInChildren<UIBox>();

        petImage = selectPetInfo.GetComponentsInChildren<Image>()[1];
        petNameText = texts[0];
        petTimeText = texts[1];
        petBeforeText = texts[2];
        petAfterText = texts[3];
        petExpText = texts[5];
        petExpSlider = selectPetInfo.GetComponentInChildren<Slider>();

        texts = selectItemInfo.GetComponentsInChildren<Text>();
        itemImage = selectItemInfo.GetComponentsInChildren<Image>()[2];
        itemNameText = texts[0];
        itemCanHaveExpText = texts[1];
    }

    // 공통 UI 변수 초기화
    public void SetDefaultVariable()
    {
        menuIndex = selectItemType = selectItemCode = 0;
        selectPage = 0;
        selectPetIndex = -1;
    }

    public void SetSelectPageUI(int _activePage)
    {
        int upgrade = 0;

        switch (menuIndex)
        {
            case 0: upgrade = SaveScript.saveData.minerUpgrades[5]; break;
            case 1: upgrade = SaveScript.saveData.adventurerUpgrades[5]; break;
        }

        for (int i = 0; i < selectPages.Length; i++)
        {
            if (i < 1 + upgrade)
            {
                selectPages[i].button.enabled = true;
                selectPages[i].images[0].color = new Color(1f, 1f, 1f, 1f);
                selectPages[i].texts[0].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                selectPages[i].button.enabled = false;
                selectPages[i].images[0].color = new Color(1f, 1f, 1f, 0.1f);
                selectPages[i].texts[0].color = new Color(1f, 1f, 1f, 0.1f);
            }
        }
        selectPages[_activePage].button.enabled = true;
        selectPages[_activePage].images[0].color = new Color(0.6f, 0.6f, 0.6f, 1f);
        selectPages[_activePage].texts[0].color = new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    public void ClickSelectPage()
    {
        uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uibox != null)
        {
            selectPage = uibox.order;
            Mine.instance.SetAudio(0);
        }

        FeedUI_SetSelectPet();
    }

    // FeedUI 초기화
    public void FeedUI_Menu()
    {
        uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uibox != null)
        {
            menuIndex = uibox.order;
            Mine.instance.SetAudio(0);
        }

        selectPage = 0;
        selectItemInfo.SetActive(false);
        selectPetInfo.SetActive(false);
        feedUI_animator.SetBool("isDown", false);
        FeedUI_SetSelectPet();
    }

    // FeedUI 펫 선택 설정
    public void FeedUI_SetSelectPet()
    {
        for (int i = 0; i < feedMenus.Length; i++)
        {
            feedMenus[i].images[0].color = Color.white;
            feedMenus[i].texts[0].color = new Color(1f, 0.9f, 0.6f);
        }
        feedMenus[menuIndex].images[0].color = new Color(0.6f, 0.6f, 0.6f);
        feedMenus[menuIndex].texts[0].color = new Color(0.6f, 0.6f, 0.4f);
        SetSelectPageUI(selectPage);

        slots = selectPetPanel.GetComponentsInChildren<Mine_TeamSlot>();
        int code;
        int index;

        switch (menuIndex)
        {
            case 0:
                for (int i = 0; i < slots.Length; i++)
                {
                    index = selectPage * 6 + i;
                    code = SaveScript.saveData.hasOnMiners[index];
                    slots[i].index = index;
                    feedInfoText.gameObject.SetActive(SaveScript.mineSlotMinNum + SaveScript.saveData.minerUpgrades[4] * SaveScript.mineUpgradePercents[4] <= selectPage * 6);
                    slots[i].SetActive(true);

                    if (index >= SaveScript.mineSlotMinNum + SaveScript.saveData.minerUpgrades[4] * SaveScript.mineUpgradePercents[4])
                    {
                        slots[i].SetActive(false);
                    }
                    else
                    {
                        if (code != -1)
                        {
                            slots[i].exist_spriteImage.sprite = MineSlime.miner_defaultSprites[code];
                            slots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasOnMinerLevels[index];
                        }
                        else
                        {
                            slots[i].exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
                            slots[i].exist_obImage.color = new Color(1f, 1f, 1f, 0.4f);
                            slots[i].exist_nameText.text = "";
                            slots[i].button.enabled = false;
                        }
                    }
                }
                break;
            case 1:
                for (int i = 0; i < slots.Length; i++)
                {
                    index = selectPage * 6 + i;
                    code = SaveScript.saveData.hasOnAdventurers[index];
                    slots[i].index = index;
                    feedInfoText.gameObject.SetActive(SaveScript.mineSlotMinNum + SaveScript.saveData.adventurerUpgrades[4] * SaveScript.mineUpgradePercents[4] <= selectPage * 6);
                    slots[i].SetActive(true);

                    if (index >= SaveScript.mineSlotMinNum + SaveScript.saveData.adventurerUpgrades[4] * SaveScript.mineUpgradePercents[4])
                    {
                        slots[i].SetActive(false);
                    }
                    else
                    {
                        if (code != -1)
                        {
                            slots[i].exist_spriteImage.sprite = MineSlime.adventurer_defaultSprites[code];
                            slots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasOnAdventurerLevels[index];
                        }
                        else
                        {
                            slots[i].exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
                            slots[i].exist_obImage.color = new Color(1f, 1f, 1f, 0.4f);
                            slots[i].exist_nameText.text = "";
                            slots[i].button.enabled = false;
                        }
                    }
                }
                break;
        }
    }

    // FeedUI 선택한 펫 정보 설정
    public void FeedUI_SetSelectPetInfo()
    {
        selectPetIndex = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Mine_TeamSlot>().index;
        selectPetInfo.SetActive(true);
        Mine.instance.SetAudio(0);

        int index = selectPetIndex;
        int code = 0;
        int level = 0;
        long exp = 0;
        long b_power01 = 0, b_power02 = 0;
        long a_power01 = 0, a_power02 = 0;
        int time = 0;
        string name = "";
        Sprite face = null;

        switch (menuIndex)
        {
            case 0:
                code = SaveScript.saveData.hasOnMiners[index];
                level = SaveScript.saveData.hasOnMinerLevels[index];
                exp = SaveScript.saveData.hasOnMinerExps[index];
                b_power01 = MinerSlime.GetAmountPower(code, level);
                b_power02 = MinerSlime.GetTimePower(level);
                a_power01 = MinerSlime.GetAmountPower(code, level + 1);
                a_power02 = MinerSlime.GetTimePower(level + 1);
                time = MinerSlime.GetTimeAsLevel(code, level);
                name = MineMap.instance.minerObjects[index].name;
                face = MineSlime.miner_defaultSprites[code];
                break;
            case 1:
                code = SaveScript.saveData.hasOnAdventurers[index];
                level = SaveScript.saveData.hasOnAdventurerLevels[index];
                exp = SaveScript.saveData.hasOnAdventurerExps[index];
                b_power01 = AdventurerSlime.GetAmountPower(code, level);
                b_power02 = AdventurerSlime.GetTimePower(level);
                a_power01 = AdventurerSlime.GetAmountPower(code, level + 1);
                a_power02 = AdventurerSlime.GetTimePower(level + 1);
                time = AdventurerSlime.GetTimeAsLevel(code, level);
                name = MineMap.instance.adventurerObjects[index].name;
                face = MineSlime.adventurer_defaultSprites[code];
                break;
        }

        petImage.sprite = face;
        petNameText.text = "[" + MineSlime.qualityNames[code] + "] (Lv." + level + ") "+ name;
        petTimeText.text = ":: 상자 획득 시간 ::\n< " + GameFuction.GetTimeText(time) + " >";
        petBeforeText.text = "[ 현재 (Lv." + level + ") 능력치\n\n보상 획득 파워 : " + GameFuction.GetNumText(b_power01) + "\n시간 단축 : " + GameFuction.GetTimeText((int)b_power02);
        petAfterText.text = "[ 다음 (Lv." + (level + 1) + ") 능력치\n\n보상 획득 파워 : " + GameFuction.GetNumText(a_power01) + "\n시간 단축 : " + GameFuction.GetTimeText((int)a_power02);
        petExpSlider.maxValue = MineSlime.GetExpAsLevel(level);
        petExpSlider.value = exp;
        petExpText.text = GameFuction.GetNumText((long)petExpSlider.value) + " / " + GameFuction.GetNumText((long)petExpSlider.maxValue);
    }

    // FeedUI 선택한 펫 확정
    public void FeedUI_ConfirmPet()
    {
        feedUI_animator.SetBool("isDown", true);
        selectItemInfo.SetActive(false);
        FeedUI_SetSelectItem();
        Mine.instance.SetAudio(0);
    }

    // FeedUI 아이템 선택 설정
    public void FeedUI_SetSelectItem()
    {
        datas = selectItemPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);

        for (int i = 0; i < SaveScript.saveData.hasElixirs.Length; i++)
        {
            if (SaveScript.saveData.hasElixirs[i] != 0)
            {
                data = Instantiate(selectItemPrefab, selectItemPanel).GetComponent<Order>();
                data.order = 2;
                data.order2 = i;
                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.elixirs[i].sprite;
                data.GetComponentsInChildren<Text>()[0].text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasElixirs[i]);
                data.GetComponent<Button>().onClick.AddListener(FeedUI_SetSelectItemInfo);
            }
        }

        for (int i = 0; i < SaveScript.saveData.hasBufItems.Length; i++)
        {
            if (SaveScript.saveData.hasBufItems[i] != 0)
            {
                data = Instantiate(selectItemPrefab, selectItemPanel).GetComponent<Order>();
                data.order = 0;
                data.order2 = i;
                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.bufItems[i].sprite;
                data.GetComponentsInChildren<Text>()[0].text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasBufItems[i]);
                data.GetComponent<Button>().onClick.AddListener(FeedUI_SetSelectItemInfo);
            }
        }

        for (int i = 0; i < SaveScript.saveData.hasReinforceItems2.Length; i++)
        {
            if (SaveScript.saveData.hasReinforceItems2[i] != 0)
            {
                data = Instantiate(selectItemPrefab, selectItemPanel).GetComponent<Order>();
                data.order = 3;
                data.order2 = i;
                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems2[i].sprite;
                data.GetComponentsInChildren<Text>()[0].text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems2[i]);
                data.GetComponent<Button>().onClick.AddListener(FeedUI_SetSelectItemInfo);
            }
        }

        for (int i = 0; i < SaveScript.saveData.hasReinforceItems.Length; i++)
        {
            if (SaveScript.saveData.hasReinforceItems[i] != 0)
            {
                data = Instantiate(selectItemPrefab, selectItemPanel).GetComponent<Order>();
                data.order = 1;
                data.order2 = i;
                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems[i].sprite;
                data.GetComponentsInChildren<Text>()[0].text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems[i]);
                data.GetComponent<Button>().onClick.AddListener(FeedUI_SetSelectItemInfo);
            }
        }
    }

    // FeedUI 선택한 아이템 정보 설정
    public void FeedUI_SetSelectItemInfo()
    {
        data = EventSystem.current.currentSelectedGameObject.GetComponent<Order>();
        selectItemType = data.order;
        selectItemCode = data.order2;
        selectItemInfo.SetActive(true);
        Mine.instance.SetAudio(0);

        switch (selectItemType)
        {
            case 0:
                itemImage.sprite = SaveScript.bufItems[selectItemCode].sprite;
                itemNameText.text = "< " + SaveScript.bufItems[selectItemCode].name + " >";
                if (SaveScript.saveData.hasBufItems[selectItemCode] > ITEM_MAX)
                    itemNumBox.slider.maxValue = ITEM_MAX;
                else
                    itemNumBox.slider.maxValue = SaveScript.saveData.hasBufItems[selectItemCode];
                break;
            case 1:
                itemImage.sprite = SaveScript.reinforceItems[selectItemCode].sprite;
                itemNameText.text = "< " + SaveScript.reinforceItems[selectItemCode].name + " >";
                if (SaveScript.saveData.hasReinforceItems[selectItemCode] > ITEM_MAX)
                    itemNumBox.slider.maxValue = ITEM_MAX;
                else
                    itemNumBox.slider.maxValue = SaveScript.saveData.hasReinforceItems[selectItemCode];
                break;
            case 2:
                itemImage.sprite = SaveScript.elixirs[selectItemCode].sprite;
                itemNameText.text = "< " + SaveScript.elixirs[selectItemCode].name + " >";
                if (SaveScript.saveData.hasElixirs[selectItemCode] > ITEM_MAX)
                    itemNumBox.slider.maxValue = ITEM_MAX;
                else
                    itemNumBox.slider.maxValue = SaveScript.saveData.hasElixirs[selectItemCode];
                break;
            case 3:
                itemImage.sprite = SaveScript.reinforceItems2[selectItemCode].sprite;
                itemNameText.text = "< " + SaveScript.reinforceItems2[selectItemCode].name + " >";
                if (SaveScript.saveData.hasReinforceItems2[selectItemCode] > ITEM_MAX)
                    itemNumBox.slider.maxValue = ITEM_MAX;
                else
                    itemNumBox.slider.maxValue = SaveScript.saveData.hasReinforceItems2[selectItemCode];
                break;
        }

        itemNumBox.itemNum = (long)itemNumBox.slider.maxValue;
        itemNumBox.slider.value = itemNumBox.slider.maxValue;
        FeedUI_SetItemSlider();
    }

    public void FeedUI_SetItemSlider()
    {
        long exp = 0; // 현재 exp
        long plusExp = 0; // 더해질 exp
        int level = 0;
        switch (selectItemType)
        {
            case 0: plusExp = itemNumBox.itemNum * BufItem.exps[selectItemCode % 3]; break;
            case 1: plusExp = itemNumBox.itemNum * ReinforceItem.exps[selectItemCode]; break;
            case 2: plusExp = itemNumBox.itemNum * Elixir.exp; break;
            case 3: plusExp = itemNumBox.itemNum * ReinforceItem2.exps[selectItemCode]; break;
        }

        itemNumBox.inputField.text = GameFuction.GetNumText(itemNumBox.itemNum);
        itemCanHaveExpText.text = "획득 가능 경험치 : " + GameFuction.GetNumText(plusExp) + " EXP";
        switch (menuIndex)
        {
            case 0:
                exp = SaveScript.saveData.hasOnMinerExps[selectPetIndex];
                level = SaveScript.saveData.hasOnMinerLevels[selectPetIndex];
                B_itemNameText.text = "Lv." + level + " [ " + MineMap.instance.minerObjects[selectPetIndex].name + " ]";
                B_itemExpSlider.maxValue = MineSlime.GetExpAsLevel(level);
                B_itemExpSlider.value = exp;
                B_itemExpText.text = GameFuction.GetNumText((long)B_itemExpSlider.value) + " / " + GameFuction.GetNumText((long)B_itemExpSlider.maxValue);

                exp += plusExp;
                while (exp >= MineSlime.GetExpAsLevel(level))
                {
                    exp -= MineSlime.GetExpAsLevel(level);
                    level++;
                }

                A_itemNameText.text = "Lv." + level + " [ " + MineMap.instance.minerObjects[selectPetIndex].name + " ]";
                A_itemExpSlider.maxValue = MineSlime.GetExpAsLevel(level);
                A_itemExpSlider.value = exp;
                A_itemExpText.text = GameFuction.GetNumText((long)A_itemExpSlider.value) + " / " + GameFuction.GetNumText((long)A_itemExpSlider.maxValue);
                break;
            case 1:
                exp = SaveScript.saveData.hasOnAdventurerExps[selectPetIndex];
                level = SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex];
                B_itemNameText.text = "Lv." + level + " [ " + MineMap.instance.adventurerObjects[selectPetIndex].name + " ]";
                B_itemExpSlider.maxValue = MineSlime.GetExpAsLevel(level);
                B_itemExpSlider.value = exp;
                B_itemExpText.text = GameFuction.GetNumText((long)B_itemExpSlider.value) + " / " + GameFuction.GetNumText((long)B_itemExpSlider.maxValue);

                exp += plusExp;
                while (exp >= MineSlime.GetExpAsLevel(level))
                {
                    exp -= MineSlime.GetExpAsLevel(level);
                    level++;
                }

                A_itemNameText.text = "Lv." + level + " [ " + MineMap.instance.adventurerObjects[selectPetIndex].name + " ]";
                A_itemExpSlider.maxValue = MineSlime.GetExpAsLevel(level);
                A_itemExpSlider.value = exp;
                A_itemExpText.text = GameFuction.GetNumText((long)A_itemExpSlider.value) + " / " + GameFuction.GetNumText((long)A_itemExpSlider.maxValue);
                break;
        }
    }

    // FeedUI 선택한 아이템 확정
    public void FeedUI_ConfirmItem()
    {
        long getExp = 0;
        switch (selectItemType)
        {
            case 0:
                getExp = itemNumBox.itemNum * BufItem.exps[selectItemCode % 3];
                SaveScript.saveData.hasBufItems[selectItemCode] -= itemNumBox.itemNum;
                Mine.instance.SetAudio(23);
                break;
            case 1:
                getExp = itemNumBox.itemNum * ReinforceItem.exps[selectItemCode];
                SaveScript.saveData.hasReinforceItems[selectItemCode] -= itemNumBox.itemNum;
                Mine.instance.SetAudio(24);
                break;
            case 2:
                getExp = itemNumBox.itemNum * Elixir.exp;
                SaveScript.saveData.hasElixirs[selectItemCode] -= itemNumBox.itemNum;
                Mine.instance.SetAudio(23);
                break;
            case 3:
                getExp = itemNumBox.itemNum * ReinforceItem2.exps[selectItemCode];
                SaveScript.saveData.hasReinforceItems2[selectItemCode] -= itemNumBox.itemNum;
                Mine.instance.SetAudio(14);
                break;
        }

        switch (menuIndex)
        {
            case 0:
                SaveScript.saveData.hasOnMinerExps[selectPetIndex] += getExp;
                while (SaveScript.saveData.hasOnMinerExps[selectPetIndex] >= MineSlime.GetExpAsLevel(SaveScript.saveData.hasOnMinerLevels[selectPetIndex]))
                {
                    SaveScript.saveData.hasOnMinerExps[selectPetIndex] -= MineSlime.GetExpAsLevel(SaveScript.saveData.hasOnMinerLevels[selectPetIndex]);
                    SaveScript.saveData.hasOnMinerLevels[selectPetIndex]++;
                    AchievementCtrl.instance.SetAchievementAmount(36, 1);
                    Mine.instance.SetAudio(5);
                }
                MinerSlime.SortPetInven();
                break;
            case 1:
                SaveScript.saveData.hasOnAdventurerExps[selectPetIndex] += getExp;
                while (SaveScript.saveData.hasOnAdventurerExps[selectPetIndex] >= MineSlime.GetExpAsLevel(SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex]))
                {
                    SaveScript.saveData.hasOnAdventurerExps[selectPetIndex] -= MineSlime.GetExpAsLevel(SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex]);
                    SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex]++;
                    AchievementCtrl.instance.SetAchievementAmount(36, 1);
                    Mine.instance.SetAudio(5);
                }
                AdventurerSlime.SortPetInven();
                break;
        }

        selectItemInfo.SetActive(false);
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 34 });
        MineMap.instance.RefreshPetObject(menuIndex, selectPetIndex);
        FeedUI_SetSelectItem();
    }
}
