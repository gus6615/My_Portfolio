using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using BackEnd.Tcp;

public class BlindScript : MonoBehaviour
{
    private const int SHOWAD_TIME = 60 * 20;

    static public BlindScript instance;
    static public bool isEndChange, isEndSwitchPos;
    static public bool isStartGame; // 초기 로딩 이미지가 뜰 경우 true로 변환

    public Animator animator;
    public new AudioSource audio;

    public GameObject loadObject;
    public Image blindImage;
    public int spawnType; // 0 = 1층, 1 = 6층
    private int currentSceneIndex;
    private bool isFade;
    private bool isStartFade;
    private float fadeTime, fadeTime_default;

    public bool isShowAD;

    /*****************************************
    0. MainScene 
    1. GameScene
    2. ShopScene
    3. UpgradeScene
    4. TutorialScene
    5. MineScene
    *****************************************/

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            fadeTime_default = 0.75f;
            isEndSwitchPos = true;
            isShowAD = false;

            StartCoroutine("CheckShowAD");
            StartCoroutine(Fade_Init());
            audio.volume = 0f;
            BGMSetting(0, false, 1f);
            audio.Play();
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (isStartFade)
            BGMFade();

        if (isEndChange)
        {
            if (currentSceneIndex == 0) // MainScene
            {
                if (MainScript.isGotoGameScene)
                {
                        currentSceneIndex = 1;
                        if (spawnType == 0)
                            StartCoroutine(switchScene(currentSceneIndex, 1, fadeTime_default, fadeTime_default));
                        else
                            StartCoroutine(switchScene(currentSceneIndex, 6, fadeTime_default, fadeTime_default));
                        MainScript.isGotoGameScene = false;
                }
                else if (MainScript.isGotoShopScene)
                {
                    currentSceneIndex = 2;
                    StartCoroutine(switchScene(currentSceneIndex, 2, fadeTime_default, fadeTime_default));
                    MainScript.isGotoShopScene = false;
                }
                else if (MainScript.isGotoUpgradeScene)
                {
                    currentSceneIndex = 3;
                    StartCoroutine(switchScene(currentSceneIndex, 3, fadeTime_default, fadeTime_default));
                    MainScript.isGotoUpgradeScene = false;
                }
                else if (MainScript.isGotoMineScene)
                {
                    currentSceneIndex = 5;
                    StartCoroutine(switchScene(currentSceneIndex, 5, fadeTime_default, fadeTime_default));
                    MainScript.isGotoMineScene = false;
                }
            }
            else if (currentSceneIndex == 1) // GameScene
            {
                if (PrintUI.isGotoMainScene)
                {
                    currentSceneIndex = 0;
                    StartCoroutine(switchScene(currentSceneIndex, 0, fadeTime_default, fadeTime_default));
                    PrintUI.isGotoMainScene = false;
                }
            }
            else if (currentSceneIndex == 2) // ShopScene
            {
                if (Shop.isGotoMainScene)
                {
                    currentSceneIndex = 0;
                    StartCoroutine(switchScene(currentSceneIndex, 0, fadeTime_default, fadeTime_default));
                    Shop.isGotoMainScene = false;
                }
            }
            else if (currentSceneIndex == 3) // UpgradeScene
            {
                if (Inventory.isGotoMainScene)
                {
                    currentSceneIndex = 0;
                    StartCoroutine(switchScene(currentSceneIndex, 0, fadeTime_default, fadeTime_default));
                    Inventory.isGotoMainScene = false;
                }
            }
            else if (currentSceneIndex == 4) // TutorialScene
            {
                if (PrintUI.isGotoMainScene)
                {
                    currentSceneIndex = 0;
                    StartCoroutine(switchScene(currentSceneIndex, 0, fadeTime_default, fadeTime_default));
                    PrintUI.isGotoMainScene = false;
                }
            }
            else if (currentSceneIndex == 5) // MineScene
            {
                if (Mine.isGotoMainScene)
                {
                    currentSceneIndex = 0;
                    StartCoroutine(switchScene(currentSceneIndex, 0, fadeTime_default, fadeTime_default));
                    Mine.isGotoMainScene = false;
                }
            }
        }
    }

    IEnumerator Fade_Init()
    {
        animator.SetBool("isFadeOn", false);
        animator.SetBool("isFadeOff", true);

        yield return new WaitForSeconds(1f);

        FadeEnd();
        isStartGame = true;
    }

    public IEnumerator Fade_Game()
    {
        BackEndLoginManager.instance.SetGame();
        if (SaveScript.saveData.isTutorial)
        {
            StartCoroutine(Fade_Tutorial());
            yield break;
        }
        FadeIn();

        yield return new WaitForSeconds(1f);

        BackEndLoginManager.instance.loginCanvas.SetActive(false);
        FadeOut();

        yield return new WaitForSeconds(1f);

        FadeEnd();
    }

    IEnumerator Fade_Tutorial()
    {
        if (MainUpdateUI.isNeededUpdate)
            yield break;
        BGMSetting(-1, true, 1f);
        FadeIn();

        yield return new WaitForSeconds(1f);

        currentSceneIndex = 4;
        SetActiveAllObject.instance.isDone = false;
        SceneManager.LoadScene(currentSceneIndex);

        while (!SetActiveAllObject.instance.isDone)
            yield return null;

        BGMSetting(1, false, 1f);
        audio.Play();
        FadeOut();
        BackEndLoginManager.instance.AllClose();
        QuestCtrl.instance.SetUI(false);
        MacroChecker.instance.SetDefaultUI();
        MacroChecker.instance.CheckOnMacro();
        SaveScript.stat.SetStat();

        yield return new WaitForSeconds(1f);

        FadeEnd();
    }

    public IEnumerator Fade_LogOut()
    {
        FadeIn();

        yield return new WaitForSeconds(1f);

        Backend.Chat.LeaveChannel(ChannelType.Public);
        Backend.BMember.Logout();
        BackEndLoginManager.instance.GotoLogin();
        BackEndLoginManager.userData = new UserData();
        FadeOut();

        yield return new WaitForSeconds(1f);

        FadeEnd();
    }

    public IEnumerator Fade_LoginFail()
    {
        FadeIn();

        yield return new WaitForSeconds(1f);

        loadObject.SetActive(false);
        FadeOut();

        yield return new WaitForSeconds(1f);

        FadeEnd();
    }

    public void FadeIn()
    {
        animator.SetBool("isFadeOn", true);
        animator.SetBool("isFadeOff", false);
        blindImage.raycastTarget = true;
        isEndChange = false;
    }

    public void FadeOut()
    {
        animator.SetBool("isFadeOn", false);
        animator.SetBool("isFadeOff", true);
        StartCoroutine(SetLoadImage());
    }

    public IEnumerator SetLoadImage()
    {
        while (!SaveScript.saveRank.isDone)
            yield return null;
        loadObject.SetActive(false);
    }

    public void FadeEnd()
    {
        animator.SetBool("isFadeOn", false);
        animator.SetBool("isFadeOff", false);
        blindImage.raycastTarget = false;
        isEndChange = true;
    }

    public IEnumerator CheckShowAD()
    {
        yield return new WaitForSeconds(SHOWAD_TIME);

        isShowAD = true;
    }

    public void DisableShowAD()
    {
        isShowAD = false;
        StopCoroutine("CheckShowAD");
        StartCoroutine("CheckShowAD");
    }

    // BGM이 변하면서 씬을 변경
    public IEnumerator switchScene(int Sceneindex, int BGMindex, float fadeInTime, float fadeOutTime)
    {
        FadeIn();
        animator.speed = 1f / fadeInTime;
        BGMSetting(-1, true, fadeInTime);
        SaveScript.instance.SaveData_Asyn(false);

        yield return new WaitForSeconds(fadeInTime);

        if (isShowAD)
        {
            // 광고 타임
            GoogleAd.instance.ADShow(0);
        }
        SetActiveAllObject.instance.isDone = false;
        SceneManager.LoadScene(Sceneindex);
        animator.speed = 1f / fadeOutTime;
        BGMSetting(BGMindex, false, fadeOutTime);
        audio.Play();

        while (!SetActiveAllObject.instance.isDone)
            yield return null;

        FadeOut();
        BackEndLoginManager.instance.AllClose();
        QuestCtrl.instance.SetUI(false);
        MacroChecker.instance.SetDefaultUI();
        MacroChecker.instance.CheckOnMacro();
        SaveScript.stat.SetStat();
        if (Tutorial.isEndTutorial)
        {
            Tutorial.isEndTutorial = false;
            SaveScript.saveData.isTutorial = false;
            QuestCtrl.instance.SetUI(true);
        }

        yield return new WaitForSeconds(fadeOutTime);

        FadeEnd();
    }

    public IEnumerator switchBGM(int BGMindex, float fadeInTime, float fadeOutTime)
    {
        BGMSetting(-1, true, fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
        BGMSetting(BGMindex, false, fadeOutTime);
        audio.Play();
    }

    public int GetStageBGM()
    {
        if (MapCreater.instance.is_10th) return 8;
        else if (MapCreater.instance.is_6th) return 6;
        else return 1;
    }

    // BGM이 변하면서 위치 이동
    public IEnumerator switchPos(Vector3 _pos, int BGMindex, float fadeInTime, float fadeOutTime)
    {
        FadeIn();
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn)
            AutoPlayCtrl.instance.SetInit();
        MoveCtrl.instance.SetInit();
        AttackCtrl.instance.SetInit();
        PlayerScript.instance.isNoDamage = true;
        PlayerScript.instance.isCanCtrl = false;
        PlayerScript.instance.animator.SetBool("isMove", false);
        PlayerScript.instance.handsAnimator.SetBool("isMove", false);
        animator.speed = 1f / fadeInTime;
        isEndSwitchPos = false;
        BGMSetting(-1, true, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        FadeOut();
        PlayerScript.instance.transform.position = _pos;
        PlayerScript.instance.isCanCtrl = true;
        Camera.main.transform.position = _pos + new Vector3Int(0, 0, -10);
        animator.speed = 1f / fadeOutTime;
        BGMSetting(BGMindex, false, fadeOutTime);
        audio.Play();

        switch (EventButton.mainType)
        {
            case 0: // 특수 던전 들어가기
                PlayerScript.instance.SetLight(1);
                PlayerScript.instance.SetButtonMode(false, true, false, true);
                DungeonCreater.dungeon_0_floor = 0;
                DungeonUICtrl.instance.SetInfoText("- 유적 던전 로비 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                PlayerScript.instance.isDungeon_0_On = true;
                break;
            case 1: // 특수 던전 나가기
                PlayerScript.instance.SetLight(0);
                PlayerScript.instance.SetButtonMode(false, true, true, true);
                DungeonUICtrl.instance.SetInfoText("", new Color(0.9f, 0.9f, 0.9f, 0f), 0f, 0f);
                PlayerScript.instance.isDungeon_0_On = false;
                MapData.instance.GetTileMap(DungeonCreater.dungeon_0_startPos, 5).SetTile(DungeonCreater.dungeon_0_startPos, MapData.instance.dungeon_0_DecoX64Tiles[43 + DungeonCreater.dungeon_0_type * 2]); // 닫힌 문 설치
                DungeonCreater.instance.DeleteDungeon_0();
                ObjectPool.ReturnObject<EventBlock>(12, DungeonCreater.dungeon_0_entranceEvent.GetComponent<EventBlock>());
                break;
            case 7: // 이벤트 맵 들어가기
                Camera.main.backgroundColor = Color.black;
                PlayerScript.instance.isEventMap_On = true;
                PlayerScript.instance.isCanCtrl = false;
                PlayerScript.instance.SetButtonMode(false, false, false, false);
                EventMap.instance.StartCountDown();
                EventMap.instance.InfoInit();
                break;
            case 8: // 이벤트 맵 나가기
                PlayerScript.instance.isEventMap_On = false;
                PlayerScript.instance.SetButtonMode(false, true, true, true);
                EventMap.instance.DeleteMap();
                break;
            case 9: // 고대 던전 들어가기
                PlayerScript.instance.SetLight(2);
                PlayerScript.instance.SetButtonMode(false, false, false, false);
                PlayerScript.instance.d_1_currentFloor = 0;
                DungeonUICtrl.instance.SetInfoText("- 고대 던전 로비 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                TorchUICtrl.instance.SetInit();
                PlayerScript.instance.isDungeon_1_On = true;
                break;
            case 10: // 고대 던전 나가기
                PlayerScript.instance.SetLight(0);
                PlayerScript.instance.SetButtonMode(false, true, true, true);
                Interaction.instance.isCtrl = true;
                DungeonUICtrl.instance.SetInfoText("", new Color(0.9f, 0.9f, 0.9f, 0f), 0f, 0f);
                PlayerScript.instance.isDungeon_1_On = false;
                MapData.instance.GetTileMap(DungeonCreater.dungeon_1_startPos, 5).SetTile(DungeonCreater.dungeon_1_startPos, MapData.instance.dungeon_1_DecoX64Tiles[8 + 7 * DungeonCreater.dungeon_1_type]); // 닫힌 문 설치
                DungeonCreater.instance.Dungeon_1_DeleteRoom();
                DungeonCreater.instance.DeleteDungeon_1();
                TorchUICtrl.instance.SetEnd();
                ObjectPool.ReturnObject<EventBlock>(12, DungeonCreater.dungeon_1_entranceEvent.GetComponent<EventBlock>());
                break;
        }

        EventButton.instance.ResetEvent();

        yield return new WaitForSeconds(fadeOutTime);
        EventButton.isNotUse = false;
        PlayerScript.instance.isNoDamage = false;
        isEndSwitchPos = true;

        FadeEnd();
    }

    // BGM이 변하지 않으면서 위치 이동
    public IEnumerator switchPos(Vector3 _pos, float fadeInTime, float fadeOutTime)
    {
        FadeIn();
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn)
            AutoPlayCtrl.instance.SetInit();
        MoveCtrl.instance.SetInit();
        AttackCtrl.instance.SetInit();
        PlayerScript.instance.isCanCtrl = false;
        PlayerScript.instance.isNoDamage = true;
        PlayerScript.instance.animator.SetBool("isMove", false);
        PlayerScript.instance.handsAnimator.SetBool("isMove", false);
        animator.speed = 1f / fadeInTime;
        isEndSwitchPos = false;

        yield return new WaitForSeconds(fadeInTime);

        FadeOut();
        PlayerScript.instance.transform.position = _pos;
        PlayerScript.instance.isCanCtrl = true;
        animator.speed = 1f / fadeOutTime;
        Camera.main.transform.position = _pos + new Vector3Int(0, 0, -10);

        switch (EventButton.mainType)
        {
            case 2:
                PlayerScript.instance.transform.localScale = new Vector3(-1, 1, 1);
                DungeonCreater.dungeon_0_floor++;
                DungeonUICtrl.instance.SetInfoText("- 유적 던전 지하 " + DungeonCreater.dungeon_0_floor + "층 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                break;
            case 3:
                PlayerScript.instance.transform.localScale = new Vector3(1, 1, 1);
                DungeonCreater.dungeon_0_floor--;
                if (DungeonCreater.dungeon_0_floor == 0)
                    DungeonUICtrl.instance.SetInfoText("- 유적 던전 로비 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                else
                    DungeonUICtrl.instance.SetInfoText("- 유적 던전 지하 " + DungeonCreater.dungeon_0_floor + "층 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                break;
            case 12:
                DungeonCreater.instance.Dungeon_1_DeleteRoom();
                break;
            case 13:
                DungeonUICtrl.instance.SetInfoText("- 고대 던전 지하 " + PlayerScript.instance.d_1_currentFloor + "층 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                DungeonCreater.instance.Dungeon_1_DeleteRoom();
                break;
            case 15:
                PlayerScript.instance.d_1_currentFloor = 0;
                DungeonUICtrl.instance.SetInfoText("- 고대 던전 로비 -", new Color(0.9f, 0.9f, 0.9f, 1f), 1f, 1.5f);
                break;
        }

        EventButton.instance.ResetEvent();

        yield return new WaitForSeconds(fadeOutTime);

        FadeEnd();
        EventButton.isNotUse = false;
        PlayerScript.instance.isNoDamage = false;
        isEndSwitchPos = true;
    }

    // isFade가 true라면 볼륨이 줄어든다.
    public void BGMSetting(int bgmIndex, bool _isFade, float _fadeTime)
    {
        if (bgmIndex != -1)
            audio.clip = SaveScript.BGMs[bgmIndex];
        isFade = _isFade;
        fadeTime = _fadeTime;
        isStartFade = true;
    }

    public void BGMFade()
    {
        if (isFade)
        {
            if (audio.volume > 0f)
            {
                audio.volume -= Time.deltaTime * 1f / fadeTime;
            }
            else
            {
                audio.volume = 0f;
                isStartFade = false;
            }
        }
        else
        {
            if (audio.volume < 1f)
            {
                audio.volume += Time.deltaTime * 1f / fadeTime;
            }
            else
            {
                audio.volume = 1f;
                isStartFade = false;
            }
        }
    }
}