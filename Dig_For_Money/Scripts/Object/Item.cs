using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    static public Color[] colors = { new Color(1f, 1f, 1f), new Color(0.4f, 1f, 1f), new Color(1f, 0.4f, 1f)
            , new Color(1f, 0.4f, 0.4f), new Color(0.4f, 1f, 0.4f), new Color(0.5f, 0.6f, 0.8f) }; // 흰색, 푸른색, 보라색, 붉은색, 초록색, 하늘색

    public Sprite sprite;
    public float forcePercent;
    public Color color;
    public new string name;
    public string info;
    public bool isInfoOn;
}

public class CashEquipment: Item
{
    static private string[] names =
    {
        "자석 아이템", "얼티밋 광물 탐지기", "미스틱 광물 탐지기", "고대 던전 탐지기"
    };
    static private string[] infos =
    {
        "모든 아이템을 끌어당기는 강력한 자석",
        "얼티밋 광물의 위치를 탐지하는 금속 탐지기",
        "미스틱 광물의 위치를 탐지하는 금속 탐지기",
        "고대 던전의 위치를 탐지하는 신비로운 장신구"
    };

    public int itemCode;

    public CashEquipment (int _itemCode)
    {
        itemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Item/CashEquipments")[itemCode];
        name = names[itemCode];
        info = infos[itemCode];
        color = color = new Color(0.5f, 0.6f, 0.8f, 1f);
    }
}

public class Elixir : Item
{
    static private string[] names =
    {
        "전설의 내구도 영약", "전설의 채광속도 영약", "전설의 방어 영약", "전설의 방어 행운 영약",  "전설의 판매 영약",
        "전설의 판매 행운 영약", "전설의 광물 영약", "전설의 광물 행운 영약", "전설의 이동속도 영약", "전설의 점프력 영약",
        "전설의 경험치 영약", "전설의 강화 영약", "전설의 공격 영약", "전설의 공격 행운 영약"
    };
    static private string[] infos =
    {
        "30분 동안 곡괭이 내구도를 < ", "30분 동안 곡괭이 채광속도를 < ", "30분 동안 방어력을 < ", "30분 동안 특수 회피 확률을 < ", "30분 동안 추가 판매율을 < ",
        "30분 동안 특수 판매 확률을 < ", "30분 동안 최대 획득 광물을 < ", "30분 동안 목걸이 효과 확률을 < ", "30분 동안 이동속도를 < ", "30분 동안 점프력을 < ",
        "30분 동안 획득하는 경험치를 < ", "30분 동안 강화 확률을 < ", "30분 동안 공격력을 < ", "30분 동안 크리티컬 확률을 < "
    };
    static private string[] infos2 =
    {
        " > 증가시킵니다.", "% > 증가시킵니다.", " > 증가시킵니다.", "% > 증가시킵니다.", "% > 증가시킵니다.",
        "% > 증가시킵니다.", "개 > 추가로 얻습니다.", "% > 증가시킵니다.", " > 증가시킵니다.", " > 증가시킵니다.",
        "% > 증가시킵니다.", "% > 증가시킵니다.", " > 증가시킵니다.", "% > 증가시킵니다."
    };
    static private string[] infos3 = { "30분 동안 특수 회피력을 < ", "30분 동안 특수 판매력을 < ", "30분 동안 최소 획득 광물을 < ", "30분 동안 크리티컬 데미지를 < " };
    static private float[][] forcePercents =
    {
        new float[] {20f, 25f, 30f, 35f, 40f, 45f, 50f, 70f, 100f, 300f, 1000f, 3000f, 5000f, 10000f, 20000f }, // 곡괭이
        new float[] {0.5f, 0.55f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 2f, 3f, 5f, 10f, 30f, 50f, 100f, 200f },
        new float[] {20f, 25f, 30f, 35f, 40f, 45f, 50f, 70f, 100f, 300f, 1000f, 3000f, 5000f, 10000f, 20000f }, // 모자
        new float[] {0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.7f, 1f, 3f, 5f, 10f, 20f, 40f, 80f},
        new float[] {20f, 25f, 30f, 35f, 40f, 50f, 50f, 70f, 100f, 300f, 1000f, 3000f, 5000f, 8000f, 15000f }, // 반지
        new float[] {0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.7f, 1f, 2f, 3f, 5f, 10f, 20f, 40f },
        new float[] {10f, 12f, 15f, 18f, 22f, 30f, 70f, 200f, 1000f, 2000f, 10000f, 30000f, 100000f, 200000f, 400000f }, // 목걸이
        new float[] {0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 1f, 2f, 5f, 10f, 20f, 30f, 50f, 100f },
        new float[] {1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f, 2.3f, 2.4f }, // 이속
        new float[] {1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f, 2.3f, 2.4f }, // 점프력
        new float[] {0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f }, // 경험치
        new float[] {0.155f, 0.16f, 0.165f, 0.17f, 0.175f, 0.18f, 0.185f, 0.19f, 0.195f, 0.2f, 0.21f, 0.22f, 0.23f, 0.24f, 0.25f }, // 강화
        new float[] {20f, 25f, 30f, 35f, 40f, 45f, 50f, 70f, 100f, 300f, 1000f, 3000f, 5000f, 10000f, 20000f }, // 검
        new float[] {0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.7f, 1f, 3f, 5f, 10f, 20f, 40f, 80f },
    };
    static public float[] dropPercents =
    {
        0.08f, 0.08f, 0.08f, 0.08f, 0.08f,
        0.08f, 0.08f, 0.08f, 0.08f, 0.08f,
        0.02f, 0.005f, 0.08f, 0.08f
    };
    static public int exp = 150;
    public int itemCode;

    public Elixir(int _itemCode)
    {
        itemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Item/ElixirItem")[itemCode];
    }

    public void SetData()
    {
        name = names[itemCode] + " [" + (SaveScript.saveData.pickLevel + 1) + " 단계]";
        forcePercent = forcePercents[itemCode][SaveScript.saveData.pickLevel];
        color = colors[4];
        isInfoOn = false;
        SetInfo();
    }

    public void SetInfo()
    {
        switch (itemCode)
        {
            case 0: info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * forcePercent)) + infos2[itemCode]; break;
            case 2:
                if (SaveScript.saveData.equipHat != -1) info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.hats[SaveScript.saveData.equipHat].reinforce_basic * forcePercent)) + infos2[itemCode];
                else info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.hats[0].reinforce_basic * forcePercent)) + infos2[itemCode];
                break;
            case 3:
                if (SaveScript.stat.hat02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[0] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(1, forcePercent) * 100f * 100f) * 0.01f + infos2[itemCode];
                break;
            case 4:
                if (SaveScript.saveData.equipRing != -1) info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * forcePercent * 100)) + infos2[itemCode];
                else info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.rings[0].reinforce_basic * forcePercent * 100)) + infos2[itemCode];
                break;
            case 5:
                if (SaveScript.stat.ring02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[1] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(2, forcePercent) * 100f * 100f) * 0.01f + infos2[itemCode];
                break;
            case 6:
                if (SaveScript.saveData.equipPendant != -1) info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * forcePercent)) + infos2[itemCode]; 
                else info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.pendants[0].reinforce_basic * forcePercent)) + infos2[itemCode];
                break;
            case 7:
                if (SaveScript.stat.pendant02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[2] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(3, forcePercent)) + infos2[6];
                break;
            case 8:
            case 9: info = infos[itemCode] + Mathf.Round(forcePercent * 10 * 10f) * 0.1f + infos2[itemCode]; break;
            case 12: info = infos[itemCode] + GameFuction.GetNumText((long)Mathf.Round(SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * forcePercent)) + infos2[itemCode]; break;
            case 13:
                if (SaveScript.stat.sword02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[3] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(4, forcePercent) * 100f * 100f) * 0.01f + infos2[itemCode];
                break;
            default: info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode]; break;
        }
    }
}

public class BufItem : Item
{
    static private string[] names = 
    {
        "내구도의 물약", "채광속도의 물약", "방어의 물약", "방어 행운의 물약",  "판매의 물약",
        "판매 행운의 물약", "광물의 물약", "광물 행운의 물약", "이동속도의 물약", "점프력의 물약",
        "경험치의 물약", "강화의 물약", "공격의 물약", "공격 행운의 물약"
    };
    static private string[] infos =
    {
        "15분 동안 곡괭이 내구도를 < ", "15분 동안 곡괭이 채광속도를 < ", "15분 동안 방어력을 < ", "15분 동안 특수 회피 확률을 < ", "15분 동안 추가 판매율을 < ",
        "15분 동안 특수 판매 확률을 < ", "15분 동안 최대 획득 광물을 < ", "15분 동안 목걸이 효과 확률을 < ", "15분 동안 이동속도를 < ", "15분 동안 점프력을 < ",
        "15분 동안 획득하는 경험치를 < ", "15분 동안 강화 확률을 < ", "15분 동안 공격력을 < ", "15분 동안 크리티컬 확률을 < "
    };
    static private string[] infos2 =
    {
        " > 증가시킵니다.", "% 증가시킵니다.", " > 증가시킵니다.", "% > 증가시킵니다.", "% > 증가시킵니다.",
        "% > 증가시킵니다.", "개 > 추가로 얻습니다.", "% > 증가시킵니다.", " > 증가시킵니다.", " > 증가시킵니다.",
        "% > 증가시킵니다.", "% > 증가시킵니다.", " > 증가시킵니다.", "% > 증가시킵니다."
    };
    static private string[] infos3 = { "15분 동안 특수 회피력을 < ", "15분 동안 특수 판매력을 < ", "15분 동안 최소 획득 광물을 < ", "15분 동안 크리티컬 데미지를 < " };
    static private float[] forcePercents =
    {
        5f, 0.1f, 5f, 0.05f, 5f,
        0.05f, 3f, 0.05f, 0.5f, 0.3f,
        0.1f, 0.05f, 5f, 0.05f
    };
    static public float[] dropPercents =
    {
        0.08f, 0.08f, 0.08f, 0.08f, 0.08f,
        0.08f, 0.08f, 0.08f, 0.08f, 0.08f,
        0.02f, 0.005f, 0.08f, 0.08f
    };
    static public float[][] cashPercents = new float[][]
    { 
        new float[] { 0.35f, 0.55f, 0.1f }, 
        new float[] { 0f, 0f, 1f},
        new float[] { 0f, 0f, 0f}
    };
    static public int[] typePlus = { 1, 2, 4 };
    static public string[] typeName = { "초급", "중급", "고급" };
    static public int[] exps = new int[] { 10, 20, 40 };
    static public long[] prices = new long[] { 1000000, 3000000, 10000000 };

    public int itemCode, totalItemCode;
    public int itemType; // 초급 = 0, 중급 = 1, 상급 = 2

    public BufItem(int _itemCode)
    {
        totalItemCode = _itemCode;
        itemCode = _itemCode / SaveScript.bufItemTypeNum;
        itemType = _itemCode % SaveScript.bufItemTypeNum;
        sprite = Resources.LoadAll<Sprite>("Images/Item/BufItem")[totalItemCode];
        name = typeName[itemType] + " " + names[itemCode];
        forcePercent = forcePercents[itemCode] * (itemType + 1);
        color = GetColorByCode(_itemCode, out _);
        isInfoOn = false;
    }

    public void SetInfo()
    {
        switch (itemCode)
        {
            case 0: info = infos[itemCode] + Mathf.Round(SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * forcePercent) + infos2[itemCode]; break;
            case 2:
                if (SaveScript.saveData.equipHat != -1) info = infos[itemCode] + Mathf.Round(SaveScript.hats[SaveScript.saveData.equipHat].reinforce_basic * forcePercent) + infos2[itemCode];
                else info = infos[itemCode] + Mathf.Round(SaveScript.hats[0].reinforce_basic * forcePercent) + infos2[itemCode];
                break;
            case 3:
                if (SaveScript.stat.hat02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[0] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(1, forcePercent) * 100f * 1000f) * 0.001f + infos2[itemCode];
                break;
            case 4:
                if (SaveScript.saveData.equipRing != -1) info = infos[itemCode] + Mathf.Round(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * forcePercent * 100) + infos2[itemCode];
                else info = infos[itemCode] + Mathf.Round(SaveScript.rings[0].reinforce_basic * forcePercent * 100) + infos2[itemCode];
                break;
            case 5:
                if (SaveScript.stat.ring02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[1] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(2, forcePercent) * 100f * 1000f) * 0.001f + infos2[itemCode];
                break;
            case 6: info = infos[itemCode] + forcePercent + infos2[itemCode]; break;
            case 7:
                if (SaveScript.stat.pendant02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[2] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(3, forcePercent)) + infos2[6];
                break;
            case 8:
            case 9: info = infos[itemCode] + Mathf.Round(forcePercent * 10 * 10f) * 0.1f + infos2[itemCode]; break;
            case 12: info = infos[itemCode] + Mathf.Round(SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * forcePercent) + infos2[itemCode]; break;
            case 13:
                if (SaveScript.stat.sword02 < 1f) info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode];
                else info = infos3[3] + Mathf.Round(GameFuction.GetReinforcePercent_Over2(4, forcePercent) * 100f * 1000f) * 0.001f + infos2[itemCode];
                break;
            default: info = infos[itemCode] + Mathf.Round(forcePercent * 100f * 10f) * 0.1f + infos2[itemCode]; break;
        }
    }

    /// <summary>
    /// 아이템 상자의 타입과 확률에 따라 BufItem의 Code를 반환하는 함수
    /// </summary>
    /// <param name="type">아이템 상자의 타입(1 = 일반 / 2 = 고급 / 3 = 전설)</param>
    /// <returns></returns>
    static public int GetCodeByCashItem(int type)
    {
        int code = Random.Range(0, SaveScript.bufItemCodeNum) * SaveScript.bufItemTypeNum;
        code += GameFuction.GetRandFlag(cashPercents[type]);
        return code;
    }

    /// <summary>
    /// 아이템 Type에 따른 Color 반환
    /// </summary>
    /// <param name="type">아이템 Type</param>
    /// <returns></returns>
    static public Color GetColorByCode(int code, out int index)
    {
        Color color = colors[0];
        index = 0;

        switch (code % 3)
        {
            case 0: color = colors[0]; index = 0; break;
            case 1: color = colors[0]; index = 0; break;
            case 2: color = colors[1]; index = 1; break;
        }

        return color;
    }
}

public class ReinforceItem : Item
{
    static private string[] names =
    {
        "초급 강화 성공의 주문서", "중급 강화 성공의 주문서", "고급 강화 성공의 주문서", "등급 하락 방지의 주문서", "파괴 방지의 주문서",
        "창조주의 축복", "추가 1성의 주문서", "추가 2성의 주문서"
    };
    static private string[] infos =
    {
        "강화 성공 확률을 15% 증가시킵니다.", "강화 성공 확률을 30% 증가시킵니다.", "강화 성공 확률을 50% 증가시킵니다.", "강화 실패시, 등급 하락을 방지합니다.", "강화 실패시, 파괴를 방지합니다.",
        "강화 실패시, 등급 하락과 파괴를 모두 방지합니다.", "100% 확률로 1성만큼 장비가 강화됩니다.", "100% 확률로 2성만큼 장비가 강화됩니다."
    };
    static private float[] forcePercents =
    {
        0.15f, 0.3f, 0.5f, 0f, 0f,
        0f, 0f, 0f
    };
    static public float[] dropPercents =
    {
        0.25f, 0.125f, 0.075f, 0.125f, 0.125f,
        0.075f, 0.01f, 0.005f
    };
    static private float[][] cashPercents = new float[][]
    {
        new float[] {0.1f, 0.265f, 0.25f, 0.05f, 0.05f, 0.25f, 0.025f, 0.01f },
        new float[] {0f, 0f, 0f, 0f, 0f, 0f, 0.6f, 0.4f },
        new float[] {0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f },
    };
    static public int[] rewardOrderCode = new int[] { 7, 6, 2, 5, 1, 4, 3, 0 };
    static public int[] exps = new int[] { 15, 30, 50, 15, 15, 50, 100, 200 };
    static public long[] prices = new long[] { 1000000, 3000000, 10000000, 3000000, 3000000, 10000000, 30000000, 100000000 };

    public int itemCode;
    public Sprite image;

    public ReinforceItem(int _itemCode)
    {
        itemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Item/ReinforceItem")[itemCode];
        image = Resources.LoadAll<Sprite>("Images/Item/ReinforceImage")[itemCode];
        name = names[itemCode];
        info = infos[itemCode];
        forcePercent = forcePercents[itemCode];
        color = GetColorByCode(itemCode, out _);
        isInfoOn = false;
    }

    /// <summary>
    /// 아이템 상자의 타입과 확률에 따라 ReinforceItem의 Code를 반환하는 함수
    /// </summary>
    /// <param name="type">아이템 상자의 타입(1 = 일반 / 2 = 고급 / 3 = 전설)</param>
    /// <returns></returns>
    static public int GetCodeByCashItem(int type)
    {
        return GameFuction.GetRandFlag(cashPercents[type]);
    }

    /// <summary>
    /// 아이템 Code에 따른 Color 반환하고 color의 index를 매개 변수로 반환
    /// </summary>
    /// <param name="code">아이템 코드</param>
    /// <param name="index">Color의 index</param>
    /// <returns></returns>
    static public Color GetColorByCode(int code, out int _index)
    {
        Color color;
        int index;

        if (code == 7)
            index = 3;
        else if (code == 6)
            index = 2;
        else if (code == 5 || code == 2)
            index = 1;
        else
            index = 0;
        color = colors[index];
        _index = index;

        return color;
    }

    static public float GetCashPercentAsType(int _type, int _type2)
    {
        float val = 0f;
        switch (_type2)
        {
            case 0: val = cashPercents[_type][0] + cashPercents[_type][3] + cashPercents[_type][4]; break;
            case 1: val = cashPercents[_type][1] + cashPercents[_type][2] + cashPercents[_type][5]; break;
            case 2: val = cashPercents[_type][6] + cashPercents[_type][7]; break;
        }
        return val;
    }
}

public class ReinforceItem2 : Item
{
    static private string[] names =
    {
        "추가 3성의 제련석", "추가 5성의 제련석", "추가 10성의 제련석", "추가 백성의 고대 유물",  "추가 천성의 고대 유물", "추가 만성의 고대 유물",
    };
    static private string[] infos =
    {
        "100% 확률로 3성만큼 장비가 강화됩니다.", "100% 확률로 5성만큼 장비가 강화됩니다.", "100% 확률로 10성만큼 장비가 강화됩니다.",
        "100% 확률로 100성만큼 장비가 강화됩니다.", "100% 확률로 1000성만큼 장비가 강화됩니다.", "100% 확률로 10000성만큼 장비가 강화됩니다.",
    };
    static private float[] forcePercents = { 3f, 5f, 10f, 100f, 1000f, 10000f };
    static public float[] dropPercents = { 0.945f, 0.05f, 0.005f, 0.0001f, 0.000001f, 0.00000001f };
    static public float[][] cashPercents = 
    {
        new float[] { 0.8f, 0.15f, 0.05f, 0f, 0f, 0f },
        new float[] { 0.4835f, 0.35f, 0.15f, 0.015f, 0.0015f, 0f },
        new float[] { 0.12975f, 0.35f, 0.5f, 0.015f, 0.005f, 0.00025f },
    };
    static public int[] exps = { 300, 1000, 5000, 100000, 1000000, 10000000 };

    public int itemCode;
    public Sprite image;

    public ReinforceItem2(int _itemCode)
    {
        itemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Item/ReinforceItem2")[itemCode + 1];
        image = Resources.LoadAll<Sprite>("Images/Item/ReinforceImage2")[itemCode + 1];
        name = names[itemCode];
        info = infos[itemCode];
        forcePercent = forcePercents[itemCode];
        if (itemCode < 3)
        {
            color = new Color(0.5f, 1f, 0.5f, 1f);
            isInfoOn = false;
        }
        else
        {
            color = new Color(0.5f, 0.6f, 0.8f, 1f);
            isInfoOn = true;
        }
    }

    static public int GetCodeByCashItem(int type)
    {
        return GameFuction.GetRandFlag(cashPercents[type]);
    }
}