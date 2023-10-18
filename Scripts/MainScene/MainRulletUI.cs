using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainRulletUI : MonoBehaviour
{
    public static MainRulletUI instance;

    public Canvas rulletObject;
    public GameObject rulletCanInfo;
    public Text rulletTimeText;
    public Text[] rulletButtonText;
    public GameObject rulletCanButton, rulletCannotButton;
    public Rullet goldRullet, abilityRullet;
    public GameObject rulletReward;
    public Image rulletGoldRewardImage, rulletAbilityRewardImage;
    public Text rulletGoldRewardText, rulletAbilityRewardText;

    public static bool isRulletTimeSet;
    private bool isOnRullet; // 룰렛이 켜져 있는가?
    private bool isStartRullet; // 룰렛이 돌아가고 있는가?

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        rulletObject.gameObject.SetActive(false);
        rulletReward.SetActive(false);

        SetRulletInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRulletTimeSet) SetRulletInfo();

        if (isStartRullet)
        {
            if (goldRullet.isEnd && abilityRullet.isEnd)
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
                    goldRullet.isStart = true;
                    abilityRullet.isStart = true;
                    GoogleAd.isReward = false;
                    break;
            }
        }
        else
        {
            if (GoogleAd.ADType == -1)
            {
                isStartRullet = false;
                GoogleAd.ADType = -2;
            }
        }
    }

    private void SetRulletButtonText()
    {
        if (SaveScript.saveData.isRemoveAD)
            rulletButtonText[0].text = rulletButtonText[1].text = "시작하기";
        else
            rulletButtonText[0].text = rulletButtonText[1].text = "광고보기";
    }

    public void OnOffRullet()
    {
        if (!isStartRullet && !MainScript.isChangeScene)
        {
            isOnRullet = !isOnRullet;
            rulletObject.gameObject.SetActive(isOnRullet);
            rulletReward.SetActive(false);
            MainScript.instance.SetAudio(0);
            SetRulletButtonText();

            goldRullet.rollImage.transform.rotation = new Quaternion(0, 0, 0, 0);
            abilityRullet.rollImage.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public void SetRulletInfo()
    {
        isRulletTimeSet = false;
        rulletTimeText.text = GameFuction.GetTimeText(SaveScript.saveData.rulletTime);

        if (SaveScript.saveData.rulletTime == 0)
        {
            rulletCanInfo.SetActive(true);
            rulletCanButton.SetActive(true);
            rulletCannotButton.SetActive(false);
        }
        else
        {
            rulletCanInfo.SetActive(false);
            rulletCanButton.SetActive(false);
            rulletCannotButton.SetActive(true);
        }
    }

    public void StartRullet()
    {
        if (!isStartRullet && !MainScript.isChangeScene)
        {
            MainScript.instance.SetAudio(3);
            isStartRullet = true;
            GoogleAd.instance.ADShow(0);
        }
    }

    public void StartRullet_Test()
    {
        if (!isStartRullet && !MainScript.isChangeScene)
        {
            MainScript.instance.SetAudio(3);
            isStartRullet = true;
            goldRullet.isStart = true;
            abilityRullet.isStart = true;
        }
    }

    public void CannotStartRullet()
    {
        MainScript.instance.SetAudio(2);
    }

    public void EndRullet()
    {
        rulletReward.SetActive(true);
        rulletGoldRewardImage.sprite = SaveScript.icons[goldRullet.selectedOrder].sprite;
        rulletAbilityRewardImage.sprite = SaveScript.icons[abilityRullet.selectedOrder].sprite;
        rulletGoldRewardText.text = "강화석 < " + GameFuction.GetNumText((long)(SaveScript.icons[goldRullet.selectedOrder].force)) + " > 개 획득";
        rulletAbilityRewardText.text = SaveScript.icons[abilityRullet.selectedOrder].info;
        SaveScript.saveData.hasReinforceOre += (int)(SaveScript.icons[goldRullet.selectedOrder].force);
        AchievementCtrl.instance.SetAchievementAmount(22, (int)(SaveScript.icons[goldRullet.selectedOrder].force));
        SaveScript.saveData.hasIcons[abilityRullet.selectedOrder] = true;
        SaveScript.saveData.iconsTime[abilityRullet.selectedOrder] = SaveScript.iconTime;
        SaveScript.saveData.rulletTime = SaveScript.rulletTime;

        isStartRullet = false;
        goldRullet.isEnd = false;
        abilityRullet.isEnd = false;

        IconInfoUI.instance.InitIconInfo();
        SetRulletInfo();
        SaveScript.instance.SaveData_Asyn(true);
    }
}
