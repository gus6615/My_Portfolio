using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct DialogSet
{
    /// <summary> 남주 대사 </summary>
    [SerializeField] private Dialog Player_M_dialog;

    /// <summary> 여주 대사 </summary>
    [SerializeField] private Dialog Player_W_dialog;

    public DialogType GetDialogType(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetDialogType();
        else return Player_W_dialog.GetDialogType();
    }

    public AudioClip GetAudioClip(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetAudioClip();
        else return Player_W_dialog.GetAudioClip();
    }

    public Sprite GetLeftSprite(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetLeftSprite();
        else return Player_W_dialog.GetLeftSprite();
    }

    public Sprite GetRightSprite(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetRightSprite();
        else return Player_W_dialog.GetRightSprite();
    }

    public string GetWords(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetWord();
        else return Player_W_dialog.GetWord();
    }

    public float GetPrintTime(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetPrintTime();
        else return Player_W_dialog.GetPrintTime();
    }
}

public enum DialogType
{
    Interaction,
    PlayerM,
    PlayerW
}


[Serializable]
public struct Dialog
{
    /// <summary> 대화 타입 (0 = 상호작용, 1 = 남주, 2 = 여주) </summary>
    [SerializeField] private DialogType type;

    /// <summary> 대사 출력 시 등장하는 오디오 (NULL = 오디오 없음) </summary>
    [SerializeField] private AudioClip audioClip;

    /// <summary> 대사 출력 시 왼쪽 이미지 (NULL = 이미지 없음) </summary>
    [SerializeField] private Sprite leftSprite;

    /// <summary> 대사 출력 시 오른쪽 이미지 (NULL = 이미지 없음) </summary>
    [SerializeField] private Sprite rightSprite;

    /// <summary> 상호작용 문장 </summary>
    [SerializeField] [TextArea] private string words;

    /// <summary> 대사 출력 속도 </summary>
    [SerializeField] private float print_time;

    public DialogType GetDialogType() => type;

    public AudioClip GetAudioClip() => audioClip;

    public Sprite GetLeftSprite() => leftSprite;

    public Sprite GetRightSprite() => rightSprite;

    public string GetWord() => words;

    public float GetPrintTime() => print_time;
}