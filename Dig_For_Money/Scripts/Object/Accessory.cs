using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Accessory
{
    public Sprite sprite;
    public int itemCode;
    public long price;
    public string name;
    public float forcePercent; // 악세서리의 힘의 량 (퍼센트), 1f = 100%
    public float reinforce_basic;
}

public class Hat : Accessory
{
    static public Sprite destroyedSprite = Resources.LoadAll<Sprite>("Images/Accessory/HatImage")[0];
    static private long[] prices = { 1000, 20000, 4000000, 1500000000, 250000000000, 50000000000000, 10, 100000, 1000000000, 10000000000000, 10, 100000, 1000000000, 10000000000000, 1 }; 
    // 1000원, 2만, 400만, 15억, 2500억, 50조, 10경, 10해, 10자, 10양, 10루니, 100,000루니, 1케니, 10,000케니, 1헤니
    static private string[] names = { "나무 모자", "돌 모자", "철 모자", "금 모자", "다이아 모자", "흑암석 모자",
        "청록석 모자", "영혼석 모자", "흑마석 모자", "태초석 모자", "연옥석 모자", "육천석 모자", "천계석 모자", "공허석 모자",
        "서플라스 모자" };
    static private float[] forcePercents = { 20f, 25f, 35f, 50f, 70f, 100f, 140f, 200f, 300f, 1000f, 3000f, 10000f, 30000f, 50000f, 100000f };
    static private float[] reinforce_basics = { 2, 2, 2, 2, 2, 2, 2, 3, 4, 5, 6, 7, 8, 10, 12 };

    public int hatItemCode;

    public Hat(int _itemCode)
    {
        itemCode = _itemCode;
        hatItemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Accessory/HatImage")[1 + hatItemCode];
        price = prices[hatItemCode];
        name = names[hatItemCode];
        forcePercent = forcePercents[hatItemCode];
        reinforce_basic = reinforce_basics[hatItemCode];
    }
}

public class Ring : Accessory
{
    static public Sprite destroyedSprite = Resources.LoadAll<Sprite>("Images/Accessory/RingImage")[0];
    static private long[] prices = { 3000, 30000, 5000000, 2000000000, 300000000000, 70000000000000, 20, 200000, 2000000000, 20000000000000, 20, 200000, 2000000000, 20000000000000, 2 }; 
    // 3000원, 3만, 500만, 20억, 3000억, 70조, 20경, 20해, 20자, 20양, 20루니, 200,000루니, 2케니, 20,000케니, 2헤니
    static private string[] names = { "나무 반지", "돌 반지", "철 반지", "금 반지", "다이아 반지", "흑암석 반지",
            "청록석 반지", "영혼석 반지", "흑마석 반지", "태초석 반지", "연옥석 반지", "육천석 반지", "천계석 반지", "공허석 반지",
            "서플라스 반지" };
    static private float[] forcePercents = { 0.2f, 0.5f, 1f, 2f, 4f, 7f, 15f, 30f, 70f, 200f, 500f, 1000f, 2000f, 5000f, 10000f };
    static private float[] reinforce_basics = { 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.06f, 0.08f, 0.1f, 0.12f };

    public int ringItemCode;

    public Ring(int _itemCode)
    {
        itemCode = _itemCode + SaveScript.hatNum;
        ringItemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Accessory/RingImage")[1 + ringItemCode];
        price = prices[ringItemCode];
        name = names[ringItemCode];
        forcePercent = forcePercents[ringItemCode];
        reinforce_basic = reinforce_basics[ringItemCode];
    }
}

public class Pendant : Accessory
{
    static public Sprite destroyedSprite = Resources.LoadAll<Sprite>("Images/Accessory/PendantImage")[0];
    static private long[] prices = { 5000, 50000, 8000000, 3000000000, 400000000000, 100000000000000, 50, 500000, 5000000000, 50000000000000, 50, 500000, 5000000000, 50000000000000, 5 }; 
    // 5000원, 5만, 800만, 30억, 4000억, 100조, 50경, 50해, 50자, 50양, 50루니, 500,000루니, 5케니, 50,000케니, 5헤니
    static private string[] names = { "나무 목걸이", "돌 목걸이", "철 목걸이", "금 목걸이", "다이아 목걸이", "흑암석 목걸이"
            , "청록석 목걸이", "영혼석 목걸이", "흑마석 목걸이", "태초석 목걸이", "연옥석 목걸이", "육천석 목걸이", "천계석 목걸이", "공허석 목걸이" 
            , "서플라스 목걸이" };
    static private float[] forcePercents = { 2f, 4f, 7f, 15f, 30f, 70f, 200f, 500f, 2000f, 10000f, 50000f, 200000f, 1000000f, 10000000f, 100000000f };
    static private float[] reinforce_basics = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 3f, 5f, 10f };

    public int pendentItemCode;

    public Pendant(int _itemCode)
    {
        itemCode = _itemCode + SaveScript.hatNum + SaveScript.RingNum;
        pendentItemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Accessory/PendantImage")[1 + pendentItemCode];
        price = prices[pendentItemCode];
        name = names[pendentItemCode];
        forcePercent = forcePercents[pendentItemCode];
        reinforce_basic = reinforce_basics[pendentItemCode];
    }
}

public class Sword : Accessory
{
    static public Sprite destroyedSprite = Resources.LoadAll<Sprite>("Images/Accessory/SwordImage")[0];
    static private long[] prices = {0, 10000, 3000000, 1500000000, 250000000000, 50000000000000, 30, 300000, 3000000000, 30000000000000, 30, 300000, 3000000000, 30000000000000, 3 }; 
    // 1만, 300만, 15억, 2500억, 50조, 30경, 30해, 30자, 30양, 30루니, 300,000루니, 3케니, 30,000케니. 3헤니
    static private string[] names = { "나무 검", "돌 검", "철 검", "금 검", "다이아 검", "흑암석 검"
            , "청록석 검", "영혼석 검", "흑마석 검", "태초석 검", "연옥석 검", "육천석 검", "천계석 검", "공허석 검"
            , "서플라스 검" };
    static private float[] forcePercents = { 10f, 15f, 20f, 25f, 35f, 50f, 70f, 100f, 150f, 500f, 1500f, 5000f, 15000f, 30000f, 50000f };
    static private float[] reinforce_basics = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 4, 5, 7, 10 };

    public int swordItemCode;

    public Sword(int _itemCode)
    {
        itemCode = _itemCode + SaveScript.hatNum + SaveScript.RingNum + SaveScript.PendantNum;
        swordItemCode = _itemCode;
        sprite = Resources.LoadAll<Sprite>("Images/Accessory/SwordImage")[1 + swordItemCode];
        price = prices[swordItemCode];
        name = names[swordItemCode];
        forcePercent = forcePercents[swordItemCode];
        reinforce_basic = reinforce_basics[swordItemCode];
    }
}