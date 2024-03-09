using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainReview : MonoBehaviour
{
    public static MainReview instance;

    public Canvas reviewObject;
    public GameObject reviewButton;
    private bool isReviewUIOn;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        reviewObject.gameObject.SetActive(false);
        if (SaveScript.saveData.hasPicks[3] && !SaveScript.saveData.isReviewOn)
            reviewButton.SetActive(true);
        else
            reviewButton.SetActive(false);
    }

    public void OnOffReview()
    {
        MainScript.instance.SetAudio(0);
        isReviewUIOn = !isReviewUIOn;
        reviewObject.gameObject.SetActive(isReviewUIOn);
    }

    public void GoReview()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.CheonnyangCompany.DigForMoney_RTM");

        SaveScript.saveData.isReviewOn = true;
        reviewButton.SetActive(!SaveScript.saveData.isReviewOn);
        StartCoroutine(RewardReview());
    }

    IEnumerator RewardReview()
    {
        yield return new WaitForSeconds(1f);

        SaveScript.saveData.cash += 300;
        AchievementCtrl.instance.SetAchievementAmount(24, 300);
        SaveScript.SaveData_Syn();
        MainAchievementUI.instance.SetReceiveCanInfo();
        MainScript.instance.SetGoldAndEXPText();
        OnOffReview();
    }
}
