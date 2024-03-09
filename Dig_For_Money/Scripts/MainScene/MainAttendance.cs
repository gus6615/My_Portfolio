using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainAttendance : MonoBehaviour
{
    static public MainAttendance instance;

    public Canvas attendanceCanvas;
    public GameObject contentPanel;
    public GameObject attendanceCanInfo;
    private AttendanceSlot[] slots;

    public bool isOnOffUI;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        slots = contentPanel.GetComponentsInChildren<AttendanceSlot>();

        attendanceCanvas.gameObject.SetActive(false);
        attendanceCanInfo.SetActive(false);

        SetCanAttendanceInfo();
    }

    public void OnOffUI()
    {
        MainScript.instance.SetAudio(0);
        if (!Backend.IsInitialized)
        {
            SystemInfoCtrl.instance.SetErrorInfo("현재 서버 연결에 실패했습니다.\n잠시 후 시도해주시길 바랍니다.");
            return;
        }

        isOnOffUI = !isOnOffUI;
        attendanceCanvas.gameObject.SetActive(isOnOffUI);
        SetCanAttendanceInfo();
        SetContent();
    }

    public void OnUI()
    {
        isOnOffUI = true;
        attendanceCanvas.gameObject.SetActive(isOnOffUI);
        SetCanAttendanceInfo();
        SetContent();
    }

    public void SetContent()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (SaveScript.saveData.attendance_count > i)
                slots[i].type = 1; // 이미 출석 체크 완료
            else if (SaveScript.saveData.attendance_count == i)
            {
                if (SaveScript.saveData.attendance_day != SaveScript.dateTime.Day)
                    slots[i].type = 0; // 오늘 출석 체크 가능
                else
                    slots[i].type = 2; // 앞으로 출석 체크 가능
            }
            else
                slots[i].type = 2;
            slots[i].code = i;
            slots[i].SetInit();
        }
    }

    public void SetCanAttendanceInfo()
    {
        if(SaveScript.saveData.attendance_day != SaveScript.dateTime.Day)
            attendanceCanInfo.SetActive(true);
        else
            attendanceCanInfo.SetActive(false);
    }
}
