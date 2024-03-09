using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_1_Boss : Monster_Boss
{
    private float bufItemNum, reinforceItemNum;

    private void OnEnable()
    {
        ParentInit();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprite.color = SaveScript.monsterColors[type];
        damage = d_1_boss_damages[type];
        maxHP = d_1_boss_hps[type];
        exp = d_1_boss_exps[type];
        manaOre = (long)(d_1_boss_manaOres[type] * SaveScript.stat.bosMana);
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOre *= 2;
        cash = (long)(d_1_boss_cashes[type] * Random.Range(0.8f, 1.2f));

        bufItemNum = 10f;
        reinforceItemNum = 4f;
        HP = maxHP;
        kind = 2;

        StartCoroutine("FadeIn");
    }

    private void Update()
    {
        float distanceFromPlayer = Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position);

        if (isDead && distanceFromPlayer > 20f)
        {
            ReturnObject();
            return;
        }
    }

    public override void Dead()
    {
        base.Dead();

        audio.clip = SaveScript.SEs[7];
        audio.Play();
        DropItem();

        QuestCtrl.instance.SetMainQuestAmount(new int[] { 54 });
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 58 });
        QuestCtrl.instance.SetSubQuestAmount(15);
        QuestCtrl.instance.SetSubQuestAmount(17);
    }

    public override void DropItem()
    {
        base.DropItem();

        float totalNum = 0f;
        float count = 0f;
;
        manaOre = GameFuction.GetNumOreByRound(manaOre, totalNum, out totalNum);
        cash = GameFuction.GetNumOreByRound(cash, totalNum, out totalNum);
        totalNum += bufItemNum + reinforceItemNum + 1;
        count = -(totalNum / 2);
        GameFuction.SetDropForce(count, true);

        // 레드 다이아몬드(Cash) 생성
        GameFuction.CreateDropCashOre(this.transform.position, cash, count, out count);

        // 마나석 생성
        GameFuction.CreateDropManaOre(this.transform.position + Vector3.up, manaOre, count, out count);

        // 영약 아이템 생성
        GameFuction.CreateElixirItem(this.transform.position + Vector3.up, ObjectPool.instance.dungeon_1_objectTr, bufItemNum, count, out count);

        // 강화 아이템 생성
        GameFuction.CreateReinforce2Item(this.transform.position + Vector3.up, ObjectPool.instance.dungeon_1_objectTr, reinforceItemNum, itemPercentAsType[type], count, out count);

        // 펫 생성
        GameFuction.CreateDropPet(this.transform.position + Vector3.up, ObjectPool.instance.dungeon_1_room_objectTr, petPercents[type - 6], count, out count);
    }

    public void ReturnObject()
    {
        ObjectPool.ReturnObject<D_1_Boss>(32, this);
    }
}
