using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon 
{
    static private bool isInit;
    static private string[] names;
    static private string[] infos, infos2;
    static private float[][] forces =
    {
         new float[] { 100f, 200f, 400f, 800f, 20f, 0.5f, 20f, 20f, 3f, 20f, 6f, 40f }, // 나무
         new float[] { 200f, 400f, 800f, 1600f, 25f, 0.55f, 25f, 25f, 5f, 25f, 10f, 50f }, // 돌
         new float[] { 400f, 800f, 1600f, 3200f, 30f, 0.6f, 30f, 30f, 7f, 30f, 14f, 60f }, // 철
         new float[] { 700f, 1400f, 2800f, 5600f, 35f, 0.7f, 35f, 35f, 10f, 35f, 20f, 70f }, // 금
         new float[] { 1000f, 2000f, 4000f, 8000f, 40f, 0.8f, 40f, 40f, 20f, 40f, 40f, 80f }, // 다이아
         new float[] { 1500f, 3000f, 7000f, 15000f, 45f, 0.9f, 45f, 45f, 40f, 45f, 80f, 90f }, // 흑암석
         new float[] { 3000f, 6000f, 15000f, 30000f, 50f, 1f, 50f, 50f, 70f, 50f, 140f, 100f }, // 청록석
         new float[] { 10000f, 20000f, 50000f, 100000f, 70f, 2f, 70f, 70f, 200f, 70f, 400f, 140f }, // 영혼석
         new float[] { 20000f, 50000f, 100000f, 200000f, 100f, 3f, 100f, 100f, 500f, 100f, 1000f, 200f }, // 흑마석
         new float[] { 100000f, 200000f, 500000f, 1000000f, 300f, 5f, 300f, 300f, 2000f, 300f, 4000f, 600f }, // 태초석
         new float[] { 500000f, 1000000f, 2500000f, 5000000f, 1000f, 10f, 1000f, 1000f, 10000f, 1000f, 20000f, 2000f }, // 연옥석
         new float[] { 3000000f, 7000000f, 15000000f, 30000000f, 3000f, 30f, 3000f, 3000f, 30000f, 3000f, 60000f, 6000f }, // 육천석
         new float[] { 20000000f, 50000000f, 100000000f, 200000000f, 5000f, 50f, 5000f, 5000f, 100000f, 5000f, 200000f, 10000f }, // 천계석
         new float[] { 100000000f, 200000000f, 500000000f, 1000000000f, 10000f, 100f, 10000f, 10000f, 200000f, 8000f, 400000f, 15000f }, // 공허석
         new float[] { 500000000f, 1000000000f, 2500000000f, 5000000000f, 20000f, 200f, 20000f, 20000f, 400000f, 15000f, 800000f, 30000f }, // 사플라스
    };

    public string name;
    public string info;
    public float force;
    public int code; // 아이콘 선별 코드
    public Sprite sprite;

    public Icon(int _code)
    {
        if (!isInit)
            Init();
        code = _code;
        sprite = Resources.LoadAll<Sprite>("Images/Icons/Rullet_Icon")[code];
    }

    public void SetData()
    {
        name = names[code] + (SaveScript.saveData.pickLevel + 1) + " 단계";
        force = forces[SaveScript.saveData.pickLevel][code];

        switch (code)
        {
            case 4: info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * force)) + infos2[code]; break;
            case 5: info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(force * 100)) + infos2[code]; break;
            case 6: 
                if (SaveScript.saveData.equipHat != -1) info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.hats[SaveScript.saveData.equipHat].reinforce_basic * force)) + infos2[code]; 
                else info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.swords[0].reinforce_basic * force)) + infos2[code];
                break;
            case 7: info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * force)) + infos2[code]; break;
            case 8:
            case 10:
                if (SaveScript.saveData.equipPendant != -1) info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * force)) + infos2[code];
                else info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.pendants[0].reinforce_basic * force)) + infos2[code];
                break;
            case 9:
            case 11:
                if (SaveScript.saveData.equipRing != -1) info = info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * force * 100f)) + infos2[code];
                else info = info = infos[code] + GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.rings[0].reinforce_basic * force * 100f)) + infos2[code];
                break;
        }
    }

    private void Init()
    {
        names = new string[SaveScript.iconNum];
        names[0] = "";
        names[1] = "";
        names[2] = "";
        names[3] = "";
        names[4] = "내구도 ";
        names[5] = "채광 속도 ";
        names[6] = "방어력 ";
        names[7] = "공격력 ";
        names[8] = "광석 행운 ";
        names[9] = "거래 효율 ";
        names[10] = "광석 행운Ⅱ ";
        names[11] = "거래 효율Ⅱ ";

        infos = new string[SaveScript.iconNum];
        infos[0] = "";
        infos[1] = "";
        infos[2] = "";
        infos[3] = "";
        infos[4] = "30분 동안 곡괭이 내구도 < ";
        infos[5] = "30분 동안 채광 속도 < ";
        infos[6] = "30분 동안 모자 효과 < ";
        infos[7] = "30분 동안 검 효과 < ";
        infos[8] = "30분 동안 광물 < ";
        infos[9] = "30분 동안 상점 < ";
        infos[10] = "30분 동안 광물 < ";
        infos[11] = "30분 동안 상점 < ";

        infos2 = new string[SaveScript.iconNum];
        infos2[0] = "";
        infos2[1] = "";
        infos2[2] = "";
        infos2[3] = "";
        infos2[4] = " > 증가";
        infos2[5] = "% > 증가";
        infos2[6] = " > 증가";
        infos2[7] = " > 증가";
        infos2[8] = "개 > 추가 휙득";
        infos2[9] = "% > 증가";
        infos2[10] = "개 > 추가 휙득";
        infos2[11] = "% > 증가";
    }
}
