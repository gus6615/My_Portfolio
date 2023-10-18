using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashSlotAni : MonoBehaviour
{
    public UIBox uibox;
    public new AudioSource audio;
    private int petSprite_order;

    public void SetAudioVolume(float _volume)
    {
        audio.volume = _volume;
    }

    public void SetAudio_RandomBox()
    {
        audio.clip = SaveScript.SEs[16];
        audio.Play();
    }

    public void SetAudio_box_1()
    {
        audio.clip = SaveScript.SEs[25];
        audio.Play();
    }

    public void SetAudio_box_2()
    {
        audio.clip = SaveScript.SEs[26];
        audio.Play();
    }

    public void SetAudio_egg()
    {
        audio.clip = SaveScript.SEs[27];
        audio.Play();
        switch (petSprite_order++)
        {
            case 0: uibox.images[3].sprite = CashItemAnimator.instance.eggSprites_1[CashItemAnimator.instance.itemType]; break;
            case 1: uibox.images[3].sprite = CashItemAnimator.instance.eggSprites_2[CashItemAnimator.instance.itemType]; break;
            case 2: uibox.images[3].sprite = CashItemAnimator.instance.eggSprites_3[CashItemAnimator.instance.itemType]; break;
        }
    }
}
