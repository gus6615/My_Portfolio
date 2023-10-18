using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainComboUI : MonoBehaviour
{
    static private Color[] colorAsLevel = { new Color(0.5f, 0.5f, 0.5f), new Color(1f, 0.5f, 0.5f), new Color(1f, 0.6f, 0.3f)
            , new Color(1f, 1f, 0f), new Color(0.5f, 1f, 0.5f), new Color(0.5f, 0.5f, 1f) };
    static private string[] infosText = { "땅 파기", "몬스터 처치", "던전 상자 열기", "마나석 광석 캐기", "보스 몬스터 처치" };
    static private string[] rewardsText = { "- 추가 경험치 획득률 < ", "- 추가 경험치 획득률 < ", "- 얼티밋 및 미스틱 광석 등장 확률  < ", "- 얼티밋 및 미스틱 광석 등장 확률  < ", "- 마나석 광물 & 상자 드랍량 < " };

    private static MainComboUI Instance;
    public static MainComboUI instance
    {
        set { Instance = value; }
        get { return Instance; }
    }

    [SerializeField]
    private Canvas comboUIObject;

    [SerializeField]
    private UIBox mainUIBox, comboUIBox;

    [SerializeField]
    private UIBox[] rewardUIBox, infoUIBox;

    private bool isOn; // 현재 콤보 UI가 켜져있는가?

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        comboUIObject.gameObject.SetActive(false);
        SetMainUI();
    }

    public void OnOffComboUI()
    {
        isOn = !isOn;
        comboUIObject.gameObject.SetActive(isOn);
        if (MainScript.instance != null) MainScript.instance.SetAudio(0);
        else PrintUI.instance.AudioPlay(0);

        if (isOn)
        {
            SetComboUI();
        }
    }

    public void SetMainUI()
    {
        // Set Level Block UI
        for (int i = 1; i < mainUIBox.images.Length; i++)
        {
            if (SaveScript.stat.combo_level + 1 < i)
            {
                mainUIBox.images[i].color = new Color(0, 0, 0, 0);
            }
            else
            {
                mainUIBox.images[i].color = Color.white;
                mainUIBox.images[i].fillAmount = 1;
            }
        }
        if (SaveScript.stat.combo_level < 5)
            mainUIBox.images[SaveScript.stat.combo_level + 1].fillAmount = (float)(SaveScript.saveData.combo_gauge % SaveScript.combo_unit) / SaveScript.combo_unit;

        // Set Star Image & Text
        mainUIBox.images[0].color = colorAsLevel[SaveScript.stat.combo_level];
        mainUIBox.tmp_texts[0].SetText(SaveScript.stat.combo_level.ToString());
    }

    private void SetComboUI()
    {
        // Set Level Block UI
        for (int i = 1; i < comboUIBox.images.Length; i++)
        {
            if (SaveScript.stat.combo_level + 1 < i)
            {
                comboUIBox.images[i].color = new Color(0, 0, 0, 0);
            }
            else
            {
                comboUIBox.images[i].color = Color.white;
                comboUIBox.images[i].fillAmount = 1;
            }
        }
        if (SaveScript.stat.combo_level < 5)
            comboUIBox.images[SaveScript.stat.combo_level + 1].fillAmount = (float)(SaveScript.saveData.combo_gauge % SaveScript.combo_unit) / SaveScript.combo_unit;

        // Set Star Image & Text
        comboUIBox.images[0].color = colorAsLevel[SaveScript.stat.combo_level];
        comboUIBox.tmp_texts[0].SetText("현재 콤보 레벨 < Lv." + SaveScript.stat.combo_level + " >");
        comboUIBox.tmp_texts[1].SetText("Combo < " + SaveScript.saveData.combo_gauge + " / " + SaveScript.combo_max + " >");

        // Set Reward Content
        for (int i = 0; i < rewardUIBox.Length; i++)
        {
            rewardUIBox[i].texts[0].text = rewardsText[i] + (SaveScript.combo_forces[i] * SaveScript.combo_multiply[GameFuction.GetPlayerGrade()] * 100f) + "% > 증가";
            if (SaveScript.stat.combo_level < i + 1)
            {
                rewardUIBox[i].texts[0].color = new Color(0.3f, 0.2f, 0.3f);
                rewardUIBox[i].images[0].color = new Color(0.15f, 0.1f, 0.15f);
                rewardUIBox[i].images[1].gameObject.SetActive(true);
            }
            else
            {
                rewardUIBox[i].texts[0].color = new Color(1f, 0.8f, 1f);
                rewardUIBox[i].images[0].color = new Color(0.2f, 0.1f, 0.2f);
                rewardUIBox[i].images[1].gameObject.SetActive(false);
            }
        }

        // Set Info Content
        for (int i = 0; i < infoUIBox.Length; i++)
            infoUIBox[i].texts[0].text = infosText[i] + "\n< " + SaveScript.combo_plus[i] +" 점 > ";
    }
}
