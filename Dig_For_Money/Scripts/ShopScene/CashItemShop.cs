using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CashItem
{
    static private string[] names =
    {
        "일반 광물 상자", "일반 아이템 상자", "일반 펫 알", "일반 강화석 캡슐", "일반 마나석 캡슐", "일반 랜덤 박스",
        "고급 광물 상자", "고급 아이템 상자", "고급 펫 알", "고급 강화석 캡슐", "고급 마나석 캡슐", "고급 랜덤 박스",
        "전설 광물 상자", "전설 아이템 상자", "전설 펫 알", "전설 강화석 캡슐", "전설 마나석 캡슐", "전설 랜덤 박스",
    };
    static private string[] infos =
    {
        "평범한 광부들 사이에서 거래되는 광물 상자이다.\n\n",
        "일반 던전에서 발견되는 흔한 아이템 상자이다.\n\n",
        "일반적으로 구매할 수 있는 펫 알이다.\n\n",
        "일반 던전 코어에서 등장하는 강화석 캡슐이다.\n\n",
        "일반 던전 코어에서 등장하는 마나석 캡슐이다.\n\n",
        "어떤 물건이 있을 지 모르는 일반 랜덤 박스이다.\n\n",

        "전설급 광부들 사이에서 거래되는 광물 상자이다.\n\n",
        "유적 던전에서 발견되는 고급 아이템 상자이다.\n\n",
        "구하기 힘든 희귀한 펫 알이다.\n\n",
        "유적 던전 코어에서 등장하는 강화석 캡슐이다.\n\n",
        "유적 던전 코어에서 등장하는 마나석 캡슐이다.\n\n",
        "어떤 물건이 있을 지 모르는 고급 랜덤 박스이다.\n\n",

        "신화급 광부들 사이에서 거래되는 광물 상자이다.\n\n",
        "고대 던전에서 발견되는 희귀 아이템 상자이다.\n\n",
        "매우 구하기 힘든 스페셜 펫 알이다.\n\n",
        "고대 던전 코어에서 등장하는 강화석 캡슐이다.\n\n",
        "고대 던전 코어에서 등장하는 마나석 캡슐이다.\n\n",
        "어떤 물건이 있을 지 모르는 전설 랜덤 박스이다.\n\n",
    };
    static private string[] infos2 =
    {
        " 획득할 수 있다.",
        "< 초급 ~ 고급 물약 > 5개 ~ 10개\n< 하급 ~ 고급 주문서 > 5개 ~ 10개",
        "< C ~ SSS 등급 펫 > 1개",
        "",
        "",
        "",

        " 획득할 수 있다. 확률적으로 < 얼티밋 > 광물이 소량 들어있다.",
        "< 고급 ~ 전설의 물약 > 10개 ~ 20개\n< 고급 주문서 ~ 고대 유물 > 5개 ~ 10개",
        "< A ~ UU 등급 펫 > 1개",
        "",
        "",
        "",

        " 획득할 수 있다. 확률적으로 < 미스틱 > 광물이 소량 들어있다.",
        "< 전설의 물약 > 20개 ~ 30개\n< 제련석 ~ 고대 유물 > 5개 ~ 10개",
        "< S ~ M 등급 펫 > 1개",
        "",
        "",
        ""
    };
    static private int[] prices = 
    { 
        15, 25, 30, 20, 25, 15,
        30, 50, 60, 40, 50, 30,
        60, 100, 120, 80, 100, 60,
    };

    public List<string> cashInfo_names = new List<string>();
    public List<float> cashInfo_percents = new List<float>();
    public Sprite sprite;
    public int itemCode;
    public string name, info;
    public int price;

    public CashItem(int _itemCode)
    {
        itemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Shop/CashItems")[itemCode];
    }

    public void SetData()
    {
        name = names[itemCode];
        price = prices[itemCode];
        SetInfo();
        SetCashList();
    }

    private void SetInfo()
    {
        string str = "";
        int type = itemCode / SaveScript.cashItemNums[0];
        int code = itemCode % SaveScript.cashItemNums[0];
        switch (code)
        {
            case 0:
                str = "< " + (SaveScript.saveData.pickLevel + 1) + " 층 > 에서 나오는 모든 일반 광물을 < " + GameFuction.GetNumText(CashItemAnimator.oreMinNums[SaveScript.saveData.pickLevel] * (long)CashItemAnimator.orePlusPercents[type])
                    + " 개 ~ " + GameFuction.GetNumText((long)(10 * CashItemAnimator.oreMinNums[SaveScript.saveData.pickLevel] * CashItemAnimator.orePlusPercents[type])) + " 개 > ";
                break;
            case 3:
                str = "강화석 '" + GameFuction.GetNumText(CashItemAnimator.R_capsuleForces[type][0]) + "'개, '" + GameFuction.GetNumText(CashItemAnimator.R_capsuleForces[type][1]) + "'개, '"
                    + GameFuction.GetNumText(CashItemAnimator.R_capsuleForces[type][2]) + "'개, '" + GameFuction.GetNumText(CashItemAnimator.R_capsuleForces[type][3]) + "'개 중 랜덤 획득";
                break;
            case 4:
                str = "마나석 '" + GameFuction.GetNumText(CashItemAnimator.M_capsuleForces[type][0]) + "'개, '" + GameFuction.GetNumText(CashItemAnimator.M_capsuleForces[type][1]) + "'개, '"
                    + GameFuction.GetNumText(CashItemAnimator.M_capsuleForces[type][2]) + "'개, '" + GameFuction.GetNumText(CashItemAnimator.M_capsuleForces[type][3]) + "'개 중 랜덤 획득";
                break;
            case 5:
                str = "< 특수 아이템 획득 확률 : " + Mathf.Round(CashItemAnimator.randomBoxPercents[itemCode / SaveScript.cashItemNums[0]][0] * 100f * 100f) / 100f + " % >";
                break;
        }
        info = infos[itemCode] + str + infos2[itemCode];
    }

    private void SetCashList()
    {
        int type = itemCode / SaveScript.cashItemNums[0];
        int code = itemCode % SaveScript.cashItemNums[0];
        cashInfo_names.Clear();
        cashInfo_percents.Clear();
        switch (code)
        {
            case 0: // 광물 상자
                cashInfo_names.Add("기본 광물");
                cashInfo_percents.Add(1f);
                if (type > 0)
                {
                    cashInfo_names.Add("얼티밋 광물");
                    cashInfo_percents.Add(CashItemAnimator.ultimateOrePercents[type]);
                }
                if (type > 1)
                {
                    cashInfo_names.Add("미스틱 광물");
                    cashInfo_percents.Add(CashItemAnimator.mysticOrePercents[type]);
                }
                break;
            case 1: // 아이템 상자
                cashInfo_names.Add("< 물약 >");
                cashInfo_percents.Add(-1f);
                switch (type)
                {
                    case 0:
                        cashInfo_names.Add("초급 물약");
                        cashInfo_names.Add("중급 물약");
                        cashInfo_names.Add("고급 물약");
                        for (int i = 0; i < 3; i++)
                            cashInfo_percents.Add(BufItem.cashPercents[type][i]);

                        cashInfo_names.Add("< 강화 >");
                        cashInfo_names.Add("초급 주문서");
                        cashInfo_names.Add("중급 주문서");
                        cashInfo_names.Add("고급 주문서");
                        cashInfo_percents.Add(-1f);
                        for (int i = 0; i < 3; i++)
                            cashInfo_percents.Add(ReinforceItem.GetCashPercentAsType(type, i));
                        break;
                    case 1:
                        cashInfo_names.Add("고급 물약");
                        cashInfo_names.Add("전설의 영약");
                        cashInfo_percents.Add(1f - CashItemAnimator.elixirPercent[type]);
                        cashInfo_percents.Add(CashItemAnimator.elixirPercent[type]);

                        cashInfo_names.Add("< 강화 >");
                        cashInfo_names.Add("고급 주문서");
                        cashInfo_percents.Add(-1f);
                        cashInfo_percents.Add(1f - CashItemAnimator.reinforce2Percent[type]);
                        for (int i = 0; i < SaveScript.reinforceItem2Num; i++)
                        {
                            cashInfo_names.Add(SaveScript.reinforceItems2[i].name);
                            cashInfo_percents.Add(CashItemAnimator.reinforce2Percent[type] * ReinforceItem2.cashPercents[type][i]);
                        }
                        break;
                    case 2:
                        cashInfo_names.Add("전설의 영약");
                        cashInfo_percents.Add(1f);
                        cashInfo_names.Add("< 강화 >");
                        cashInfo_percents.Add(-1f);
                        for (int i = 0; i < SaveScript.reinforceItem2Num; i++)
                        {
                            cashInfo_names.Add(SaveScript.reinforceItems2[i].name);
                            cashInfo_percents.Add(ReinforceItem2.cashPercents[type][i]);
                        }
                        break;
                }
                break;
            case 2: // 펫 알
                for (int i = 0; i < SaveScript.mineSlimeQualityNum; i++)
                {
                    cashInfo_names.Add(MineSlime.qualityNames[i]);
                    cashInfo_percents.Add(MineSlime.cashPercents[type][i]);
                }
                break;
            case 3: // 강화석 캡슐
                for (int i = 0; i < 4; i++)
                {
                    cashInfo_names.Add("강화석 " + GameFuction.GetNumText(CashItemAnimator.R_capsuleForces[type][i]) + " 개");
                    cashInfo_percents.Add(CashItemAnimator.capsulePercent[i]);
                }
                break;
            case 4: // 마나석 캡슐
                for (int i = 0; i < 4; i++)
                {
                    cashInfo_names.Add("마나석 " + GameFuction.GetNumText(CashItemAnimator.M_capsuleForces[type][i]) + " 개");
                    cashInfo_percents.Add(CashItemAnimator.capsulePercent[i]);
                }
                break;
            case 5: // 랜덤 박스
                cashInfo_names.Add("특수 아이템");
                cashInfo_percents.Add(CashItemAnimator.randomBoxPercents[type][0]);
                for (int i = 0; i < CashItemAnimator.randomBox_capsurePercent.Length; i++)
                {
                    cashInfo_names.Add("강화석 " + GameFuction.GetNumText(CashItemAnimator.randomBox_reinforceNums[type][i]) + " 개");
                    cashInfo_percents.Add(CashItemAnimator.randomBoxPercents[type][1] * CashItemAnimator.randomBox_capsurePercent[i]);
                }
                for (int i = 0; i < CashItemAnimator.randomBox_capsurePercent.Length; i++)
                {
                    cashInfo_names.Add("마나석 " + GameFuction.GetNumText(CashItemAnimator.randomBox_manaNums[type][i]) + " 개");
                    cashInfo_percents.Add(CashItemAnimator.randomBoxPercents[type][2] * CashItemAnimator.randomBox_capsurePercent[i]);
                }
                cashInfo_names.Add("초급 물약");
                cashInfo_names.Add("중급 물약");
                cashInfo_names.Add("고급 물약");
                cashInfo_names.Add("전설의 영약");
                for (int i = 0; i < CashItemAnimator.randomBox_bufPercent[type].Length; i++)
                    cashInfo_percents.Add(CashItemAnimator.randomBoxPercents[type][3] * CashItemAnimator.randomBox_bufPercent[type][i]);
                cashInfo_names.Add("강화 주문서");
                cashInfo_names.Add("제련석");
                cashInfo_names.Add("고대 유물");
                for (int i = 0; i < CashItemAnimator.randomBox_reinforcePercent[type].Length; i++)
                    cashInfo_percents.Add(CashItemAnimator.randomBoxPercents[type][4] * CashItemAnimator.randomBox_reinforcePercent[type][i]);
                break;
        }
    }

    static public int GetItemCode(int _type, int _index)
    {
        int code = _index;
        if (_type > 0) code += SaveScript.cashItemNums[0];
        if (_type > 1) code += SaveScript.cashItemNums[1];
        return code;
    }
};

public class CashItemShop : MonoBehaviour
{
    private const int SPRITE_PIXEL_CONTENT = 90;
    public const float DC_PERCENT = 0.1f;
    public const int PACKAGE_NUM = 10;

    static public CashItemShop instance;
    static public int[] cashes = { 150, 600, 1500, 3500 };

    public GameObject cashInfoPrefab;
    public Transform cashInfoPanel;
    public GameObject cashItemPrefab;
    public Transform cashItemPanel;
    public UIBox[] menuButtons;
    public GameObject selectContent;
    public UIBox buySlot, buyAllSlot;
    public GameObject reconfirmContent;
    public GameObject animationContent; // 애니메이션 총괄 오브젝트
    public GameObject[] animObjects; // 0 = 광고 제거, 1 = 그 외 캐시 아이템
    public GameObject removeADSelect;
    public Sprite removeADSprite;
    public Image selectItemImage, selectCashImage;
    public Text selectName, selectInfo;
    public Image reconfirmImage;
    public Text reconfirmName;
    public Animator animator;
    public GameObject showButton;
    [SerializeField] private GameObject cashInfo;

    public int menuIndex;
    public int itemIndex;
    public bool isCanBuy;
    public bool isPackage;

    Sprite sprite;
    UIBox uibox;
    UIBox[] uIBoxes;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        selectContent.SetActive(false);
        reconfirmContent.SetActive(false);
        animationContent.SetActive(false);
        cashInfo.SetActive(false);
        for (int i = 0; i < animObjects.Length; i++)
            animObjects[i].SetActive(false);
        SetRemoveADSelect();
    }

    // 광고 제거 Selector 설정
    public void SetRemoveADSelect()
    {
        if (GoogleInApp.instance.HadPurchased_removeAD() || GoogleInApp.instance.HadPurchased_package())
            Destroy(removeADSelect);
    }

    private bool CheckCanBuy(int _menuIndex)
    {
        return GameFuction.GetPlayerGrade() >= _menuIndex;
    }

    public void OnMenuButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>() != null)
        {
            uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
            if (!CheckCanBuy(uibox.order))
            {
                Shop.instance.SetAudio(2);
                switch (uibox.order)
                {
                    case 1: SystemInfoCtrl.instance.SetErrorInfo("< 전설 > 광부 자격이 없습니다. 청록석 곡괭이(7층)에 도달하세요!"); break;
                    case 2: SystemInfoCtrl.instance.SetErrorInfo("< 신화 > 광부 자격이 없습니다. 연옥석 곡괭이(11층)에 도달하세요!"); break;
                }
                return;
            }

            menuIndex = uibox.order;
            Shop.instance.SetAudio(0);
        }

        for (int i = 0; i < menuButtons.Length; i++)
        {
            if ((i == 1 && GameFuction.GetPlayerGrade() < 1) || (i == 2 && GameFuction.GetPlayerGrade() < 2))
            {
                menuButtons[i].images[0].color = new Color(0.6f, 0.4f, 0.4f, 0.4f);
                menuButtons[i].texts[0].color = new Color(0.2f, 0.1f, 0.1f, 0.4f);
            }
            else
            {
                menuButtons[i].images[0].color = new Color(1f, 0.8f, 0.8f);
                menuButtons[i].texts[0].color = new Color(0.5f, 0.2f, 0.2f);
            }
        }
        menuButtons[menuIndex].images[0].color = new Color(1f, 0.6f, 0.6f);
        menuButtons[menuIndex].texts[0].color = new Color(0.5f, 0.2f, 0.2f);

        uIBoxes = cashItemPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uIBoxes.Length; i++)
            if (uIBoxes[i].order != -1)
                Destroy(uIBoxes[i].gameObject);
        selectContent.SetActive(false);
        cashInfo.SetActive(false);

        for (int i = 0; i < SaveScript.cashItemNums[menuIndex]; i++)
        {
            int code = CashItem.GetItemCode(menuIndex, i);
            sprite = SaveScript.cashItems[code].sprite;

            uibox = Instantiate(cashItemPrefab, cashItemPanel).GetComponent<UIBox>();
            uibox.images[0].sprite = sprite;
            uibox.images[0].rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y) * SPRITE_PIXEL_CONTENT;
            uibox.texts[0].text = SaveScript.cashItems[code].name;
            uibox.button.onClick.AddListener(OnItemButton);
            uibox.order = i;
        }
    }

    public void OnItemButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>() != null)
        {
            itemIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
            Shop.instance.SetAudio(0);
        }

        cashInfo.SetActive(false);
        selectContent.SetActive(true);
        SetBuySlot();
        if (itemIndex == -1)
        {
            showButton.SetActive(false);
            selectCashImage.gameObject.SetActive(false);
            selectItemImage.sprite = removeADSprite;
            selectItemImage.rectTransform.sizeDelta = new Vector2(removeADSprite.bounds.size.x, removeADSprite.bounds.size.y) * SPRITE_PIXEL_CONTENT;
            selectName.text = "[ 광고 제거 ]";
            selectInfo.text = "광고를 제거합니다.\n\n※ 게임 계정이 아닌, 안드로이드 구글 계정을 기준으로 적용";
        }
        else
        {
            int code = CashItem.GetItemCode(menuIndex, itemIndex);
            sprite = SaveScript.cashItems[code].sprite;

            showButton.SetActive(true);
            selectCashImage.gameObject.SetActive(true);
            selectItemImage.sprite = sprite;
            selectItemImage.rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y) * SPRITE_PIXEL_CONTENT;
            selectName.text = "[ " + SaveScript.cashItems[code].name + " ]";
            selectInfo.text = SaveScript.cashItems[code].info;
        }
    }

    public void BuyButton()
    {
        if (itemIndex == -1)
        {
            Shop.instance.SetAudio(0);
            reconfirmContent.SetActive(true);
            reconfirmImage.sprite = removeADSprite;
            reconfirmImage.rectTransform.sizeDelta = new Vector2(removeADSprite.bounds.size.x, removeADSprite.bounds.size.y) * SPRITE_PIXEL_CONTENT;
            reconfirmName.text = "[ 광고 제거 ]";
            return;
        }

        int code = CashItem.GetItemCode(menuIndex, itemIndex);
        if (SaveScript.saveData.cash >= SaveScript.cashItems[code].price)
        {
            sprite = SaveScript.cashItems[code].sprite;

            Shop.instance.SetAudio(0);
            isPackage = false;
            reconfirmContent.SetActive(true);
            reconfirmImage.sprite = sprite;
            reconfirmImage.rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y) * SPRITE_PIXEL_CONTENT;
            reconfirmName.text = "[ " + SaveScript.cashItems[code].name + " ]";
        }
        else
        {
            Shop.instance.SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("레드 다이아몬드가 부족합니다.");
        }
    }

    public void BuyAllButton()
    {
        int code = CashItem.GetItemCode(menuIndex, itemIndex);
        if (SaveScript.saveData.cash >= SaveScript.cashItems[code].price * PACKAGE_NUM * (1 - DC_PERCENT))
        {
            sprite = SaveScript.cashItems[code].sprite;

            Shop.instance.SetAudio(0);
            isPackage = true;
            reconfirmContent.SetActive(true);
            reconfirmImage.sprite = sprite;
            reconfirmImage.rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y) * SPRITE_PIXEL_CONTENT;
            reconfirmName.text = "[ " + SaveScript.cashItems[code].name + " ] x " + PACKAGE_NUM;
        }
        else
        {
            Shop.instance.SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("레드 다이아몬드가 부족합니다.");
        }
    }

    // 아이템을 구매할 것인지에 대한 버튼
    public void YesButton()
    {
        // 광고 제거 구매
        if (itemIndex == -1)
        {
            reconfirmContent.SetActive(false);
            GoogleInApp.instance.Purchase_RemoveAD();
            return;
        }

        // 캐시 체크
        int code = CashItem.GetItemCode(menuIndex, itemIndex);
        if (!isPackage)
        {
            if (itemIndex != -1 && SaveScript.saveData.cash < SaveScript.cashItems[code].price)
            {
                Shop.instance.SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("레드 다이아몬드가 부족합니다.");
                return;
            }
        }
        else
        {
            if (itemIndex != -1 && SaveScript.saveData.cash < SaveScript.cashItems[code].price * PACKAGE_NUM * (1 - DC_PERCENT))
            {
                Shop.instance.SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("레드 다이아몬드가 부족합니다.");
                return;
            }
        }

        // 펫에 자리가 없는 경우
        if (itemIndex == 2)
        {
            if (!isPackage && (AdventurerSlime.FindEmptyPetInven() == -1 || MinerSlime.FindEmptyPetInven() == -1))
            {
                Shop.instance.SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("펫 인벤토리를 하나 이상 비워주세요!");
                return;
            }
            else if (isPackage && (!AdventurerSlime.FindEmptyPetInven_Package() || !MinerSlime.FindEmptyPetInven_Package()))
            {
                Shop.instance.SetAudio(2);
                SystemInfoCtrl.instance.SetErrorInfo("펫 인벤토리를 하나 이상 비워주세요!");
                return;
            }
        }

        Shop.instance.SetAudio(0);
        animationContent.SetActive(true);
        reconfirmContent.SetActive(false);

        animObjects[1].SetActive(true);
        animator.SetInteger("itemType", -1);
        animator.Play("PackageOpen", -1, 0f);
        switch (itemIndex)
        {
            case 0: animator.speed = 1f / 3.5f; break;
            case 1: animator.speed = 1f / 4.5f; break;
            case 2: animator.speed = 1f / 5f; break;
            case 3:
            case 4: animator.speed = 1f / 4f; break;
            case 5: animator.speed = 1f / 3.5f; break;
        }
    }

    // 아이템 구매를 취소하는 버튼
    public void NoButton()
    {
        Shop.instance.SetAudio(0);
        reconfirmContent.SetActive(false);
    }

    // 아이템 구매에 성공 확인 UI를 닫는 버튼
    public void CloseButton()
    {
        Shop.instance.SetAudio(0);
        for (int i = 0; i < animObjects.Length; i++)
            animObjects[i].SetActive(false);
        SetBuySlot();
    }

    public void CloseRemoveAD()
    {
        Shop.instance.SetAudio(0);
        animObjects[0].SetActive(false);
        selectContent.SetActive(false);
        SetRemoveADSelect();
    }

    public void SetBuySlot()
    {
        Color color;

        if (itemIndex == -1)
        {
            buySlot.gameObject.SetActive(true);
            buyAllSlot.gameObject.SetActive(false);

            buySlot.texts[1].text = "₩ 3,300";
            for (int i = 0; i < buySlot.images.Length; i++)
            {
                color = buySlot.images[i].color;
                color.a = 1f;
                buySlot.images[i].color = color;
            }
            for (int i = 0; i < buySlot.texts.Length; i++)
            {
                color = buySlot.texts[i].color;
                color.a = 1f;
                buySlot.texts[i].color = color;
            }
        }
        else
        {
            int code = CashItem.GetItemCode(menuIndex, itemIndex);
            float alpha;

            buySlot.gameObject.SetActive(true);
            buyAllSlot.gameObject.SetActive(true);

            // 1회 구매 세팅
            if (SaveScript.saveData.cash >= SaveScript.cashItems[code].price)
                alpha = 1f;
            else
                alpha = 0.5f;

            buySlot.texts[1].text = SaveScript.cashItems[code].price.ToString();
            for (int i = 0; i < buySlot.images.Length; i++)
            {
                color = buySlot.images[i].color;
                color.a = alpha;
                buySlot.images[i].color = color;
            }
            for (int i = 0; i < buySlot.texts.Length; i++)
            {
                color = buySlot.texts[i].color;
                color.a = alpha;
                buySlot.texts[i].color = color;
            }

            // 10회 구매 세팅
            if (SaveScript.saveData.cash >= SaveScript.cashItems[code].price * PACKAGE_NUM * (1 - DC_PERCENT))
                alpha = 1f;
            else
                alpha = 0.5f;

            buyAllSlot.texts[1].text = (SaveScript.cashItems[code].price * PACKAGE_NUM * (1 - DC_PERCENT)).ToString();
            for (int i = 0; i < buyAllSlot.images.Length; i++)
            {
                color = buyAllSlot.images[i].color;
                color.a = alpha;
                buyAllSlot.images[i].color = color;
            }
            for (int i = 0; i < buyAllSlot.texts.Length; i++)
            {
                color = buyAllSlot.texts[i].color;
                color.a = alpha;
                buyAllSlot.texts[i].color = color;
            }
        }
    }

    public void ShowButton()
    {
        cashInfo.SetActive(!cashInfo.activeSelf);
        uIBoxes = cashInfoPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uIBoxes.Length; i++)
            Destroy(uIBoxes[i].gameObject);

        int code = CashItem.GetItemCode(menuIndex, itemIndex);
        int percentNum = SaveScript.cashItems[code].cashInfo_percents.Count;
        for (int i = 0; i < percentNum; i++)
        {
            if (SaveScript.cashItems[code].cashInfo_percents[i] == 0f)
                continue;

            uibox = Instantiate(cashInfoPrefab, cashInfoPanel).GetComponent<UIBox>();
            uibox.texts[0].text = SaveScript.cashItems[code].cashInfo_names[i];
            if (SaveScript.cashItems[code].cashInfo_percents[i] != -1f)
            {
                uibox.texts[1].text = Mathf.Round(SaveScript.cashItems[code].cashInfo_percents[i] * 100f * 10000f) / 10000f + " %";
                uibox.texts[0].color = uibox.texts[1].color = Color.white;
            }
            else
            {
                uibox.texts[1].text = "< -- 슬롯 단위 -- >";
                uibox.texts[0].color = uibox.texts[1].color = new Color(0.7f, 0.7f, 0.7f);
            }
        }
    }
}