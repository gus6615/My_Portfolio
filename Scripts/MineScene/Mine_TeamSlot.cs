using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mine_TeamSlot : MonoBehaviour
{
    public GameObject existOb;
    public Image exist_obImage, exist_spriteImage;
    public Text exist_nameText, exist_timeText;
    public GameObject noneOb;
    public GameObject menuOb;
    public Button button;

    public int index;
    public static Mine_TeamSlot currentSlot;

    public void SetMenu(bool _isOn)
    {
        if (menuOb != null) menuOb.SetActive(_isOn);
        if (_isOn)
        {
            if (exist_spriteImage != null) exist_spriteImage.color = new Color(1f, 1f, 1f, 0.4f);
            if (exist_obImage != null) exist_obImage.color = new Color(1f, 1f, 1f, 0.4f);
            if (exist_nameText != null) SetText(exist_nameText, 0.5f);
            if (exist_timeText != null) SetText(exist_timeText, 0.5f);
        }
        else
        {
            if (exist_spriteImage != null) exist_spriteImage.color = new Color(1f, 1f, 1f, 1f);
            if (exist_obImage != null) exist_obImage.color = new Color(1f, 1f, 1f, 1f);
            if (exist_nameText != null) SetText(exist_nameText, 1f);
            if (exist_timeText != null) SetText(exist_timeText, 1f);
        }
    }

    public void SetActive(bool _isOn)
    {
        if (menuOb != null) menuOb.SetActive(_isOn);
        if (button != null) button.enabled = _isOn;
        if (_isOn)
        {
            if (exist_spriteImage != null) exist_spriteImage.color = new Color(1f, 1f, 1f, 1f);
            if (exist_obImage != null) exist_obImage.color = new Color(1f, 1f, 1f, 1f);
            if (exist_nameText != null) SetText(exist_nameText, 1f);
            if (exist_timeText != null) SetText(exist_timeText, 1f);
            
        }
        else
        {
            if (exist_spriteImage != null) exist_spriteImage.color = new Color(1f, 1f, 1f, 0f);
            if (exist_obImage != null) exist_obImage.color = new Color(1f, 1f, 1f, 0f);
            if (exist_nameText != null) SetText(exist_nameText, 0f);
            if (exist_timeText != null) SetText(exist_timeText, 0f);
        }
    }

    private Text SetText(Text text, float a)
    {
        Color color = text.color;
        color.a = a;
        text.color = color;

        return text;
    }
}
