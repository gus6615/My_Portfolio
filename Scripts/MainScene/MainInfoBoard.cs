using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainInfoBoard : MonoBehaviour
{
    private static MainInfoBoard Instance;
    public static MainInfoBoard instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }

    public Canvas infoBoardObject;
    public Image whiteBoardImage;

    [SerializeField] private Image weekEventImage;
    [SerializeField] private TMP_Text weekNameText, weekInfoText;
    [SerializeField] private Sprite weekEventOnSprite, weekEventOffSprite;

    private bool isOn; // 켜져 있는가?

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        infoBoardObject.gameObject.SetActive(false);
    }

    public void OnOffInfoBoard()
    {
        if (!MainScript.isChangeScene)
        {
            isOn = !isOn;
            infoBoardObject.gameObject.SetActive(isOn);
            MainScript.instance.SetAudio(0);
        }
    }

    public void SetBoardInfo()
    {
        if (EventCtrl.instance.isWeekEventOn)
        {
            weekNameText.SetText("<color=black>매주 <color=red><토, 일> <color=black>마다 아래 효과를 받습니다! <color=blue> [활성]");
            weekInfoText.SetText("<color=black>[1] (기본) 경험치(EXP) 2배 획득\r\n\r\n<color=red>[2] " + EventCtrl.instance.GetWeekEventName());
            weekInfoText.color = new Color(1f, 1f, 1f, 1f);
            whiteBoardImage.sprite = weekEventOnSprite;
        }
        else
        {
            weekNameText.SetText("<color=black>매주 <color=red><토, 일> <color=black>마다 아래 효과를 받습니다! <color=grey> [비활성]");
            weekInfoText.SetText("<color=black>[1] (기본) 경험치(EXP) 2배 획득\r\n\r\n<color=red>[2] " + EventCtrl.instance.GetWeekEventName());
            weekInfoText.color = new Color(1f, 1f, 1f, 0.4f);
            whiteBoardImage.sprite = weekEventOffSprite;
        }
    }
}
