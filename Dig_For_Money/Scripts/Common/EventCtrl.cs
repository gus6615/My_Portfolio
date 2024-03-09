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
        "������ ����� 2�� ����",
        "���� �Ǹŷ� 2�� ����",
        "���� ���� Ȯ�� 2�� ����",
        "��Ƽ�� ���� �� �̽�ƽ ���� ���� Ȯ�� 2�� ����",
        "���� ȹ�� 2�� ����",
        "���� ���̾�(CASH) ���� ȹ�淮 1.5�� ����",
        "������ �� �� ����� 2�� ����",
        "�����ϴ� �� ���� Ȯ�� 2�� ����",
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
        // ISO 8601 ������ ����ϱ� ���� Calendar Ŭ������ ����մϴ�.
        Calendar calendar = CultureInfo.InvariantCulture.Calendar;

        // ������ ����ϱ� ���� ���� �⵵�� ���� ����ϴ�.
        int year = calendar.GetYear(time);
        int month = calendar.GetMonth(time);

        // ���� ������ ����մϴ�.
        int weekNumber = calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        // ���� ������ ��ȯ�մϴ�.
        return weekNumber;
    }
}
