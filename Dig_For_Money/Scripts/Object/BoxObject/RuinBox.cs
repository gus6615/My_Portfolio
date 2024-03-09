using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinBox : BoxObject
{
    private static float[] bufItemForce = { 0f, 0f, 0.5f, 1f, 0.5f, 1f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f, 5f, 6f, 7f };
    private static int[] bufItemType = { 0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static float[] ruinBoxForces = { 0f, 0f, 1f, 1.2f, 1.5f, 3f, 5f, 10f, 30f, 50f, 100f, 200f, 300f, 500f, 1000f };
    private static int[] ruinBoxExps = { 0, 0, 24, 30, 40, 53, 70, 90, 115, 145, 180, 230, 285, 340, 400 };
    private static int[] reinforceNums = { 5, 10, 15, 25, 40, 70, 100, 150, 200, 500, 1000, 2000, 3000, 5000, 10000 };
    private static float[][] orePercents =
{
        new float[] { 1f, 1f, 1f, 1f, 0f, 0f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 0f, 0f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 0.2f, 0f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 0.5f, 0f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 0.8f, 0f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0f, 0f },
        // 7층
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.2f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.3f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.4f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.5f, 0f },
        // 11층
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.7f, 0.025f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.8f, 0.05f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.9f, 0.1f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.2f },
        // 15층
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.4f },
    };

    private List<long> temp_jems = new List<long>();

    public override List<long> GetJems(float totalNum, out float out_totalNum)
    {
        temp_jems.Clear();

        int maxJemCode = 0;
        int minJemCode = 0;
        float[] percents = orePercents[boxType];
        for (int i = 0; i < boxType; i++)
            minJemCode += SaveScript.stageItemNums[i];
        maxJemCode = minJemCode + SaveScript.stageItemNums[boxType];

        for (int i = 0; i < maxJemCode; i++)
        {
            long rand = 0;

            if (i >= minJemCode && SaveScript.jems[i].quality > 2)
            {
                if (GameFuction.GetRandFlag(percents[SaveScript.jems[i].quality]))
                {
                    switch (SaveScript.jems[i].quality)
                    {
                        case 3: rand = Random.Range(4 * (boxType - 1), 8 * (boxType - 1)); break;
                        case 4: rand = Random.Range(3 * (boxType - 1), 6 * (boxType - 1)); break;
                        case 5:
                        case 6: rand = 1; break;
                    }
                    if (rand != 0) rand = rand + GameFuction.GetOreNum();
                    rand = (long)(rand * ruinBoxForces[boxType]);

                    if (SaveScript.jems[i].quality == 5)
                        rand = (long)(rand * Random.Range(0.05f, 0.1f));
                    else if (SaveScript.jems[i].quality == 6)
                        rand = (long)(rand * Random.Range(0.01f, 0.02f));
                }
            }

            rand = GameFuction.GetNumOreByRound(rand, totalNum, out totalNum);
            temp_jems.Add(rand);
        }
        out_totalNum = totalNum;

        return temp_jems;
    }

    public override void BoxOpen()
    {
        if (isOpen) return;
        base.BoxOpen();

        long reinforceNum = 0;
        long manaOreNum = 0;
        float manaOrePercent = 0f;
        float totalNum = 0;
        temp_jems = GetJems(totalNum, out totalNum);

        QuestCtrl.instance.SetMainQuestAmount(new int[] { 20 });
        QuestCtrl.instance.SetSubQuestAmount(9);
        PrintUI.instance.ExpInfo(ruinBoxExps[boxType], true);
        sprite.sprite = MapData.instance.dungeon_0_DecoX64Tiles[1].sprite;
        audio.clip = SaveScript.SEs[15];
        audio.Play();

        // 강화석 생성
        reinforceNum = (long)(reinforceNums[boxType] * Random.Range(1f, 1.2f));
        reinforceNum = GameFuction.GetNumOreByRound(reinforceNum, totalNum, out totalNum);

        // 마나석 생성
        manaOrePercent = (0.025f + 0.0025f * boxType) * (1f + SaveScript.stat.boxManaPercent);
        if (GameFuction.GetRandFlag(manaOrePercent))
            manaOreNum = (int)(Random.Range(boxType * 2, boxType * 3) * (1f + SaveScript.stat.boxMana));
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOreNum *= 2;
        manaOreNum = GameFuction.GetNumOreByRound(manaOreNum, totalNum, out totalNum);

        // 드랍될 아이템 생성
        float count = -(totalNum / 2);
        GameFuction.SetDropForce(count, false);

        // 강화석 생성
        GameFuction.CreateDropReinforceOre(this.transform.position, reinforceNum, count, out count);

        // 마나석 생성
        GameFuction.CreateDropManaOre(this.transform.position, manaOreNum, count, out count);

        // 광물 생성
        GameFuction.CreateDropJem(this.transform.position, temp_jems, count, out count);

        // 아이템 생성
        if (boxType >= 6) GameFuction.CreateElixirItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce[boxType], count, out count);
        else GameFuction.CreateBufItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce[boxType], bufItemType[boxType], count, out count);

        // 펫 생성
        GameFuction.CreateDropPet(this.transform.position, ObjectPool.instance.dungeon_0_objectTr, 1f, count, out count);
    }
}