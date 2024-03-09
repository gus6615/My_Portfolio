using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExpUpgradeUI : MonoBehaviour
{
    public static ExpUpgradeUI instance;

    public GameObject[] upgradeMenus;
    public FadeEffect[] expButtons;

    private Sprite[] tier_sprites;
    private Color[] colorsAsTier = { new Color(0.6f, 0.3f, 0.1f), new Color(0.8f, 0.8f, 0.8f), new Color(1f, 1f, 0.4f), new Color(0.5f, 1f, 0.8f)
            , new Color(0.4f, 1f, 1f), new Color(1f, 0.5f, 0.5f), new Color(0.8f, 1f, 1f) };
    private int upgradeMenuIndex, upgradeListIndex;
    public int currnetPage;

    // 임시 변수들
    Text[] texts1, texts2;
    Image[] images1, images2;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tier_sprites = Resources.LoadAll<Sprite>("Images/Inventory/Tiers");
    }

    public void SetExpUpgradeInfo()
    {
        Inventory.instance.expText.text = GameFuction.GetNumText(SaveScript.saveData.exp);
        SpriteRenderer[] contents = upgradeMenus[upgradeMenuIndex].GetComponentsInChildren<SpriteRenderer>();
        images1 = contents[0].GetComponentsInChildren<Image>();
        images2 = contents[1].GetComponentsInChildren<Image>();
        texts1 = contents[0].GetComponentsInChildren<Text>();
        texts2 = contents[1].GetComponentsInChildren<Text>();
        int type1 = 0;
        int type2 = 0;
        long expPrice1;
        long expPrice2;

        switch (upgradeMenuIndex)
        {
            case 0: type1 = SaveScript.saveData.pick1Upgrades; type2 = SaveScript.saveData.pick2Upgrades; break;
            case 1: type1 = SaveScript.saveData.hat1Upgrades; type2 = SaveScript.saveData.hat2Upgrades; break;
            case 2: type1 = SaveScript.saveData.ring1Upgrades; type2 = SaveScript.saveData.ring2Upgrades; break;
            case 3: type1 = SaveScript.saveData.Pendant1Upgrades; type2 = SaveScript.saveData.Pendant2Upgrades; break;
            case 4: type1 = SaveScript.saveData.sword1Upgrades; type2 = SaveScript.saveData.sword2Upgrades; break;
        }

        // 티어 설정
        int tier1 = GameFuction.GetTier_EXP(type1);
        int tier2 = GameFuction.GetTier_EXP(type2);
        images1[0].color = images1[3].color = colorsAsTier[tier1 / 4];
        images2[0].color = images2[3].color = colorsAsTier[tier2 / 4];
        images1[1].sprite = tier_sprites[tier1];
        images2[1].sprite = tier_sprites[tier2];

        // UI 설정
        expPrice1 = GameFuction.GetEXPUpgradeValue(type1);
        expPrice2 = GameFuction.GetEXPUpgradeValue(type2);
        texts1[3].text = GameFuction.GetNumText(expPrice1);
        texts2[3].text = GameFuction.GetNumText(expPrice2);
        texts1[0].text = "[ Lv." + type1 + " ]";
        texts2[0].text = "[ Lv." + type2 + " ]";

        if (SaveScript.saveData.exp >= expPrice1) texts1[3].color = Color.green;
        else texts1[3].color = Color.red;
        if (SaveScript.saveData.exp >= expPrice2) texts2[3].color = Color.green;
        else texts2[3].color = Color.red;

        string contentText01 = "현재 장비를 착용하고 있지 않습니다!";
        string contentText02 = "현재 장비를 착용하고 있지 않습니다!";

        switch (upgradeMenuIndex)
        {
            case 0:
                if (SaveScript.saveData.equipPick >= 0)
                {
                    contentText01 = "곡괭이의 내구도 < " + Mathf.Round(SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * SaveScript.expReinforcePercents[0][tier1]) + " > 증가";
                    contentText02 = "곡괭이의 속도 < " + SaveScript.expReinforcePercents[1][tier2] * 100 + "% > 증가";
                }
                break;
            case 1:
                if (SaveScript.saveData.equipHat >= 0)
                {
                    contentText01 = "모자 방어력 < " + Mathf.Round(SaveScript.hats[SaveScript.saveData.equipHat].reinforce_basic * SaveScript.expReinforcePercents[2][tier1]) + " > 증가";
                    if (SaveScript.stat.hat02 < 1f) contentText02 = "특수 회피 확률 < " + SaveScript.expReinforcePercents[3][tier2] * 100 + "% > 증가";
                    else contentText02 = "특수 회피력 < " + GameFuction.GetReinforcePercent_Over2(1, SaveScript.expReinforcePercents[3][tier2]) * 100 + "% > 증가";
                }
                break;
            case 2:
                if (SaveScript.saveData.equipRing >= 0)
                {
                    contentText01 = "상점 판매량 < " + Mathf.Round(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * SaveScript.expReinforcePercents[4][tier1] * 100) * 100 / 100 + "% > 증가";
                    if (SaveScript.stat.ring02 < 1f) contentText02 = "특수 판매 확률 < " + SaveScript.expReinforcePercents[5][tier2] * 100 + "% > 증가";
                    else contentText02 = "특수 판매력 < " + GameFuction.GetReinforcePercent_Over2(2, SaveScript.expReinforcePercents[5][tier2]) * 100 + "% > 증가";
                }
                break;
            case 3:
                if (SaveScript.saveData.equipPendant >= 0)
                {
                    contentText01 = "최대 획득 가능 광석 < " + Mathf.Round(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * SaveScript.expReinforcePercents[6][tier1]) + "개 > 추가";
                    if (SaveScript.stat.pendant02 < 1f) contentText02 = "광물 추가 획득 확률 < " + SaveScript.expReinforcePercents[7][tier2] * 100 + "% > 증가";
                    else contentText02 = "최소 획득 가능 광석 < " + Mathf.Round(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * GameFuction.GetReinforcePercent_Over2(3, SaveScript.expReinforcePercents[7][tier2])) + "개 > 추가";
                }
                break;
            case 4:
                if (SaveScript.saveData.equipSword >= 0)
                {
                    contentText01 = "검 공격력 < " + Mathf.Round(SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * SaveScript.expReinforcePercents[8][tier1]) + " > 증가";
                    if (SaveScript.stat.sword02 < 1f) contentText02 = "크리티컬 확률 < " + SaveScript.expReinforcePercents[9][tier2] * 100 + "% > 증가";
                    else contentText02 = "크리티컬 데미지 < " + GameFuction.GetReinforcePercent_Over2(4, SaveScript.expReinforcePercents[9][tier2]) * 100 + "% > 증가";
                }
                break;
        }

        texts1[2].text = contentText01;
        texts2[2].text = contentText02;
    }

    public bool CheckUpTier(int level)
    {
        if (level < 100) // 브론즈 ~ 다이아  
        {
            if (level % 5 == 0) return true;
            else return false;
        }
        else if (level < 500) // 마스터
        {
            if (level % 100 == 0) return true;
            else return false;
        }
        else if (level == 500) // 첼린저
            return true;
        else
            return false;
    }

    // 경험치 업그레이드 도구 선택 버튼
    public void UpgradeSelectButton()
    {
        Inventory.instance.SetAudio(0);
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            upgradeMenuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

        Inventory.instance.animator.SetInteger("PageType", 2);
        for (int i = 0; i < upgradeMenus.Length; i++)
            upgradeMenus[i].SetActive(false);
        upgradeMenus[upgradeMenuIndex].SetActive(true);
        SetExpUpgradeInfo();
    }

    // 경험치 업그레이드 사기 버튼
    public void UpgradeBuyButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            upgradeListIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

        int upgrade01, upgrade02;
        string text01, text02;
        bool isCanUpgrade = false;

        switch (upgradeMenuIndex)
        {
            case 0:
                upgrade01 = SaveScript.saveData.pick1Upgrades; upgrade02 = SaveScript.saveData.pick2Upgrades;
                text01 = "[곡괭이] 내구도 업그레이드 <color=#FF9696>[ Lv." + (upgrade01 + 1) + " ] <color=white>강화 완료!"; 
                text02 = "[곡괭이] 속도 업그레이드 <color=#FF9696>[ Lv." + (upgrade02 + 1) + " ] <color=white>강화 완료!";
                break;
            case 1:
                upgrade01 = SaveScript.saveData.hat1Upgrades; upgrade02 = SaveScript.saveData.hat2Upgrades;
                text01 = "[모자] 방어력 업그레이드 <color=#FF9696>[ Lv." + (upgrade01 + 1) + " ] <color=white>강화 완료!"; 
                text02 = "[모자] 특수 효과 발동 확률 업그레이드 <color=#FF9696>[ Lv." + (upgrade02 + 1) + " ] <color=white>강화 완료!";
                break;
            case 2:
                upgrade01 = SaveScript.saveData.ring1Upgrades; upgrade02 = SaveScript.saveData.ring2Upgrades;
                text01 = "[반지] 판매량 업그레이드 <color=#FF9696>[ Lv." + (upgrade01 + 1) + " ] <color=white>강화 완료!"; 
                text02 = "[반지] 특수 효과 발동 확률 업그레이드 <color=#FF9696>[ Lv." + (upgrade02 + 1) + " ] <color=white>강화 완료!";
                break;
            case 3:
                upgrade01 = SaveScript.saveData.Pendant1Upgrades; upgrade02 = SaveScript.saveData.Pendant2Upgrades;
                text01 = "[목걸이] 획득 량 업그레이드 <color=#FF9696>[ Lv." + (upgrade01 + 1) + " ] <color=white>강화 완료!"; 
                text02 = "[목걸이] 발동 확률 업그레이드 <color=#FF9696>[ Lv." + (upgrade02 + 1) + " ] <color=white>강화 완료!";
                break;
            default:
                upgrade01 = SaveScript.saveData.sword1Upgrades; upgrade02 = SaveScript.saveData.sword2Upgrades;
                text01 = "[검] 공격력 업그레이드 <color=#FF9696>[ Lv." + (upgrade01 + 1) + " ] <color=white>강화 완료!"; 
                text02 = "[검] 크리티컬 확률 업그레이드 <color=#FF9696>[ Lv." + (upgrade02 + 1) + " ] <color=white>강화 완료!";
                break;
        }

        if (upgradeListIndex == 0)
        {
            if (SaveScript.saveData.exp >= GameFuction.GetEXPUpgradeValue(upgrade01))
            {
                SaveScript.saveData.exp -= GameFuction.GetEXPUpgradeValue(upgrade01);
                AchievementCtrl.instance.SetAchievementAmount(27, 1);
                SystemInfoCtrl.instance.SetShowInfo(text01);
                isCanUpgrade = true;

                switch (upgradeMenuIndex)
                {
                    case 0: SaveScript.saveData.pick1Upgrades++; break;
                    case 1: SaveScript.saveData.hat1Upgrades++; break;
                    case 2: SaveScript.saveData.ring1Upgrades++; break;
                    case 3: SaveScript.saveData.Pendant1Upgrades++; break;
                    default: SaveScript.saveData.sword1Upgrades++; break;
                }
            }
        }
        else
        {
            if (SaveScript.saveData.exp >= GameFuction.GetEXPUpgradeValue(upgrade02))
            {
                SaveScript.saveData.exp -= GameFuction.GetEXPUpgradeValue(upgrade02);
                AchievementCtrl.instance.SetAchievementAmount(27, 1);
                SystemInfoCtrl.instance.SetShowInfo(text02);
                isCanUpgrade = true;

                switch (upgradeMenuIndex)
                {
                    case 0: SaveScript.saveData.pick2Upgrades++; break;
                    case 1: SaveScript.saveData.hat2Upgrades++; break;
                    case 2: SaveScript.saveData.ring2Upgrades++; break;
                    case 3: SaveScript.saveData.Pendant2Upgrades++; break;
                    default: SaveScript.saveData.sword2Upgrades++; break;
                }
            }
        }

        if (isCanUpgrade)
        {
            Inventory.instance.SetAudio(4);
            if ((upgradeListIndex == 0 && CheckUpTier(upgrade01 + 1)) || (upgradeListIndex == 1 && CheckUpTier(upgrade02 + 1)))
                Inventory.instance.SetAudio(34);

            // 퀘스트
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 7 }); // 목걸이 강화 1회
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 14, 26 }); // 경험치 강화 +1
            CheckExpQuest(); // 모든 장비 경험치 강화
            SetTutorial();
        }
        else
        {
            Inventory.instance.SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("경험치(EXP)가 부족합니다.");
        }
        SetExpUpgradeInfo();
    }

    // 페이지 넘기기 버튼(exp강화)
    public void PageButton()
    {
        Inventory.instance.SetAudio(0);
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            currnetPage = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        Page();
    }

    public void Page()
    {
        Inventory.instance.expObject.SetActive(true);
        Inventory.instance.invenButton.gameObject.SetActive(false);
        Inventory.instance.fusionButton.gameObject.SetActive(false);
        ReinforceItemUse.instance.invenOnOffButton.gameObject.SetActive(false);
        switch (currnetPage)
        {
            case 0: // 업그레이드 버튼
                Inventory.instance.animator.SetInteger("PageType", 1);
                break;
            case 1: // 인벤토리로 돌아가기 버튼
                Inventory.instance.animator.SetInteger("PageType", 0);
                Inventory.instance.expObject.SetActive(false);
                Inventory.instance.SetBasicInfo();
                Inventory.instance.SlotClear();
                break;
            case 2: // 업그레이드 도구 선택창으로 돌아가기
                Inventory.instance.animator.SetInteger("PageType", 1);
                break;
        }
    }

    public void SetTutorial()
    {
        for (int i = 0; i < expButtons.Length; i++)
            expButtons[i].enabled = expButtons[i].isReSize = QuestCtrl.CheckFadeUI(new int[] { 7 }, SaveScript.saveData.mainQuest_list);
    }

    static public void CheckExpQuest()
    {
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 61, 69, 79, 89, 99, 109, 119, 129, 139 }, TotalExpUpgrades()); // 모든 장비 강화 수 체크
    }

    static private int TotalExpUpgrades()
    {
        int[] upgrades = { 
            SaveScript.saveData.pick1Upgrades, SaveScript.saveData.pick2Upgrades, SaveScript.saveData.hat1Upgrades, SaveScript.saveData.hat2Upgrades,
            SaveScript.saveData.ring1Upgrades, SaveScript.saveData.ring2Upgrades, SaveScript.saveData.Pendant1Upgrades, SaveScript.saveData.Pendant2Upgrades,
            SaveScript.saveData.sword1Upgrades, SaveScript.saveData.sword2Upgrades
        };

        return Mathf.Min(upgrades);
    }
}
