using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop instance;
    static public bool isGotoMainScene;

    public new AudioSource audio;
    public Text goldText, cashText;
    public Image topUI;
    public UIBox[] menuButtons;
    public FadeEffect chargeButton_fadeEffect;
    public GameItemShop gameItemShop;
    public CashItemShop cashItemShop;
    public CashChargeShop cashChargeShop;

    [SerializeField] public FadeEffect allSellButton;

    private Color gameColor, cashColor, eventColor;
    private int menuIndex; 

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        gameColor = new Color(0.8f, 0.8f, 0.8f);
        cashColor = new Color(1f, 0.6f, 0.6f);
        eventColor = new Color(1f, 1f, 0f);

        audio.mute = !SaveScript.saveData.isSEOn;
        gameItemShop.gameObject.SetActive(true);
        cashItemShop.gameObject.SetActive(false);
        cashChargeShop.gameObject.SetActive(false);
        menuIndex = 0;

        SetTutorial();
        SetBasicInfo();
        SetMenuButton();
        gameItemShop.ResetMenu();
    }

    public void GotoMainScene()
    {
        SetAudio(0);
        isGotoMainScene = true;
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    public void SetBasicInfo()
    {
        goldText.text = GameFuction.GetGoldText(SaveScript.saveData.gold, SaveScript.saveData.gold2, SaveScript.saveData.gold3, SaveScript.saveData.gold4);
        cashText.text = SaveScript.saveData.cash.ToString();
    }

    private void SetTutorial()
    {
        allSellButton.enabled = allSellButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 4 }, SaveScript.saveData.mainQuest_list);
    }

    private void SetMenuButton()
    {
        switch (menuIndex)
        {
            case 0:
                for (int i = 0; i < menuButtons.Length; i++)
                    menuButtons[i].images[0].color = menuButtons[i].images[1].color = gameColor;
                menuButtons[menuIndex + 1].images[0].color = menuButtons[menuIndex + 1].images[1].color = new Color(gameColor.r * 0.7f, gameColor.b * 0.7f, gameColor.g * 0.7f, 1f);
                topUI.color = gameColor;
                goldText.color = cashText.color = new Color(gameColor.r * 0.25f, gameColor.b * 0.25f, gameColor.g * 0.25f, 1f);
                Camera.main.backgroundColor = new Color(gameColor.r * 0.25f, gameColor.b * 0.25f, gameColor.g * 0.25f, 1f);
                break;
            default:
                for (int i = 0; i < menuButtons.Length; i++)
                    menuButtons[i].images[0].color = menuButtons[i].images[1].color = cashColor;
                menuButtons[menuIndex + 1].images[0].color = menuButtons[menuIndex + 1].images[1].color = new Color(cashColor.r * 0.7f, cashColor.b * 0.7f, cashColor.g * 0.7f, 1f);
                topUI.color = cashColor;
                goldText.color = cashText.color = new Color(cashColor.r * 0.25f, cashColor.b * 0.25f, cashColor.g * 0.25f, 1f);
                Camera.main.backgroundColor = new Color(cashColor.r * 0.25f, cashColor.b * 0.25f, cashColor.g * 0.25f, 1f);
                break;
        }

        switch (menuIndex)
        {
            case 1:
                cashItemShop.menuIndex = cashItemShop.itemIndex = 0;
                cashItemShop.OnMenuButton();
                break;
        }

        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 5)
        {
            chargeButton_fadeEffect.enabled = true;
            menuButtons[menuButtons.Length - 1].images[1].color = eventColor;
        }
        else
        {
            chargeButton_fadeEffect.enabled = false;
        }
    }

    public void SetGameItemShop()
    {
        menuIndex = 0;
        SetMenuButton();
        SetAudio(0);

        gameItemShop.gameObject.SetActive(true);
        cashItemShop.gameObject.SetActive(false);
        cashChargeShop.gameObject.SetActive(false);
        gameItemShop.ResetMenu();
    }

    public void SetCashItemShop()
    {
        menuIndex = 1;
        SetMenuButton();
        SetAudio(0);

        CashItemShop.instance.selectContent.SetActive(false);
        gameItemShop.gameObject.SetActive(false);
        cashItemShop.gameObject.SetActive(true);
        cashChargeShop.gameObject.SetActive(false);
    }

    public void SetCashChargeShop()
    {
        menuIndex = 2;
        SetMenuButton();
        SetAudio(0);

        gameItemShop.gameObject.SetActive(false);
        cashItemShop.gameObject.SetActive(false);
        cashChargeShop.gameObject.SetActive(true);
    }
}
