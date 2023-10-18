using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;

public class IconInfoUI : MonoBehaviour
{
    public static IconInfoUI instance;

    static public bool isIconTimeSet;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private GameObject iconPanel;
    public Button onOffButton;
    public Animator animator;
    public new AudioSource audio;
    private Sprite[] icon_frames;
    private List<TouchIcon> icons;

    private bool isOnOff;

    // 임시 데이터들
    TouchIcon[] datas;
    TouchIcon data;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        icon_frames = Resources.LoadAll<Sprite>("Images/Icons/Icon_Frame");
        icons = new List<TouchIcon>();
        
        audio.mute = !SaveScript.saveData.isSEOn;
        InitIconInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (isIconTimeSet)
            SetIconInfo();
    }

    /// <summary>
    /// 아이콘 Panel에 존재하는 모든 Icon을 제거하고 플레이어에게 적용된 모든 버프 아이콘을 생성합니다.
    /// </summary>
    public void InitIconInfo()
    {
        if (iconPanel == null) return;

        datas = iconPanel.GetComponentsInChildren<TouchIcon>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);
        icons.Clear();

        // 주말 이벤트
        if (EventCtrl.instance.isWeekEventOn)
        {
            data = Instantiate(iconPrefab, iconPanel.transform).GetComponent<TouchIcon>();
            data.iconCode = 0;
            data.type = 5;
            data.uibox.images[0].sprite = icon_frames[5];
            data.uibox.images[1].color = data.uibox.images[2].color = data.uibox.images[3].color = new Color(0f, 0f, 0f, 0f);
            data.uibox.texts[0].text = "주말 이벤트!";
            data.uibox.texts[1].text = "1) 경험치(EXP) 2배 획득\n2) " + EventCtrl.instance.GetWeekEventName();
            data.uibox.texts[2].text = "";

            icons.Add(data);
        }

        // 특수 아이템
        for (int i = 0; i < SaveScript.cashEquipmentNum; i++)
        {
            if (SaveScript.saveData.isCashEquipmentOn[i])
            {
                data = Instantiate(iconPrefab, iconPanel.transform).GetComponent<TouchIcon>();
                data.iconCode = i;
                data.type = 4;
                data.uibox.images[0].sprite = data.uibox.images[1].sprite = icon_frames[4];
                data.uibox.images[2].sprite = data.uibox.images[3].sprite = SaveScript.cashEquipments[i].sprite;
                data.uibox.images[1].fillAmount = data.uibox.images[3].fillAmount = 0f;
                data.uibox.texts[0].text = SaveScript.cashEquipments[i].name;
                data.uibox.texts[1].text = SaveScript.cashEquipments[i].info;
                data.uibox.texts[2].text = "";

                icons.Add(data);
            }
        }

        // 룰렛 버프
        for (int i = 0; i < SaveScript.iconNum; i++)
        {
            if (SaveScript.saveData.hasIcons[i])
            {
                data = Instantiate(iconPrefab, iconPanel.transform).GetComponent<TouchIcon>();
                data.iconCode = i;
                data.type = 0;
                data.uibox.images[0].sprite = data.uibox.images[1].sprite = icon_frames[0];
                data.uibox.images[2].sprite = data.uibox.images[3].sprite = SaveScript.icons[i].sprite;
                data.uibox.images[1].fillAmount = data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.iconsTime[i] / SaveScript.iconTime);
                data.uibox.texts[0].text = SaveScript.icons[i].name;
                data.uibox.texts[1].text = SaveScript.icons[i].info;
                data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.iconsTime[i]) + "]";

                icons.Add(data);
            }
        }

        // 마나석 버프
        for (int i = 0; i < SaveScript.manaBufNum; i++)
        {
            if (SaveScript.saveData.isManaBuffOns[i] && i / 3 != 0)
            {
                data = Instantiate(iconPrefab, iconPanel.transform).GetComponent<TouchIcon>();
                data.iconCode = i;
                data.type = 1;
                data.uibox.images[0].sprite = data.uibox.images[1].sprite = icon_frames[1];
                data.uibox.images[2].sprite = data.uibox.images[3].sprite = SaveScript.manaBufs[i].sprite;
                data.uibox.images[2].GetComponent<RectTransform>().sizeDelta = data.uibox.images[3].GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                data.uibox.images[1].fillAmount = data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.manaBufTimes[i] / SaveScript.manaBufTime);
                data.uibox.texts[0].text = SaveScript.manaBufs[i].name;
                data.uibox.texts[1].text = SaveScript.manaBufs[i].info + GameFuction.GetNumText(GameFuction.GetManaBufForceForText(i)) + SaveScript.manaBufs[i].info2;
                data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.manaBufTimes[i]) + "]";

                icons.Add(data);
            }
        }

        // 영약 버프
        for (int i = 0; i < SaveScript.bufItemCodeNum; i++)
        {
            if (SaveScript.saveData.isElixirOns[i])
            {
                data = Instantiate(iconPrefab, iconPanel.transform).GetComponent<TouchIcon>();
                data.iconCode = i;
                data.type = 3;
                data.uibox.images[0].sprite = data.uibox.images[1].sprite = icon_frames[3];
                data.uibox.images[2].sprite = data.uibox.images[3].sprite = SaveScript.elixirs[i].sprite;
                data.uibox.images[2].GetComponent<RectTransform>().sizeDelta = data.uibox.images[3].GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                data.uibox.images[1].fillAmount = data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.elixirTimes[i] / SaveScript.elixirTime);
                data.uibox.texts[0].text = SaveScript.elixirs[i].name;
                data.uibox.texts[1].text = SaveScript.elixirs[i].info;
                data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.elixirTimes[i]) + "]";

                icons.Add(data);
            }
        }

        // 물약 버프
        for (int i = 0; i < SaveScript.bufItemNum; i++)
        {
            if (SaveScript.saveData.isBufItemOns[i])
            {
                data = Instantiate(iconPrefab, iconPanel.transform).GetComponent<TouchIcon>();
                data.iconCode = i;
                data.type = 2;
                data.uibox.images[0].sprite = data.uibox.images[1].sprite = icon_frames[2];
                data.uibox.images[2].sprite = data.uibox.images[3].sprite = SaveScript.bufItems[i].sprite;
                data.uibox.images[2].GetComponent<RectTransform>().sizeDelta = data.uibox.images[3].GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                data.uibox.images[1].fillAmount = data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.bufItemTimes[i] / SaveScript.bufItemTime);
                data.uibox.texts[0].text = SaveScript.bufItems[i].name;
                data.uibox.texts[1].text = SaveScript.bufItems[i].info;
                data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.bufItemTimes[i]) + "]";

                icons.Add(data);
            }
        }

        // GameScene의 경우, 플레이어의 실제 능력치를 재조정합니다.
        if (SceneManager.GetActiveScene().name == "GameScene" && PlayerScript.instance != null)
        {
            // HP관련 버프 재설정
            if (!SaveScript.saveData.isElixirOns[0] && !SaveScript.saveData.isBufItemOns[0] && !SaveScript.saveData.isBufItemOns[1] && !SaveScript.saveData.isBufItemOns[2])
            {
                if (PlayerScript.instance.bufPlusPercentHP != 0f)
                {
                    PlayerScript.instance.plusPercentHP -= PlayerScript.instance.bufPlusPercentHP;
                    PlayerScript.instance.bufPlusPercentHP = 0f;
                    PlayerScript.instance.pickFullHP = (long)(SaveScript.picks[SaveScript.saveData.equipPick].durability 
                        + SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * PlayerScript.instance.plusPercentHP);
                    if (PlayerScript.instance.pickHP > PlayerScript.instance.pickFullHP)
                        PlayerScript.instance.pickHP = PlayerScript.instance.pickFullHP;
                    PickStateUI.instance.ShowPickState();
                }
            }
            // 이속 버프 재설정
            if (!SaveScript.saveData.isElixirOns[8] && !SaveScript.saveData.isBufItemOns[24] && !SaveScript.saveData.isBufItemOns[25] && !SaveScript.saveData.isBufItemOns[26])
            {
                if (PlayerScript.instance.bufMoveSpeed != 0f)
                {
                    PlayerScript.instance.moveSpeed -= PlayerScript.instance.bufMoveSpeed;
                    PlayerScript.instance.moveSpeedData -= PlayerScript.instance.bufMoveSpeed;
                    PlayerScript.instance.bufMoveSpeed = 0f;
                }
            }
            // 점프력 버프 재설정
            if (!SaveScript.saveData.isElixirOns[9] && !SaveScript.saveData.isBufItemOns[27] && !SaveScript.saveData.isBufItemOns[28] && !SaveScript.saveData.isBufItemOns[29])
            {
                if (PlayerScript.instance.bufJumpPower != 0f)
                {
                    PlayerScript.instance.jumpPower -= PlayerScript.instance.bufJumpPower;
                    PlayerScript.instance.jumpPowerData -= PlayerScript.instance.bufJumpPower;
                    PlayerScript.instance.bufJumpPower = 0f;
                }
            }
        }

        if (icons.Count > 0) onOffButton.gameObject.SetActive(true);
        else onOffButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이콘 Panel에 존재하는 모든 아이콘의 정보를 다시 출력합니다.
    /// </summary>
    public void SetIconInfo()
    {
        isIconTimeSet = false;
        if (icons.Count > 0) onOffButton.gameObject.SetActive(true);
        else
        {
            onOffButton.gameObject.SetActive(false);
            isOnOff = false;
            animator.SetBool("isOn", isOnOff);
        }

        if (SaveScript.isChangedIcons)
        {
            // 시간이 지남에 따라 어느 버프가 종료되었을 경우, 새롭게 아이콘을 구축합니다.
            SaveScript.isChangedIcons = false;
            InitIconInfo();
        }
        else
        {
            for (int i = 0; i < icons.Count; i++)
            {
                if (icons[i].type == 5)
                    continue; // 주말 이벤트
                data = icons[i];

                switch (data.type)
                {
                    case 0:
                        data.uibox.images[1].fillAmount = 1f - ((float)SaveScript.saveData.iconsTime[data.iconCode] / SaveScript.iconTime);
                        data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.iconsTime[data.iconCode] / SaveScript.iconTime);
                        if (data.isOpenInfo) data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.iconsTime[data.iconCode]) + "]";
                        break;
                    case 1:
                        data.uibox.images[1].fillAmount = 1f - ((float)SaveScript.saveData.manaBufTimes[data.iconCode] / SaveScript.manaBufTime);
                        data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.manaBufTimes[data.iconCode] / SaveScript.manaBufTime);
                        if (data.isOpenInfo) data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.manaBufTimes[data.iconCode]) + "]";
                        break;
                    case 2:
                        data.uibox.images[1].fillAmount = 1f - ((float)SaveScript.saveData.bufItemTimes[data.iconCode] / SaveScript.bufItemTime);
                        data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.bufItemTimes[data.iconCode] / SaveScript.bufItemTime);
                        if (data.isOpenInfo) data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.bufItemTimes[data.iconCode]) + "]";
                        break;
                    case 3:
                        data.uibox.images[1].fillAmount = 1f - ((float)SaveScript.saveData.elixirTimes[data.iconCode] / SaveScript.elixirTime);
                        data.uibox.images[3].fillAmount = 1f - ((float)SaveScript.saveData.elixirTimes[data.iconCode] / SaveScript.elixirTime);
                        if (data.isOpenInfo) data.uibox.texts[2].text = "[" + GameFuction.GetTimeText(SaveScript.saveData.elixirTimes[data.iconCode]) + "]";
                        break;
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            // 영약 자동 사용 체크
            if (SaveScript.saveData.isAutoUse_2)
            {
                for (int i = 0; i < SaveScript.bufItemCodeNum; i++)
                {
                    if (i == 4 || i == 5 || i == 11) continue; // (상점 판매 & 경험치 & 강화 제외)
                    if (SaveScript.saveData.isElixirOns[i] || SaveScript.saveData.isBufItemOns[i * 3] || SaveScript.saveData.isBufItemOns[i * 3 + 1]
                        || SaveScript.saveData.isBufItemOns[i * 3 + 2]) continue; // 물약 적용 중

                    if (SaveScript.saveData.hasElixirs[i] > 0)
                    {
                        SaveScript.saveData.hasElixirs[i]--;
                        SaveScript.saveData.elixirTimes[i] = SaveScript.elixirTime;
                        SaveScript.saveData.isElixirOns[i] = true;
                        AchievementCtrl.instance.SetAchievementAmount(25, 1);
                        // 퀘스트
                        QuestCtrl.instance.SetMainQuestAmount(new int[] { 16 });

                        SaveScript.isChangedIcons = true;
                    }
                }
            }

            // 자동 사용 체크
            if (SaveScript.saveData.isAutoUse_1)
            {
                for (int i = 0; i < SaveScript.bufItemCodeNum; i++)
                {
                    int index = i * SaveScript.bufItemTypeNum;
                    if (i == 4 || i == 5 || i == 11) continue; // (상점 판매 & 경험치 & 강화 제외)
                    if (SaveScript.saveData.isElixirOns[i] || SaveScript.saveData.isBufItemOns[index] || SaveScript.saveData.isBufItemOns[index + 1]
                        || SaveScript.saveData.isBufItemOns[index + 2]) continue; // 물약 적용 중

                    for (int j = SaveScript.bufItemTypeNum - 1; j >= 0; j--)
                    {
                        int real_index = index + j;
                        if (SaveScript.saveData.hasBufItems[real_index] > 0)
                        {
                            SaveScript.saveData.hasBufItems[real_index]--;
                            SaveScript.saveData.bufItemTimes[real_index] = SaveScript.bufItemTime;
                            SaveScript.saveData.isBufItemOns[real_index] = true;
                            AchievementCtrl.instance.SetAchievementAmount(25, 1);
                            // 퀘스트
                            QuestCtrl.instance.SetMainQuestAmount(new int[] { 16 });

                            SaveScript.isChangedIcons = true;
                            break;
                        }
                    }
                }
            }

            GameFuction.SetPlayerStat();
        }
    }

    public void OnOffButton()
    {
        isOnOff = !isOnOff;
        animator.SetBool("isOn", isOnOff);
        audio.clip = SaveScript.SEs[0];
        audio.Play();
    }
}
