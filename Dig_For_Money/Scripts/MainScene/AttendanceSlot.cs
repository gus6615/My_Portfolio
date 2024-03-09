using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttendanceSlot : MonoBehaviour
{
    static public Color[] slotColors = { new Color(1f, 1f, 1f), new Color(0.4f, 0.6f, 1f), new Color(1f, 0.6f, 0.6f), new Color(0.6f, 1f, 0.6f) };
    static public int[] rewardNums =
    {
        1000, 100, 3, 25, 1, 1, 50,
        10000, 1000, 3, 50, 1, 1, 100,
        100000, 10000, 3, 75, 1, 1, 150,
        1000000, 100000, 3, 100, 1, 1, 200
    };

    public FadeEffect effect;
    public Button slotButton;
    public Image slotImage, itemImage, isDoneImage;
    public Text slotDayText, slotNumText;
    public int type; // 0 = Can Do Attendance, 1 = isDone, 2 = etc
    public int code; // 0 ~ 27

    // 출석 체크 버튼
    public void Attendance()
    {
        // 펫의 경우 인벤토리 빈칸 확인
        if ((code % 7 == 4 && MinerSlime.FindEmptyPetInven() == -1) || (code % 7 == 5 && AdventurerSlime.FindEmptyPetInven() == -1))
        {
            MainScript.instance.SetAudio(2);
            SystemInfoCtrl.instance.SetErrorInfo("펫 인벤토리를 하나 이상 비워주세요!");
            return;
        }

        MainScript.instance.SetAudio(4);
        type = 1;
        SaveScript.saveData.attendance_count++;
        SaveScript.saveData.attendance_day = SaveScript.dateTime.Day;
        GetReward();
        SaveScript.instance.SaveData_Asyn(true);
        MainAttendance.instance.SetCanAttendanceInfo();
        SetInit();
    }

    // Slot Setting, Code와 Type을 지정하고 호출해야 함.
    public void SetInit()
    {
        int unit = code % 7;
        slotDayText.text = (code + 1).ToString();
        slotNumText.text = "x " + GameFuction.GetNumText(rewardNums[code]);
        isDoneImage.gameObject.SetActive(false);
        slotButton.enabled = effect.enabled = false;

        if (type == 1)
        {
            if (unit == 3 || unit == 6)
                slotImage.color = slotColors[2] * 0.5f;
            else if (unit == 4 || unit == 5)
                slotImage.color = slotColors[1] * 0.5f;
            else
                slotImage.color = slotColors[0] * 0.5f;
            itemImage.color = new Color(1f, 1f, 1f, 0.5f);
            isDoneImage.gameObject.SetActive(true);
        }
        else
        {
            if(unit == 3 || unit == 6)
                slotImage.color = slotColors[2];
            else if (unit == 4 || unit == 5)
                slotImage.color = slotColors[1];
            else
                slotImage.color = slotColors[0];
            itemImage.color = Color.white;
            if (type == 0)
            {
                slotButton.enabled = effect.enabled = true;
                slotImage.color = slotColors[3];
            }
        }
    }

    public void GetReward()
    {
        int temp = code % 7;
        switch (temp)
        {
            case 0: // 강화석
                SaveScript.saveData.hasReinforceOre += rewardNums[code];
                AchievementCtrl.instance.SetAchievementAmount(22, rewardNums[code]);
                break;
            case 1: // 마나석
                SaveScript.saveData.manaOre += rewardNums[code];
                AchievementCtrl.instance.SetAchievementAmount(23, rewardNums[code]);
                break;
            case 2: // 주문서
                temp = code / 7;
                switch (temp)
                {
                    case 0: SaveScript.saveData.hasReinforceItems[7] += rewardNums[code]; break;
                    case 1: SaveScript.saveData.hasReinforceItems2[1] += rewardNums[code]; break;
                    case 2: SaveScript.saveData.hasReinforceItems2[2] += rewardNums[code]; break;
                    case 3: SaveScript.saveData.hasReinforceItems2[3] += rewardNums[code]; break;
                }
                AchievementCtrl.instance.SetAchievementAmount(18, rewardNums[code]);
                break;
            case 3: // 레드 다이아
                SaveScript.saveData.cash += rewardNums[code];
                AchievementCtrl.instance.SetAchievementAmount(24, rewardNums[code]);
                break;
            case 4: // 광부 펫
                int index = MinerSlime.FindEmptyPetInven();
                SaveScript.saveData.hasMiners[index] = code / 7 + 3;
                SaveScript.saveData.hasMinerLevels[index] = 1;
                SaveScript.saveData.hasMinerExps[index] = 0;
                AchievementCtrl.instance.SetAchievementAmount(19, 1);
                break;
            case 5: // 모험가 펫
                temp = AdventurerSlime.FindEmptyPetInven();
                SaveScript.saveData.hasAdventurers[temp] = code / 7 + 3;
                SaveScript.saveData.hasAdventurerLevels[temp] = 1;
                SaveScript.saveData.hasAdventurerExps[temp] = 0;
                AchievementCtrl.instance.SetAchievementAmount(19, 1);
                break;
            case 6: // 레드 다이아
                SaveScript.saveData.cash += rewardNums[code];
                AchievementCtrl.instance.SetAchievementAmount(24, rewardNums[code]);
                break;
        }
        MainAchievementUI.instance.SetReceiveCanInfo();
    }
}
