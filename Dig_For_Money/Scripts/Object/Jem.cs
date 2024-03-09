using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jem
{
    static public int unitNum = 10;
    static private Sprite[] jemSprites, jem_unitSprites, jem_unit2Sprites, jem_unit3Sprites, jem_unit4Sprites, jem_unit5Sprites
        , jem_unit6Sprites, jem_unit7Sprites, jem_unit8Sprites, jem_unit9Sprites;
    static private string[] names = 
    {
        "석탄", "석영", "장석", "에메랄드",
        "구리", "아연", "납", "토파즈", "은",
        "규소", "대리석", "철", "사파이어", "자수정", "루비", "옥",
        "보크사이트", "텅스텐", "우라늄", "가넷", "아쿠아마린", "오팔", "미스릴",
        "흑요석", "리튬", "아다만티움", "금", "다이아몬드", "코발트",
        "열금석", "장미석", "화염석", "제미나이트", "주얼라이트", "오나마이트", "황수정",

        "크리스탈(흑)", "크리스탈(백)", "크리스탈(황)", "크리스탈(청)", "크리스탈(홍)", "크리스탈(담청)",
        "페이나이트", "타파이트", "베니토아이트", "크로마이트", "레드노이트", "블루노이트",
        "에르사이트", "키노사이트", "리덴사이트", "페르사이트", "백룡석", "흑룡석",
        "스타더스트", "루나케이트", "서니사이트", "겔러시튬", "아토니움", "태초석",

        "경골석", "화룡암석", "광암석", "정화석", "용루석", "영혼석", "연옥석",
        "사황천석", "야마천석", "도리천석", "도솔천석", "화락천석", "타화자채천석", "육천석",
        "회재석", "황력석", "녹존석", "순백석", "체화석", "혼화석", "천계석",
        "수원소석", "화원소석", "기원소석", "지원소석", "공간석", "시간석", "공허석",

        "마나 메탈", "리온 메탈", "헵톤", "크리톤", "오리 메탈", "카이트론", "서플리스",
    };
    static private long[] prices =
    {
        100, 200, 400, 2000,
        500, 1000, 3000, 5000, 10000,
        2000, 10000, 20000, 30000, 50000, 100000, 50000000,
        40000, 100000, 200000, 300000, 1000000, 10000000, 500000000,
        500000, 3000000, 5000000, 20000000, 50000000, 5000000000,
        50000000, 100000000, 500000000, 1000000000, 2000000000, 3000000000, 50000000000,

        10000000000, 20000000000, 30000000000, 50000000000, 100000000000, 5000000000000,
        1000000000000, 2000000000000, 3000000000000, 5000000000000, 10000000000000, 500000000000000,
        100000000000000, 200000000000000, 300000000000000, 500000000000000, 1, 5,
        1, 2, 3, 5, 100, 500,

        10, 20, 30, 50, 10000, 30000, 10000000,
        100, 200, 300, 500, 200000, 500000, 300000000,
        1000, 2000, 3000, 5000, 3000000, 8000000, 50000000000,
        10000, 20000, 30000, 50000, 50000000, 100000000, 1000000000000,

        100000, 200000, 300000, 500000, 700000000, 1500000000, 20000000000000,
    };
    static private int[] qualitys =
    {
        0, 0, 0, 1,
        0, 0, 1, 1, 2,
        0, 1, 1, 2, 2, 3, 5,
        1, 2, 2, 3, 3, 4, 5,
        2, 3, 3, 4, 4, 5,
        3, 3, 4, 4, 4, 4, 5,

        4, 4, 4, 4, 4, 5,
        4, 4, 4, 4, 5, 5,
        4, 4, 4, 4, 5, 5,
        4, 4, 4, 4, 5, 5,

        4, 4, 4, 4, 5, 5, 6,
        4, 4, 4, 4, 5, 5, 6,
        4, 4, 4, 4, 5, 5, 6,
        4, 4, 4, 4, 5, 5, 6,

        4, 4, 4, 4, 5, 5, 6,
    };
    static private float[] createPercents =
    {
        0.125f, 0.1f, 0.075f, 0.05f,
        0.125f, 0.1f, 0.075f, 0.05f, 0.025f,
        0.125f, 0.1f, 0.08f, 0.06f, 0.04f, 0.02f, 0.001f,
        0.125f, 0.1f, 0.08f, 0.06f, 0.04f, 0.02f, 0.001f,
        0.125f, 0.1f, 0.075f, 0.05f, 0.025f, 0.001f,
        0.125f, 0.1f, 0.075f, 0.05f, 0.025f, 0.015f, 0.001f,

        0.1f, 0.08f, 0.07f, 0.06f, 0.05f, 0.001f,
        0.09f, 0.08f, 0.07f, 0.07f, 0.001f, 0.00075f,
        0.08f, 0.08f, 0.08f, 0.08f, 0.001f, 0.00075f,
        0.07f, 0.07f, 0.09f, 0.1f, 0.001f, 0.00075f,

        0.05f, 0.075f, 0.1f, 0.125f, 0.001f, 0.00075f, 0.00015f,
        0.05f, 0.075f, 0.1f, 0.125f, 0.001f, 0.00075f, 0.00015f,
        0.05f, 0.075f, 0.1f, 0.125f, 0.001f, 0.00075f, 0.00015f,
        0.05f, 0.075f, 0.1f, 0.125f, 0.001f, 0.00075f, 0.00015f,

        0.05f, 0.075f, 0.1f, 0.125f, 0.001f, 0.00075f, 0.00015f,
    };
    static public int[][] codesAsQuality =
    {
        new int[] { 0, 1, 2, 4, 5, 9 },
        new int[] { 3, 6, 7, 10, 11, 16 },
        new int[] { 8, 12, 13, 17, 18, 23 },
        new int[] { 14, 19, 20, 24, 25, 29, 30 },
        new int[] { 21, 26, 27, 31, 32, 33, 34, 36, 37, 38, 39, 40, 42, 43, 44, 45, 48, 49, 50, 51, 54, 55, 56, 57, 60, 61, 62, 63, 67, 68, 69, 70, 74, 75, 76, 77, 81, 82, 83, 84, 88, 89, 90, 91 },
        new int[] { 15, 22, 28, 35, 41, 46, 47, 52, 53, 58, 59, 64, 65, 71, 72, 78, 79, 85, 86, 92, 93 }, // 얼티밋
        new int[] { 66, 73, 80, 87, 94 } // 미스틱
    };
    static private bool isInit = false;

    public Sprite jemSprite, jem_unitSprite, jem_unit2Sprite, jem_unit3Sprite, jem_unit4Sprite, jem_unit5Sprite, jem_unit6Sprite
        , jem_unit7Sprite, jem_unit8Sprite, jem_unit9Sprite;
    public int itemCode; // 일차원 배열에서 쓰일 인덱스를 담는 변수
    public string name;
    public long price;
    public int quality;
    public float createPercent;

    static public bool IsPrice2(int _jemCode)
    {
        return _jemCode >= 52;
    }

    public long GetRealPrice()
    {
        long gold = (long)Mathf.Round(price * (1 + 0.01f * SaveScript.saveData.collection_levels[itemCode]));
        if (gold >= 1000)
            gold = long.Parse(gold.ToString().Substring(0, 3).PadRight(gold.ToString().Length, '0'));
        return gold;
    }

    public Jem(int _itemCode)
    {
        if (!isInit)
        {
            Init();
            isInit = true;
        }

        itemCode = _itemCode;
        SettingJem();
    }

    public void Init()
    {
        jemSprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem");
        jem_unitSprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units");
        jem_unit2Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units2");
        jem_unit3Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units3");
        jem_unit4Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units4");
        jem_unit5Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units5");
        jem_unit6Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units6");
        jem_unit7Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units7");
        jem_unit8Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units8");
        jem_unit9Sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_units9");
    }

    public void SettingJem()
    {
        jemSprite = jemSprites[itemCode];
        jem_unitSprite = jem_unitSprites[itemCode];
        jem_unit2Sprite = jem_unit2Sprites[itemCode];
        jem_unit3Sprite = jem_unit3Sprites[itemCode];
        jem_unit4Sprite = jem_unit4Sprites[itemCode];
        jem_unit5Sprite = jem_unit5Sprites[itemCode];
        jem_unit6Sprite = jem_unit6Sprites[itemCode];
        jem_unit7Sprite = jem_unit7Sprites[itemCode];
        jem_unit8Sprite = jem_unit8Sprites[itemCode];
        jem_unit9Sprite = jem_unit9Sprites[itemCode];
        name = names[itemCode];
        price = prices[itemCode];
        quality = qualitys[itemCode];
        createPercent = createPercents[itemCode];
    }

    static public int GetStartCode(int floor)
    {
        int code = 0;

        for (int i = 0; i < floor; i++)
            code += SaveScript.stageItemNums[i];

        return code;
    }

    /// <summary>
    /// 해당 층에 존재하는 얼티밋 광물 Code를 반환
    /// </summary>
    /// <param name="floor">층 정보</param>
    /// <returns></returns>
    static public int GetUItimateCode(int floor)
    {
        int jemIndex = 0;

        for (int i = 0; i <= floor; i++)
        {
            if (i > 2 && i < 8) jemIndex++;
            else if (i >= 8) jemIndex += 2;
        }
        jemIndex += Random.Range(0, GetUltimateNum(floor));

        return codesAsQuality[5][jemIndex];
    }

    /// <summary>
    /// 해당 층에 존재하는 얼티밋 광물 개수를 반환
    /// </summary>
    /// <param name="floor">층 정보</param>
    /// <returns></returns>
    static public int GetUltimateNum(int floor)
    {
        if (floor > 6) return 2;
        else return 1;
    }

    /// <summary>
    /// 해당 층에 존재하는 미스틱 광물 Code를 반환
    /// </summary>
    /// <param name="floor">층 정보</param>
    /// <returns></returns>
    static public int GetMysticCode(int floor)
    {
        if (floor < 10)
            return -1;
        return codesAsQuality[6][floor - 10];
    }
}
