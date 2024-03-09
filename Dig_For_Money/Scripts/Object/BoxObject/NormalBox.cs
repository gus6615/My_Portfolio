using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBox : BoxObject
{
    private static float[] bufItemForce = { 0.01f, 0.015f, 0.02f, 0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f, 0.045f, 0.05f, 0.055f, 0.06f, 0.07f };
    private static int[] bufItemType = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
    private static float[] normalBoxForces = { 1f, 1f, 1.1f, 1.3f, 1.5f, 2f, 3f, 5f, 7f, 10f, 20f, 30f, 50f, 70f, 100f };
    private static int[] normalBoxExps = { 3, 5, 8, 11, 15, 21, 28, 40, 55, 75, 100, 130, 165, 190, 220 };
    private static int[] reinforceNums = { 5, 10, 15, 20, 25, 30, 50, 80, 120, 200, 300, 500, 1000, 2000, 5000 };
    private static float[][] orePercents =
    {
        new float[] { 0.7f, 0.3f, 0f, 0f, 0f, 0f, 0f },
        new float[] { 1f, 0.5f, 0.2f, 0f, 0f, 0f, 0f },
        new float[] { 1f, 0.7f, 0.4f, 0.15f, 0f, 0f, 0f },
        new float[] { 1f, 0.9f, 0.6f, 0.3f, 0.1f, 0f, 0f },
        new float[] { 1f, 1f, 0.9f, 0.65f, 0.35f, 0f, 0f },
        new float[] { 1f, 1f, 1f, 0.8f, 0.6f, 0f, 0f },
        // 7층
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.05f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.1f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.15f, 0f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.2f, 0f },
        // 11층
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.25f, 0.01f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.3f, 0.02f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.4f, 0.03f },
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.5f, 0.05f },
        // 15층
        new float[] { 1f, 1f, 1f, 1f, 1f, 0.6f, 0.1f },
    };

    private List<long> temp_jems = new List<long>();

    public override List<long> GetJems(float totalNum, out float out_totalNum)
    {
        temp_jems.Clear();
        if (SaveScript.saveData.isTutorial)
        {
            temp_jems.Add(4); temp_jems.Add(4); temp_jems.Add(4); temp_jems.Add(4);
            totalNum = 16;
        }
        else
        {
            int maxJemCode = 0;
            int minJemCode = 0;
            float[] percents = orePercents[boxType];
            for (int i = 0; i < boxType; i++)
                minJemCode += SaveScript.stageItemNums[i];
            maxJemCode = minJemCode + SaveScript.stageItemNums[boxType];


            for (int i = 0; i < maxJemCode; i++)
            {
                long rand = 0;

                if (i >= minJemCode && GameFuction.GetRandFlag(percents[SaveScript.jems[i].quality]))
                {
                    switch (SaveScript.jems[i].quality)
                    {
                        case 0: rand = Random.Range(3 * (boxType + 1), 3 * (boxType + 2)); break;
                        case 1: rand = Random.Range(2 * (boxType + 1), 2 * (boxType + 2)); break;
                        case 2: rand = Random.Range(2 * boxType, 3 * boxType); break;
                        case 3: rand = Random.Range(1 * (boxType - 1), 2 * (boxType - 1)); break;
                        case 4: rand = Random.Range(1 * (boxType - 2), 2 * (boxType - 2)); break;
                        case 5:
                        case 6: rand = 1; break;
                    }
                    if (rand != 0) rand = rand + GameFuction.GetOreNum();
                    rand = (long)(rand * normalBoxForces[boxType]);

                    if (SaveScript.jems[i].quality == 5)
                        rand = (long)(rand * Random.Range(0.05f, 0.1f));
                    else if (SaveScript.jems[i].quality == 6)
                        rand = (long)(rand * Random.Range(0.01f, 0.02f));
                }

                rand = GameFuction.GetNumOreByRound(rand, totalNum, out totalNum);
                temp_jems.Add(rand);
            }
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

        PrintUI.instance.ExpInfo(normalBoxExps[boxType], true);
        sprite.sprite = MapData.instance.mapBlockTiles[1].sprite;
        audio.clip = SaveScript.SEs[6];
        audio.Play();

        temp_jems = GetJems(totalNum, out totalNum);
        if (SaveScript.saveData.isTutorial)
        {
            float tempCount = -(totalNum / 2);
            GameFuction.SetDropForce(tempCount, false);
            GameFuction.CreateDropJem(this.transform.position, temp_jems, tempCount, out tempCount);
            return;
        }

        // 강화석 생성
        reinforceNum = (long)(reinforceNums[boxType] * Random.Range(0.8f, 1.2f));
        reinforceNum = GameFuction.GetNumOreByRound(reinforceNum, totalNum, out totalNum);

        // 마나석 생성
        manaOrePercent = 0.0025f * boxType * (1f + SaveScript.stat.boxManaPercent);
        if (GameFuction.GetRandFlag(manaOrePercent))
            manaOreNum = (int)(Random.Range(boxType, boxType + 4) * (1f + SaveScript.stat.boxMana));
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOreNum *= 2;
        manaOreNum = GameFuction.GetNumOreByRound(manaOreNum, totalNum, out totalNum);

        // 퀘스트
        QuestCtrl.instance.SetSubQuestAmount(2);

        // 드랍될 아이템 생성
        float count = -(totalNum / 2);
        GameFuction.SetDropForce(count, false);
        // 강화석 생성
        GameFuction.CreateDropReinforceOre(this.transform.position, reinforceNum, count, out count);
        // 마나석 생성
        GameFuction.CreateDropManaOre(this.transform.position, manaOreNum, count, out count);
        // 광물 생성
        GameFuction.CreateDropJem(this.transform.position, temp_jems, count, out count);
        if (boxKind == 0)
        {
            // 아이템 생성
            if (boxType >= 10) GameFuction.CreateElixirItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce[boxType], count, out count);
            else GameFuction.CreateBufItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce[boxType], bufItemType[boxType], count, out count);
        }
    }
}