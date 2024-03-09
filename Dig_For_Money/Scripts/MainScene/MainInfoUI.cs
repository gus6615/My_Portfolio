using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class MainInfoUI : MonoBehaviour
{
    static public MainInfoUI instance;

    public Canvas infoObject;
    public GameObject[] infoUIObjects;
    public GameObject tempUIObject;
    public GameObject infoPageObject, updateInfoPageObject;
    private GameObject[] infoPages, updateInfoPages;
    public GameObject infoMenuObject, updateInfoMenuObject;
    private Button[] infoMenuButtons, updateInfoMenuButtons;
    public Button infoPreviousButton, infoNextButton;
    public Text infoPageText;
    public Text tempUIText;
    private int infoPageIndex, infoBundleIndex;
    private int[] infoBundles, updateInfoBundles;
    private bool isInfoUIOn;
    private int infoType; // 0 = 메뉴얼, 1 = 업데이트

    Order[] datas;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        datas = infoMenuObject.GetComponentsInChildren<Order>();
        infoBundles = new int[datas.Length];
        infoMenuButtons = new Button[datas.Length];
        for (int i = 0; i < datas.Length; i++)
            infoMenuButtons[i] = datas[i].GetComponent<Button>();

        datas = updateInfoMenuObject.GetComponentsInChildren<Order>();
        updateInfoBundles = new int[datas.Length];
        updateInfoMenuButtons = new Button[datas.Length];
        for (int i = 0; i < datas.Length; i++)
            updateInfoMenuButtons[i] = datas[i].GetComponent<Button>();

        datas = infoPageObject.GetComponentsInChildren<Order>();
        infoPages = new GameObject[datas.Length];
        for (int i = 0; i < datas.Length; i++)
            infoPages[i] = datas[i].gameObject;

        datas = updateInfoPageObject.GetComponentsInChildren<Order>();
        updateInfoPages = new GameObject[datas.Length];
        for (int i = 0; i < datas.Length; i++)
            updateInfoPages[i] = datas[i].gameObject;

        infoBundles[0] = 3;
        infoBundles[1] = 4;
        infoBundles[2] = 3;
        infoBundles[3] = 3;
        infoBundles[4] = 2;
        updateInfoBundles[0] = updateInfoPages.Length - 2;
        updateInfoBundles[1] = 1;
        updateInfoBundles[2] = 1;

        infoObject.gameObject.SetActive(false);
    }

    public void OnOffInfo()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            infoType = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        MainScript.instance.SetAudio(0);

        isInfoUIOn = !isInfoUIOn;
        infoObject.gameObject.SetActive(isInfoUIOn);
        tempUIObject.SetActive(false);
        for (int i = 0; i < infoUIObjects.Length; i++)
            infoUIObjects[i].SetActive(false);
        infoUIObjects[infoType].SetActive(isInfoUIOn);
        infoBundleIndex = 0;
        infoPageIndex = 0;
        SetInfoInfo();
    }

    public void SetInfoInfo()
    {
        infoPreviousButton.gameObject.SetActive(true);
        infoNextButton.gameObject.SetActive(true);
        if (infoPageIndex == 0)
            infoPreviousButton.gameObject.SetActive(false);

        if (infoType == 0)
        {
            for (int i = 0; i < infoMenuButtons.Length; i++)
            {
                infoMenuButtons[i].GetComponent<Image>().color = Color.white;
                infoMenuButtons[i].GetComponentInChildren<Text>().color = Color.white;
            }
            infoMenuButtons[infoBundleIndex].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            infoMenuButtons[infoBundleIndex].GetComponentInChildren<Text>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            for (int i = 0; i < infoPages.Length; i++)
                infoPages[i].SetActive(false);
            infoPages[infoPageIndex].SetActive(true);

            if (infoPageIndex == TotalBundle(infoType, infoMenuButtons.Length - 1))
                infoNextButton.gameObject.SetActive(false);

            infoPageText.text = (infoPageIndex + 1) + " / " + (TotalBundle(infoType, infoMenuButtons.Length - 1) + 1);
        }
        else
        {
            for (int i = 0; i < updateInfoMenuButtons.Length; i++)
            {
                updateInfoMenuButtons[i].GetComponent<Image>().color = Color.white;
                updateInfoMenuButtons[i].GetComponentInChildren<Text>().color = Color.white;
            }
            updateInfoMenuButtons[infoBundleIndex].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            updateInfoMenuButtons[infoBundleIndex].GetComponentInChildren<Text>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            for (int i = 0; i < updateInfoPages.Length; i++)
                updateInfoPages[i].SetActive(false);
            updateInfoPages[infoPageIndex].SetActive(true);

            if (infoPageIndex == TotalBundle(infoType, updateInfoMenuButtons.Length - 1))
                infoNextButton.gameObject.SetActive(false);

            infoPageText.text = (infoPageIndex + 1) + " / " + (TotalBundle(infoType, updateInfoMenuButtons.Length - 1) + 1);
        }
    }

    public void InfoMenuButton()
    {
        MainScript.instance.SetAudio(0);

        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Order>() != null)
            infoBundleIndex = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;

        infoPageIndex = 0;
        for (int i = 0; i < infoBundleIndex; i++)
        {
            if (infoType == 0)
                infoPageIndex += infoBundles[i];
            else
                infoPageIndex += updateInfoBundles[i];
        }
        SetInfoInfo();
    }

    public void InfoPreviousButton()
    {
        MainScript.instance.SetAudio(0);

        infoPageIndex--;
        SetBundleIndex();
        SetInfoInfo();
    }

    public void InfoNextButton()
    {
        MainScript.instance.SetAudio(0);

        infoPageIndex++;
        SetBundleIndex();
        SetInfoInfo();
    }

    public void SetBundleIndex()
    {
        if (infoPageIndex > TotalBundle(infoType, 2))
            infoBundleIndex = 3;
        else if (infoPageIndex > TotalBundle(infoType, 1))
            infoBundleIndex = 2;
        else if (infoPageIndex > TotalBundle(infoType, 0))
            infoBundleIndex = 1;
        else
            infoBundleIndex = 0;

        if (infoType == 0)
        {
            if (infoPageIndex > TotalBundle(infoType, 3))
                infoBundleIndex = 4;
            else if (infoPageIndex > TotalBundle(infoType, 2))
                infoBundleIndex = 3;
        }
    }

    public int TotalBundle(int type, int index)
    {
        int data = -1; // index이기에 -1을 해준다.

        if (type == 0)
        {
            for (int i = 0; i <= index; i++)
                data += infoBundles[i];
        }
        else
        {
            for (int i = 0; i <= index; i++)
                data += updateInfoBundles[i];
        }

        return data;
    }

    public void GetTempNotice()
    {
        if (!Backend.IsInitialized) return;
        bool isUseTempNotice = false;
        string tempContent = "";

        Backend.Notice.GetTempNotice(callback =>
        {
            JsonData data = JsonMapper.ToObject(callback);

            isUseTempNotice = (bool)data["isUse"];
            tempContent = data["contents"].ToString();

            isInfoUIOn = true;
            SetInfoInfo();
            if (isUseTempNotice)
            {
                infoObject.gameObject.SetActive(true);
                infoUIObjects[0].SetActive(false);
                infoUIObjects[1].SetActive(false);
                tempUIObject.SetActive(true);
                tempUIText.text = tempContent;
            }
            else
            {
                infoObject.gameObject.SetActive(false);
                tempUIObject.SetActive(false);
            }
        });
    }
}
