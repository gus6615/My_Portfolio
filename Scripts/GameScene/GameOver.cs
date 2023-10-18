using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject AllUIs;
    [SerializeField] private Image pickImage;
    [SerializeField] private Button reviveButton;
    public Text reviveText, lifeText;
    private bool isGameOverSet;
    private int life;
    
    // Start is called before the first frame update
    void Start()
    {
        isGameOverSet = false;
        AllUIs.SetActive(false);
        life = 5;
        pickImage.sprite = SaveScript.picks[SaveScript.saveData.equipPick].sprites[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerScript.instance.isEnd && BlindScript.isEndChange && !isGameOverSet)
            ShowReviveUI();

        // 광고 관련
        if (GoogleAd.isReward)
        {
            switch (GoogleAd.ADType)
            {
                case 0: // 광고 시청 종료. 광고 보상(부활) 지급
                    SetRevive();
                    GoogleAd.isReward = false;
                    break;
            }
        }
        else
        { 
            if (GoogleAd.ADType == -1)
                GoogleAd.ADType = -2;
        }
    }

    /// <summary>
    /// 광고 제거 유무에 따른 텍스트 변경
    /// </summary>
    private void SetReviveText()
    {
        lifeText.text = "남은 부활 횟수 : " + life + " 회";
        reviveButton.gameObject.SetActive(life != 0);
        if (SaveScript.saveData.isRemoveAD)
            reviveText.text = "수리하기";
        else
            reviveText.text = "광고보기(부활)";
    }

    /// <summary>
    /// 부활 전용 UI를 출력하는 함수
    /// </summary>
    private void ShowReviveUI()
    {
        isGameOverSet = true;
        Time.timeScale = 0f;
        AllUIs.SetActive(true);
        SetReviveText();
    }

    /// <summary>
    /// 부활에 필요한 광고를 사용자에게 보여주는 함수
    /// </summary>
    public void SeeAD()
    {
        Time.timeScale = 1f;
        GoogleAd.instance.ADShow(0);
    }

    /// <summary>
    /// 플레이어를 부활시키는 함수
    /// </summary>
    public void SetRevive()
    {
        AllUIs.SetActive(false);
        isGameOverSet = false;
        life--;

        PlayerScript.instance.isEnd = false;
        PlayerScript.instance.pickHP = PlayerScript.instance.pickFullHP;
        PickStateUI.instance.ShowPickState();
        MacroChecker.instance.CheckOnMacro();
        AttackCtrl.instance.SetInit();
        MoveCtrl.instance.SetInit();
    }
}
