using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainSettingUI : MonoBehaviour
{
    public static MainSettingUI instance;
    private static string[] options = { "제한 없음", "144 FPS", "120 FPS", "90 FPS", "60 FPS", "30 FPS" };
    private static int change_price = 500;

    [SerializeField] private Canvas settingObject;
    [SerializeField] private GameObject BGMSet, SESet, BlockSoundSet;
    [SerializeField] private Dropdown fpsSet;
    [SerializeField] private GameObject nickname_object;
    [SerializeField] private InputField nickname_input;
    [SerializeField] private UIBox nickname_button;
    [SerializeField] private GameObject nickname_reconfirm;
    [SerializeField] private Text nickname_infoText;
    [SerializeField] private Text nickname_errorText;
    private Image[] BGMSetImages, SESetImages, BlockSoundImages;
    private Dropdown.OptionData option;
    private bool isOnOffChangeNickname;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        BGMSetImages = BGMSet.GetComponentsInChildren<Image>();
        SESetImages = SESet.GetComponentsInChildren<Image>();
        BlockSoundImages = BlockSoundSet.GetComponentsInChildren<Image>();

        settingObject.enabled = false;
    }

    public void OpenSetting()
    {
        MainScript.instance.SetAudio(0);
        settingObject.enabled = true;
        SettingSetInfo();
    }

    public void CloseSetting()
    {
        MainScript.instance.SetAudio(0);
        settingObject.enabled = false;
        SaveScript.instance.SaveData_Asyn(false);
    }

    public void SettingSetInfo()
    {
        BGMSetImages[1].gameObject.SetActive(SaveScript.saveData.isBGMOn);
        BGMSetImages[2].gameObject.SetActive(!SaveScript.saveData.isBGMOn);
        SESetImages[1].gameObject.SetActive(SaveScript.saveData.isSEOn);
        SESetImages[2].gameObject.SetActive(!SaveScript.saveData.isSEOn);
        BlockSoundImages[1].gameObject.SetActive(SaveScript.saveData.isBlockSoundOn);
        BlockSoundImages[2].gameObject.SetActive(!SaveScript.saveData.isBlockSoundOn);
        SetDropdownOptions();
        fpsSet.template.gameObject.SetActive(false);
        fpsSet.value = SaveScript.saveData.fpsType;
        fpsSet.captionText.text = options[fpsSet.value];
        nickname_object.SetActive(false);
        nickname_reconfirm.SetActive(false);
    }

    public void BGMSetButton()
    {
        MainScript.instance.SetAudio(0);
        SaveScript.saveData.isBGMOn = !SaveScript.saveData.isBGMOn;
        SettingSetInfo();
        BlindScript.instance.audio.mute = !SaveScript.saveData.isBGMOn;
    }

    public void SESetButton()
    {
        MainScript.instance.SetAudio(0);
        SaveScript.saveData.isSEOn = !SaveScript.saveData.isSEOn;

        SettingSetInfo();
        MainScript.instance.audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void BlockSoundSetButton()
    {
        MainScript.instance.SetAudio(0);
        SaveScript.saveData.isBlockSoundOn = !SaveScript.saveData.isBlockSoundOn;
        SettingSetInfo();
    }

    private void SetDropdownOptions()
    {
        fpsSet.options.Clear();
        for (int i = 0; i < options.Length; i++)
        {
            option = new Dropdown.OptionData();
            option.text = options[i];
            fpsSet.options.Add(option);
        }
    }

    public void SelectFPS()
    {
        SaveScript.saveData.fpsType = fpsSet.value;
        SaveScript.SetFPS();
        SettingSetInfo();
    }

    public void LogOut()
    {
        CloseSetting();
        StartCoroutine(BlindScript.instance.Fade_LogOut());
    }

    public void OnOffChangeNickname()
    {
        isOnOffChangeNickname = !isOnOffChangeNickname;
        nickname_object.SetActive(isOnOffChangeNickname);
        if (isOnOffChangeNickname)
        {
            // Setting UI
            nickname_input.text = "";
            nickname_errorText.text = "";
            if (SaveScript.saveData.cash >= 500)
            {
                nickname_button.images[0].color = new Color(1f, 0.4f, 0.4f, 1f);
                nickname_button.images[1].color = Color.white;
                nickname_button.texts[0].color = Color.white;
            }
            else
            {
                nickname_button.images[0].color = new Color(0.7f, 0.4f, 0.4f, 0.4f);
                nickname_button.images[1].color = new Color(1f, 1f, 1f, 0.6f);
                nickname_button.texts[0].color = new Color(1f, 1f, 1f, 0.6f);
            }
        }

        MainScript.instance.SetAudio(0);
    }

    public void OnBuyNickname()
    {
        MainScript.instance.SetAudio(2);
        if (SaveScript.saveData.cash >= 500)
        {
            string nickname = nickname_input.text;
            // 길이 예외 처리
            if (nickname == "" || nickname.Length < 2 || nickname.Length > 8)
            {
                nickname_errorText.text = "※ 닉네임의 길이를 2자리 ~ 8자리 사이로 정해주세요!";
                return;
            }

            if (Regex.IsMatch(nickname, "^[0-9a-zA-Z가-힣]*$") == false)
            {
                nickname_errorText.text = "※ 닉네임에는 영어, 한글, 숫자만 가능합니다! (특수문자 및 공백 불가능)";
                return;
            }

            MainScript.instance.SetAudio(0);
            nickname_reconfirm.SetActive(true);
            nickname_infoText.text = "닉네임을 < " + nickname_input.text + " >\n로 변경하시겠습니까?";
        }
        else
        {
            SystemInfoCtrl.instance.SetErrorInfo("레드 다이아가 부족합니다!");
        }
    }

    public void OnChangeYes()
    {
        string nickname = nickname_input.text;
        BackendReturnObject BRO = Backend.BMember.UpdateNickname(nickname);
        switch (BRO.GetStatusCode())
        {
            // 성공
            case "204":
                SaveScript.saveData.cash -= change_price;
                SystemInfoCtrl.instance.SetShowInfo("닉네임 변경에 성공하였습니다! 게임을 재시작하시면 반영이 됩니다 :)");
                MainScript.instance.SetAudio(0);
                CloseSetting();
                break;
            // 중복된 닉네임
            case "409":
                nickname_errorText.text = "※ 이 닉네임은 현재 사용중입니다!";
                break;
            // 앞/뒤 공백이 있는 경우
            case "400":
                nickname_errorText.text = "※ 앞/뒤에 공백이 존재합니다! 공백을 제거해주세요.";
                break;
            // 그 외
            default:
                nickname_errorText.text = "※ 알 수 없는 문제가 발생했습니다. 에러 코드: " + BRO.GetErrorCode();
                break;
        }
    }

    public void OnChangeNo()
    {
        MainScript.instance.SetAudio(0);
        nickname_reconfirm.SetActive(false);
    }
}
