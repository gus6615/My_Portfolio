using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemFusion : MonoBehaviour
{
    public static ItemFusion instance;

    public new AudioSource audio;
    public Animator animator;
    public GameObject fusionObject;
    public Text manaText;
    public GameObject itemPrefab;
    public GameObject fusionInfoBox;
    public GameObject fusionPanel;
    public GameObject fusionEmptyText;
    public Text fusionTitleText;
    public Text fusionInfoText;
    public Text fusionNumText;
    public Text fusionManaText;
    public Slider fusionSlider;
    public Image[] menuButtons;
    public Image leftItemImage, rightItemImage, resultImage;
    public Button fusionButton;
    public Sprite bufItem_resultSprite, reinforceItem_resultSprite;

    public int menuIndex;
    public int itemCode; // BufItem Index (0 ~ 42)
    public int itemIndex, itemType; // BufItem Code(0 ~ 14), BufItem Type(0 ~ 2)
    public int count;
    public int manaPrice;
    private bool isOnOffFusion;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        fusionObject.SetActive(false);
        fusionInfoBox.SetActive(false);
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    private int GetPrice()
    {
        if (menuIndex == 0)
        {
            return count * (itemType + 1);
        }
        else
        {
            return count * (GetReinforceType(itemCode) + 1);
        }
    }

    public int GetReinforceType(int _code)
    {
        int type = 0;

        if (_code == 10) type = 7;
        else if (_code == 9) type = 6;
        else if (_code == 8) type = 5;
        else if (_code == 7) type = 4;
        else if (_code == 6) type = 3;
        else if (_code == 2 || _code == 5) type = 2;
        else if (_code == 1) type = 1;
        else type = 0;

        return type;
    }

    // 인벤토리 열기
    public void OnOffFusion()
    {
        isOnOffFusion = !isOnOffFusion;
        fusionObject.SetActive(isOnOffFusion);
        SetAudio(0);

        if (isOnOffFusion)
        {
            menuIndex = 0;
            SettingFusion();
            if (Inventory.instance != null)
                Inventory.instance.SlotClear();
        }
    }
    
    public void ClickMenu()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
        {
            menuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
            SetAudio(0);
        }

        if(menuIndex == 0)
        {
            menuButtons[0].color = new Color(0.8f, 0.8f, 0.8f);
            menuButtons[1].color = new Color(1f, 1f, 1f);
        }
        else
        {
            menuButtons[0].color = new Color(1f, 1f, 1f);
            menuButtons[1].color = new Color(0.8f, 0.8f, 0.8f);
        }
        SettingFusion();
    }

    // 아이템 창 정보 갱신
    public void SettingFusion()
    {
        bool isEmpty = true;

        Order[] datas = fusionPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);

        if (menuIndex == 0)
        {
            for (int i = 0; i < SaveScript.bufItemNum; i++)
            {
                if (SaveScript.saveData.hasBufItems[i] != 0)
                {
                    Order data = Instantiate(itemPrefab, fusionPanel.transform).GetComponent<Order>();
                    data.GetComponent<Button>().onClick.AddListener(SelectItem);
                    data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.bufItems[i].sprite;
                    data.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasBufItems[i]);
                    data.order = i;

                    if (isEmpty)
                        isEmpty = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++) // 제련석 까지만 가능
            {
                if (SaveScript.saveData.hasReinforceItems2[i] != 0)
                {
                    Order data = Instantiate(itemPrefab, fusionPanel.transform).GetComponent<Order>();
                    data.GetComponent<Button>().onClick.AddListener(SelectItem);
                    data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems2[i].sprite;
                    data.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems2[i]);
                    data.order = SaveScript.reinforceItemNum + i;

                    if (isEmpty)
                        isEmpty = false;
                }
            }

            for (int i = 0; i < SaveScript.reinforceItemNum; i++)
            {
                if (SaveScript.saveData.hasReinforceItems[i] != 0)
                {
                    Order data = Instantiate(itemPrefab, fusionPanel.transform).GetComponent<Order>();
                    data.GetComponent<Button>().onClick.AddListener(SelectItem);
                    data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.reinforceItems[i].sprite;
                    data.GetComponentInChildren<Text>().text = "x" + GameFuction.GetNumText(SaveScript.saveData.hasReinforceItems[i]);
                    data.order = i;

                    if (isEmpty)
                        isEmpty = false;
                }
            }
        }

        manaText.text = GameFuction.GetNumText(SaveScript.saveData.manaOre);
        fusionEmptyText.SetActive(isEmpty);
        fusionInfoBox.SetActive(false);
    }

    // 아이템 선택
    public void SelectItem()
    {
        Order data = EventSystem.current.currentSelectedGameObject.GetComponent<Order>();
        itemCode = data.order;
        itemIndex = itemCode / SaveScript.bufItemTypeNum;
        itemType = itemCode % SaveScript.bufItemTypeNum;
        fusionInfoBox.SetActive(true);
        SettingInfo();
        SetAudio(0);
    }

    public void ChangedSliderValue()
    {
        fusionNumText.text = GameFuction.GetNumText((long)fusionSlider.value);
        count = (int)fusionSlider.value / 2;
        manaPrice = GetPrice();
        fusionManaText.text = GameFuction.GetNumText(manaPrice);
        if (manaPrice <= SaveScript.saveData.manaOre)
            fusionManaText.color = Color.green;
        else
            fusionManaText.color = Color.red;
    }

    public void SettingInfo()
    {
        if (menuIndex == 0)
        {
            fusionTitleText.text = "물약 합성 - [ " + SaveScript.bufItems[itemCode].name + " ]";
            leftItemImage.sprite = rightItemImage.sprite = SaveScript.bufItems[itemCode].sprite;
            resultImage.sprite = bufItem_resultSprite;
            fusionSlider.maxValue = SaveScript.saveData.hasBufItems[itemCode];

            if (SaveScript.saveData.hasBufItems[itemCode] == 1)
            {
                rightItemImage.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
                fusionInfoText.text = "※ 합성을 하기 위해선 최소 2개 이상의 물약이 필요합니다.";
                fusionSlider.value = 0;
            }
            else
            {
                rightItemImage.color = Color.white;
                fusionInfoText.text = "※ 합성할 물약의 개수를 결정해주세요.";
                fusionSlider.value = 2;
            }
        }
        else
        {
            if (itemCode < SaveScript.reinforceItemNum)
            {
                fusionTitleText.text = "주문서 합성 - [ " + SaveScript.reinforceItems[itemCode].name + " ]";
                leftItemImage.sprite = rightItemImage.sprite = SaveScript.reinforceItems[itemCode].sprite;
                resultImage.sprite = reinforceItem_resultSprite;
                fusionSlider.maxValue = SaveScript.saveData.hasReinforceItems[itemCode];

                if (SaveScript.saveData.hasReinforceItems[itemCode] == 1)
                {
                    rightItemImage.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
                    fusionInfoText.text = "※ 합성을 하기 위해선 최소 2개 이상의 주문서가 필요합니다.";
                    fusionSlider.value = 0;
                }
                else
                {
                    rightItemImage.color = Color.white;
                    fusionInfoText.text = "※ 합성할 주문서의 개수를 결정해주세요.";
                    fusionSlider.value = 2;
                }
            }
            else
            {
                fusionTitleText.text = "주문서 합성 - [ " + SaveScript.reinforceItems2[itemCode - SaveScript.reinforceItemNum].name + " ]";
                leftItemImage.sprite = rightItemImage.sprite = SaveScript.reinforceItems2[itemCode - SaveScript.reinforceItemNum].sprite;
                resultImage.sprite = reinforceItem_resultSprite;
                fusionSlider.maxValue = SaveScript.saveData.hasReinforceItems2[itemCode - SaveScript.reinforceItemNum];

                if (SaveScript.saveData.hasReinforceItems2[itemCode - SaveScript.reinforceItemNum] == 1)
                {
                    rightItemImage.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
                    fusionInfoText.text = "※ 합성을 하기 위해선 최소 2개 이상의 주문서가 필요합니다.";
                    fusionSlider.value = 0;
                }
                else
                {
                    rightItemImage.color = Color.white;
                    fusionInfoText.text = "※ 합성할 주문서의 개수를 결정해주세요.";
                    fusionSlider.value = 2;
                }
            }
        }

        if (fusionSlider.maxValue > 10000)
            fusionSlider.maxValue = 10000;
        ChangedSliderValue();
    }

    public void FusionButton()
    {
        if ((menuIndex == 0 && SaveScript.saveData.hasBufItems[itemCode] == 1) || 
            (menuIndex == 1 && itemCode < SaveScript.reinforceItemNum && SaveScript.saveData.hasReinforceItems[itemCode] == 1) ||
            (menuIndex == 1 && itemCode >= SaveScript.reinforceItemNum && SaveScript.saveData.hasReinforceItems2[itemCode - SaveScript.reinforceItemNum] == 1)
            || manaPrice > SaveScript.saveData.manaOre || count < 1)
        {
            SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("합성 재료가 부족합니다.");
        }
        else
        {
            SetAudio(0);
            Invoke("SetPassPanel", 0.5f);
            animator.gameObject.SetActive(true);
            animator.SetBool("isStart", true);
            animator.Play("Fusion_Start", -1, 0f);
            if (menuIndex == 0)
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 47 }, SaveScript.saveData.mainQuest_goal + count);
            else
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 48 }, SaveScript.saveData.mainQuest_goal + count);
            ItemFusion_Ani.instance.result_object.SetActive(false);
            ItemFusion_Ani.instance.SetResult();
            ItemFusion_Ani.instance.SetAni();
        }
    }

    private void SetPassPanel()
    {
        ItemFusion_Ani.instance.passClickPanel.SetActive(true);
    }
}
