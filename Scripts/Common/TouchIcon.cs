using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchIcon : MonoBehaviour
{
    static public TouchIcon instance;

    [SerializeField] private GameObject infoObject;
    public UIBox uibox;
    public bool isOpenInfo;
    public int iconCode;
    public int type; // 0 = 룰렛 버프, 1 = 마나석 버프, 2 = 물약 버프, 3 = 영약 버프, 4 = 특수 아이템

    private void Start()
    {
        infoObject.SetActive(false);
        StartCoroutine("CoWaitForPosition");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject != this.gameObject)
            {
                instance = null;
                isOpenInfo = false;
                infoObject.SetActive(isOpenInfo);
            }
        }
    }

    public void ButtonOn()
    {
        instance = this;
        instance.isOpenInfo = !instance.isOpenInfo;
        instance.infoObject.SetActive(isOpenInfo);
        if (!isOpenInfo) instance = null;
        if(isOpenInfo)
        {
            switch (type)
            {
                case 0: uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.iconsTime[iconCode]) + "]"; break;
                case 1: uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.manaBufTimes[iconCode]) + "]"; break;
                case 2: uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.bufItemTimes[iconCode]) + "]"; break;
                case 3: uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.elixirTimes[iconCode]) + "]"; break;
            }
        }
    }

    IEnumerator CoWaitForPosition()
    {
        yield return new WaitForEndOfFrame();
    }
}
