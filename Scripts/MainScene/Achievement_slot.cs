using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement_slot : MonoBehaviour
{
    public Image image;
    public Text nameText, infoText, sliderText;
    public Slider slider;
    public GameObject[] rewardObjects;
    public Order rewardButton;
    private Text[] rewardTexts;

    // Start is called before the first frame update
    void Start()
    {
        rewardTexts = new Text[rewardObjects.Length];
        for (int i = 0; i < rewardObjects.Length; i++)
            rewardTexts[i] = rewardObjects[i].GetComponentInChildren<Text>();
    }
}
