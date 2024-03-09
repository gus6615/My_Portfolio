using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MineUpgradeUI : MonoBehaviour
{
    public static MineUpgradeUI instance;

    public GameObject upgradeMinerOb, upgradeAdventurerOb;
    public UIBox[] upgradeMenus;
    public UIBox[] upgradeMinerSlots, upgradeAdventurerSlots;
    public Text upgradeExpText;

    private int menuIndex;

    // 임시 데이터
    UIBox uibox;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    static public void Check_PetUpgrade()
    {
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 125 }, Total_PetUpgrade());
    }

    static private int Total_PetUpgrade()
    {
        int[] upgrades =
        {
            SaveScript.saveData.minerUpgrades[0],
            SaveScript.saveData.minerUpgrades[1],
            SaveScript.saveData.adventurerUpgrades[0],
            SaveScript.saveData.adventurerUpgrades[1]
        }; 

        return Mathf.Min(upgrades);
    }

    // 공통 UI 변수 초기화
    public void SetDefaultVariable()
    {
        menuIndex = 0;
    }

    // UpgradeUI 초기화
    public void UpgradeUI_Menu()
    {
        SetDefaultVariable();
        if (!EventSystem.current.currentSelectedGameObject.name.Contains("Slot"))
        {
            uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
            if (uibox != null)
            {
                menuIndex = uibox.order;
                Mine.instance.SetAudio(0);
            }
        }

        UpgradeUI_SetSelectUpgrade();
    }

    public int GetEXPUpgradeValue(int type, int num)
    {
        int exp = 0;

        switch (type)
        {
            case 0: // 보상 획득 관련
            case 1:
                exp = 100 + 200 * num;
                break;
            case 2: // 상자 추가 획득
                exp = 100 + 300 * num;
                break;
            case 3: // 보관함 증설
                exp = ((num % 10) + 1) * (int)Mathf.Pow(10, (num / 10) + 4);
                break;
            case 4: // 팀 슬롯 증설
                exp = (((num + 3) % 6) + 1) * (int)Mathf.Pow(10, ((num + 3) / 6) + 2);
                break;
            case 5: // 페이지 증설
                exp = (int)Mathf.Pow(10, (num + 4));
                break;
        }

        return exp;
    }

    // UpgradeUI 메인 세팅
    public void UpgradeUI_SetSelectUpgrade()
    {
        for (int i = 0; i < upgradeMenus.Length; i++)
        {
            upgradeMenus[i].images[0].color = Color.white;
            upgradeMenus[i].texts[0].color = new Color(0.8f, 0.8f, 0.8f);
        }
        upgradeMenus[menuIndex].images[0].color = new Color(0.6f, 0.6f, 0.6f);
        upgradeMenus[menuIndex].texts[0].color = new Color(0.5f, 0.5f, 0.5f);

        long exp = 0;
        upgradeExpText.text = GameFuction.GetNumText(SaveScript.saveData.exp);

        switch (menuIndex)
        {
            case 0:
                upgradeMinerOb.SetActive(true);
                upgradeAdventurerOb.SetActive(false);
                // 첫번째 슬롯
                exp = GetEXPUpgradeValue(0, SaveScript.saveData.minerUpgrades[0]);
                upgradeMinerSlots[0].texts[0].text = "[Lv." + (SaveScript.saveData.minerUpgrades[0] + 1) + "] 시간 단축";
                upgradeMinerSlots[0].texts[1].text = "모든 광부 슬라임의 채광 시간을 " + SaveScript.mineUpgradePercents[0] / 60f + "분 단축, 이때 1시간 미만으로 단축할 순 없음.\n(현재 "
                    + GameFuction.GetTimeText((int)(SaveScript.saveData.minerUpgrades[0] * SaveScript.mineUpgradePercents[0])) + " 단축)";
                upgradeMinerSlots[0].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeMinerSlots[0].texts[2].color = Color.green;
                else upgradeMinerSlots[0].texts[2].color = Color.red;
                // 두번째 슬롯
                exp = GetEXPUpgradeValue(1, SaveScript.saveData.minerUpgrades[1]);
                upgradeMinerSlots[1].texts[0].text = "[Lv." + (SaveScript.saveData.minerUpgrades[1] + 1) + "] 상자 획득량 증가";
                upgradeMinerSlots[1].texts[1].text = "모든 광부 슬라임의 상자 획득량이 " + SaveScript.mineUpgradePercents[1] * 100f + "% 증가\n(현재 획득량 "
                    + SaveScript.saveData.minerUpgrades[1] * SaveScript.mineUpgradePercents[1] * 100f + "%)";
                upgradeMinerSlots[1].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeMinerSlots[1].texts[2].color = Color.green;
                else upgradeMinerSlots[1].texts[2].color = Color.red;
                // 세번째 슬롯
                exp = GetEXPUpgradeValue(2, SaveScript.saveData.minerUpgrades[2]);
                upgradeMinerSlots[2].texts[0].text = "[Lv." + (SaveScript.saveData.minerUpgrades[2] + 1) + "] 추가 상자 획득";
                upgradeMinerSlots[2].texts[1].text = "모든 광부 슬라임의 추가 상자 휙득 확률 " + SaveScript.mineUpgradePercents[2] * 100f + "% 증가\n(현재 추가 상자 획득 확률 "
                    + SaveScript.saveData.minerUpgrades[2] * SaveScript.mineUpgradePercents[2] * 100f + "%)";
                upgradeMinerSlots[2].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeMinerSlots[2].texts[2].color = Color.green;
                else upgradeMinerSlots[2].texts[2].color = Color.red;
                // 네번째 슬롯
                exp = GetEXPUpgradeValue(3, SaveScript.saveData.minerUpgrades[3]);
                upgradeMinerSlots[3].texts[0].text = "[Lv." + (SaveScript.saveData.minerUpgrades[3] + 1) + "] 보관함 슬롯 추가";
                upgradeMinerSlots[3].texts[1].text = "광부 슬라임을 보관할 수 있는 인벤토리 슬롯 " + SaveScript.mineUpgradePercents[3] + "칸 증가\n(현재 "
                    + SaveScript.saveData.minerUpgrades[3] * SaveScript.mineUpgradePercents[3] + "칸 증가)";
                upgradeMinerSlots[3].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeMinerSlots[3].texts[2].color = Color.green;
                else upgradeMinerSlots[3].texts[2].color = Color.red;
                // 다섯번째 슬롯
                exp = GetEXPUpgradeValue(4, SaveScript.saveData.minerUpgrades[4]);
                upgradeMinerSlots[4].button = upgradeMinerSlots[4].GetComponent<Button>();
                upgradeMinerSlots[4].texts[0].text = "[Lv." + (SaveScript.saveData.minerUpgrades[4] + 1) + "] 팀 슬롯 추가";
                upgradeMinerSlots[4].texts[1].text = "현재 채광 중인 광부 슬라임 팀 슬롯 " + SaveScript.mineUpgradePercents[4] + "칸 증가\n";
                upgradeMinerSlots[4].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.mineSlotMinNum + SaveScript.saveData.minerUpgrades[4] < (SaveScript.saveData.minerUpgrades[5] + 1) * 6)
                {
                    upgradeMinerSlots[4].images[0].color = upgradeMinerSlots[4].images[1].color = upgradeMinerSlots[4].images[2].color = new Color(1f, 1f, 1f, 1f);
                    upgradeMinerSlots[4].texts[0].color = upgradeMinerSlots[4].texts[1].color = upgradeMinerSlots[4].texts[2].color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    upgradeMinerSlots[4].button.interactable = true;
                    upgradeMinerSlots[4].texts[1].text += "(현재 " + SaveScript.saveData.minerUpgrades[4] * SaveScript.mineUpgradePercents[4] + "칸 증가)";
                    if (SaveScript.saveData.exp >= exp) upgradeMinerSlots[4].texts[2].color = Color.green;
                    else upgradeMinerSlots[4].texts[2].color = Color.red;
                }
                else
                {
                    upgradeMinerSlots[4].images[0].color = upgradeMinerSlots[4].images[1].color = upgradeMinerSlots[4].images[2].color = new Color(0.6f, 0.6f, 0.6f, 0.4f);
                    upgradeMinerSlots[4].texts[0].color = upgradeMinerSlots[4].texts[1].color = upgradeMinerSlots[4].texts[2].color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                    upgradeMinerSlots[4].button.interactable = false;
                    upgradeMinerSlots[4].texts[1].text += "(현재 팀 페이지가 부족합니다!)";
                }
                // 여섯번째 슬롯
                exp = GetEXPUpgradeValue(5, SaveScript.saveData.minerUpgrades[5]);
                upgradeMinerSlots[5].texts[0].text = "[Lv." + (SaveScript.saveData.minerUpgrades[5] + 1) + "] 팀 페이지 추가";
                upgradeMinerSlots[5].texts[1].text = "현재 채광중인 광부 슬라임 팀 페이지 " + SaveScript.mineUpgradePercents[5] + "페이지 증가\n(현재 "
                    + SaveScript.saveData.minerUpgrades[5] * SaveScript.mineUpgradePercents[5] + "페이지 증가)";
                upgradeMinerSlots[5].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeMinerSlots[5].texts[2].color = Color.green;
                else upgradeMinerSlots[5].texts[2].color = Color.red;
                break;
            case 1:
                upgradeMinerOb.SetActive(false);
                upgradeAdventurerOb.SetActive(true);
                // 첫번째 슬롯
                exp = GetEXPUpgradeValue(0, SaveScript.saveData.adventurerUpgrades[0]);
                upgradeAdventurerSlots[0].texts[0].text = "[Lv." + (SaveScript.saveData.adventurerUpgrades[0] + 1) + "] 시간 단축";
                upgradeAdventurerSlots[0].texts[1].text = "모든 모험가 슬라임의 탐험 시간을 " + SaveScript.mineUpgradePercents[0] / 60f * 2 + "분 단축, 이때 2시간 미만으로 단축할 순 없음.\n(현재 "
                    + GameFuction.GetTimeText((int)(SaveScript.saveData.adventurerUpgrades[0] * SaveScript.mineUpgradePercents[0] * 2)) + " 단축)";
                upgradeAdventurerSlots[0].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeAdventurerSlots[0].texts[2].color = Color.green;
                else upgradeAdventurerSlots[0].texts[2].color = Color.red;
                // 두번째 슬롯
                exp = GetEXPUpgradeValue(1, SaveScript.saveData.adventurerUpgrades[1]);
                upgradeAdventurerSlots[1].texts[0].text = "[Lv." + (SaveScript.saveData.adventurerUpgrades[1] + 1) + "] 상자 획득량 증가";
                upgradeAdventurerSlots[1].texts[1].text = "모든 모험가 슬라임의 상자 획득량이 " + SaveScript.mineUpgradePercents[1] * 100f + "% 증가\n(현재 획득량 "
                    + SaveScript.saveData.adventurerUpgrades[1] * SaveScript.mineUpgradePercents[1] * 100f + "%)";
                upgradeAdventurerSlots[1].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeAdventurerSlots[1].texts[2].color = Color.green;
                else upgradeAdventurerSlots[1].texts[2].color = Color.red;
                // 세번째 슬롯
                exp = GetEXPUpgradeValue(2, SaveScript.saveData.adventurerUpgrades[2]);
                upgradeAdventurerSlots[2].texts[0].text = "[Lv." + (SaveScript.saveData.adventurerUpgrades[2] + 1) + "] 추가 상자 획득";
                upgradeAdventurerSlots[2].texts[1].text = "모든 모험가 슬라임의 추가 상자 휙득 확률 " + SaveScript.mineUpgradePercents[2] * 100f + "% 증가\n(현재 추가 상자 획득 확률 "
                    + SaveScript.saveData.adventurerUpgrades[2] * SaveScript.mineUpgradePercents[2] * 100f + "%)";
                upgradeAdventurerSlots[2].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeAdventurerSlots[2].texts[2].color = Color.green;
                else upgradeAdventurerSlots[2].texts[2].color = Color.red;
                // 네번째 슬롯
                exp = GetEXPUpgradeValue(3, SaveScript.saveData.adventurerUpgrades[3]);
                upgradeAdventurerSlots[3].texts[0].text = "[Lv." + (SaveScript.saveData.adventurerUpgrades[3] + 1) + "] 보관함 슬롯 추가";
                upgradeAdventurerSlots[3].texts[1].text = "모험가 슬라임을 보관할 수 있는 인벤토리 슬롯 " + SaveScript.mineUpgradePercents[3] + "칸 증가\n(현재 "
                    + SaveScript.saveData.adventurerUpgrades[3] * SaveScript.mineUpgradePercents[3] + "칸 증가)";
                upgradeAdventurerSlots[3].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeAdventurerSlots[3].texts[2].color = Color.green;
                else upgradeAdventurerSlots[3].texts[2].color = Color.red;
                // 다섯번째 슬롯
                exp = GetEXPUpgradeValue(4, SaveScript.saveData.adventurerUpgrades[4]);
                upgradeAdventurerSlots[4].texts[0].text = "[Lv." + (SaveScript.saveData.adventurerUpgrades[4] + 1) + "] 팀 슬롯 추가";
                upgradeAdventurerSlots[4].texts[1].text = "현재 탐험 중인 모험가 슬라임 팀 슬롯 " + SaveScript.mineUpgradePercents[4] + "칸 증가\n";
                upgradeAdventurerSlots[4].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.mineSlotMinNum + SaveScript.saveData.adventurerUpgrades[4] < (SaveScript.saveData.adventurerUpgrades[5] + 1) * 6)
                {
                    upgradeAdventurerSlots[4].images[0].color = upgradeAdventurerSlots[4].images[1].color = upgradeAdventurerSlots[4].images[2].color = new Color(1f, 1f, 1f, 1f);
                    upgradeAdventurerSlots[4].texts[0].color = upgradeAdventurerSlots[4].texts[1].color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    upgradeAdventurerSlots[4].button.interactable = true;
                    upgradeAdventurerSlots[4].texts[1].text += "(현재 " + SaveScript.saveData.adventurerUpgrades[4] * SaveScript.mineUpgradePercents[4] + "칸 증가)";
                    if (SaveScript.saveData.exp >= exp) upgradeAdventurerSlots[4].texts[2].color = Color.green;
                    else upgradeAdventurerSlots[4].texts[2].color = Color.red;
                }
                else
                {
                    upgradeAdventurerSlots[4].images[0].color = upgradeAdventurerSlots[4].images[1].color = upgradeAdventurerSlots[4].images[2].color = new Color(0.6f, 0.6f, 0.6f, 0.4f);
                    upgradeAdventurerSlots[4].texts[0].color = upgradeAdventurerSlots[4].texts[1].color = upgradeAdventurerSlots[4].texts[2].color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                    upgradeAdventurerSlots[4].button.interactable = false;
                    upgradeAdventurerSlots[4].texts[1].text += "(현재 팀 페이지가 부족합니다!)";
                }
                // 여섯번째 슬롯
                exp = GetEXPUpgradeValue(5, SaveScript.saveData.adventurerUpgrades[5]);
                upgradeAdventurerSlots[5].texts[0].text = "[Lv." + (SaveScript.saveData.adventurerUpgrades[5] + 1) + "] 팀 페이지 추가";
                upgradeAdventurerSlots[5].texts[1].text = "현재 탐험 중인 모험가 슬라임 팀 페이지 " + SaveScript.mineUpgradePercents[5] + "페이지 증가\n(현재 "
                    + SaveScript.saveData.adventurerUpgrades[5] * SaveScript.mineUpgradePercents[5] + "페이지 증가)";
                upgradeAdventurerSlots[5].texts[2].text = GameFuction.GetNumText(exp);
                if (SaveScript.saveData.exp >= exp) upgradeAdventurerSlots[5].texts[2].color = Color.green;
                else upgradeAdventurerSlots[5].texts[2].color = Color.red;
                break;
        }
        UpgradeUI_SetSelectSlots();
    }

    // UpgradeUI 메인 슬롯 체크
    public void UpgradeUI_SetSelectSlots()
    {
        switch (menuIndex)
        {
            case 0:
                if (SaveScript.saveData.minerUpgrades[2] * SaveScript.mineUpgradePercents[2] >= 1f) upgradeMinerSlots[2].gameObject.SetActive(false);
                if (SaveScript.saveData.minerUpgrades[3] >= SaveScript.mineInvenMaxNum - SaveScript.mineInvenMinNum) upgradeMinerSlots[3].gameObject.SetActive(false);
                if (SaveScript.saveData.minerUpgrades[4] >= SaveScript.mineSlotMaxNum - SaveScript.mineSlotMinNum) upgradeMinerSlots[4].gameObject.SetActive(false);
                if (SaveScript.saveData.minerUpgrades[5] >= 4) upgradeMinerSlots[5].gameObject.SetActive(false);
                break;
            case 1:
                if (SaveScript.saveData.adventurerUpgrades[2] * SaveScript.mineUpgradePercents[2] >= 1f) upgradeAdventurerSlots[2].gameObject.SetActive(false);
                if (SaveScript.saveData.adventurerUpgrades[3] >= SaveScript.mineInvenMaxNum - SaveScript.mineInvenMinNum) upgradeAdventurerSlots[3].gameObject.SetActive(false);
                if (SaveScript.saveData.adventurerUpgrades[4] >= SaveScript.mineSlotMaxNum - SaveScript.mineSlotMinNum) upgradeAdventurerSlots[4].gameObject.SetActive(false);
                if (SaveScript.saveData.adventurerUpgrades[5] >= 4) upgradeAdventurerSlots[5].gameObject.SetActive(false);
                break;
        }
    }

    // UpgradeUI 슬롯 구매 버튼
    public void UpgradeUI_BuyButton()
    {
        int index = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
        Mine.instance.audio.clip = SaveScript.SEs[2];

        switch (menuIndex)
        {
            case 0:
                if (SaveScript.saveData.exp >= GetEXPUpgradeValue(index, SaveScript.saveData.minerUpgrades[index]))
                {
                    Mine.instance.audio.clip = SaveScript.SEs[4];
                    SaveScript.saveData.exp -= GetEXPUpgradeValue(index, SaveScript.saveData.minerUpgrades[index]);
                    SaveScript.saveData.minerUpgrades[index]++;
                    AchievementCtrl.instance.SetAchievementAmount(38, 1);
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { 36 });
                    Check_PetUpgrade();
                    switch (index)
                    {
                        case 0: SystemInfoCtrl.instance.SetShowInfo("시간 단축 <color=#FF9696>[ Lv." + SaveScript.saveData.minerUpgrades[index] + " ] <color=white>강화 완료!"); break;
                        case 1: SystemInfoCtrl.instance.SetShowInfo("상자 획득량 <color=#FF9696>[ Lv." + SaveScript.saveData.minerUpgrades[index] + " ] <color=white>강화 완료!"); break;
                        case 2: SystemInfoCtrl.instance.SetShowInfo("추가 상자 획득 확률 <color=#FF9696>[ Lv." + SaveScript.saveData.minerUpgrades[index] + " ] <color=white>강화 완료"); break;
                        case 3: SystemInfoCtrl.instance.SetShowInfo("보관함 슬롯 <color=#FF9696>[ Lv." + SaveScript.saveData.minerUpgrades[index] + " ] <color=white>확장 완료"); break;
                        case 4: SystemInfoCtrl.instance.SetShowInfo("팀 슬롯 <color=#FF9696>[ Lv." + SaveScript.saveData.minerUpgrades[index] + " ] <color=white>확장 완료!"); break;
                        case 5: SystemInfoCtrl.instance.SetShowInfo("팀 페이지 <color=#FF9696>[ Lv." + SaveScript.saveData.minerUpgrades[index] + " ] <color=white>확장 완료!"); break;
                    }
                }
                else
                    SystemInfoCtrl.instance.SetErrorInfo("경험치(EXP)가 부족합니다.");
                break;
            case 1:
                if (SaveScript.saveData.exp >= GetEXPUpgradeValue(index, SaveScript.saveData.adventurerUpgrades[index]))
                {
                    Mine.instance.audio.clip = SaveScript.SEs[4];
                    SaveScript.saveData.exp -= GetEXPUpgradeValue(index, SaveScript.saveData.adventurerUpgrades[index]);
                    SaveScript.saveData.adventurerUpgrades[index]++;
                    AchievementCtrl.instance.SetAchievementAmount(38, 1);
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { 36 });
                    Check_PetUpgrade();
                    switch (index)
                    {
                        case 0: SystemInfoCtrl.instance.SetShowInfo("시간 단축 <color=#FF9696>[ Lv." + SaveScript.saveData.adventurerUpgrades[index] + " ] <color=white>강화 완료!"); break;
                        case 1: SystemInfoCtrl.instance.SetShowInfo("상자 획득량 <color=#FF9696>[ Lv." + SaveScript.saveData.adventurerUpgrades[index] + " ] <color=white>강화 완료!"); break;
                        case 2: SystemInfoCtrl.instance.SetShowInfo("추가 상자 획득 확률 <color=#FF9696>[ Lv." + SaveScript.saveData.adventurerUpgrades[index] + " ] <color=white>강화 완료"); break;
                        case 3: SystemInfoCtrl.instance.SetShowInfo("보관함 슬롯 <color=#FF9696>[ Lv." + SaveScript.saveData.adventurerUpgrades[index] + " ] <color=white>확장 완료"); break;
                        case 4: SystemInfoCtrl.instance.SetShowInfo("팀 슬롯 <color=#FF9696>[ Lv." + SaveScript.saveData.adventurerUpgrades[index] + " ] <color=white>확장 완료"); break;
                        case 5: SystemInfoCtrl.instance.SetShowInfo("팀 페이지 <color=#FF9696>[ Lv." + SaveScript.saveData.adventurerUpgrades[index] + " ] <color=white>확장 완료"); break;
                    }
                }
                else
                    SystemInfoCtrl.instance.SetErrorInfo("경험치(EXP)가 부족합니다.");
                break;
        }

        Mine.instance.audio.Play();
        UpgradeUI_SetSelectUpgrade();
    }
}
