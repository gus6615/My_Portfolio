using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HtmlAgilityPack;
using UnityEngine.EventSystems;

public class MainUpdateUI : MonoBehaviour
{
    static private bool isCheck = false;
    static public bool isNeededUpdate = false;
    public Canvas updateObject;
    public Text updateInfoText;
    private bool[] isCloses;

    // Start is called before the first frame update
    void Start()
    {
        isCloses = new bool[4];
        updateObject.enabled = false;
        if (!isCheck)
        {
            CheckUpdate();
            isCheck = true;
        }
    }

    private void CheckUpdate()
    {
        UnSafeSecurityPolicy.Instate(); // 무결성 검사
        string marketVersion = ""; // 안드로이드 스토어 웹 사이트에 기재된 게임 버전
        char split_ch = '\"'; // 구분자
        bool isFind = false;

        // 아래 URL(내 게임이 기재된 스토어 웹 사이트)의 정보를 불러와 Version을 담는 데이터 참조
        string url = "https://play.google.com/store/apps/details?id=com.CheonnyangCompany.DigForMoney_RTM";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id='yDmH0d']/script");

        // 데이터에서 Version을 추출
        foreach (HtmlNode node in htmlNodes)
        {
            string[] strs = node.InnerText.Split(split_ch);
            for (int i = 0; i < strs.Length; i++)
            {
                if (isFind) break;
                if (strs[i] != null)
                {
                    // 정규표현식을 이용하여 Version(x.x.x or x.x.xx) 형태의 데이터를 탐색
                    if (System.Text.RegularExpressions.Regex.IsMatch(strs[i], @"^\d{1}\.\d{1}\.\d{2}$")
                        || System.Text.RegularExpressions.Regex.IsMatch(strs[i], @"^\d{1}\.\d{1}\.\d{1}$"))
                    {
                        // Version 탐색 성공 및 현재 Version과 비교
                        marketVersion = strs[i];
                        isFind = true;

                        string a = strs[i].ToString();
                        string b = Application.version.ToString();

                        if (a == b)
                        {
                            // 최신 버전과 동일
                            updateObject.enabled = false;
                        }
                        else
                        {
                            // 구 버전
                            updateObject.enabled = true;
                            updateInfoText.text = "현재 업데이트 버전이 있습니다!\n" + "현재 버전 : " + b + "\n패치 버전 : " + a;
                            isNeededUpdate = true;
                            Time.timeScale = 0f;
                        }
                    }
                }
            }
        }
    }

    // 스토어 웹 사이트 오픈
    public void GoUpdate()
    {
        MainScript.instance.SetAudio(0);
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.CheonnyangCompany.DigForMoney_RTM");
    }

    // 업데이트 UI 창 닫기
    public void CloseUpdate()
    {
        bool isClose = true;
        int index = EventSystem.current.currentSelectedGameObject.GetComponent<Order>().order;
        isCloses[index] = true;

        for (int i = 0; i < isCloses.Length; i++)
            if (!isCloses[i])
                isClose = false;

        if (isClose)
        {
            updateObject.enabled = false;
            Time.timeScale = 1f;
        }
    }
}
