using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class GoogleAd : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    static public GoogleAd instance;

    private Canvas systemInfo; // 시스템 알림 전용 Canvas
    private Image systemImage;
    private Text systemText;

    private string projectID, adTypeID;

    /// <summary>
    /// 광고 보상을 줄 것인가?
    /// </summary>
    static public bool isReward;
    /// <summary>
    /// 광고을 보는 목적(타입), ADtype이 -1인 경우는 실패, 그 외에는 성공을 나타내며 각 value에는 광고 목적이 부여된다.(Default:-2)
    /// </summary>
    static public int ADType;

    private bool isShowFailToLoad;
    private float showFailToLoadTime, currentTime;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            systemInfo = this.GetComponentInChildren<Canvas>();
            systemImage = systemInfo.GetComponentsInChildren<Image>()[1];
            systemText = systemInfo.GetComponentInChildren<Text>();

            showFailToLoadTime = 5.9f;
            systemInfo.enabled = false;
            systemImage.color = new Color(1f, 1f, 1f, 0f);
            systemText.color = new Color(0f, 0f, 0f, 0f);

            projectID = "4010799";
            adTypeID = "rewardAD";
            Advertisement.Initialize(projectID, false, this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (isShowFailToLoad) ShowInfoDependOnFail();
    }

    /// <summary>
    /// 광고를 보여줍니다.
    /// </summary>
    /// <param name="type">광고의 목적(타입)</param>
    public void ADShow(int type)
    {
        ADType = type;
        // 광고 제거가 있다면 광고 스킵
        if (SaveScript.saveData.isRemoveAD)
        {
            isReward = true;
            if (BlindScript.instance.isShowAD)
                isReward = false;
            BlindScript.instance.DisableShowAD();
            return;
        }

        Advertisement.Load(adTypeID, this);
    }

    /// <summary>
    /// 사용자 대기 시간에 따라 
    /// </summary>
    private void ShowInfoDependOnFail()
    {
        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f)
                currentTime = 0f;
            systemText.text = "현재 불러올 광고가 없습니다." + "\n" + "[" + Mathf.FloorToInt(currentTime) + "초]";
        }
        else
        {
            // 사용자 대기 시간이 모두 종료 됨 => 시스템 알림을 종료하고 광고 보상 지급
            isShowFailToLoad = false;
            systemInfo.enabled = false;
            systemImage.color = new Color(1f, 1f, 1f, 0f);
            systemText.color = new Color(0f, 0f, 0f, 0f);
            isReward = true;
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(adTypeID, this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError("Unity AD Load Error : " + error + " / " + message);

        isShowFailToLoad = true;
        currentTime = showFailToLoadTime;
        systemInfo.enabled = true;
        systemImage.color = Color.white;
        systemText.color = Color.black;
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError("Unity AD Show Error : " + error + " / " + message);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //Debug.Log("Unity AD Show is start");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //Debug.Log("Unity AD Show is clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("광고 시청 완료");
        // 광고를 끝까지 시청 함
        isReward = true;

        if (BlindScript.instance.isShowAD)
            isReward = false;
        BlindScript.instance.DisableShowAD();
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity AD Initialization is success");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError("Unity AD Initialization Error : " + error + " / " + message);
    }
}
