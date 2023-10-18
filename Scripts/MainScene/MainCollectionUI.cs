using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainCollectionUI : MonoBehaviour
{
    public static MainCollectionUI instance;

    public Canvas CollectionObject;
    public Animator animator;
    public Transform menuPanel;
    public GameObject menuPrefab;
    public Transform contentPanel;
    public GameObject slotPrefab;
    public GameObject floorObject;
    public Text floorName;
    public Image card_Image, card_jem;
    public Text card_name, card_quality, card_level;
    public Text card_force, card_info, card_gold;
    public Slider card_slider;
    public Text card_sliderText;
    public Image[] contents;

    private bool isCollectionUIOn;
    private int menuIndex;

    // 임시 데이터들
    CollectionSlot[] slots;
    CollectionSlot slot;
    UIBox[] uiboxs;
    UIBox uibox;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        CollectionObject.gameObject.SetActive(false);
    }

    public void OnOffCollectionUI()
    {
        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        isCollectionUIOn = !isCollectionUIOn;
        CollectionObject.gameObject.SetActive(isCollectionUIOn);

        if (isCollectionUIOn)
        {
            menuIndex = 0;
            CreateMenu();
            SetMenuScale();
            SetContent();
            SetFloor();
        }
    }

    public void MenuButton()
    {
        uibox = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>();
        menuIndex = uibox.order;

        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);
        SetMenuScale();
        SetContent();
        SetFloor();
    }

    public void CreateMenu()
    {
        uiboxs = menuPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uiboxs.Length; i++)
            Destroy(uiboxs[i].gameObject);

        for (int i = 0; i < SaveScript.stageNum; i++)
        {
            uibox = Instantiate(menuPrefab, menuPanel).GetComponent<UIBox>();
            uibox.images[0].color = SaveScript.stageColors[i];
            uibox.texts[0].text = (i + 1) + "층";
            uibox.button.onClick.AddListener(MenuButton);
            uibox.order = i;
        }
    }

    private void SetMenuScale()
    {
        uiboxs = menuPanel.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uiboxs.Length; i++)
            uiboxs[i].transform.localScale = Vector3.one;
        uiboxs[menuIndex].transform.localScale = Vector3.one * 1.2f;
    }

    public void SetContent()
    {
        int startJemIndex = 0;
        for (int i = 0; i < menuIndex; i++)
            startJemIndex += SaveScript.stageItemNums[i];

        slots = contentPanel.GetComponentsInChildren<CollectionSlot>();
        for (int i = 0; i < slots.Length; i++)
            Destroy(slots[i].gameObject);

        for (int i = 0; i < contents.Length; i++)
            contents[i].color = SaveScript.stageColors[menuIndex];
        for (int i = 0; i < SaveScript.stageItemNums[menuIndex]; i++)
        {
            int jemCode = startJemIndex + i;
            slot = Instantiate(slotPrefab, contentPanel).GetComponent<CollectionSlot>();
            slot.jemCode = jemCode;
            slot.button.onClick.AddListener(SlotButton);
            slot.SetInit(SaveScript.saveData.collection_levels[jemCode] != 0);
        }
    }

    private void SlotButton()
    {
        int jemCode = -1;
        slot = EventSystem.current.currentSelectedGameObject.GetComponent<CollectionSlot>();
        if (slot != null) 
            jemCode = slot.jemCode;

        card_Image.color = SaveScript.qualityColors_weak[SaveScript.jems[jemCode].quality];
        card_jem.sprite = SaveScript.jems[jemCode].jemSprite;
        card_level.text = SaveScript.saveData.collection_levels[jemCode].ToString();
        card_name.text = SaveScript.jems[jemCode].name;
        card_quality.text = SaveScript.qualityNames_kr[SaveScript.jems[jemCode].quality] + " 카드";
        card_slider.maxValue = GameFuction.GetGoalCardNum(jemCode);
        card_slider.value = SaveScript.saveData.collection_cards[jemCode];
        card_sliderText.text = "( " + card_slider.value + " / " + card_slider.maxValue + " )";
        card_force.text = Collection.GetJemForceInfo(jemCode);
        card_info.text = SaveScript.collections[jemCode].info;

        long gold0 = 0, gold1 = 0, gold2 = 0, gold3 = 0;
        if (Jem.IsPrice2(jemCode))
            gold1 = SaveScript.jems[jemCode].GetRealPrice();
        else
            gold0 = SaveScript.jems[jemCode].GetRealPrice();
        card_gold.text = GameFuction.GetGoldText(gold0, gold1, gold2, gold3) + " < " + SaveScript.saveData.collection_levels[jemCode] + "%↑ >";

        SetAnimator(true);
    }



    public void SetFloor()
    {
        int floorLevel = GameFuction.GetFloorLevel(menuIndex);
        floorName.text = (menuIndex + 1) + "층 플로어 컬렉션 [Lv." + floorLevel + "]";

        uiboxs = floorObject.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uiboxs.Length; i++)
        {
            uiboxs[i].texts[0].text = "(Lv." + SaveScript.cardLevelsPerFloor[i] + " 이상) " + Collection.GetFloorForceInfo(menuIndex, i);
            if (floorLevel >= SaveScript.cardLevelsPerFloor[i])
            {
                // On
                uiboxs[i].images[1].gameObject.SetActive(false);
                uiboxs[i].texts[0].color = new Color(0.1f, 0.1f, 0.1f);
                uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 1.3f;
                if (i == 4) uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 1.4f;
                else if (i == 5) uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 1.5f;
                else if (i == 6) uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 1.6f;
            }
            else
            {
                // Off
                uiboxs[i].images[1].gameObject.SetActive(true);
                uiboxs[i].texts[0].color = new Color(0.2f, 0.2f, 0.2f);
                uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 0.7f;
                if (i == 4) uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 0.6f;
                else if (i == 5) uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 0.5f;
                else if (i == 6) uiboxs[i].images[0].color = SaveScript.stageColors[menuIndex] * 0.4f;
            }
        }
    }

    public void SetAnimator(bool isOn)
    {
        animator.SetBool("isOn", isOn);
    }

    public void CloseSlot()
    {
        SetAnimator(false);
    }
}
