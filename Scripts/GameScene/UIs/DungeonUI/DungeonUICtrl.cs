using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonUICtrl : MonoBehaviour
{
    public static DungeonUICtrl instance;
    private static Color infoColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    public new AudioSource audio;
    public Image info;
    private Text infoText;
    public Text eventMapText;

    private void Start()
    {
        instance = this;

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            infoText = info.GetComponentInChildren<Text>();
            info.color = infoText.color = new Color(0f, 0f, 0f, 0f);
            eventMapText.color = new Color(0f, 0f, 0f, 0f);
        }
    }

    public void AudioPlay(int _clip)
    {
        audio.clip = SaveScript.SEs[_clip];
        audio.Play();
    }

    public void SetInfoText(string text, Color color, float fadeStart, float fadeTime)
    {
        infoText.text = text;
        info.color = infoColor;
        infoText.color = color;
        info.GetComponent<FadeUI>().SetFadeValues(0f, fadeStart, fadeTime);
    }
}
