using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement
{
    static public int TIER_MAX = 40;
    static public int TIER_ACHIEVEMENT = 100;
    static public string[] names =
    {
        "땅을 파는 광부", "노멀 광물 수집가", "레어 광물 수집가 ", "에픽 광물 수집가", "유니크 광물 수집가",
        "레전드리 광물 수집가", "얼티밋 광물 수집가", "미스틱 광물 수집가", "일반 던전 상자 도굴꾼", "유적 던전 상자 도굴꾼",
        "고대 던전 상자 도굴꾼", "일반 몬스터 사냥꾼", "보스 몬스터 사냥꾼", "유적 탐험가", "고대 탐험가", 
        "고대 서적을 찾는 학자", "거래를 좋아하는 상인", "물약 수집가", "주문서 수집가", "펫 수집가", 
        "돈 부자", "경험치 부자", "강화석 부자", "마나석 부자", "캐시 부자",
        "약쟁이", "주술사", "경험이 많은 대장장이", "자원이 많은 대장장이", "일확천금을 노리는 자",
        "광물을 간절히 원하는 자", "아이템을 간절히 원하는 자", "펫을 간절히 원하는 자", "특수 돌을 간절히 원하는 자", "특수 아이템을 간절히 원하는 자",
        "펫 지주", "펫 맘", "펫 연금술사", "펫 조련사", "펫 학살자"
    };
    static public string[] infos =
    {
        "땅 아무 블럭 파괴하기", "노멀 등급 광석 캐기", "레어 등급 광석 캐기", "에픽 등급 광석 캐기", "유니크 등급 광석 캐기",
        "레전드리 등급 광석 캐기", "얼티밋 등급 광석 캐기","미스틱 등급 광석 캐기",  "일반 던전 상자 열기", "유적 던전 상자 열기",
        "고대 던전 상자 열기", "일반 몬스터 처치하기", "보스 몬스터 처치하기", "유적 던전 입장하기", "고대 던전 입장하기",
        "특수 던전 도서관 조사하기", "특수 던전 NPC와 거래하기", "물약 아이템 획득하기", "주문서 획득하기", "펫 획득하기",
        "돈 벌기", "EXP 얻기 (업적 제외)", "강화석 얻기 (업적 제외)","마나석 얻기 (업적 제외)", "레드 다이아몬드 얻기 (업적 제외)",
        "물약 아이템 사용하기", "주문서 아이템 사용하기", "장비 경험치 강화하기", "장비 강화석 강화 성공하기", "단 한 번으로 광물을 팔아 얻은 돈의 최대 수치",
        "아무 종류의‘광물 상자’구매하기", "아무 종류의‘아이템 상자’구매하기", "아무 종류의‘펫 알’구매하기", "아무 종류의‘캡슐’구매하기", "아무 종류의‘랜덤 박스’구매하기",
        "펫 상자 열기", "펫 레벨업 하기", "펫 합성하기", "펫 강화하기", "펫 분해하기"
    };
    static public Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Main/AchievementImage");
    static public int[] indexAsMenu = { 20, 9, 6, 5 };
    static public int[][] rewards = new int[][] 
    { 
        new int[] { 5, 5, 10, 15, 20, 30, 50, 100, 1000, 10000 }, // 경험치 
        new int[] { 0, 5, 10, 20, 30, 50, 100, 500, 1000, 10000 }, // 강화석
        new int[] { 0, 0, 1, 2, 3, 5, 7, 10, 100, 1000 }, // 마나석
        new int[] { 0, 0, 0, 0, 0}, // 레드 다이아
    };
}

public class AchievementCtrl : MonoBehaviour
{
    static public AchievementCtrl instance;
    static public string[] tierNames =
    {
        "브론즈 V", "브론즈 IV", "브론즈 III", "브론즈 II", "브론즈 I",
        "실버 V", "실버 IV", "실버 III", "실버 II", "실버 I",
        "골드 V", "골드 IV", "골드 III", "골드 II", "골드 I",
        "플레티넘 V", "플레티넘 IV", "플레티넘 III", "플레티넘 II", "플레티넘 I",
        "다이아몬드 V", "다이아몬드 IV", "다이아몬드 III", "다이아몬드 II", "다이아몬드 I",
        "마스터 V", "마스터 IV", "마스터 III", "마스터 II", "마스터 I",
        "그랜드마스터 V", "그랜드마스터 IV", "그랜드마스터 III", "그랜드마스터 II", "그랜드마스터 I",
        "챌린저 V", "챌린저 IV", "챌린저 III", "챌린저 II", "챌린저 I",
        "챔피언", "챔피언"
    };
    public Sprite[] tierReward_sprites;
    public Sprite[] tierSprites;
    public Animator animator;
    public Image image;
    public Text nameText, infoText;

    public long[] achievementGoalsAsLevel; // 현재 업적 레벨에 따른 목표치
    public bool[] achievementIsPrint; // 업적 달성 UI를 출력한 적이 있는 가?

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            tierSprites = Resources.LoadAll<Sprite>("Images/Main/TierImage");

            achievementGoalsAsLevel = new long[SaveScript.achievementNum];
            achievementIsPrint = new bool[SaveScript.achievementNum];

            for (int i = 0; i < SaveScript.achievementNum; i++)
                SetAchievementGoal(i);

            /*
            long temp1 = 0, temp2 = 0;
            for (int i = 0; i < 100; i++)
            {
                temp1 = GameFuction.GetAchievementAmount(0.0005, i, 2.25, 1);
                temp2 = GameFuction.GetAchievementAmount(0.00025, i, 2.25, 1);
                Debug.Log(i + "레벨(이전) : " + GameFuction.GetGoldText(temp1, 0, 0) + " / (최근):" + GameFuction.GetGoldText(temp2, 0, 0));
            }
            */
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// SaveData를 기반으로 Goal 데이터를 구성
    /// </summary>
    /// <param name="type">업적의 종류</param>
    public void SetAchievementGoal(int type)
    {
        double a = 0;
        double pow = 0;
        double c = 0;

        switch (type)
        {
            case 0: a = 0.01; pow = 3.75; c = 10; break; // 땅
            case 1: a = 0.03; pow = 2.25; c = 5; break; // 노멀
            case 2: a = 0.035; pow = 2.25; c = 5; break; // 레어
            case 3: a = 0.04; pow = 2.25; c = 5; break; // 에픽
            case 4: a = 0.05; pow = 2.25; c = 5; break; // 유니크
            case 5: a = 0.0025; pow = 3.5; c = 5; break; // 레전드리
            case 6: a = 0.000003; pow = 4.25; c = 1; break; // 얼티밋
            case 7: a = 0.0000005; pow = 4.5; c = 1; break; // 미스틱
            case 8: a = 0.0002; pow = 3.5; c = 3; break; // 일반 던전 상자
            case 9: a = 0.0015; pow = 2.75; c = 1; break; // 유적 던전 상자
            case 10: a = 0.0001; pow = 3.25; c = 1; break; // 고대 던전 상자
            case 11: a = 0.0002; pow = 3.6; c = 3; break; // 일반 몬스터
            case 12: a = 0.00002; pow = 3.6; c = 1; break; // 보스 몬스터
            case 13: a = 0.0003; pow = 2.75; c = 1; break; // 유적 던전 입장
            case 14: a = 0.000075; pow = 2.75; c = 1; break; // 고대 던전 입장
            case 15: a = 0.001; pow = 2.75; c = 3; break; // 도서관
            case 16: a = 0.0001; pow = 2.75; c = 1; break; // 상인
            case 17: a = 0.00000000001; pow = 9; c = 3; break; // 물약 획득
            case 18: a = 0.00000000001; pow = 8.5; c = 3; break; // 주문서 획득
            case 19: a = 0.005; pow = 2; c = 1; break; // 펫 획득
            case 20: break; // 돈
            case 21: a = 0.0000000001; pow = 9.5; c = 10; break; // 경험치
            case 22: a = 0.00000000001; pow = 9.5; c = 10; break; // 강화석
            case 23: a = 0.00000001; pow = 7.5; c = 5; break; // 마나석
            case 24: a = 0.00001; pow = 4.25; c = 5; break; // 캐시
            case 25: a = 0.0002; pow = 3.6; c = 2; break; // 물약 사용
            case 26: a = 0.0002; pow = 3.6; c = 2; break; // 주문서 사용
            case 27: a = 0.000005; pow = 4; c = 2; break; // 경험치 강화
            case 28: a = 0.00000005; pow = 6; c = 3; break; // 강화석 강화
            case 29: break; // 일획천금
            case 30: a = 0.00025; pow = 2.25; c = 1; break; // 광물 상자
            case 31: a = 0.00025; pow = 2.25; c = 1; break; // 아이템 상자
            case 32: a = 0.00025; pow = 2.25; c = 1; break; // 펫 알
            case 33: a = 0.0005; pow = 2.25; c = 1; break; // 캡슐
            case 34: a = 0.00025; pow = 2.25; c = 1; break; // 랜덤 박스
            case 35: a = 0.000001; pow = 5.25; c = 3; break; // 펫 상자
            case 36: a = 0.000002; pow = 4.5; c = 1; break; // 펫 레벨업
            case 37: a = 0.000001; pow = 4; c = 1; break; // 펫 합성
            case 38: a = 0.00000015; pow = 4.5; c = 1; break; // 펫 강화
            case 39: a = 0.0035; pow = 2; c = 1; break; // 펫 분해
        }

        if (type == 20 || type == 29)
            achievementGoalsAsLevel[type] = GameFuction.GetAchievementAmount_gold(type);
        else
            achievementGoalsAsLevel[type] = GameFuction.GetAchievementAmount(a, SaveScript.saveData.achievementLevels[type], pow, c);
    }

    public int GetSecondAmountIndex(int type)
    {
        int index = -1;
        switch (type)
        {
            case 20: index = 0; break;
            case 29: index = 1; break;
        }

        return index;
    }

    /// <summary>
    /// 업적 데이터의 값을 판단하여 추가한다. 사전에 조건을 체크해야 한다.
    /// </summary>
    /// <param name="type">업적의 종류</param>
    public void SetAchievementAmount(int type, long amount, long amount2, long amount3)
    {
        if (type == 20 || type == 29)
        {
            SaveScript.saveData.achievementAmount2[GetSecondAmountIndex(type)] += amount2;
            SaveScript.saveData.achievementAmount3[GetSecondAmountIndex(type)] += amount3;
        }
        SetAchievementAmount(type, amount);
    }

    /// <summary>
    /// 업적 데이터의 값을 판단하여 추가한다. 사전에 조건을 체크해야 한다.
    /// </summary>
    /// <param name="type">업적의 종류</param>
    public void SetAchievementAmount(int type, long amount)
    {
        SaveScript.saveData.achievementAmounts[type] += amount;
        if (SaveScript.saveData.achievementLevels[type] != 100 && CheckAchievement(type)) PrintAchievement(type);
    }

    /// <summary>
    /// 업적 성공 UI을 출력하는 함수이다.
    /// </summary>
    /// <param name="type">업적의 종류</param>
    public void PrintAchievement(int type)
    {
        animator.SetBool("isPrint", true);
        animator.Play("Achievement_ctrl_print", -1, 0f);
        image.sprite = Achievement.sprites[type];
        nameText.text = "[ " + Achievement.names[type] + " ]";
        infoText.text = "업적 레벨 " + (SaveScript.saveData.achievementLevels[type] + 1) + " 달성!";
    }

    /// <summary>
    /// 업적 성공을 체크하는 함수이다.
    /// </summary>
    /// <param name="type">업적의 종류</param>
    public bool CheckAchievement(int type)
    {
        if (!achievementIsPrint[type] && SaveScript.saveData.achievementAmounts[type] >= achievementGoalsAsLevel[type])
        {
            achievementIsPrint[type] = true;
            return true;
        }
        else
            return false;
    }

    public void EndAnimation()
    {
        animator.SetBool("isPrint", false);
    }
}
