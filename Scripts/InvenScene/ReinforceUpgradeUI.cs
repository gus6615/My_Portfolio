using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReinforceUpgradeUI : MonoBehaviour
{
    public static ReinforceUpgradeUI instance;

    private int slotIndex;
    private int R_page;
    private int R_upgradeMenuIndex, R_upgradeListIndex;
    private int R_upgradeSelectPage;
    public Image reinforceItemAniImage01, reinforceItemAniImage02;
    public GameObject passClickPanel;

    public GameObject R_upgradeSelectBox;
    public GameObject R_upgradeSelectPageObject;
    private Image[] R_upgradeSelectPageImages;
    private Image[] R_select_DestroyedImages, R_select_QualityImages, R_selectImages;
    private Button[] R_selectButtons;
    public Image R_ReinforceBox;
    public GameObject[] R_ReinforceTiers;
    private Image[][] R_ReinforceStars;
    private Sprite[] R_ReinforceSprites, R_ReinforceStarSprites;
    public Image R_StartButton;
    public GameObject R_StarBox;
    public GameObject R_ToolBox;
    private Image R_ToolImage, R_ToolQualityImage, R_ToolDestroyedImage;
    public GameObject R_percentInfo;
    private Text[] R_percentInfoTexts; // 0 = 강화 성, 1 = 성공 확률, 2 = 실패 확률, 3 = 파괴 확률
    public GameObject R_plusStatInfo;
    private Text[] R_plusStatInfoTexts; // 1 = 첫번째 효과, 2 = 두번째 효과
    public GameObject R_needItemInfo;
    private Text[] R_itemTexts; // 1 = 강화석 개수, 2 = 마나석 개수
    public GameObject R_animation;
    public Image R_ani_ToolImage;
    public GameObject R_result;
    private Image R_result_toolImage, R_result_toolQualityImage, R_result_toolDestroyImage;
    private Text R_result_infotext, R_result_nametext;
    public Text R_startButtonText;
    public FadeEffect[] rButtons, rButtons_2;
    public GameObject R_ani_itemPrefab;
    public Transform R_ani_itemPanel;

    private float successPercent, successPercentPlused; // 강화 총 성공 확률, 강화 성공 Plus된 확률 
    private float destroyPercent; // 강화 파괴 확률
    private float failPercent; // 강화 실패 확률
    private bool isNotDowngrade; // 등급 하락 방지가 적용되었는가?
    private bool isNotDestroy; // 파괴 방지가 적용되었는가?
    private int itemPlusStar; // 1성 2성 강화 주문서가 적용되었는가?, 1 = 1성 적용, 2 = 2성 적용, 0 = 적용 X
    private bool isCanReinforce, isNeedRefair;
    private bool[] rButton_isOn = new bool[5];

    public Sprite R_sprite;
    private int R_quality, R_star, R_tier_0, R_tier_1;
    public int reinforceOrePrice = 0; // 아이템 강화에 필요한 강화석 수
    public int manaOrePrice = 0; // 아이템 강화에 필요한 마나석 수
    private Color[] stageColors = { new Color(0.7f, 0.5f, 0.3f), new Color(1f, 1f, 1f), new Color(0.6f, 0.6f, 0.9f), new Color(0.9f, 0.4f, 0.4f), new Color(0.4f, 0.9f, 1f) };

    // 임시 변수
    Order[] orders;
    UIBox[] uiboxes;
    Image[] images;
    UIBox uibox;
    Text[] texts;
    Button startButton;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        orders = R_upgradeSelectBox.GetComponentsInChildren<Order>();
        R_select_QualityImages = new Image[orders.Length];
        R_select_DestroyedImages = new Image[orders.Length];
        R_selectImages = new Image[orders.Length];
        R_selectButtons = new Button[orders.Length];
        for (int i = 0; i < orders.Length; i++)
        {
            images = orders[i].GetComponentsInChildren<Image>();
            R_select_QualityImages[i] = images[0];
            R_select_DestroyedImages[i] = images[1];
            R_selectImages[i] = images[2];
            R_selectButtons[i] = orders[i].GetComponent<Button>();
        }

        startButton = R_StartButton.GetComponent<Button>();
        R_ReinforceSprites = Resources.LoadAll<Sprite>("Images/Inventory/Reinforce_00");
        R_ReinforceStarSprites = Resources.LoadAll<Sprite>("Images/Inventory/Reinforce_02");
        R_percentInfoTexts = R_percentInfo.GetComponentsInChildren<Text>();
        R_plusStatInfoTexts = R_plusStatInfo.GetComponentsInChildren<Text>();
        R_itemTexts = R_needItemInfo.GetComponentsInChildren<Text>();
        R_upgradeSelectPageImages = R_upgradeSelectPageObject.GetComponentsInChildren<Image>();
        R_ReinforceStars = new Image[R_ReinforceTiers.Length][];
        for (int i = 0; i < R_ReinforceTiers.Length; i++)
            R_ReinforceStars[i] = R_ReinforceTiers[i].GetComponentsInChildren<Image>();

        images = R_ToolBox.GetComponentsInChildren<Image>();
        R_ToolQualityImage = images[1];
        R_ToolDestroyedImage = images[2];
        R_ToolImage = images[3];

        images = R_result.GetComponentsInChildren<Image>();
        texts = R_result.GetComponentsInChildren<Text>();
        R_result_toolQualityImage = images[1];
        R_result_toolDestroyImage = images[2];
        R_result_toolImage = images[3];
        R_result_infotext = texts[0];
        R_result_nametext = texts[1];

        R_upgradeSelectBox.SetActive(false);
        R_animation.SetActive(false);
        R_result.SetActive(false);
        R_upgradeSelectPageObject.SetActive(false);
    }

    // 강화석 강화 선택 UI 세팅
    public void R_UpgradeSelectSetting()
    {
        R_upgradeSelectPageImages[0].gameObject.SetActive(true);
        R_upgradeSelectPageImages[1].gameObject.SetActive(true);
        if (R_upgradeSelectPage == 0)
            R_upgradeSelectPageImages[0].gameObject.SetActive(false);

        int equipNum = 0;

        switch (slotIndex)
        {
            case 0: equipNum = SaveScript.pickNum; break;
            case 1: equipNum = SaveScript.hatNum; break;
            case 2: equipNum = SaveScript.RingNum; break;
            case 3: equipNum = SaveScript.PendantNum; break;
            case 4: equipNum = SaveScript.swordNum; break;
        }

        if (R_upgradeSelectPage == equipNum / 5 || (R_upgradeSelectPage == equipNum / 5 - 1 && equipNum % 5 == 0))
            R_upgradeSelectPageImages[1].gameObject.SetActive(false);
    }

    // 강화석 강화 앞으로
    public void R_UpgradeSelectNext()
    {
        R_upgradeSelectPage++;
        R_UpgradeSelectButton();
    }

    // 강화석 강화 뒤로
    public void R_UpgradeSelectPrevious()
    {
        R_upgradeSelectPage--;
        R_UpgradeSelectButton();
    }

    // 도구 강화석 업그레이드 정보 갱신
    public void SetReinforceInfo()
    {
        bool isDestroyed = false;
        string plusStat01 = ""; // 아이템 강화 능력 01
        string plusStat02 = ""; // 아이템 강화 능력 02
        float plusPercent = 0f;
        int itemNum = 0;

        // 본체 작성
        switch (R_upgradeMenuIndex)
        {
            case 0: if (SaveScript.saveData.isPickReinforceDestroy[R_upgradeListIndex]) isDestroyed = true; break;
            case 1: if (SaveScript.saveData.isHatReinforceDestroy[R_upgradeListIndex]) isDestroyed = true; break;
            case 2: if (SaveScript.saveData.isRingReinforceDestroy[R_upgradeListIndex]) isDestroyed = true; break;
            case 3: if (SaveScript.saveData.isPendantReinforceDestroy[R_upgradeListIndex]) isDestroyed = true; break;
            case 4: if (SaveScript.saveData.isSwordReinforceDestroy[R_upgradeListIndex]) isDestroyed = true; break;
        }

        switch (R_upgradeMenuIndex)
        {
            case 0: // 곡괭이
                R_star = SaveScript.saveData.pickReinforces[R_upgradeListIndex];
                R_tier_0 = GameFuction.GetTier_R(R_star, out R_tier_1);
                R_sprite = SaveScript.picks[R_upgradeListIndex].sprites[0];
                plusStat01 = "+ 곡괭이 내구도 < " + Mathf.Round(SaveScript.picks[R_upgradeListIndex].reinforce_basic * SaveScript.pickReinforce1Percents[R_tier_0][R_tier_1]) + " > 증가 ";
                plusStat02 = "+ 곡괭이 효율 < " + SaveScript.pickReinforce2Percents[R_tier_0][R_tier_1] * 100 + "% > 증가 ";
                break;
            case 1: // 모자
                R_star = SaveScript.saveData.hatReinforces[R_upgradeListIndex];
                R_tier_0 = GameFuction.GetTier_R(R_star, out R_tier_1);
                R_sprite = SaveScript.hats[R_upgradeListIndex].sprite;
                plusStat01 = "+ 방어력 < " + Mathf.Round(SaveScript.hats[R_upgradeListIndex].reinforce_basic * SaveScript.hatReinforce1Percents[R_tier_0][R_tier_1]) + " > 증가 ";
                if (SaveScript.stat.hat02 < 1f) plusStat02 = "+ 특수 회피 확률 < " + SaveScript.hatReinforce2Percents[R_tier_0][R_tier_1] * 100 + "% > 증가 ";
                else plusStat02 = "+  특수 회피력 < " + GameFuction.GetReinforcePercent_Over2(1, SaveScript.ringReinforce2Percents[R_tier_0][R_tier_1]) * 100 + "% > 증가 ";
                break;
            case 2: // 반지
                R_star = SaveScript.saveData.ringReinforces[R_upgradeListIndex];
                R_tier_0 = GameFuction.GetTier_R(R_star, out R_tier_1);
                R_sprite = SaveScript.rings[R_upgradeListIndex].sprite;
                plusStat01 = "+ 상점 판매금 < " + Mathf.Round(SaveScript.rings[R_upgradeListIndex].reinforce_basic * SaveScript.ringReinforce1Percents[R_tier_0][R_tier_1] * 100 * 100) / 100 + "% > 증가 ";
                if (SaveScript.stat.ring02 < 1f) plusStat02 = "+ 특수 판매 확률 < " + SaveScript.ringReinforce2Percents[R_tier_0][R_tier_1] * 100 + "% > 증가 ";
                else plusStat02 = "+ 특수 판매력 < " + GameFuction.GetReinforcePercent_Over2(2, SaveScript.ringReinforce2Percents[R_tier_0][R_tier_1]) * 100 + "% > 증가 ";
                break;
            case 3: // 목걸이
                R_star = SaveScript.saveData.pendantReinforces[R_upgradeListIndex];
                R_tier_0 = GameFuction.GetTier_R(R_star, out R_tier_1);
                R_sprite = SaveScript.pendants[R_upgradeListIndex].sprite;
                plusStat01 = "+ 최대 추가 광물 획득 \n< " + Mathf.Round(SaveScript.pendants[R_upgradeListIndex].reinforce_basic * SaveScript.pendantReinforce1Percents[R_tier_0][R_tier_1]) + "개 > 증가 ";
                if (SaveScript.stat.pendant02 < 1f) plusStat02 = "+ 발동 확률 < " + SaveScript.pendants[R_upgradeListIndex].reinforce_basic * SaveScript.pendantReinforce2Percents[R_tier_0][R_tier_1] * 100 + "% > 증가 ";
                else plusStat02 = "+ 최소 추가 광물 획득 \n< " + Mathf.Round(SaveScript.pendants[R_upgradeListIndex].reinforce_basic * GameFuction.GetReinforcePercent_Over2(3, SaveScript.ringReinforce2Percents[R_tier_0][R_tier_1])) + "개 > 증가 ";
                break;
            case 4: // 검
                R_star = SaveScript.saveData.swordReinforces[R_upgradeListIndex];
                R_tier_0 = GameFuction.GetTier_R(R_star, out R_tier_1);
                R_sprite = SaveScript.swords[R_upgradeListIndex].sprite;
                plusStat01 = "+ 공격력 < " + Mathf.Round(SaveScript.swords[R_upgradeListIndex].reinforce_basic * SaveScript.swordReinforce1Percents[R_tier_0][R_tier_1]) + " > 증가 ";
                if (SaveScript.stat.sword02 < 1f) plusStat02 = "+ 크리티컬 확률 < " + SaveScript.swordReinforce2Percents[R_tier_0][R_tier_1] * 100 + "% > 증가 ";
                else plusStat02 = "+ 크리티컬 데미지 \n< " + GameFuction.GetReinforcePercent_Over2(4, SaveScript.ringReinforce2Percents[R_tier_0][R_tier_1]) * 100 + "% > 증가 ";
                break;
        }

        R_quality = GameFuction.GetQualityOfEquipment(R_star);
        isNotDestroy = isNotDowngrade = false;
        itemPlusStar = 0;

        switch (R_tier_0)
        {
            case 0:
                successPercent = SaveScript.reinforceSuccessPercents[R_star];
                destroyPercent = SaveScript.reinforceDestroyPercents[R_star];
                reinforceOrePrice = SaveScript.reinforceOreNeededNums[R_star / SaveScript.reinforceNumAsQulity];
                manaOrePrice = 0;
                break;
            case 1:
                successPercent = SaveScript.reinforceSuccessPercents[19] - 0.05f * ((R_star / 10f) - 1);
                if (successPercent < 0.01f) successPercent = 0.01f;
                successPercentPlused = successPercent;
                destroyPercent = SaveScript.reinforceDestroyPercents[19] + 0.05f * ((R_star / 10f) - 1);
                if (destroyPercent > 0.8f) destroyPercent = 0.8f;
                reinforceOrePrice = 200 + (R_star - 20) * 10;
                manaOrePrice = 0;
                break;
            case 2:
                successPercent = 0.01f;
                destroyPercent = 0.8f;
                reinforceOrePrice = 1000 + (R_star - 100) * 20;
                manaOrePrice = ((R_star - 100) / 100 + 1) * 3;
                break;
            case 3:
                successPercent = 0.01f;
                destroyPercent = 0.9f;
                reinforceOrePrice = 20000 + (R_star - 1000) * 30;
                manaOrePrice = 30 + (R_star - 1000) / 1000 * 5;
                break;
            case 4:
                successPercent = 0.01f;
                destroyPercent = 0.99f;
                reinforceOrePrice = 300000 + (R_star - 10000) * 1000;
                manaOrePrice = 100 + (R_star - 10000) / 10 * 5;
                break;
        }
        successPercentPlused = successPercent;

        // 아이템 버프 및 마나석 버프 효과 적용
        plusPercent += GameFuction.GetBufItemPercent(11) + GameFuction.GetElixirPercent(11);
        plusPercent += GameFuction.GetManaBufForceForData(4);
        successPercentPlused += plusPercent;

        for (int i = 0; i < ReinforceItemUse.slotCodes.Length; i++)
        {
            if (ReinforceItemUse.slotCodes[i] != -1)
                itemNum++;

            for (int j = 0; j < 3; j++)
                if (ReinforceItemUse.slotCodes[i] == j)
                    successPercentPlused += SaveScript.reinforceItems[j].forcePercent;
            if (ReinforceItemUse.slotCodes[i] == 3)
                isNotDowngrade = true;
            if (ReinforceItemUse.slotCodes[i] == 4)
                isNotDestroy = true;
            if (ReinforceItemUse.slotCodes[i] == 5)
                isNotDowngrade = isNotDestroy = true;
            if (ReinforceItemUse.slotCodes[i] > 5)
            {
                isNotDestroy = true;
                successPercentPlused = 1f;
            }
            if (ReinforceItemUse.slotCodes[i] > 5)
            {
                if (ReinforceItemUse.slotCodes[i] > 7)
                    itemPlusStar += (int)SaveScript.reinforceItems2[ReinforceItemUse.slotCodes[i] - SaveScript.reinforceItemNum].forcePercent;
                else
                    itemPlusStar += ReinforceItemUse.slotCodes[i] - 5;
            }
        }

        if (isNotDestroy) destroyPercent = 0f;
        failPercent = 1f - (successPercentPlused + destroyPercent);
        reinforceOrePrice = reinforceOrePrice * (1 + itemNum);
        manaOrePrice = manaOrePrice * (1 + itemNum);

        // 한계 돌파 조정
        if ((R_upgradeMenuIndex == 0 && R_upgradeListIndex == 0) && successPercentPlused >= 1f)
        {
            successPercentPlused = 1f;
            failPercent = 0f;
        }
        if ((R_upgradeMenuIndex != 0 || R_upgradeListIndex != 0) && successPercentPlused + destroyPercent >= 1f)
        {
            successPercentPlused = 1f - destroyPercent;
            failPercent = 0f;
        }
        if ((R_upgradeMenuIndex == 0 && R_upgradeListIndex == 0) || (R_upgradeMenuIndex == 4 && R_upgradeListIndex == 0))
        {
            destroyPercent = 0f;
        }

        // UI Setting
        Inventory.instance.R_oreObject.SetActive(true);
        Inventory.instance.R_oreText.text = GameFuction.GetNumText(SaveScript.saveData.hasReinforceOre);
        Inventory.instance.manaObject.SetActive(true);
        Inventory.instance.manaText.text = GameFuction.GetNumText(SaveScript.saveData.manaOre);

        R_ReinforceBox.sprite = R_ReinforceSprites[R_tier_0];
        for (int i = 0; i < R_ReinforceTiers.Length; i++)
            R_ReinforceTiers[i].SetActive(false);
        R_ReinforceTiers[R_tier_0].SetActive(true);
        for (int i = 1; i < R_ReinforceStars[R_tier_0].Length; i++)
        {
            if (i <= R_tier_1)
                R_ReinforceStars[R_tier_0][i].sprite = R_ReinforceStarSprites[R_tier_0 * 2];
            else
                R_ReinforceStars[R_tier_0][i].sprite = R_ReinforceStarSprites[R_tier_0 * 2 + 1];
        }
        R_ToolQualityImage.color = SaveScript.qualityColors[R_quality];
        R_ToolImage.sprite = R_sprite;
        R_StartButton.color = stageColors[R_tier_0];

        if (itemPlusStar == 0)
            R_percentInfoTexts[0].text = R_star + "성 ▶ " + (R_star + 1) + "성";
        else
            R_percentInfoTexts[0].text = R_star + "성 ▶ " + (R_star + itemPlusStar) + "성";

        if ((R_upgradeMenuIndex == 0 && R_upgradeListIndex == 0) || (R_upgradeMenuIndex == 4 && R_upgradeListIndex == 0)) // 나무 곡괭이 && 검
        {
            R_percentInfoTexts[1].text = "성공 확률 : " + Mathf.Round(successPercent * 100 * 10f) / 10f + "%";
            R_percentInfoTexts[2].text = "실패 확률 : " + Mathf.Round(failPercent * 100 * 10f) / 10f + "%";
            R_percentInfoTexts[3].text = "";
        }
        else
        {
            R_percentInfoTexts[1].text = "성공 확률 : " + successPercent * 100 + "%";
            if (destroyPercent == 0f)
            {
                R_percentInfoTexts[2].text = "실패 확률 : " + Mathf.Round(failPercent * 100 * 10f) / 10f + "%";
                R_percentInfoTexts[3].text = "";
            }
            else
            {
                R_percentInfoTexts[2].text = "실패 확률 : " + Mathf.Round(failPercent * 100 * 10f) / 10f + "%";
                R_percentInfoTexts[3].text = "파괴 확률 : " + destroyPercent * 100 + "%";
            }
        }

        if (successPercent != successPercentPlused)
            R_percentInfoTexts[1].text += " -> " + Mathf.Round(successPercentPlused * 100 * 10f) / 10f + "%";
        if (isNotDowngrade)
            R_percentInfoTexts[2].text += "(등급 하락 방지)";

        R_plusStatInfoTexts[0].text = "[ 추가 효과 ]";
        R_plusStatInfoTexts[1].text = plusStat01;
        R_plusStatInfoTexts[2].text = plusStat02;

        R_itemTexts[0].text = "< 강화에 필요한 재료 >";
        R_itemTexts[1].text = GameFuction.GetNumText(reinforceOrePrice);
        R_itemTexts[2].text = GameFuction.GetNumText(manaOrePrice);
        if (SaveScript.saveData.hasReinforceOre >= reinforceOrePrice)
            R_itemTexts[1].color = Color.green;
        else
            R_itemTexts[1].color = Color.red;
        if (SaveScript.saveData.manaOre >= manaOrePrice)
            R_itemTexts[2].color = Color.green;
        else
            R_itemTexts[2].color = Color.red;
        if (SaveScript.saveData.hasReinforceOre >= reinforceOrePrice && SaveScript.saveData.manaOre >= manaOrePrice)
            isCanReinforce = true;
        else
            isCanReinforce = false;

        if (isDestroyed)
        {
            switch (R_upgradeMenuIndex)
            {
                case 0: R_sprite = Pick.destroyedSprite; break;
                case 1: R_sprite = Hat.destroyedSprite; break;
                case 2: R_sprite = Ring.destroyedSprite; break;
                case 3: R_sprite = Pendant.destroyedSprite; break;
                case 4: R_sprite = Sword.destroyedSprite; break;
            }

            isNeedRefair = true;
            R_ToolImage.sprite = R_sprite;
            R_ToolDestroyedImage.gameObject.SetActive(true);
            R_ToolQualityImage.color = new Color(1f, 1f, 1f, 0f);

            R_percentInfoTexts[0].text = "[ 파괴된 장비 ]";
            R_percentInfoTexts[1].text = "";
            R_percentInfoTexts[2].text = "";
            R_percentInfoTexts[3].text = "";

            R_plusStatInfoTexts[0].text = "[ 추가 효과 ]";
            R_plusStatInfoTexts[1].text = "";
            R_plusStatInfoTexts[2].text = "";

            R_itemTexts[0].text = "< 파괴된 장비를 수리하려면 광고를 시청해야 합니다 >";
            if(SaveScript.saveData.isRemoveAD)
                R_itemTexts[0].text = "";
            R_itemTexts[1].text = "";
            R_startButtonText.text = "수 리";
        }
        else
        {
            isNeedRefair = false;
            R_ToolDestroyedImage.gameObject.SetActive(false);
            R_startButtonText.text = "강 화";
        }
    }

    // 페이지 넘기기 버튼(강화석 강화)
    public void R_PageButton()
    {
        Inventory.instance.SetAudio(0);
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            R_page = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

        Inventory.instance.R_oreObject.SetActive(false);
        Inventory.instance.manaObject.SetActive(false);
        Inventory.instance.invenButton.gameObject.SetActive(false);
        Inventory.instance.fusionButton.gameObject.SetActive(false);
        ReinforceItemUse.instance.invenOnOffButton.gameObject.SetActive(false);
        switch (R_page)
        {
            case 0: // 업그레이드 버튼
                Inventory.instance.animator.SetInteger("R_PageType", 1);
                R_upgradeSelectBox.SetActive(false);
                R_upgradeSelectPageObject.SetActive(false);
                R_upgradeSelectPage = 0;
                break;
            case 1: // 인벤토리로 돌아가기 버튼
                Inventory.instance.animator.SetInteger("R_PageType", 0);
                Inventory.instance.SetBasicInfo();
                Inventory.instance.SlotClear();
                break;
            case 2: // 업그레이드 도구 선택창으로 돌아가기
                Inventory.instance.animator.SetInteger("R_PageType", 1);
                Inventory.instance.animator.SetInteger("R_Reinforce", 0);
                R_upgradeSelectBox.SetActive(false);
                R_upgradeSelectPageObject.SetActive(false);
                R_upgradeSelectPage = 0;
                ReinforceItemUse.SetReinforceVariable();
                ReinforceItemUse.instance.SetInvenSlots();
                break;
        }
    }

    // 강화석 업그레이드 슬롯 잠금
    private void ReinforceSlotClear()
    {
        R_upgradeSelectPageObject.SetActive(false);

        for (int i = 0; i < R_selectImages.Length; i++)
        {
            R_selectImages[i].sprite = null;
            R_selectImages[i].color = new Color(1f, 1f, 1f, 0f);
            R_selectButtons[i].enabled = false;
            R_select_DestroyedImages[i].gameObject.SetActive(false);
            R_select_QualityImages[i].color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // 강화석 업그레이드 도구 선택 버튼
    public void R_UpgradeSelectButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
        {
            R_upgradeMenuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
            switch (R_upgradeMenuIndex)
            {
                case 0: R_upgradeSelectPage = SaveScript.saveData.equipPick / 5; break;
                case 1: R_upgradeSelectPage = SaveScript.saveData.equipHat / 5; break;
                case 2: R_upgradeSelectPage = SaveScript.saveData.equipRing / 5; break;
                case 3: R_upgradeSelectPage = SaveScript.saveData.equipPendant / 5; break;
                case 4: R_upgradeSelectPage = SaveScript.saveData.equipSword / 5; break;
            }
        }

        ReinforceSlotClear();
        Inventory.instance.SetAudio(0);
        SetTutorial();
        R_upgradeSelectBox.SetActive(true);
        R_upgradeSelectPageObject.SetActive(true);
        R_UpgradeSelectSetting();

        int slotOrder = 0;
        int equipNum = 0;
        bool[] isDestroyed, hasEquips;
        int[] reinforces;
        Sprite destoryedSprite;

        switch (R_upgradeMenuIndex)
        {
            case 0:
                equipNum = SaveScript.pickNum;
                isDestroyed = SaveScript.saveData.isPickReinforceDestroy;
                hasEquips = SaveScript.saveData.hasPicks;
                reinforces = SaveScript.saveData.pickReinforces;
                destoryedSprite = Pick.destroyedSprite;
                break;
            case 1:
                equipNum = SaveScript.hatNum;
                isDestroyed = SaveScript.saveData.isHatReinforceDestroy;
                hasEquips = SaveScript.saveData.hasHats;
                reinforces = SaveScript.saveData.hatReinforces;
                destoryedSprite = Hat.destroyedSprite;
                break;
            case 2:
                equipNum = SaveScript.RingNum;
                isDestroyed = SaveScript.saveData.isRingReinforceDestroy;
                hasEquips = SaveScript.saveData.hasRings;
                reinforces = SaveScript.saveData.ringReinforces;
                destoryedSprite = Ring.destroyedSprite;
                break;
            case 3:
                equipNum = SaveScript.PendantNum;
                isDestroyed = SaveScript.saveData.isPendantReinforceDestroy;
                hasEquips = SaveScript.saveData.hasPenants;
                reinforces = SaveScript.saveData.pendantReinforces;
                destoryedSprite = Pendant.destroyedSprite;
                break;
            default:
                equipNum = SaveScript.swordNum;
                isDestroyed = SaveScript.saveData.isSwordReinforceDestroy;
                hasEquips = SaveScript.saveData.hasSwords;
                reinforces = SaveScript.saveData.swordReinforces;
                destoryedSprite = Sword.destroyedSprite;
                break;
        }

        for (int i = 0; i < R_selectImages.Length; i++)
        {
            slotOrder = R_upgradeSelectPage * 5 + i;
            if (slotOrder >= SaveScript.pickNum) return;

            if (!isDestroyed[slotOrder])
            {
                switch (R_upgradeMenuIndex)
                {
                    case 0: R_selectImages[i].sprite = SaveScript.picks[slotOrder].sprites[0]; break;
                    case 1: R_selectImages[i].sprite = SaveScript.hats[slotOrder].sprite; break;
                    case 2: R_selectImages[i].sprite = SaveScript.rings[slotOrder].sprite; break;
                    case 3: R_selectImages[i].sprite = SaveScript.pendants[slotOrder].sprite; break;
                    default: R_selectImages[i].sprite = SaveScript.swords[slotOrder].sprite; break;
                }

                if (hasEquips[slotOrder])
                {
                    R_select_QualityImages[i].color = GameFuction.GetColorEquipment(reinforces, slotOrder);
                    R_selectImages[i].color = new Color(1f, 1f, 1f, 1f);
                    R_selectButtons[i].enabled = true;
                }
                else
                {
                    R_selectImages[i].color = new Color(1f, 1f, 1f, 0.3f);
                }
            }
            else
            {
                R_selectImages[i].sprite = destoryedSprite;
                R_selectButtons[i].enabled = true;
                R_select_DestroyedImages[i].gameObject.SetActive(true);
                R_selectImages[i].color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    // 강화석 업그레이드 상세 도구 선택 버튼
    public void R_UpgradeToolSelectButton()
    {
        Inventory.instance.animator.SetInteger("R_PageType", 2);
        Inventory.instance.SetAudio(0);
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            R_upgradeListIndex = R_upgradeSelectPage * 5 + EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        ReinforceItemUse.instance.invenOnOffButton.gameObject.SetActive(true);
        startButton.enabled = true;

        SetTutorial();
        SetReinforceInfo();
    }

    // 강화석 업그레이드 시작 버튼
    public void R_UpgradeBuyButton()
    {
        if (!isNeedRefair)
        {
            if (isCanReinforce)
            {
                Inventory.instance.SetAudio(0);
                R_animation.SetActive(true);
                passClickPanel.SetActive(true);

                Inventory.instance.animator.SetInteger("R_Reinforce", 1);
                Inventory.instance.animator.Play("R_Start", -1, 0f);
                SaveScript.saveData.hasReinforceOre -= reinforceOrePrice;
                SaveScript.saveData.manaOre -= manaOrePrice;

                R_ani_ToolImage.sprite = R_sprite;
            }
            else
            {
                SystemInfoCtrl.instance.SetErrorInfo("재료가 부족합니다.");
                Inventory.instance.SetAudio(2);
            }
        }
        else
        {
            GoogleAd.instance.ADShow(0);
        }
    }

    // 강화석 업그레이드 결과
    public void R_ReinforceResult()
    {
        uiboxes = R_ani_itemPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uiboxes.Length; i++)
            Destroy(uiboxes[i].gameObject);
        passClickPanel.SetActive(false);

        if (!isNeedRefair)
        {
            bool isDestroy = false;
            bool isSuccess = false;
            int itemNum = 0;

            for (int i = 0; i < ReinforceItemUse.slotCodes.Length; i++)
                if (ReinforceItemUse.slotCodes[i] != -1)
                    itemNum++;
            AchievementCtrl.instance.SetAchievementAmount(26, itemNum);
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 17 }, itemNum);

            // 주문서 슬롯 세팅
            for (int i = 0; i < ReinforceItemUse.slotCodes.Length; i++)
            {
                if (ReinforceItemUse.slotCodes[i] != -1)
                {
                    if (ReinforceItemUse.slotCodes[i] < SaveScript.reinforceItemNum && SaveScript.saveData.hasReinforceItems[ReinforceItemUse.slotCodes[i]] > 0)
                        SaveScript.saveData.hasReinforceItems[ReinforceItemUse.slotCodes[i]]--;
                    else if (ReinforceItemUse.slotCodes[i] >= SaveScript.reinforceItemNum && SaveScript.saveData.hasReinforceItems2[ReinforceItemUse.slotCodes[i] - SaveScript.reinforceItemNum] > 0)
                        SaveScript.saveData.hasReinforceItems2[ReinforceItemUse.slotCodes[i] - SaveScript.reinforceItemNum]--;
                    else
                        ReinforceItemUse.slotCodes[i] = -1;
                }
            }

            isSuccess = GameFuction.GetRandFlag(successPercentPlused);
            if (((R_upgradeMenuIndex != 0 || R_upgradeListIndex != 0) || (R_upgradeMenuIndex != 4 || R_upgradeListIndex != 0)) && !isNotDestroy)
                isDestroy = GameFuction.GetRandFlag(destroyPercent);

            if (!isDestroy)
            {
                if (isSuccess)
                {
                    switch (R_upgradeMenuIndex)
                    {
                        case 0:
                            if (itemPlusStar == 0) SaveScript.saveData.pickReinforces[R_upgradeListIndex]++;
                            else SaveScript.saveData.pickReinforces[R_upgradeListIndex] += itemPlusStar;
                            break;
                        case 1:
                            if (itemPlusStar == 0) SaveScript.saveData.hatReinforces[R_upgradeListIndex]++;
                            else SaveScript.saveData.hatReinforces[R_upgradeListIndex] += itemPlusStar;
                            break;
                        case 2:
                            if (itemPlusStar == 0) SaveScript.saveData.ringReinforces[R_upgradeListIndex]++;
                            else SaveScript.saveData.ringReinforces[R_upgradeListIndex] += itemPlusStar;
                            break;
                        case 3:
                            if (itemPlusStar == 0) SaveScript.saveData.pendantReinforces[R_upgradeListIndex]++;
                            else SaveScript.saveData.pendantReinforces[R_upgradeListIndex] += itemPlusStar;
                            break;
                        case 4:
                            if (itemPlusStar == 0) SaveScript.saveData.swordReinforces[R_upgradeListIndex]++;
                            else SaveScript.saveData.swordReinforces[R_upgradeListIndex] += itemPlusStar;
                            break;
                    }

                    if (itemPlusStar == 0) AchievementCtrl.instance.SetAchievementAmount(28, 1);
                    else AchievementCtrl.instance.SetAchievementAmount(28, itemPlusStar);
                    R_result_infotext.text = "강화 성공!";
                    R_result_infotext.color = new Color(0.5f, 1f, 0.5f, 1f);
                    Inventory.instance.SetAudio(9);
                }
                else
                {
                    if ((R_star > 20 || (R_star <= 20 && R_star % SaveScript.reinforceNumAsQulity != 0)) && !isNotDowngrade)
                    {
                        switch (R_upgradeMenuIndex)
                        {
                            case 0: SaveScript.saveData.pickReinforces[R_upgradeListIndex]--; break;
                            case 1: SaveScript.saveData.hatReinforces[R_upgradeListIndex]--; break;
                            case 2: SaveScript.saveData.ringReinforces[R_upgradeListIndex]--; break;
                            case 3: SaveScript.saveData.pendantReinforces[R_upgradeListIndex]--; break;
                            case 4: SaveScript.saveData.swordReinforces[R_upgradeListIndex]--; break;
                        }
                    }

                    R_result_infotext.text = "강화 실패...";
                    R_result_infotext.color = new Color(1f, 0.5f, 0.5f, 1f);
                    Inventory.instance.SetAudio(10);
                }

                SetReinforceInfo();
                R_result.SetActive(true);
                R_result_toolQualityImage.color = SaveScript.qualityColors[R_quality];
                R_result_toolDestroyImage.gameObject.SetActive(false);
                R_result_toolImage.sprite = R_sprite;
                R_result_nametext.color = SaveScript.qualityColors[R_quality];

                switch (R_upgradeMenuIndex)
                {
                    case 0: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.picks[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 1: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.hats[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 2: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.rings[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 3: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.pendants[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 4: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.swords[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                }
            }
            else
            {
                Inventory.instance.SetAudio(11);
                R_result_infotext.text = "장비 파괴...";
                R_result_infotext.color = new Color(1f, 0.25f, 0.5f, 1f);

                for (int i = 0; i < ReinforceItemUse.slotCodes.Length; i++)
                    ReinforceItemUse.slotCodes[i] = -1;

                switch (R_upgradeMenuIndex)
                {
                    case 0:
                        SaveScript.saveData.isPickReinforceDestroy[R_upgradeListIndex] = true;
                        if (SaveScript.saveData.equipPick == R_upgradeListIndex) SaveScript.saveData.equipPick = 0;
                        break;
                    case 1:
                        SaveScript.saveData.isHatReinforceDestroy[R_upgradeListIndex] = true;
                        if (SaveScript.saveData.equipHat == R_upgradeListIndex) SaveScript.saveData.equipHat = -1;
                        break;
                    case 2:
                        SaveScript.saveData.isRingReinforceDestroy[R_upgradeListIndex] = true;
                        if (SaveScript.saveData.equipRing == R_upgradeListIndex) SaveScript.saveData.equipRing = -1;
                        break;
                    case 3:
                        SaveScript.saveData.isPendantReinforceDestroy[R_upgradeListIndex] = true;
                        if (SaveScript.saveData.equipPendant == R_upgradeListIndex) SaveScript.saveData.equipPendant = -1;
                        break;
                    case 4:
                        SaveScript.saveData.isSwordReinforceDestroy[R_upgradeListIndex] = true;
                        if (SaveScript.saveData.equipSword == R_upgradeListIndex) SaveScript.saveData.equipSword = 0;
                        break;
                }

                SetReinforceInfo();
                R_result_toolQualityImage.color = new Color(1f, 1f, 1f, 0f);
                R_result_toolDestroyImage.gameObject.SetActive(true);
                R_result_toolImage.sprite = R_sprite;
                R_result_nametext.color = new Color(0.6f, 0.8f, 1f, 1f);
                R_result.SetActive(true);

                switch (R_upgradeMenuIndex)
                {
                    case 0: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.picks[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 1: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.hats[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 2: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.rings[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 3: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.pendants[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                    case 4: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.swords[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                }
            }
        }
        else
        {
            switch (R_upgradeMenuIndex)
            {
                case 0:
                    if (SaveScript.saveData.pickReinforces[R_upgradeListIndex] < 24)
                        SaveScript.saveData.pickReinforces[R_upgradeListIndex] = (SaveScript.saveData.pickReinforces[R_upgradeListIndex] / SaveScript.reinforceNumAsQulity) * SaveScript.reinforceNumAsQulity;
                    else
                        SaveScript.saveData.pickReinforces[R_upgradeListIndex] -= SaveScript.reinforceNumAsQulity;
                    SaveScript.saveData.isPickReinforceDestroy[R_upgradeListIndex] = false;
                    SaveScript.saveData.equipPick = R_upgradeListIndex;
                    break;
                case 1:
                    if (SaveScript.saveData.hatReinforces[R_upgradeListIndex] < 24)
                        SaveScript.saveData.hatReinforces[R_upgradeListIndex] = (SaveScript.saveData.hatReinforces[R_upgradeListIndex] / SaveScript.reinforceNumAsQulity) * SaveScript.reinforceNumAsQulity;
                    else
                        SaveScript.saveData.hatReinforces[R_upgradeListIndex] -= SaveScript.reinforceNumAsQulity;
                    SaveScript.saveData.isHatReinforceDestroy[R_upgradeListIndex] = false;
                    SaveScript.saveData.equipHat = R_upgradeListIndex;
                    break;
                case 2:
                    if (SaveScript.saveData.ringReinforces[R_upgradeListIndex] < 24)
                        SaveScript.saveData.ringReinforces[R_upgradeListIndex] = (SaveScript.saveData.ringReinforces[R_upgradeListIndex] / SaveScript.reinforceNumAsQulity) * SaveScript.reinforceNumAsQulity;
                    else
                        SaveScript.saveData.ringReinforces[R_upgradeListIndex] -= SaveScript.reinforceNumAsQulity;
                    SaveScript.saveData.isRingReinforceDestroy[R_upgradeListIndex] = false;
                    SaveScript.saveData.equipRing = R_upgradeListIndex;
                    break;
                case 3:
                    if (SaveScript.saveData.pendantReinforces[R_upgradeListIndex] < 24)
                        SaveScript.saveData.pendantReinforces[R_upgradeListIndex] = (SaveScript.saveData.pendantReinforces[R_upgradeListIndex] / SaveScript.reinforceNumAsQulity) * SaveScript.reinforceNumAsQulity;
                    else
                        SaveScript.saveData.pendantReinforces[R_upgradeListIndex] -= SaveScript.reinforceNumAsQulity;
                    SaveScript.saveData.isPendantReinforceDestroy[R_upgradeListIndex] = false;
                    SaveScript.saveData.equipPendant = R_upgradeListIndex;
                    break;
                case 4:
                    if (SaveScript.saveData.swordReinforces[R_upgradeListIndex] < 24)
                        SaveScript.saveData.swordReinforces[R_upgradeListIndex] = (SaveScript.saveData.swordReinforces[R_upgradeListIndex] / SaveScript.reinforceNumAsQulity) * SaveScript.reinforceNumAsQulity;
                    else
                        SaveScript.saveData.swordReinforces[R_upgradeListIndex] -= SaveScript.reinforceNumAsQulity;
                    SaveScript.saveData.isSwordReinforceDestroy[R_upgradeListIndex] = false;
                    SaveScript.saveData.equipSword = R_upgradeListIndex;
                    break;
            }

            R_result_infotext.text = "수리 완료!";
            R_result_infotext.color = new Color(0.5f, 1f, 0.5f, 1f);
            Inventory.instance.SetAudio(9);
            SaveScript.instance.SaveData_Asyn(true);

            SetReinforceInfo();
            R_result.SetActive(true);
            R_result_toolQualityImage.color = SaveScript.qualityColors[R_quality];
            R_result_toolDestroyImage.gameObject.SetActive(false);
            R_result_toolImage.sprite = R_sprite;
            R_result_nametext.color = SaveScript.qualityColors[R_quality];

            switch (R_upgradeMenuIndex)
            {
                case 0: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.picks[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                case 1: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.hats[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                case 2: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.rings[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                case 3: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.pendants[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
                case 4: R_result_nametext.text = "[" + SaveScript.qualityNames_kr[R_quality] + "] " + SaveScript.swords[R_upgradeListIndex].name + " (+" + R_star + ")"; break;
            }
        }

        // 퀘스트
        CheckReinforceQuest();
    }

    // 강화석 업그레이드 결과창 닫기
    public void R_CloseResult()
    {
        R_result.SetActive(false);
        R_animation.SetActive(false);
        ReinforceItemUse.instance.SetInvenSlots();
        Inventory.instance.SetAudio(0);
        StopCoroutine("DelayButton");
        StartCoroutine("DelayButton");
        SetTutorial();
    }

    //  강화석 애니메이션 세팅
    public void SetReinforceAni()
    {
        uiboxes = R_ani_itemPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uiboxes.Length; i++)
            Destroy(uiboxes[i].gameObject);

        for (int i = 0; i < ReinforceItemUse.slotCodes.Length; i++)
        {
            if (ReinforceItemUse.slotCodes[i] != -1)
            {
                uibox = Instantiate(R_ani_itemPrefab, R_ani_itemPanel).GetComponent<UIBox>();
                if (ReinforceItemUse.slotCodes[i] < SaveScript.reinforceItemNum)
                    uibox.images[0].sprite = SaveScript.reinforceItems[ReinforceItemUse.slotCodes[i]].image;
                else
                    uibox.images[0].sprite = SaveScript.reinforceItems2[ReinforceItemUse.slotCodes[i] - SaveScript.reinforceItemNum].image;
            }
        }
    }

    public void PassReinforceAni()
    {
        passClickPanel.SetActive(false);
        Inventory.instance.animator.Play("R_Start", -1, 0.9f);
        R_ReinforceResult();
    }

    IEnumerator DelayButton()
    {
        startButton.enabled = false;
        yield return new WaitForSeconds(0.075f);
        startButton.enabled = true;
    }

    public void SetTutorial()
    {
        int type = -1;

        for (int i = 0; i < rButtons.Length; i++)
        {
            if (i < 5)
            {
                if (QuestCtrl.CheckFadeUI(new int[] { 8, 13 }, SaveScript.saveData.mainQuest_list))
                {
                    switch (i)
                    {
                        case 0: rButton_isOn[i] = SaveScript.saveData.equipPick == -1 || SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick] < SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal; break;
                        case 1: rButton_isOn[i] = SaveScript.saveData.equipHat == -1 || SaveScript.saveData.hatReinforces[SaveScript.saveData.equipHat] < SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal; break;
                        case 2: rButton_isOn[i] = SaveScript.saveData.equipRing == -1 || SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing] < SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal; break;
                        case 3: rButton_isOn[i] = SaveScript.saveData.equipPendant == -1 || SaveScript.saveData.pendantReinforces[SaveScript.saveData.equipPendant] < SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal; break;
                        case 4: rButton_isOn[i] = SaveScript.saveData.equipSword == -1 || SaveScript.saveData.swordReinforces[SaveScript.saveData.equipSword] < SaveScript.mainQuests[SaveScript.saveData.mainQuest_list].goal; break;
                    }
                }
                else
                    rButton_isOn[i] = false;
                rButtons[i].enabled = rButtons[i].isReSize = rButton_isOn[i];
            }
            else
            {
                // 그 외 UI들
                rButtons[i].enabled = rButtons[i].isReSize = QuestCtrl.CheckFadeUI(new int[] { 8, 13, 17 }, SaveScript.saveData.mainQuest_list);
            }
        }

        for (int i = 0; i < rButtons_2.Length; i++)
            rButtons_2[i].enabled = rButtons_2[i].isReSize = false;
        rButtons[5].enabled = rButtons[5].isReSize = false;

        if (rButton_isOn[R_upgradeMenuIndex])
        {
            switch (R_upgradeMenuIndex)
            {
                case 0: type = SaveScript.saveData.equipPick; break;
                case 1: type = SaveScript.saveData.equipHat; break;
                case 2: type = SaveScript.saveData.equipRing; break;
                case 3: type = SaveScript.saveData.equipPendant; break;
                case 4: type = SaveScript.saveData.equipSword; break;
            }

            if (type != -1 && type / 5 == R_upgradeSelectPage)
                rButtons_2[type % 5].enabled = rButtons_2[type % 5].isReSize = true;
            rButtons[5].enabled = rButtons[5].isReSize = type == R_upgradeListIndex;
        }
    }

    static public void CheckReinforceQuest()
    {
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 8, 13, 25, 30, 44, 62, 70, 80, 90, 100, 110, 120, 130, 140 }, TotalReinforceUpgrades());
    }

    static private int TotalReinforceUpgrades()
    {
        if (SaveScript.saveData.equipPick == -1 || SaveScript.saveData.equipHat == -1 || SaveScript.saveData.equipRing == -1
            || SaveScript.saveData.equipPendant == -1 || SaveScript.saveData.equipSword == -1)
            return 0;

        int[] upgrades = {
            SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick], SaveScript.saveData.hatReinforces[SaveScript.saveData.equipHat], SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing],
            SaveScript.saveData.pendantReinforces[SaveScript.saveData.equipPendant], SaveScript.saveData.swordReinforces[SaveScript.saveData.equipSword]
        };

        return Mathf.Min(upgrades);
    }
}
