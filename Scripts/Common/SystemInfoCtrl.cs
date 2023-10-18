using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Device;

public class SystemInfoCtrl : MonoBehaviour
{
    private static SystemInfoCtrl Instance;
    public static SystemInfoCtrl instance
    {
        get 
        { 
            return Instance; 
        }
        set 
        { 
            if (Instance == null)
                Instance = value; 
        }
    }

    [SerializeField]
    private FadeUI errorSystem, showSystem;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetErrorInfo(string _text, float _fadeInTime, float _idleTime, float _fadeOutTime)
    {
        errorSystem.uiBox.images[0].color = Color.white;
        errorSystem.uiBox.tmp_texts[0].color = Color.black;
        errorSystem.uiBox.tmp_texts[0].text = _text;
        errorSystem.SetFadeValues(_fadeInTime, _idleTime, _fadeOutTime);
    }

    public void SetErrorInfo(string _text)
    {
        errorSystem.uiBox.images[0].color = Color.white;
        errorSystem.uiBox.tmp_texts[0].color = Color.black;
        errorSystem.uiBox.tmp_texts[0].text = _text;
        errorSystem.SetFadeValues(0.25f, 2.5f, 0.25f);
    }

    public void SetShowInfo(string _text, float _fadeInTime, float _idleTime, float _fadeOutTime)
    {
        showSystem.uiBox.images[0].color = new Color(0f, 0f, 0f, 0.6f);
        showSystem.uiBox.tmp_texts[0].color = Color.white;
        showSystem.uiBox.tmp_texts[0].text = _text;
        showSystem.SetFadeValues(_fadeInTime, _idleTime, _fadeOutTime);
    }

    public void SetShowInfo(string _text)
    {
        showSystem.uiBox.images[0].color = new Color(0f, 0f, 0f, 0.6f);
        showSystem.uiBox.tmp_texts[0].color = Color.white;
        showSystem.uiBox.tmp_texts[0].text = _text;
        showSystem.SetFadeValues(0.25f, 1f, 0.25f);
    }
}
