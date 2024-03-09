using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientTresure : BreakObject
{
    private static int[] reinforceItemNum = { 0, 0, 0, 0, 0, 0, 4, 4, 5, 5, 6, 6, 7, 7, 8 };
    private static float[] itemPercentAsType = { 0f, 0f, 0f, 0f, 0f, 0f, 15f, 35f, 50f, 80f, 150f, 1000f, 5000f, 10000f, 20000f };
    private static long[] manaOres = { 0, 0, 0, 0, 0, 0, 100, 130, 160, 200, 270, 400, 650, 900, 1200 }; // StageNum 변동
    static public float[][] petPercents =
    {
        new float[] { 0f, 0f, 0f, 0.6f, 0.35f, 0.05f, 0f, 0f, 0f, 0f }, // 7층 - A
        new float[] { 0f, 0f, 0f, 0f, 0.85f, 0.125f, 0.025f, 0f, 0f, 0f }, // 8층 - S
        new float[] { 0f, 0f, 0f, 0f, 0.7f, 0.25f, 0.05f, 0.0001f, 0f, 0f }, // 9층 - S ~ SS
        new float[] { 0f, 0f, 0f, 0f, 0.6f, 0.325f, 0.075f, 0.00025f, 0.0001f, 0f }, // 10층 - SS
        new float[] { 0f, 0f, 0f, 0f, 0.4f, 0.5f, 0.1f, 0.001f, 0.0001f, 0.00001f }, // 11층 - SS ~ SSS
        new float[] { 0f, 0f, 0f, 0f, 0.2f, 0.65f, 0.15f, 0.0015f, 0.00015f, 0.000015f  }, // 12층 - SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0.8f, 0.2f, 0.0025f, 0.00025f, 0.000025f }, // 13층 - SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0.3f, 0.7f, 0.005f, 0.0005f, 0.00005f }, // 14층 - SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0.99f, 0.01f, 0.00075f, 0.000075f }, // 15층 - SSS
    };

    public override void DamageToObject(long _damage)
    {
        base.DamageToObject(_damage);
        audio.clip = SaveScript.SEs[41];
        audio.Play();

        if (HP <= 0f)
            this.Reward();
    }

    public override void Reward()
    {
        if (isOpen) return;
        base.Reward();
        sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[3].sprite;
        audio.clip = SaveScript.SEs[15];
        audio.Play();

        QuestCtrl.instance.SetMainQuestAmount(new int[] { 57 });
        long manaOreNum = 0;
        float totalNum = 0;

        // 마나석 데이터 설정
        manaOreNum = (long)(manaOres[boxType] * (1f + SaveScript.stat.boxMana));
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOreNum *= 2;
        manaOreNum = GameFuction.GetNumOreByRound(manaOreNum, totalNum, out totalNum);
        totalNum += reinforceItemNum[boxType] + 1;

        // 드랍될 아이템 생성
        float count = -(totalNum / 2);
        GameFuction.SetDropForce(count, false);

        // 마나석 생성
        GameFuction.CreateDropManaOre(this.transform.position + Vector3.up, manaOreNum, count, out count);

        // 강화 아이템 생성
        GameFuction.CreateReinforce2Item(this.transform.position + Vector3.up, ObjectPool.instance.dungeon_1_room_objectTr, reinforceItemNum[boxType], itemPercentAsType[boxType], count, out count);

        // 펫 생성
        GameFuction.CreateDropPet(this.transform.position + Vector3.up, ObjectPool.instance.dungeon_1_room_objectTr, petPercents[boxType - 6], count, out count);
    }
}
