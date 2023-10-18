using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDetectorCtrl : MonoBehaviour
{
    private new AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void SetAudio_Detector()
    {
        audio.clip = SaveScript.SEs[43];
        audio.Play();
    }
}
