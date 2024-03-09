using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManaBuf
{
    static public Sprite[] sprites;
    static public string[] names =
    {
        "시간 단축 [ 1단계 ]", "시간 단축 [ 2단계 ]", "시간 단축 [ 3단계 ]",
        "협상가의 축복 [ 1단계 ]", "협상가의 축복 [ 2단계 ]", "협상가의 축복 [ 3단계 ]",
        "광부의 축복 [ 1단계 ]", "광부의 축복 [ 2단계 ]", "광부의 축복 [ 3단계 ]",
        "모험가의 축복 [ 1단계 ]", "모험가의 축복 [ 2단계 ]", "모험가의 축복 [ 3단계 ]",
        "대장장이의 축복 [ 1단계 ]", "대장장이의 축복 [ 2단계 ]", "대장장이의 축복 [ 3단계 ]",
    };
    static public string[] infos =
    {
        "모든 펫 보상 시간이 < ",
        "15분 동안 상점 추가 판매율이 < ",
        "15분 동안 광물을 추가로 < ",
        "15분 동안 획득하는 경험치가 < ",
        "15분 동안 강화 성공 확률을 < ",
    };
    static public string[] infos2 =
    {
        " > 단축됩니다.",
        "% > 증가합니다.",
        "개 > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
    };
    static public int[] prices =
    {
        100, 300, 1000,
        10, 100, 1000,
        10, 100, 1000,
        60, 150, 500,
        100, 500, 2000,
    };
    static public float[] forces =
    {
        60 * 60, 3 * 60 * 60, 12 * 60 * 60,
        10f, 30f, 100f,
        10, 100, 1000,
        0.25f, 0.5f, 1f,
        0.1f, 0.2f, 0.3f,
    };
    static bool isInit;

    public int code;
    public string name, info, info2;
    public int price;
    public float force;
    public Sprite sprite;

    public ManaBuf(int _code)
    {
        if (!isInit)
        {
            isInit = true;
            sprites = Resources.LoadAll<Sprite>("Images/Mine/Facility/ManaShop/Mine_FacilityBufs");
        }

        code = _code;
        name = names[code];
        info = infos[code / 3];
        info2 = infos2[code / 3];
        price = prices[code];
        force = forces[code];
        sprite = sprites[code];
    }
}

public class ManaItem
{
    static public Sprite[] sprites;
    static public string[] names =
    {
        "강화석 [동]상자", "강화석 [은]상자", "강화석 [금]상자",
        "고급 강화 주문서", "창조주의 축복", "추가 1성의 주문서",
        "추가 2성의 주문서", "[D급] 삽 슬라임", "[D급] 새총 슬라임"
    };
    static public string[] infos =
    {
        "강화석을 1000개 구매합니다.",
        "강화석을 10K개 구매합니다.",
        "강화석을 100K개 구매합니다.",
        "고급 강화 주문서를 1개 구매합니다.",
        "창조주의 축복을 1개 구매합니다.",
        "추가 1성의 주문서를 1개 구매합니다.",
        "추가 2성의 주문서를 1개 구매합니다.",
        "[D급] 삽 슬라임을 1마리 구매합니다.",
        "[D급] 새총 슬라임을 1마리 구매합니다."
    };
    static public int[] prices = { 10, 80, 700, 10, 10, 30, 100, 100, 100 };
    static public int[] forces = { 1000, 10000, 100000, 1, 1, 1, 1, 1, 1 };
    static bool isInit;

    public int code;
    public string name, info;
    public int force;
    public int price;
    public Sprite sprite;

    public ManaItem(int _code)
    {
        if (!isInit)
        {
            isInit = true;
            sprites = Resources.LoadAll<Sprite>("Images/Mine/Facility/ManaShop/Mine_FacilityItems");
        }

        code = _code;
        name = names[code];
        info = infos[code];
        force = forces[code];
        price = prices[code];
        sprite = sprites[code];
    }
}

public class ManaUpgrade
{
    static public Sprite[] sprites;
    static public string[] names =
    {
        "고고학자의 정신", "상인의 정신", "지주의 정신", "모험가의 정신",
        "탐험가의 정신", "마나석 채굴꾼의 정신", "마나석 도굴꾼의 정신", "수집가의 정신",
        "펫 맘의 정신", "컬렉터의 정신", "보스 사냥꾼의 정신", "얼티밋 사냥꾼의 정신",
        "시간술사의 정신", "펫 연금술사의 정신", "미스틱 사냥꾼의 정신"
    };
    static public string[] infos =
    {
        "도서관에서 고급 아이템 등장 확률이 < ",
        "NPC 상인에서 고급 아이템 등장 확률이 < ",
        "펫 보상 최대 저장 개수가 < ",
        "추가 경험치 획득량이 < ",
        "경험치 2배 추가 획득 확률이 < ",
        "마나석 광석에서 얻는 마나석 < ",
        "상자에서 얻는 마나석 획득량이 < ",
        "아이템 드랍률이 < ",
        "펫 드랍률이 < ",
        "광물 컬렉션 획득 확률이 < ",
        "보스 몬스터 마나석 드랍량이 < ",
        "얼티밋 광물 등장 확률이 < ",
        "이벤트 맵 지속 시간이 < ",
        "펫 합성 확률이 < ",
        "미스틱 광물 등장 확률이 < ",
    };
    static public string[] infos2 =
{
        "% > 증가합니다.",
        "% > 증가합니다.",
        "개 > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
        "개 > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
        "초 > 증가합니다.",
        "% > 증가합니다.",
        "% > 증가합니다.",
    };
    static public float[] forces = { 0.05f, 0.05f, 10f, 0.05f, 0.03f, 1f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.5f, 0.003f, 0.05f };
    static public int[] prices = { 40, 40, 40, 40, 40, 40, 40, 40, 40, 50, 50, 50, 300, 4000, 150 };
    static bool isInit;

    public int code;
    public string name, info;
    public int price;
    public float force;
    public Sprite sprite;

    public ManaUpgrade(int _code)
    {
        if (!isInit)
        {
            isInit = true;
            sprites = Resources.LoadAll<Sprite>("Images/Mine/Facility/ManaShop/Mine_FacilityUpgrades");
        }

        code = _code;
        name = names[code];
        if(code == 2 || code == 5 || code == 12)
            info = infos[code] + forces[code] + infos2[code];
        else
            info = infos[code] + (forces[code] * 100) + infos2[code];
        
        price = prices[code];
        force = forces[code];
        sprite = sprites[code];
    }

    static public int GetRealPrice(int code)
    {
        return prices[code] * (SaveScript.saveData.manaUpgrades[code] + 1);
    }
}

public class ManaShopSlot : MonoBehaviour
{
    public Order order;
    public Button button;
    public Image bufImage;
    public Text nameText, infoText;
    public Text manaText;
}
