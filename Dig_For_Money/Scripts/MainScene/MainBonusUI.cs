using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBonusUI : MonoBehaviour
{
    private static string[] names = { "리즈", "비올레", "전설의광부", "뇨키", "불콘",
        "스티브", "김현직", "잼민이당얘들아", "돼지지민", "얼룩송아리다", "joco", "댕이원1",
        "꼭웃어", "GNom", "오구의복귀", "부히", "medi4180", "이주현뀨", "이날", "귤이",
        "귤2", "티티tt", "우우우우우유", "태유니", "les5856", "ww" };

    private const int cashNum = 300;
    static public MainBonusUI instance;

    public Canvas canvasObject;
    public Text rewardText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        rewardText.text = "X " + cashNum;
        OnOffCanvas();
    }

    public void OnOffCanvas()
    {
        canvasObject.enabled = !SaveScript.saveData.isGetBonus;
    }

    public void OnGetReward()
    {
        MainScript.instance.SetAudio(0);
        canvasObject.enabled = false;

        SaveScript.saveData.isGetBonus = true;
        SaveScript.saveData.cash += cashNum;
        AchievementCtrl.instance.SetAchievementAmount(24, cashNum);
        MainAchievementUI.instance.SetReceiveCanInfo();
        SaveScript.SaveData_Syn();
    }

    public void CheckBonus()
    {
        StartCoroutine("Check");
    }

    IEnumerator Check()
    {
        while (SaveScript.saveRank.myRankData.nickname == null)
            yield return null;

        for (int i = 0; i < names.Length; i++)
        {
            if (SaveScript.saveRank.myRankData.nickname == names[i])
            {
                SaveScript.saveData.isGetBonus = false;
                OnOffCanvas();
            }
        }
    }
}
