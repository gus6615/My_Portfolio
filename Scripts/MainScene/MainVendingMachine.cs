using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainVendingMachine : MonoBehaviour
{
    private static MainVendingMachine Instance;
    public static MainVendingMachine instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }

    public static bool isVendingMachineTimeSet;
    private static int[] multiples = { 1, 3, 10 };
    private static long[] manaOres = { 20, 30, 40, 50, 70, 100, 200, 500, 1000, 3000, 10000, 20000, 50000, 100000, 300000 }; // stageNum 변동
    private static int[] cashOres = { 25, 50, 150 };
    private static string[] manaNames = { "마나석 요람 (동)", "마나석 요람 (은)", "마나석 요람 (금)" };
    private static string[] cashNames = { "캐시 요람 (동)", "캐시 요람 (은)", "캐시 요람 (금)" };
    private static string[] colorNames = { "<color=white>", "<color=#00FFFF>", "<color=#FFFF00>" };

    public Canvas vendingMachineObject;
    public Transform infoSlotTr;
    public GameObject canInfo;
    public Text floorText;
    public TMP_Text timeText;
    public Image buttonIcon;
    public Sprite button_isAD_sprite, button_isNotAD_sprite; 
    public Rullet rullet;

    private UIBox[] infoSlots;
    private bool isOn; // 룰렛이 켜져 있는가?
    private bool isStart; // 룰렛이 돌아가고 있는가?

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        vendingMachineObject.gameObject.SetActive(false);
        infoSlots = infoSlotTr.GetComponentsInChildren<UIBox>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isVendingMachineTimeSet) 
            SetVendingMachineInfo();

        if (isStart)
        {
            if (rullet.isEnd)
            {
                EndRullet();
            }
        }

        // 광고 관련
        if (GoogleAd.isReward)
        {
            switch (GoogleAd.ADType)
            {
                case 0:
                    rullet.isStart = true;
                    GoogleAd.isReward = false;
                    break;
            }
        }
        else
        {
            if (GoogleAd.ADType == -1)
            {
                isStart = false;
                GoogleAd.ADType = -2;
            }
        }
    }

    public void OnOffVendingMachine()
    {
        if (!isStart && !MainScript.isChangeScene)
        {
            isOn= !isOn;
            vendingMachineObject.gameObject.SetActive(isOn);
            MainScript.instance.SetAudio(0);
            SetInfoSlot_Default();
            SetButtomUI();

            rullet.rollImage.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public void SetInfoSlot_Default()
    {
        for (int i = 0; i < infoSlots.Length; i++)
        {
            int index = i % 3;

            if (i < 3)
            {
                infoSlots[i].images[0].color = new Color(0.5f, 0.5f, 0.8f, 0.9f);
                infoSlots[i].tmp_texts[0].SetText(colorNames[index] + manaNames[index]);
                infoSlots[i].tmp_texts[1].SetText("<color=white>x " + GameFuction.GetNumText(multiples[index] * manaOres[SaveScript.saveData.pickLevel]));
            }
            else
            {
                infoSlots[i].images[0].color = new Color(0.8f, 0.5f, 0.5f, 0.9f);
                infoSlots[i].tmp_texts[0].SetText(colorNames[index] + cashNames[index]);
                infoSlots[i].tmp_texts[1].SetText("<color=white>x " + GameFuction.GetNumText(cashOres[index]));
            }
        }
    }

    public void SetInfoSlot_Get(int _index)
    {
        infoSlots[_index].images[0].color = new Color(0.8f, 0.8f, 0f, 0.9f);
        StartCoroutine(SetInfoSlot_Reback());
    }

    IEnumerator SetInfoSlot_Reback()
    {
        yield return new WaitForSeconds(5f);

        SetInfoSlot_Default();
    }

    public void SetVendingMachineInfo()
    {
        isVendingMachineTimeSet = false;
        floorText.text = "보상 리스트 (" + (SaveScript.saveData.pickLevel + 1) + "층)";
        timeText.text = "다음 기회까지 <color=#FF9696>[ " + GameFuction.GetTimeText(SaveScript.saveData.vendingMachineTime) + " ] <color=white>남음";

        if (SaveScript.saveData.vendingMachineTime == 0)
            canInfo.SetActive(true);
        else
            canInfo.SetActive(false);
    }

    private void SetButtomUI()
    {
        if (SaveScript.saveData.isRemoveAD)
            buttonIcon.sprite = button_isNotAD_sprite;
        else
            buttonIcon.sprite = button_isAD_sprite;
    }

    public void StartVendingMachine()
    {
        if (!isStart && !MainScript.isChangeScene)
        {
            if (SaveScript.saveData.vendingMachineTime != 0)
            {
                SystemInfoCtrl.instance.SetErrorInfo("뽑기 까지 [ " + GameFuction.GetTimeText(SaveScript.saveData.vendingMachineTime) + " ] 남았습니다.");
                MainScript.instance.SetAudio(2);
                return;
            }

            MainScript.instance.SetAudio(3);
            isStart= true;
            GoogleAd.instance.ADShow(0);
        }
    }

    public void EndRullet()
    {
        string showInfo;
        long num;

        switch (rullet.selectedOrder)
        {
            case 0:
                num = manaOres[SaveScript.saveData.pickLevel] * multiples[rullet.selectedOrder2];
                showInfo = colorNames[rullet.selectedOrder2] + "[ " + manaNames[rullet.selectedOrder2] + " ] <color=white>에서 <color=#9696FF>'마나석 "
                    + GameFuction.GetNumText(num) + " 개' <color=white>를 획득하셨습니다!";
                SaveScript.saveData.manaOre += num;
                break;
            default:
                num = cashOres[rullet.selectedOrder2];
                showInfo = colorNames[rullet.selectedOrder2] + "[ " + cashNames[rullet.selectedOrder2] + " ] <color=white>에서 <color=#FF9696>'레드 다이아 "
                    + GameFuction.GetNumText(num) + " 개' <color=white>를 획득하셨습니다!";
                SaveScript.saveData.cash += num;
                break;
        }

        SystemInfoCtrl.instance.SetShowInfo(showInfo, 0.25f, 3f, 0.25f);
        SaveScript.saveData.vendingMachineTime = SaveScript.vendingMachineTime;

        isStart = false;
        rullet.isEnd = false;

        SetVendingMachineInfo();
        SetInfoSlot_Get(rullet.selectedOrder * 3 + rullet.selectedOrder2);
        MainScript.instance.SetAudio(31 + rullet.selectedOrder2);
        SaveScript.instance.SaveData_Asyn(true);
    }
}
