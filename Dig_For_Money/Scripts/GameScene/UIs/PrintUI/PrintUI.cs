using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrintUI : MonoBehaviour
{
    static public PrintUI instance;
    public new AudioSource audio;
    [SerializeField] private GameObject expInfo;
    public GameObject itemButton, collectionButton;

    static public bool isGotoMainScene;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void AudioPlay(int _clip)
    {
        audio.clip = SaveScript.SEs[_clip];
        audio.Play();
    }

    public void ExpInfo(int _exp, int _type)
    {
        ExpInfo exp = Instantiate(expInfo, this.transform.position, Quaternion.identity, ObjectPool.instance.objectUI).GetComponent<ExpInfo>();

        PlayerScript.instance.exp += _exp;
        SaveScript.saveData.exp += _exp;
        exp.amount = _exp;
        exp.type = _type;
        AchievementCtrl.instance.SetAchievementAmount(21, _exp);
    }

    /// <summary>
    /// 플레이어가 (_exp)만큼 EXP를 얻을 때, UI 출력 및 데이터 설정
    /// </summary>
    public void ExpInfo(int _exp, bool isCheckDouble)
    {
        ExpInfo exp = Instantiate(expInfo, this.transform.position, Quaternion.identity, ObjectPool.instance.objectUI).GetComponent<ExpInfo>();
        int realExp = _exp;
        int type = 0;
        if (isCheckDouble)
            realExp = GameFuction.GetRealExp(realExp, out type);

        PlayerScript.instance.exp += realExp;
        SaveScript.saveData.exp += realExp;
        exp.amount = realExp;
        exp.type = type;
        AchievementCtrl.instance.SetAchievementAmount(21, realExp);
    }

    /// <summary>
    /// 메인 로비로 이동합니다.
    /// </summary>
    public void GotoMainScene()
    {
        isGotoMainScene = true;
        Time.timeScale = 1f;
        if (SaveScript.saveData.isTutorial)
        {
            isGotoMainScene = false;
            Time.timeScale = 0f;
            AudioPlay(2);
        }
    }
}
