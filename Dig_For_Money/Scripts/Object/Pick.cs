using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pick 
{
    static public Sprite destroyedSprite = Resources.LoadAll<Sprite>("Images/Picks")[0];
    static private string[] names = { "나무 곡괭이", "돌 곡괭이", "철 곡괭이", "금 곡괭이", "다이아 곡괭이", "흑암석 곡괭이"
            , "청록석 곡괭이", "영혼석 곡괭이", "흑마석 곡괭이", "태초석 곡괭이", "연옥석 곡괭이", "육천석 곡괭이", "천계석 곡괭이", "공허석 곡괭이"
            , "서플라스 곡괭이" };
    static private long[] prices = { 0, 10000, 3000000, 1000000000, 100000000000, 10000000000000, 1, 10000, 100000000, 1000000000000, 1, 10000, 1000000000, 10000000000000, 1 }; 
    // 0원, 1만원, 300만원, 10억, 1000억, 10조, 1경, 1해, 1자, 1양, 1루니, 10,000루니, 1케니, 10,000케니, 1헤니
    static private float[] breakTimes = { 0.1f, 0.09f, 0.08f, 0.07f, 0.06f, 0.05f, 0.04f, 0.03f, 0.02f, 0.01f, 0.005f, 0.001f, 0.0005f, 0.0001f, 0.00005f};
    static private int[] durabilitys = { 1000, 1500, 2000, 3000, 4000, 5000, 6000, 8000, 11000, 14000, 20000, 30000, 45000, 70000, 100000 };
    static private int[] reinforce_basics = { 20, 25, 30, 35, 40, 45, 50, 70, 100, 150, 200, 300, 500, 800, 1500 };

    public Sprite[] sprites;
    public string name;
    public int itemCode;
    public long price;
    public float breakTime;
    public int durability;
    public int reinforce_basic;

    public Pick(int _itemCode)
    {
        itemCode = _itemCode;
        SettingPick();
    }

    public void SettingPick()
    {
        name = names[itemCode];
        price = prices[itemCode];
        breakTime = breakTimes[itemCode];
        durability = durabilitys[itemCode];
        reinforce_basic = reinforce_basics[itemCode];
        sprites = new Sprite[SaveScript.pickStateNum];
        for (int i = 0; i < sprites.Length; i++)
            sprites[i] = Resources.LoadAll<Sprite>("Images/Picks")[1 + itemCode * 3 + i];
    }
}
