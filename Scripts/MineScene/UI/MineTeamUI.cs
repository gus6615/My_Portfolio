using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MineTeamUI : MonoBehaviour
{
    public static MineTeamUI instance;

    public GameObject teamPetSlotPanel;
    public GameObject teamSelectPetObject;
    public GameObject teamSelectPetSpace;
    public GameObject teamSelectPageObject;
    public GameObject teamInvenPageObject;
    public GameObject teamInfoText;
    private UIBox[] selectPages, invenPages;
    public UIBox[] teamMenus;

    private int menuIndex, selectPetIndex;
    private int eventType;
    private int selectPage, invenPage;

    // 임시 데이터
    Mine_TeamSlot[] slots;
    UIBox uIBox;

    private void Start()
    {
        instance = this;
        selectPages = teamSelectPageObject.GetComponentsInChildren<UIBox>();
        invenPages = teamInvenPageObject.GetComponentsInChildren<UIBox>();
    }

    // 공통 UI 변수 초기화
    public void SetDefaultVariable()
    {
        eventType = menuIndex = 0;
        selectPage = invenPage = 0;
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
            if (i < 1 + (SaveScript.mineInvenMinNum - 1 + upgrade) / 10)
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

    public void ClickSelectPage()
    {
        uIBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uIBox != null)
        {
            selectPage = uIBox.order;
            Mine.instance.SetAudio(0);
        }

        if (Mine_TeamSlot.currentSlot != null)
        {
            Mine_TeamSlot.currentSlot.SetMenu(false);
            Mine_TeamSlot.currentSlot = null;
        }

        TeamUI_SetSelectPet();
    }

    public void ClickInvenPage()
    {
        uIBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uIBox != null)
        {
            invenPage = uIBox.order;
            Mine.instance.SetAudio(0);
        }

        TeamUI_SetAddPet();
    }

    // TeamUI 초기화
    public void TeamUI_Menu()
    {
        SetDefaultVariable();
        uIBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (uIBox != null)
        {
            menuIndex = uIBox.order;
            Mine.instance.SetAudio(0);
        }

        teamSelectPetObject.SetActive(false);
        TeamUI_SetSelectPet();
    }

    // TeamUI 펫 선택 창 설정
    public void TeamUI_SetSelectPet()
    {
        if (Mine_TeamSlot.currentSlot != null)
        {
            Mine_TeamSlot.currentSlot.SetMenu(false);
            Mine_TeamSlot.currentSlot = null;
        }

        for (int i = 0; i < teamMenus.Length; i++)
        {
            teamMenus[i].images[0].color = Color.white;
            teamMenus[i].texts[0].color = new Color(1f, 0.9f, 0.6f);
        }
        teamMenus[menuIndex].images[0].color = new Color(0.6f, 0.6f, 0.6f);
        teamMenus[menuIndex].texts[0].color = new Color(0.6f, 0.6f, 0.4f);
        SetSelectPageUI(selectPage);

        slots = teamPetSlotPanel.GetComponentsInChildren<Mine_TeamSlot>();
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
                    slots[i].menuOb.SetActive(false);
                    teamInfoText.SetActive(SaveScript.mineSlotMinNum + SaveScript.saveData.minerUpgrades[4] * SaveScript.mineUpgradePercents[4] <= selectPage * 6);
                    slots[i].noneOb.SetActive(index < SaveScript.mineSlotMinNum + SaveScript.saveData.minerUpgrades[4] * SaveScript.mineUpgradePercents[4]);

                    if (code != -1 && index < SaveScript.mineSlotMinNum + SaveScript.saveData.minerUpgrades[4] * SaveScript.mineUpgradePercents[4])
                    {
                        slots[i].existOb.SetActive(true);
                        slots[i].noneOb.SetActive(false);
                        slots[i].exist_spriteImage.sprite = MineSlime.miner_defaultSprites[code];
                        slots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] (Lv." + SaveScript.saveData.hasOnMinerLevels[index] + ") " + MineMap.instance.minerObjects[index].name;
                        slots[i].exist_timeText.text = ":: 상자 획득 시간 ::\n < " + GameFuction.GetTimeText(MinerSlime.GetTimeAsLevel(code, SaveScript.saveData.hasOnMinerLevels[index])) + " >";
                    }
                    else
                    {
                        slots[i].existOb.SetActive(false);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < slots.Length; i++)
                {
                    index = selectPage * 6 + i;
                    code = SaveScript.saveData.hasOnAdventurers[index];
                    slots[i].index = index;
                    slots[i].menuOb.SetActive(false);
                    teamInfoText.SetActive(SaveScript.mineSlotMinNum + SaveScript.saveData.adventurerUpgrades[4] * SaveScript.mineUpgradePercents[4] <= selectPage * 6);
                    slots[i].noneOb.SetActive(index < SaveScript.mineSlotMinNum + SaveScript.saveData.adventurerUpgrades[4] * SaveScript.mineUpgradePercents[4]);

                    if (code != -1 && index < SaveScript.mineSlotMinNum + SaveScript.saveData.adventurerUpgrades[4] * SaveScript.mineUpgradePercents[4])
                    {
                        slots[i].existOb.SetActive(true);
                        slots[i].noneOb.SetActive(false);
                        slots[i].exist_spriteImage.sprite = MineSlime.adventurer_defaultSprites[code];
                        slots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] (Lv." + SaveScript.saveData.hasOnAdventurerLevels[index] + ") " + MineMap.instance.adventurerObjects[index].name;
                        slots[i].exist_timeText.text = ":: 상자 획득 시간 ::\n < " + GameFuction.GetTimeText(AdventurerSlime.GetTimeAsLevel(code, SaveScript.saveData.hasOnAdventurerLevels[index])) + " >";
                    }
                    else
                    {
                        slots[i].existOb.SetActive(false);
                    }
                }
                break;
        }
    }

    // TeamUI 펫 메뉴 
    public void TeamUI_PetMenu()
    {
        Mine_TeamSlot data = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Mine_TeamSlot>();
        Mine.instance.SetAudio(0);
        if (Mine_TeamSlot.currentSlot == null || Mine_TeamSlot.currentSlot != data)
        {
            if (Mine_TeamSlot.currentSlot != null)
                Mine_TeamSlot.currentSlot.SetMenu(false);
            data.SetMenu(true);
            Mine_TeamSlot.currentSlot = data;
            selectPetIndex = data.index;
        }
        else
        {
            data.SetMenu(false);
            Mine_TeamSlot.currentSlot = null;
        }
    }

    // TeamUI 펫 변경
    public void TeamUI_Change()
    {
        teamSelectPetObject.SetActive(true);
        eventType = 0;
        Mine.instance.SetAudio(0);
        if (Mine_TeamSlot.currentSlot != null)
        {
            Mine_TeamSlot.currentSlot.SetMenu(false);
            Mine_TeamSlot.currentSlot = null;
        }
        TeamUI_SetAddPet();
    }

    // TeamUI 펫 추가
    public void TeamUI_Add()
    {
        selectPetIndex = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Mine_TeamSlot>().index;
        teamSelectPetObject.SetActive(true);
        eventType = 1;
        Mine.instance.SetAudio(0);
        if (Mine_TeamSlot.currentSlot != null)
        {
            Mine_TeamSlot.currentSlot.SetMenu(false);
            Mine_TeamSlot.currentSlot = null;
        }
        TeamUI_SetAddPet();
    }

    // TeamUI 펫 빼기
    public void TeamUI_Remove()
    {
        int code = 0;
        int index = -1; // 빠질 위치 index  
        bool isCanRemove = false;
        Mine.instance.SetAudio(0);

        switch (menuIndex)
        {
            case 0:
                code = SaveScript.saveData.hasOnMiners[selectPetIndex];
                if ((index = MinerSlime.FindEmptyPetInven()) != -1)
                    isCanRemove = true;
                break;
            case 1:
                code = SaveScript.saveData.hasOnAdventurers[selectPetIndex];
                if ((index = AdventurerSlime.FindEmptyPetInven()) != -1)
                    isCanRemove = true;
                break;
        }

        if (isCanRemove)
        {
            switch (menuIndex)
            {
                case 0:
                    SaveScript.saveData.hasMiners[index] = code;
                    SaveScript.saveData.hasMinerLevels[index] = SaveScript.saveData.hasOnMinerLevels[selectPetIndex];
                    SaveScript.saveData.hasMinerExps[index] = SaveScript.saveData.hasOnMinerExps[selectPetIndex];
                    SaveScript.saveData.hasOnMiners[selectPetIndex] = SaveScript.saveData.hasOnMinerLevels[selectPetIndex] = -1;
                    SaveScript.saveData.hasOnMinerExps[selectPetIndex] = -1;
                    SaveScript.saveData.hasMinerRewards[selectPetIndex] = SaveScript.saveData.hasMinerTimes[selectPetIndex] = 0;
                    MinerSlime.SortPetInven();
                    break;
                case 1:
                    SaveScript.saveData.hasAdventurers[index] = code;
                    SaveScript.saveData.hasAdventurerLevels[index] = SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex];
                    SaveScript.saveData.hasAdventurerExps[index] = SaveScript.saveData.hasOnAdventurerExps[selectPetIndex];
                    SaveScript.saveData.hasOnAdventurers[selectPetIndex] = SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex] = -1;
                    SaveScript.saveData.hasOnAdventurerExps[selectPetIndex] = -1;
                    SaveScript.saveData.hasAdventurerRewards[selectPetIndex] = SaveScript.saveData.hasAdventurerTimes[selectPetIndex] = 0;
                    AdventurerSlime.SortPetInven();
                    break;
            }

            MineMap.instance.RefreshPetObject(menuIndex, selectPetIndex);
            TeamUI_SetSelectPet();
        }
        else
        {
            SystemInfoCtrl.instance.SetErrorInfo("펫 인벤토리가 꽉 찼습니다!");
        }
    }

    // TeamUI AddPetUI 닫기
    public void TeamUI_CloseAddPet()
    {
        teamSelectPetObject.SetActive(false);
        invenPage = 0;
        Mine.instance.SetAudio(0);
    }

    // TeamUI AddPetUI 설정
    public void TeamUI_SetAddPet()
    {
        slots = teamSelectPetSpace.GetComponentsInChildren<Mine_TeamSlot>();
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

    // TeamUI 펫 선택
    public void TeamUI_ClickPet()
    {
        int ChangingIndex = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Mine_TeamSlot>().index;

        switch (eventType)
        {
            case 0: // 펫 변경
                void Temp(ref int a, ref int b) { int temp = a; a = b; b = temp; }
                void Temp2(ref long a, ref long b) { long temp = a; a = b; b = temp; }
                switch (menuIndex)
                {
                    case 0:
                        Temp(ref SaveScript.saveData.hasOnMiners[selectPetIndex], ref SaveScript.saveData.hasMiners[ChangingIndex]);
                        Temp(ref SaveScript.saveData.hasOnMinerLevels[selectPetIndex], ref SaveScript.saveData.hasMinerLevels[ChangingIndex]);
                        Temp2(ref SaveScript.saveData.hasOnMinerExps[selectPetIndex], ref SaveScript.saveData.hasMinerExps[ChangingIndex]);
                        SaveScript.saveData.hasMinerRewards[selectPetIndex] = 0;
                        SaveScript.saveData.hasMinerTimes[selectPetIndex] = MinerSlime.GetTimeAsLevel(SaveScript.saveData.hasOnMiners[selectPetIndex], SaveScript.saveData.hasOnMinerLevels[selectPetIndex]);
                        MinerSlime.SortPetInven();
                        break;
                    case 1:
                        Temp(ref SaveScript.saveData.hasOnAdventurers[selectPetIndex], ref SaveScript.saveData.hasAdventurers[ChangingIndex]);
                        Temp(ref SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex], ref SaveScript.saveData.hasAdventurerLevels[ChangingIndex]);
                        Temp2(ref SaveScript.saveData.hasOnAdventurerExps[selectPetIndex], ref SaveScript.saveData.hasAdventurerExps[ChangingIndex]);
                        SaveScript.saveData.hasAdventurerRewards[selectPetIndex] = 0;
                        SaveScript.saveData.hasAdventurerTimes[selectPetIndex] = AdventurerSlime.GetTimeAsLevel(SaveScript.saveData.hasOnAdventurers[selectPetIndex], SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex]);
                        AdventurerSlime.SortPetInven();
                        break;
                }
                break;
            case 1: // 펫 추가
                switch (menuIndex)
                {
                    case 0:
                        SaveScript.saveData.hasOnMiners[selectPetIndex] = SaveScript.saveData.hasMiners[ChangingIndex];
                        SaveScript.saveData.hasOnMinerLevels[selectPetIndex] = SaveScript.saveData.hasMinerLevels[ChangingIndex];
                        SaveScript.saveData.hasOnMinerExps[selectPetIndex] = SaveScript.saveData.hasMinerExps[ChangingIndex];
                        SaveScript.saveData.hasMiners[ChangingIndex] = SaveScript.saveData.hasMinerLevels[ChangingIndex] = -1;
                        SaveScript.saveData.hasMinerExps[ChangingIndex] = -1;
                        SaveScript.saveData.hasMinerRewards[selectPetIndex] = 0;
                        SaveScript.saveData.hasMinerTimes[selectPetIndex] = MinerSlime.GetTimeAsLevel(SaveScript.saveData.hasOnMiners[selectPetIndex], SaveScript.saveData.hasOnMinerLevels[selectPetIndex]);
                        MinerSlime.SortPetInven();
                        break;
                    case 1:
                        SaveScript.saveData.hasOnAdventurers[selectPetIndex] = SaveScript.saveData.hasAdventurers[ChangingIndex];
                        SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex] = SaveScript.saveData.hasAdventurerLevels[ChangingIndex];
                        SaveScript.saveData.hasOnAdventurerExps[selectPetIndex] = SaveScript.saveData.hasAdventurerExps[ChangingIndex];
                        SaveScript.saveData.hasAdventurers[ChangingIndex] = SaveScript.saveData.hasAdventurerLevels[ChangingIndex] = -1;
                        SaveScript.saveData.hasAdventurerExps[ChangingIndex] = -1;
                        SaveScript.saveData.hasAdventurerRewards[selectPetIndex] = 0;
                        SaveScript.saveData.hasAdventurerTimes[selectPetIndex] = AdventurerSlime.GetTimeAsLevel(SaveScript.saveData.hasOnAdventurers[selectPetIndex], SaveScript.saveData.hasOnAdventurerLevels[selectPetIndex]);
                        AdventurerSlime.SortPetInven();
                        break;
                }
                break;
        }

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 33 });
        teamSelectPetObject.SetActive(false);
        invenPage = 0;
        Mine.instance.SetAudio(0);
        MineMap.instance.RefreshPetObject(menuIndex, selectPetIndex);
        TeamUI_SetSelectPet();
    }
}
