using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PetInven : MonoBehaviour
{
    public static PetInven instance;

    public GameObject petInven;
    public GameObject slotContent;
    public GameObject teamInvenPageObject;
    public GameObject reconfirm;
    public Mine_TeamSlot currentPetSlot, selectPetSlot;
    private Image[] invenPageImages;
    private Text[] invenPageTexts;
    private Mine_TeamSlot[] petSlots;
    private int currentPetType, currentPetCode, selectPetIndex;
    private int invenPage;

    private void Start()
    {
        instance = this;
        invenPageImages = teamInvenPageObject.GetComponentsInChildren<Image>();
        invenPageTexts = teamInvenPageObject.GetComponentsInChildren<Text>();
        petSlots = slotContent.GetComponentsInChildren<Mine_TeamSlot>();

        currentPetType = currentPetCode = selectPetIndex = -1;
        petInven.SetActive(false);
    }

    public void SetInvenPageUI()
    {
        int upgrade = 0;

        switch (currentPetType)
        {
            case 0: upgrade = SaveScript.saveData.minerUpgrades[3]; break;
            case 1: upgrade = SaveScript.saveData.adventurerUpgrades[3]; break;
        }

        for (int i = 0; i < invenPageImages.Length; i++)
        {
            if (i < 1 + (SaveScript.mineInvenMinNum - 1 + upgrade) / 10)
            {
                invenPageImages[i].GetComponent<Button>().enabled = true;
                invenPageImages[i].color = new Color(1f, 1f, 1f, 1f);
                invenPageTexts[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                invenPageImages[i].GetComponent<Button>().enabled = false;
                invenPageImages[i].color = new Color(1f, 1f, 1f, 0.4f);
                invenPageTexts[i].color = new Color(1f, 1f, 1f, 0.4f);
            }
        }
    }

    public void ClickInvenPage()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
        {
            invenPage = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
            PrintUI.instance.AudioPlay(0);
        }

        SetInven();
    }

    /// <summary>
    /// 펫 인벤토리 UI를 설정하여 보여줍니다.
    /// </summary>
    /// <param name="_type">펫 종류</param>
    /// <param name="_code">펫 코드</param>
    public void ShowPetInven(int _type, int _code)
    {
        currentPetType = _type;
        currentPetCode = _code;
        Time.timeScale = 0f;
        petInven.SetActive(true);
        reconfirm.SetActive(false);
        SetInven();
    }

    public void SetInven()
    {
        int code;
        int index;
        SetInvenPageUI();

        switch (currentPetType)
        {
            case 0:
                for (int i = 0; i < petSlots.Length; i++)
                {
                    index = invenPage * 10 + i;
                    code = SaveScript.saveData.hasMiners[index];
                    petSlots[i].index = index;
                    if (index < SaveScript.mineInvenMinNum + SaveScript.saveData.minerUpgrades[3] * SaveScript.mineUpgradePercents[3])
                        petSlots[i].existOb.SetActive(true);
                    else
                        petSlots[i].existOb.SetActive(false);

                    if (code != -1)
                    {
                        petSlots[i].SetMenu(false);
                        petSlots[i].existOb.GetComponent<Button>().enabled = true;
                        petSlots[i].exist_spriteImage.sprite = MineSlime.miner_defaultSprites[code];
                        petSlots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasMinerLevels[index];
                    }
                    else
                    {
                        petSlots[i].SetMenu(true);
                        petSlots[i].existOb.GetComponent<Button>().enabled = false;
                        petSlots[i].exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
                        petSlots[i].exist_nameText.text = "";
                    }
                }
                break;
            case 1:
                for (int i = 0; i < petSlots.Length; i++)
                {
                    index = invenPage * 10 + i;
                    code = SaveScript.saveData.hasAdventurers[index];
                    petSlots[i].index = index;
                    if (index < SaveScript.mineInvenMinNum + SaveScript.saveData.adventurerUpgrades[3] * SaveScript.mineUpgradePercents[3])
                        petSlots[i].existOb.SetActive(true);
                    else
                        petSlots[i].existOb.SetActive(false);

                    if (code != -1)
                    {
                        petSlots[i].SetMenu(false);
                        petSlots[i].existOb.GetComponent<Button>().enabled = true;
                        petSlots[i].exist_spriteImage.sprite = MineSlime.adventurer_defaultSprites[code];
                        petSlots[i].exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasAdventurerLevels[index];
                    }
                    else
                    {
                        petSlots[i].SetMenu(true);
                        petSlots[i].existOb.GetComponent<Button>().enabled = false;
                        petSlots[i].exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
                        petSlots[i].exist_nameText.text = "";
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 펫 슬롯 선택 시, 교체 UI를 보여줍니다.
    /// </summary>
    public void PetSlotClick()
    {
        int code = 0;
        selectPetIndex = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Mine_TeamSlot>().index;
        reconfirm.SetActive(true);
        PrintUI.instance.AudioPlay(0);
        switch (currentPetType)
        {
            case 0:
                code = SaveScript.saveData.hasMiners[selectPetIndex];
                currentPetSlot.exist_spriteImage.sprite = MineSlime.miner_defaultSprites[currentPetCode];
                selectPetSlot.exist_spriteImage.sprite = MineSlime.miner_defaultSprites[code];
                break;
            case 1:
                code = SaveScript.saveData.hasAdventurers[selectPetIndex];
                currentPetSlot.exist_spriteImage.sprite = MineSlime.adventurer_defaultSprites[currentPetCode];
                selectPetSlot.exist_spriteImage.sprite = MineSlime.adventurer_defaultSprites[code];
                break;
        }
        currentPetSlot.exist_nameText.text = "[" + MineSlime.qualityNames[currentPetCode] + "] Lv.1";
        selectPetSlot.exist_nameText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + SaveScript.saveData.hasAdventurerLevels[selectPetIndex];
    }

    /// <summary>
    /// 펫 교환을 수락합니다.
    /// </summary>
    public void PetYesChange()
    {
        switch (currentPetType)
        {
            case 0:
                SaveScript.saveData.hasMiners[selectPetIndex] = currentPetCode;
                SaveScript.saveData.hasMinerLevels[selectPetIndex] = 1;
                SaveScript.saveData.hasMinerExps[selectPetIndex] = 0;
                MinerSlime.SortPetInven();
                break;
            case 1:
                SaveScript.saveData.hasAdventurers[selectPetIndex] = currentPetCode;
                SaveScript.saveData.hasAdventurerLevels[selectPetIndex] = 1;
                SaveScript.saveData.hasAdventurerExps[selectPetIndex] = 0;
                AdventurerSlime.SortPetInven();
                break;
        }
        if (D_0_NPCUICtrl.instance.npc == null)
        {
            DropInfoUI.instance.SetPetInfo(currentPetType, currentPetCode);
            PrintUI.instance.AudioPlay(0);
        }
        else
            D_0_NPCUICtrl.instance.Buy();

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 32 });
        switch (currentPetCode)
        {
            case 7: QuestCtrl.instance.SetMainQuestAmount(new int[] { 106 }); break;
            case 8: QuestCtrl.instance.SetMainQuestAmount(new int[] { 107 }); break;
            case 9: QuestCtrl.instance.SetMainQuestAmount(new int[] { 108 }); break;
        }

        ClosePetInven();
    }

    /// <summary>
    /// 펫 교환을 취소합니다.
    /// </summary>
    public void PetNoChange()
    {
        reconfirm.SetActive(false);
        selectPetIndex = -1;
        PrintUI.instance.AudioPlay(0);
    }

    /// <summary>
    /// 펫 교환 인벤토리를 닫습니다.
    /// </summary>
    public void ClosePetInven()
    {
        petInven.SetActive(false);
        currentPetType = currentPetCode = -1;
        invenPage = 0;
        Time.timeScale = 1f;
        PrintUI.instance.AudioPlay(0);
    }
}
