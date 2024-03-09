using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventCtrl : MonoBehaviour
{
    private static EventCtrl Instance;

    public static EventCtrl instance
    {
        set 
        {   
            if (Instance == null)
                Instance = value; 
        }
        get { return Instance; }
    }

    private static string[] weekEventNames =
    {
        "마나석 드랍량 2배 증가",
        "상점 판매량 2배 증가",
        "던전 등장 확률 2배 증가",
        "얼티밋 광물 및 미스틱 광물 등장 확률 2배 증가",
        "광물 획득 2배 증가",
        "레드 다이아(CASH) 결제 획득량 1.5배 증가",
        "아이템 및 펫 드랍률 2배 증가",
        "성장하는 돌 등장 확률 2배 증가",
    };

    public DateTime dateTime;
    public int weekEventType;
    public bool isWeekEventOn;
    private int weekEventNum;
    private bool isInitOn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        weekEventNum = weekEventNames.Length;
    }

    public void SetInit()
    {
        if (isInitOn)
            return;

        isInitOn = true;
        dateTime = SaveScript.dateTime;
        weekEventType = GetWeekEventType();
        isWeekEventOn = GetWeekEventOn();
        if (SceneManager.GetActiveScene().name == "MainScene")
            MainInfoBoard.instance.SetBoardInfo();

        StopCoroutine(InitServerTime());
        StartCoroutine(InitServerTime());
    }

    IEnumerator InitServerTime()
    {
        int leftSec = 60 - dateTime.Second;

        yield return new WaitForSeconds(leftSec);
        dateTime = dateTime.AddSeconds(leftSec);
        weekEventType= GetWeekEventType();
        isWeekEventOn = GetWeekEventOn();

        StopCoroutine(RenewServerTime());
        StartCoroutine(RenewServerTime());
    }

    IEnumerator RenewServerTime()
    {
        yield return new WaitForSeconds(60f);

        dateTime = dateTime.AddMinutes(1);
        weekEventType = GetWeekEventType();
        isWeekEventOn = GetWeekEventOn();
        if (SceneManager.GetActiveScene().name == "MainScene")
            MainInfoBoard.instance.SetBoardInfo();
        StartCoroutine(RenewServerTime());
    }

    private int GetWeekEventType()
    {
        return GetIso8601WeekOfYear(dateTime) % weekEventNum;
    }

    private bool GetWeekEventOn()
    {
        return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
    }

    public string GetWeekEventName()
    {
        return weekEventNames[weekEventType];
    }

    static int GetIso8601WeekOfYear(DateTime time)
    {
        // ISO 8601 주차를 계산하기 위해 Calendar 클래스를 사용합니다.
        Calendar calendar = CultureInfo.InvariantCulture.Calendar;

        // 주차를 계산하기 위해 현재 년도와 월을 얻습니다.
        int year = calendar.GetYear(time);
        int month = calendar.GetMonth(time);

        // 현재 주차를 계산합니다.
        int weekNumber = calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        // 현재 주차를 반환합니다.
        return weekNumber;
    }
}
