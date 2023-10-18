using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pause : MonoBehaviour
{
    private static string[] dropInfoStrs = { "[ 노멀 광물 ]\n", "[ 레어 광물 ]\n", "[ 에픽 광물 ]\n", 
        "[ 유니크 광물 ]\n", "[ 레전드리 광물 ]\n", "[ 얼티밋 광물 ]\n", "[ 미스틱 광물 ]\n" };
    public static Pause instance;

    [SerializeField] private GameObject pauseObject;
    [SerializeField] private GameObject dropInfoObject;
    private TMP_Text[] dropInfoTexts;
    private bool isPause;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        dropInfoTexts = dropInfoObject.GetComponentsInChildren<TMP_Text>();

        pauseObject.SetActive(false);
    }

    /// <summary>
    /// 게임을 일시정지하거나 풉니다.
    /// </summary>
    public void SetPause()
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();

        if (!isPause) // PauseOn
        {
            Time.timeScale = 0f;

            // 광물 정보 
            long[] jemNumsAsQulity = new long[SaveScript.qualityNum];
            for (int i = 0; i < PlayerScript.instance.jems.Length; i++)
                jemNumsAsQulity[SaveScript.jems[i].quality] += PlayerScript.instance.jems[i];

            for (int i = 0; i < jemNumsAsQulity.Length; i++)
                dropInfoTexts[i].text = dropInfoStrs[i] + "- <color=#FF9696>" + GameFuction.GetNumText(jemNumsAsQulity[i]) + " <color=white>획득";
            dropInfoTexts[jemNumsAsQulity.Length].text = "[ 성장하는 돌 ]\n- <color=#FF9696>" + GameFuction.GetNumText(PlayerScript.instance.growthOre) + " <color=white>획득";
            dropInfoTexts[jemNumsAsQulity.Length + 1].text = "[ 강화석 ]\n- <color=#FF9696>" + GameFuction.GetNumText(PlayerScript.instance.reinforceOre) + " <color=white>획득";
            dropInfoTexts[jemNumsAsQulity.Length + 2].text = "[ 마나석 ]\n- <color=#FF9696>" + GameFuction.GetNumText(PlayerScript.instance.manaOre) + " <color=white>획득";
            dropInfoTexts[jemNumsAsQulity.Length + 3].text = "[ 경험치 ]\n- <color=#FF9696>" + GameFuction.GetNumText(PlayerScript.instance.exp) + " <color=white>획득";
        }
        else // PuaseOff
        {
            Time.timeScale = 1f;
            if (SaveScript.saveData.isTutorial)
            {
                Tutorial.instance.tutorialInfo.SetActive(true);
            }
        }

        isPause = !isPause;
        pauseObject.SetActive(isPause);
        PrintUI.instance.AudioPlay(0);
    }
}
