using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MineInfo : MonoBehaviour
{
    static public MineInfo instance;

    public GameObject infoPageObject;
    private GameObject[] infoPages;
    public Button infoPreviousButton, infoNextButton;
    public Text infoPageText;
    private int infoPageIndex;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Order[] datas = infoPageObject.GetComponentsInChildren<Order>();
        infoPages = new GameObject[datas.Length];
        for (int i = 0; i < datas.Length; i++)
            infoPages[i] = datas[i].gameObject;
    }

    public void SetDefaultVariable()
    {
        infoPageIndex = 0;
    }

    public void SetInfoInfo()
    {
        infoPreviousButton.gameObject.SetActive(true);
        infoNextButton.gameObject.SetActive(true);

        for (int i = 0; i < infoPages.Length; i++)
            infoPages[i].SetActive(false);
        infoPages[infoPageIndex].SetActive(true);

        if (infoPageIndex == 0)
            infoPreviousButton.gameObject.SetActive(false);
        if (infoPageIndex == infoPages.Length - 1)
            infoNextButton.gameObject.SetActive(false);

        infoPageText.text = (infoPageIndex + 1) + " / " + infoPages.Length;
    }

    public void InfoPreviousButton()
    {
        Mine.instance.SetAudio(0);

        infoPageIndex--;
        SetInfoInfo();
    }

    public void InfoNextButton()
    {
        Mine.instance.SetAudio(0);

        infoPageIndex++;
        SetInfoInfo();
    }
}
