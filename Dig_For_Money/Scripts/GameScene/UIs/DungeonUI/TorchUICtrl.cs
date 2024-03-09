using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorchUICtrl : MonoBehaviour
{
    private const float MAX_TIME = 90f;
    private const float ADD_TIME = 30f;
    private const float WARNING_TIME = 15f;
    private const float ADD_FILLSPEED = 2f;
    private readonly Color countColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);

    public static TorchUICtrl instance;
    
    public GameObject infoObject;
    public Image fillImage;
    public Image countInfo;
    public Text timeText;

    private Image[] infoImages;
    private Text countInfoText;
    private float currentTime;
    private uint state; // 0 = 시작, 1 = 위급, 2 = 게임 종료
    private bool isCountDown;
    public bool isOn;
    private int startCount;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        infoImages = infoObject.GetComponentsInChildren<Image>();
        countInfoText = countInfo.GetComponentInChildren<Text>();

        infoObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountDown)
            StartCoroutine("CountDown");

        if (isOn)
        {
            currentTime -= Time.deltaTime;
            SetLightState();
            if (currentTime > WARNING_TIME)
                SetTimerText(((int)currentTime).ToString(), new Color(0.2f, 0.2f, 0.2f, 1f));
            else if (currentTime > 0f)
                SetTimerText(((int)currentTime).ToString(), new Color(1f, 0.4f, 0.4f, 1f));
            else
            {
                StartCoroutine("EndTime");
                SetTimerText("0", new Color(1f, 0.4f, 0.4f, 1f));
            }
        }
    }

    public void SetInit()
    {
        infoObject.SetActive(true);
        currentTime = MAX_TIME;
        state = 0;
        startCount = 4;
        PlayerScript.instance.isCanCtrl = false;
        SetTimerText(((int)currentTime).ToString(), new Color(0.2f, 0.2f, 0.2f, 1f));
        StartCoroutine("StartFade");
    }

    public void SetEnd()
    {
        isOn = false;
        isCountDown = false;
        infoObject.SetActive(false);
    }

    public IEnumerator AddTime()
    {
        currentTime += ADD_TIME;
        if (currentTime > MAX_TIME)
            currentTime = MAX_TIME;

        float gap = (currentTime / MAX_TIME) - fillImage.fillAmount;
        while (Mathf.Abs(gap) > 0f)
        {
            fillImage.fillAmount += gap * Time.deltaTime * ADD_FILLSPEED;
            gap -= gap * Time.deltaTime * ADD_FILLSPEED;
            yield return null;
        }
    }

    IEnumerator EndTime()
    {
        isOn = false;
        PlayerScript.instance.isCanCtrl = false;
        Interaction.instance.isCtrl = false;
        Interaction.instance.SetInit();

        yield return new WaitForSeconds(1f);

        while (!BlindScript.isEndChange)
            yield return null;

        if (PlayerScript.instance.isDungeon_1_On)
        {
            EventButton.mainType = 10;
            StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.dungeon_1_startPos, BlindScript.instance.GetStageBGM(), 1.5f, 1.5f));
        }
    }

    IEnumerator CountDown()
    {
        isCountDown = false;
        if (startCount == 0)
        {
            SetCountInfo("시작", Color.white, 0f, 1f);
            PlayerScript.instance.SetButtonMode(false, true, false, true);
        }
        else if (startCount < 0)
        {
            StopCoroutine("StartFade");
            PlayerScript.instance.isCanCtrl = true;
            isOn = true;
        }
        else
            SetCountInfo(startCount.ToString(), Color.white, 0f, 1f);
        startCount--;

        yield return new WaitForSeconds(1f);
        
        if (!isOn)
            isCountDown = true;
    }

    public void StartCountDown()
    {
        isCountDown = true;
        startCount = 4;
    }

    public void SetCountInfo(string text, Color color, float fadeStart, float fadeTime)
    {
        countInfoText.text = text;
        countInfoText.color = color;
        countInfo.color = countColor;
        countInfo.GetComponent<FadeUI>().SetFadeValues(0f, fadeStart, fadeTime);
    }

    public void SetTimerText(string text, Color color)
    {
        fillImage.fillAmount -= Time.deltaTime / MAX_TIME;
        timeText.text = text;
        timeText.color = color;
    }

    private void SetLightState()
    {
        if (currentTime < 0f)
        {
            // 게임 오버
            state = 2;
        }
        else if (currentTime < WARNING_TIME)
        {
            // 위급 상태
            if (state != 1)
                PlayerScript.instance.SetFadeLight(0, 5);
            state = 1;
        }
        else
        {
            // 안전 상태
            if (state != 0)
                PlayerScript.instance.SetFadeLight(1, 7);
            state = 0;
        }
    }

    IEnumerator StartFade()
    {
        for (int i = 0; i < infoImages.Length; i++)
            infoImages[i].color = new Color(1f, 1f, 1f, 0f);
        timeText.color = new Color(0.2f, 0.2f, 0.2f, 0f);
        StartCountDown();

        yield return new WaitForSeconds(0.5f);

        float alpha = 0f;
        float goal = 0.5f;
        while (alpha < goal)
        {
            alpha += Time.deltaTime * 2.5f * Mathf.Clamp(goal - alpha, 0.01f, 1f);
            fillImage.fillAmount = alpha / goal;
            for (int i = 0; i < infoImages.Length; i++)
                infoImages[i].color = new Color(1f, 1f, 1f, alpha);
            timeText.color = new Color(0.2f, 0.2f, 0.2f, alpha);
            yield return null;
        }
        fillImage.fillAmount = 1f;
    }
}
