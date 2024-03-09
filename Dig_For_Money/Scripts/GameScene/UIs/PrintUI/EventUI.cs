using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventUI : MonoBehaviour
{
    public static EventUI instance;

    public GameObject eventNextButton, eventYesButton, eventNoButton;
    public GameObject eventBox;
    public GameObject eventItemInfo;
    public Image eventItemImage;
    public TMP_Text eventText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        eventNextButton.SetActive(false);
        eventBox.SetActive(false);
        eventItemInfo.SetActive(false);
    }
}
