using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBonusUI : MonoBehaviour
{
    private static string[] names = { "����", "��÷�", "�����Ǳ���", "��Ű", "����",
        "��Ƽ��", "������", "����̴����", "��������", "���۾Ƹ���", "joco", "���̿�1",
        "������", "GNom", "�����Ǻ���", "����", "medi4180", "��������", "�̳�", "����",
        "��2", "ƼƼtt", "��������", "������", "les5856", "ww" };

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
