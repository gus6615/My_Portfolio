using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientBox : BoxObject
{
    private static float petDropPercent = 0.25f;
    private static float[] itemPercentAsType = { 0f, 0f, 0f, 0f, 0f, 0f, 7f, 15f, 25f, 50f, 100f, 200f, 500f, 3000f, 10000f };
    private static long[] ancientBoxForces = { 0, 0, 0, 0, 0, 0, 5, 10, 15, 20, 30, 40, 50, 60, 70 };
    private static int[] ancientBoxExps = { 0, 0, 0, 0, 0, 0, 100, 120, 140, 170, 200, 250, 300, 350, 400 };
    private static long[] ancientBoxManaOres = { 0, 0, 0, 0, 0, 0, 30, 45, 70, 100, 150, 230, 350, 500, 700 }; // StageNum 변동
    private static float[][] petPercents =
    {
        new float[] { 0f, 0f, 0.6f, 0.35f, 0.05f, 0f, 0f, 0f, 0f, 0f }, // 7층 - B
        new float[] { 0f, 0f, 0.3f, 0.575f, 0.1f, 0.025f, 0f, 0f, 0f, 0f }, // 8층 - A
        new float[] { 0f, 0f, 0.1f, 0.65f, 0.2f, 0.045f, 0.005f, 0.000001f, 0f, 0f }, // 9층 - A ~ S
        new float[] { 0f, 0f, 0f, 0.55f, 0.35f, 0.0925f, 0.0075f, 0.000005f, 0f, 0f }, // 10층 - S
        new float[] { 0f, 0f, 0f, 0.35f, 0.525f, 0.125f, 0.01f, 0.00001f, 0.000001f, 0f }, // 11층 - S ~ SS
        new float[] { 0f, 0f, 0f, 0f, 0.8f, 0.18f, 0.02f, 0.00003f, 0.000002f, 0.000001f }, // 12층 - SS
        new float[] { 0f, 0f, 0f, 0f, 0.75f, 0.22f, 0.03f, 0.00005f, 0.000005f, 0.000002f }, // 13층 - SS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0.5f, 0.5f, 0.0001f, 0.00001f, 0.000005f }, // 14층 - SS ~ SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0.2f, 0.8f, 0.0002f, 0.00002f, 0.00001f }, // 15층 - SSS ~ U
    };

    List<long> temp_jems = new List<long>();

    public override List<long> GetJems(float totalNum, out float out_totalNum)
    {
        int ultimate = Jem.GetUItimateCode(boxType);
        int mystic = Jem.GetMysticCode(boxType);
        long num = GameFuction.GetOreNum();

        temp_jems.Clear();
        for (int i = 0; i < SaveScript.totalItemNum; i++)
            temp_jems.Add(0);

        num = num * ancientBoxForces[boxType];
        num = GameFuction.GetNumOreByRound(num, totalNum, out totalNum);
        temp_jems[ultimate] = num;

        if (mystic != -1 && GameFuction.GetRandFlag(0.2f))
        {
            num = GameFuction.GetNumOreByRound(num, totalNum, out totalNum);
            temp_jems[mystic] = (long)(num * 0.1f);
        }

        out_totalNum = totalNum;
        return temp_jems;
    }

    public override void BoxOpen()
    {
        if (isOpen) return;
        base.BoxOpen();

        long manaOreNum = 0;
        float totalNum = 0;
        temp_jems = GetJems(totalNum, out totalNum);

        QuestCtrl.instance.SetMainQuestAmount(new int[] { 55 });
        QuestCtrl.instance.SetSubQuestAmount(16);
        PrintUI.instance.ExpInfo(ancientBoxExps[boxType], true);
        sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[1].sprite;
        audio.clip = SaveScript.SEs[39];
        audio.Play();

        // 마나석 데이터 설정
        manaOreNum = (long)(ancientBoxManaOres[boxType] * (1f + SaveScript.stat.boxMana));
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOreNum *= 2;
        manaOreNum = GameFuction.GetNumOreByRound(manaOreNum, totalNum, out totalNum);

        // 드랍될 아이템 생성
        float count = -(totalNum / 2);
        GameFuction.SetDropForce(count, false);

        // 마나석 생성
        GameFuction.CreateDropManaOre(this.transform.position + Vector3.up, manaOreNum, count, out count);

        // 광물 생성
        GameFuction.CreateDropJem(this.transform.position + Vector3.up, temp_jems, count, out count);

        // 강화 아이템 생성
        GameFuction.CreateReinforce2Item(this.transform.position + Vector3.up, ObjectPool.instance.dungeon_1_room_objectTr, 0.99f, itemPercentAsType[boxType], count, out count);

        // 펫 생성
        if (GameFuction.GetRandFlag(petDropPercent))
            GameFuction.CreateDropPet(this.transform.position + Vector3.up, ObjectPool.instance.objectTr, petPercents[boxType - 6], count, out count);
    }
}