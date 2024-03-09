using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public static bool isGotoMainScene;

    public new AudioSource audio;
    public Animator animator;
    private bool isSlotOn;
    private int slotIndex, slotListIndex;
    private int mainSelectPage;

    public RectTransform moveUI;
    public GameObject expObject;
    public Text expText;
    public GameObject R_oreObject;
    public GameObject manaObject;
    public Text manaText;
    public Text R_oreText;
    public Button TakeOffButton;
    public GameObject SelectBox;
    private Image[] select_DestroyedImages, select_QualityImages, selectImages;
    private Button[] selectButtons;
    public GameObject mainSelectPageObject;
    private Image[] mainSelectPageImages;

    public Image[] slotImages;
    private Image[] slot_QualityImages;
    public GameObject playerImageObject;
    public GameObject content;
    public FadeEffect invenButton, fusionButton, reinforceItemSlot;
    private TMP_Text[] infoTexts;
    private Image[] playerImages;
    [SerializeField] private UIBox[] cashEquipments;
    [SerializeField] private UIBox systemSubInfo;

    private Vector2 cashEquipment_subInfoVec;
    private int cashEquipment_index;
    public bool[] rButton_isOn = new bool[5];

    UIBox UIBox;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        infoTexts = content.GetComponentsInChildren<TMP_Text>();
        playerImages = playerImageObject.GetComponentsInChildren<Image>();

        Order[] datas = SelectBox.GetComponentsInChildren<Order>();
        select_QualityImages = new Image[datas.Length];
        select_DestroyedImages = new Image[datas.Length];
        selectImages = new Image[datas.Length];
        selectButtons = new Button[datas.Length];
        for (int i = 0; i < datas.Length; i++)
        {
            select_QualityImages[i] = datas[i].GetComponentsInChildren<Image>()[0];
            select_DestroyedImages[i] = datas[i].GetComponentsInChildren<Image>()[1];
            selectImages[i] = datas[i].GetComponentsInChildren<Image>()[2];
            selectButtons[i] = datas[i].GetComponent<Button>();
        }

        slot_QualityImages = new Image[slotImages.Length];
        for (int i = 0; i < slot_QualityImages.Length; i++)
            slot_QualityImages[i] = slotImages[i].transform.parent.GetComponentsInChildren<Image>()[1];

        for (int i = 0; i < cashEquipments.Length; i++)
        {
            if (SaveScript.saveData.hasCashEquipment[i])
                cashEquipments[i].images[0].color = cashEquipments[i].images[1].color = Color.white;
            else
                cashEquipments[i].images[0].color = cashEquipments[i].images[1].color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        }

        mainSelectPageImages = mainSelectPageObject.GetComponentsInChildren<Image>();

        TakeOffButton.gameObject.SetActive(false);
        expObject.SetActive(false);
        manaObject.SetActive(false);
        R_oreObject.SetActive(false);
        mainSelectPageObject.SetActive(false);
        systemSubInfo.gameObject.SetActive(false);
        cashEquipment_subInfoVec = new Vector2(-337, -154);

        audio.mute = !SaveScript.saveData.isSEOn;

        SlotClear();
        SetBasicInfo();
        SetTutorial();
    }

    private void Update()
    {
        // 광고 관련
        if (GoogleAd.isReward)
        {
            switch (GoogleAd.ADType)
            {
                case 0:
                    ReinforceUpgradeUI.instance.R_animation.SetActive(true);
                    ReinforceUpgradeUI.instance.R_ani_ToolImage.sprite = ReinforceUpgradeUI.instance.R_sprite;
                    animator.SetInteger("R_Reinforce", 1);
                    animator.Play("R_Start", -1, 0f);
                    GoogleAd.isReward = false;
                    break;
            }
        }
        else
        {
            if (GoogleAd.ADType == -1)
            {
                GoogleAd.ADType = -2;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            systemSubInfo.gameObject.SetActive(false);
        }
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    public void GotoMainScene()
    {
        SetAudio(0);
        isGotoMainScene = true;
    }

    public void SetTutorial()
    {
        invenButton.enabled = invenButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 16 }, SaveScript.saveData.mainQuest_list);
        fusionButton.enabled = fusionButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 47, 48 }, SaveScript.saveData.mainQuest_list);
        reinforceItemSlot.enabled = reinforceItemSlot.isReSize = QuestCtrl.CheckFadeUI(new int[] { 17 }, SaveScript.saveData.mainQuest_list);
        ReinforceUpgradeUI.instance.SetTutorial();
        ExpUpgradeUI.instance.SetTutorial();
    }

    // 메인 선택 UI 세팅
    public void MainSelectSetting()
    {
        mainSelectPageImages[0].gameObject.SetActive(true);
        mainSelectPageImages[1].gameObject.SetActive(true);
        if (mainSelectPage == 0)
            mainSelectPageImages[0].gameObject.SetActive(false);

        int equipNum = 0;

        switch (slotIndex)
        {
            case 0: equipNum = SaveScript.pickNum; break;
            case 1: equipNum = SaveScript.hatNum; break;
            case 2: equipNum = SaveScript.RingNum; break;
            case 3: equipNum = SaveScript.PendantNum; break;
            case 4: equipNum = SaveScript.swordNum; break;
        }

        if (mainSelectPage == equipNum / 5 || (mainSelectPage == equipNum / 5 - 1 && equipNum % 5 == 0))
            mainSelectPageImages[1].gameObject.SetActive(false);
    }

    // 메인 선택 앞으로
    public void MainSelectNext()
    {
        mainSelectPage++;
        isSlotOn = false;
        SlotButton();
    }

    // 메인 선택 뒤로
    public void MainSelectPrevious()
    {
        mainSelectPage--;
        isSlotOn = false;
        SlotButton();
    }

    // 로비 정보 갱신
    public void SetBasicInfo()
    {
        SaveScript.stat.SetStat();
        invenButton.gameObject.SetActive(true);
        fusionButton.gameObject.SetActive(true);
        expText.text = GameFuction.GetNumText(SaveScript.saveData.exp);
        for (int i = 0; i < slotImages.Length; i++) slotImages[i].color = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < SaveScript.accessoryNum; i++)
        {
            int equipNum = 0;
            Color color = Color.white;
            Sprite equipSprite = null;
            Sprite destroyed = null;

            switch (i)
            {
                case 0:
                    equipNum = SaveScript.saveData.equipPick;
                    destroyed = Pick.destroyedSprite;
                    if(equipNum != -1)
                    {
                        color = GameFuction.GetColorEquipment(SaveScript.saveData.pickReinforces, equipNum);
                        equipSprite = SaveScript.picks[equipNum].sprites[0];
                    }
                    break;
                case 1:
                    equipNum = SaveScript.saveData.equipHat;
                    destroyed = Hat.destroyedSprite;
                    if (equipNum != -1)
                    {
                        color = GameFuction.GetColorEquipment(SaveScript.saveData.hatReinforces, equipNum);
                        equipSprite = SaveScript.hats[equipNum].sprite;
                    }
                    break;
                case 2:
                    equipNum = SaveScript.saveData.equipRing;
                    destroyed = Ring.destroyedSprite;
                    if (equipNum != -1)
                    {
                        color = GameFuction.GetColorEquipment(SaveScript.saveData.ringReinforces, equipNum);
                        equipSprite = SaveScript.rings[equipNum].sprite;
                    }
                    break;
                case 3:
                    equipNum = SaveScript.saveData.equipPendant;
                    destroyed = Pendant.destroyedSprite;
                    if (equipNum != -1)
                    {
                        color = GameFuction.GetColorEquipment(SaveScript.saveData.pendantReinforces, equipNum);
                        equipSprite = SaveScript.pendants[equipNum].sprite;
                    }
                    break;
                case 4:
                    equipNum = SaveScript.saveData.equipSword;
                    destroyed = Sword.destroyedSprite;
                    if (equipNum != -1)
                    {
                        color = GameFuction.GetColorEquipment(SaveScript.saveData.swordReinforces, equipNum);
                        equipSprite = SaveScript.swords[equipNum].sprite;
                    }
                    break;
            }
            
            if(equipNum != -1)
            {
                slotImages[i].sprite = equipSprite;
                slot_QualityImages[i].color = color;
                playerImages[1 + i].color = SaveScript.toolColors[equipNum];
                playerImages[1 + i].gameObject.SetActive(true);

                Color nameColor = color * 0.5f;
                nameColor.a = 1f;
                infoTexts[i * 2].color = nameColor;
            }
            else
            {
                slotImages[i].sprite = destroyed;
                slot_QualityImages[i].color = new Color(1f, 1f, 1f, 0f);
                playerImages[1 + i].gameObject.SetActive(false);
                infoTexts[i * 2].text = "";
                infoTexts[i * 2 + 1].text = "";
            }
        }

        if (SaveScript.saveData.equipPick != -1)
        {
            int quality = GameFuction.GetQualityOfEquipment(SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick]);
            float plus01 = SaveScript.stat.pick01;
            float plus02 = SaveScript.stat.pick02;

            long hp = (long)(SaveScript.picks[SaveScript.saveData.equipPick].durability + SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * plus01);

            infoTexts[0].text = SaveScript.picks[SaveScript.saveData.equipPick].name + " (" + SaveScript.qualityNames_kr[quality] + ")";
            infoTexts[1].text = "- '" + SaveScript.qualityNames_kr[quality] + "<color=black>' 등급까지 채광 가능";
            if (quality != 6) infoTexts[1].text += " | '" + "얼티밋<color=black>' 및 '" + "미스틱<color=black>' 등급도 채광 가능";
            infoTexts[1].text += "\n- 채광 속도 증가율 < <color=#AA1212>" + GameFuction.GetNumText((long)(Mathf.Round(plus02 * 100f * 100f) / 100)) + "% <color=black>> , " + "내구도 < <color=#AA1212>" + GameFuction.GetNumText(hp) + " <color=black>>";
        }

        if (SaveScript.saveData.equipHat != -1)
        {
            int quality = GameFuction.GetQualityOfEquipment(SaveScript.saveData.hatReinforces[SaveScript.saveData.equipHat]);
            float plus01 = SaveScript.stat.hat01;
            float plus02 = SaveScript.stat.hat02;

            long armor = (long)Mathf.Round(SaveScript.hats[SaveScript.saveData.equipHat].forcePercent + SaveScript.hats[SaveScript.saveData.equipHat].reinforce_basic * plus01);
            float percent = Mathf.Round(plus02 * 100f) / 100f;
            if (percent >= 1f)
                armor = (long)(armor * GameFuction.GetReinforcePercent_Over2(1));

            infoTexts[2].text = SaveScript.hats[SaveScript.saveData.equipHat].name + " (" + SaveScript.qualityNames_kr[quality] + ")";
            infoTexts[3].text = "- 모든 몬스터의 공격에 대해 데미지 감소";
            if (percent < 1f)
                infoTexts[3].text += "\n- 몬스터에 대한 방어력 < <color=#AA1212>" + GameFuction.GetNumText(armor) + " <color=black>>, 특수 회피 확률 < <color=#AA1212>" + (percent * 100) + "% <color=black>>";
            else
                infoTexts[3].text += "\n- 몬스터에 대한 방어력(+특수 회피) < <color=#AA1212>" + GameFuction.GetNumText(armor) + " <color=black>>, 특수 회피력 < <color=#AA1212>"
                    + (Mathf.Round(GameFuction.GetReinforcePercent_Over2(1) * 100f) / 100f * 100) + "% <color=black>>";
        }

        if (SaveScript.saveData.equipRing != -1)
        {
            int quality = GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[SaveScript.saveData.equipRing]);
            float plus01 = SaveScript.stat.ring01;
            float plus02 = SaveScript.stat.ring02;

            float amount = Mathf.Round((SaveScript.rings[SaveScript.saveData.equipRing].forcePercent + SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * plus01) * 100f) / 100f;
            float percent = Mathf.Round(plus02 * 100f) / 100f;
            if (percent >= 1f)
                amount *= GameFuction.GetReinforcePercent_Over2(2);

            infoTexts[4].text = SaveScript.rings[SaveScript.saveData.equipRing].name + " (" + SaveScript.qualityNames_kr[quality] + ")";
            infoTexts[5].text = "- '" + SaveScript.qualityNames_kr[quality] + "<color=black>' 등급까지 추가 판매 가능 / '" + SaveScript.qualityNames_kr[quality] + "<color=black>' 등급까지 특수 판매 발동";
            if (percent < 1f)
                infoTexts[5].text += "\n- 상점 추가 판매율 < <color=#AA1212>" + GameFuction.GetNumText((long)(Mathf.Round(amount * 100f * 100f) / 100f)) + "% <color=black>>, " + "특수 판매 확률 < <color=#AA1212>" + (percent * 100) + "% <color=black>>";
            else
                infoTexts[5].text += "\n- 상점 추가 판매율(+특수 판매) < <color=#AA1212>" + GameFuction.GetNumText((long)(Mathf.Round(amount * 100f * 100f) / 100f)) + "% <color=black>>, "
                    + "특수 판매력 < <color=#AA1212>" + (Mathf.Round(GameFuction.GetReinforcePercent_Over2(2) * 100f) / 100f * 100) + "% <color=black>>";
        }

        if (SaveScript.saveData.equipPendant != -1)
        {
            int quality = GameFuction.GetQualityOfEquipment(SaveScript.saveData.pendantReinforces[SaveScript.saveData.equipPendant]);
            float plus01 = SaveScript.stat.pendant01;
            float plus02 = SaveScript.stat.pendant02;
            long min, max = Mathf.RoundToInt(SaveScript.pendants[SaveScript.saveData.equipPendant].forcePercent 
                + SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * plus01);
            float percent = Mathf.Round(plus02 * 100f) / 100f;
            string num;

            if (percent < 1f) min = 1;
            else min = Mathf.RoundToInt(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * GameFuction.GetReinforcePercent_Over2(3));
            if (min >= max) num = GameFuction.GetNumText(max) + "개";
            else num = GameFuction.GetNumText(min) + "개 ~ " + GameFuction.GetNumText(max) + "개"; 

            infoTexts[6].text = SaveScript.pendants[SaveScript.saveData.equipPendant].name + " (" + SaveScript.qualityNames_kr[quality] + ")";
            infoTexts[7].text = "- '" + SaveScript.qualityNames_kr[quality] + "' 등급까지 추가 광물 획득 가능";
            if (quality != 6) infoTexts[7].text += " / '미스틱' 등급은 확정 발동"; ;
            if (percent < 1f)
                infoTexts[7].text += "\n- 하나의 광석에서 광물 < <color=#AA1212>" + num + " <color=black>> 획득 가능, " + "발동 확률 < <color=#AA1212>" + (percent * 100) + "% <color=black>>";
            else
                infoTexts[7].text += "\n- 하나의 광석에서 광물 < <color=#AA1212>" + num + " <color=black>> 획득 가능";
        }

        if (SaveScript.saveData.equipSword != -1)
        {
            int quality = GameFuction.GetQualityOfEquipment(SaveScript.saveData.swordReinforces[SaveScript.saveData.equipSword]);
            float plus01 = SaveScript.stat.sword01;
            float plus02 = SaveScript.stat.sword02;

            long damage = (long)Mathf.Round(SaveScript.swords[SaveScript.saveData.equipSword].forcePercent + SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * plus01);
            float percent = Mathf.Round(plus02 * 100f) / 100f;
            if (percent >= 1f)
                damage = (long)(damage * GameFuction.GetReinforcePercent_Over2(4));

            infoTexts[8].text = SaveScript.swords[SaveScript.saveData.equipSword].name + " (" + SaveScript.qualityNames_kr[quality] + ")";
            infoTexts[9].text = "- 모든 몬스터에 대해 공격";
            if (percent < 1f)
                infoTexts[9].text += "\n- 몬스터에게 입히는 피해량 < <color=#AA1212>" + GameFuction.GetNumText(damage) + " <color=black>>, 크리티컬 확률 < <color=#AA1212>" + (percent * 100) + "% <color=black>>";
            else
                infoTexts[9].text += "\n- 몬스터에게 입히는 피해량(+ 크리티컬) < <color=#AA1212>" + GameFuction.GetNumText(damage) + " <color=black>>, 크리티컬 데미지 < <color=#AA1212>"
                    + (Mathf.Round(GameFuction.GetReinforcePercent_Over2(4) * 100f) / 100f * 100) + "% <color=black>>";
        }
    }

    // 애니메이션 초기화
    public void SetIdle()
    {
        animator.SetInteger("R_PageType", -1);
        animator.SetInteger("PageType", -1);
    }

    // 슬롯 변경 버튼
    public void SlotButton()
    {
        SlotClear();
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
        {
            if (slotIndex != EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order)
            {
                isSlotOn = false;
                mainSelectPage = 0;
            }
                
            slotIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        }

        if (!isSlotOn)
        {
            SetAudio(0);
            isSlotOn = true;
            for (int i = 0; i < slotImages.Length; i++)
                slotImages[i].color = new Color(1f, 1f, 1f, 1f);
            slotImages[slotIndex].color = new Color(1f, 1f, 1f, 0.3f);
            mainSelectPageObject.SetActive(true);
            MainSelectSetting();

            int slotOrder = 0;
            int equipNum = 0;
            bool[] isDestroyed, hasEquips;
            int[] reinforces;
            Sprite destoryedSprite;

            switch (slotIndex)
            {
                case 0:
                    equipNum = SaveScript.pickNum;
                    isDestroyed = SaveScript.saveData.isPickReinforceDestroy;
                    hasEquips = SaveScript.saveData.hasPicks;
                    reinforces = SaveScript.saveData.pickReinforces;
                    destoryedSprite = Pick.destroyedSprite;
                    TakeOffButton.gameObject.SetActive(false);
                    break;
                case 1:
                    equipNum = SaveScript.hatNum;
                    isDestroyed = SaveScript.saveData.isHatReinforceDestroy;
                    hasEquips = SaveScript.saveData.hasHats;
                    reinforces = SaveScript.saveData.hatReinforces;
                    destoryedSprite = Hat.destroyedSprite;
                    TakeOffButton.gameObject.SetActive(true);
                    break;
                case 2:
                    equipNum = SaveScript.RingNum;
                    isDestroyed = SaveScript.saveData.isRingReinforceDestroy;
                    hasEquips = SaveScript.saveData.hasRings;
                    reinforces = SaveScript.saveData.ringReinforces;
                    destoryedSprite = Ring.destroyedSprite;
                    TakeOffButton.gameObject.SetActive(true);
                    break;
                case 3:
                    equipNum = SaveScript.PendantNum;
                    isDestroyed = SaveScript.saveData.isPendantReinforceDestroy;
                    hasEquips = SaveScript.saveData.hasPenants;
                    reinforces = SaveScript.saveData.pendantReinforces;
                    destoryedSprite = Pendant.destroyedSprite;
                    TakeOffButton.gameObject.SetActive(true);
                    break;
                default:
                    equipNum = SaveScript.swordNum;
                    isDestroyed = SaveScript.saveData.isSwordReinforceDestroy;
                    hasEquips = SaveScript.saveData.hasSwords;
                    reinforces = SaveScript.saveData.swordReinforces;
                    destoryedSprite = Sword.destroyedSprite;
                    TakeOffButton.gameObject.SetActive(false);
                    break;
            }

            for (int i = 0; i < selectImages.Length; i++)
            {
                slotOrder = mainSelectPage * 5 + i;
                if (slotOrder >= SaveScript.pickNum) return;

                if (!isDestroyed[slotOrder])
                {
                    switch (slotIndex)
                    {
                        case 0: selectImages[i].sprite = SaveScript.picks[slotOrder].sprites[0]; break;
                        case 1: selectImages[i].sprite = SaveScript.hats[slotOrder].sprite; break;
                        case 2: selectImages[i].sprite = SaveScript.rings[slotOrder].sprite; break;
                        case 3: selectImages[i].sprite = SaveScript.pendants[slotOrder].sprite; break;
                        default: selectImages[i].sprite = SaveScript.swords[slotOrder].sprite; break;
                    }

                    if (hasEquips[slotOrder])
                    {
                        select_QualityImages[i].color = GameFuction.GetColorEquipment(reinforces, slotOrder);
                        selectImages[i].color = new Color(1f, 1f, 1f, 1f);
                        selectButtons[i].enabled = true;
                    }
                    else
                    {
                        selectImages[i].color = new Color(1f, 1f, 1f, 0.3f);
                    }
                }
                else
                {
                    selectImages[i].sprite = destoryedSprite;
                    select_DestroyedImages[i].gameObject.SetActive(true);
                    selectImages[i].color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
        else
        {
            mainSelectPage = 0;
        }
    }

    // 슬롯 - 도구 선택 버튼
    public void SelectButton() 
    {
        SetAudio(0);
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            slotListIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

        switch (slotIndex)
        {
            case 0: if(SaveScript.saveData.hasPicks[mainSelectPage * 5 + slotListIndex]) SaveScript.saveData.equipPick = mainSelectPage * 5 + slotListIndex; break;
            case 1: if (SaveScript.saveData.hasHats[mainSelectPage * 5 + slotListIndex]) SaveScript.saveData.equipHat = mainSelectPage * 5 + slotListIndex; break;
            case 2: if (SaveScript.saveData.hasRings[mainSelectPage * 5 + slotListIndex]) SaveScript.saveData.equipRing = mainSelectPage * 5 + slotListIndex; break;
            case 3: if (SaveScript.saveData.hasPenants[mainSelectPage * 5 + slotListIndex]) SaveScript.saveData.equipPendant = mainSelectPage * 5 + slotListIndex; break;
            case 4: if (SaveScript.saveData.hasSwords[mainSelectPage * 5 + slotListIndex]) SaveScript.saveData.equipSword = mainSelectPage * 5 + slotListIndex; break;
        }

        mainSelectPage = 0;
        SlotClear();
        SetBasicInfo();
    }

    // 로비 슬롯 잠금
    public void SlotClear()
    {
        TakeOffButton.gameObject.SetActive(false);
        mainSelectPageObject.SetActive(false);
        isSlotOn = false;

        for (int i = 0; i < slotImages.Length; i++)
            slotImages[i].color = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < selectImages.Length; i++)
        {
            selectImages[i].sprite = null;
            selectImages[i].color = new Color(1f, 1f, 1f, 0f);
            selectButtons[i].enabled = false;
            select_DestroyedImages[i].gameObject.SetActive(false);
            select_QualityImages[i].color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // 장비 해제
    public void TakeOff()
    {
        switch (slotIndex)
        {
            case 0: SaveScript.saveData.equipPick = -1; break;
            case 1: SaveScript.saveData.equipHat = -1; break;
            case 2: SaveScript.saveData.equipRing = -1; break;
            case 3: SaveScript.saveData.equipPendant = -1; break;
            case 4: SaveScript.saveData.equipSword = -1; break;
        }

        mainSelectPage = 0;
        SlotClear();
        SetBasicInfo();
    }

    public void OnClickCashEquipment()
    {
        UIBox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        if (UIBox == null)
            return;

        cashEquipment_index = UIBox.order;
        SetSystemSubInfo(UIBox.GetComponent<RectTransform>().anchoredPosition);
    }

    private void SetSystemSubInfo(Vector2 pos)
    {
        systemSubInfo.gameObject.SetActive(true);
        systemSubInfo.GetComponent<RectTransform>().anchoredPosition = pos + cashEquipment_subInfoVec;
        if (SaveScript.saveData.hasCashEquipment[cashEquipment_index])
        {
            systemSubInfo.texts[0].text = "[ " + SaveScript.cashEquipments[cashEquipment_index].name + " ] (획득)";
            systemSubInfo.texts[1].text = SaveScript.cashEquipments[cashEquipment_index].info;
        }
        else
        {
            systemSubInfo.texts[0].text = "[ " + SaveScript.cashEquipments[cashEquipment_index].name + " ] (미획득)";
            systemSubInfo.texts[1].text = SaveScript.cashEquipments[cashEquipment_index].info + "\n< 캐시 아이템 - 랜덤 박스 > 에서 획득 가능";
        }
    }

    public void R_ReinforceResult()
    {
        ReinforceUpgradeUI.instance.R_ReinforceResult();
    }

    public void R_ReinforceSetAni()
    {
        ReinforceUpgradeUI.instance.SetReinforceAni();
    }
}