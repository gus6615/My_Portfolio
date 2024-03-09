using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_1_Midboss : Monster_Boss
{
    private float bufItemNum, reinforceItemNum;
    private int state;
    private float moveSpeed;
    private float turnDis;
    private float coolTime_skill_0, coolTime_skill_1;
    private float scale;
    private bool isMove, isSkill_0, isSkill_1;
    private bool isGotoRight;

    D_1_Midboss_Skill0 skill0;
    D_1_Midboss_Skill1 skill1;

    private void OnEnable()
    {
        ParentInit();
        animator.SetBool("isMove", false);
        animator.SetBool("isSkill_0", false);
        animator.SetBool("isSkill_1", false);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprite.color = SaveScript.monsterColors[type];
        damage = d_1_midboss_damages[type];
        maxHP = d_1_midboss_hps[type];
        exp = d_1_midboss_exps[type];
        manaOre = (long)(d_1_midboss_manaOres[type] * SaveScript.stat.bosMana);
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOre *= 2;
        cash = (long)(d_1_boss_cashes[type] * Random.Range(0.8f, 1.2f));

        bufItemNum = 10f;
        reinforceItemNum = 4f;
        HP = maxHP;
        kind = 1;
        state = 0;
        moveSpeed = 0.75f;
        turnDis = 0.5f;
        coolTime_skill_0 = 5f;
        coolTime_skill_1 = 8f;
        scale = 1.25f;
        isMove = false;
        isSkill_0 = isSkill_1 = true;

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
        
        if (!isDead)
        {
            if (state == 1) // Move
                GotoPlayer();

            if (distanceFromPlayer < 6f)
            {
                if (!isActive)
                {
                    isActive = true;
                    StartCoroutine("StartMove");
                    StartCoroutine("SetSkillTime");
                }

                if (!isSkill_0)
                    StartCoroutine("StartSkill_0");
                else if (!isSkill_1)
                    StartCoroutine("StartSkill_1");
            }
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

    IEnumerator StartMove()
    {
        switch (state)
        {
            case 0:
                state = 1;
                isMove = true;
                animator.SetBool("isMove", true);
                break;
            case 1: // Move
                if (GameFuction.GetRandFlag(0.5f))
                {
                    state = 0;
                    isMove = false;
                    animator.SetBool("isMove", false);
                }
                break;
        }

        yield return new WaitForSeconds(3f);

        StartCoroutine("StartMove");
    }

    IEnumerator SetSkillTime()
    {
        isSkill_0 = isSkill_1 = true;

        yield return new WaitForSeconds(Random.Range(0f, 2f));

        isSkill_0 = false;

        yield return new WaitForSeconds(Random.Range(2f, 3f));

        isSkill_1 = false;
    }

    IEnumerator LinkSkill()
    {
        yield return new WaitForSeconds(0.5f);
        StopCoroutine("StartSkill_0");
        isSkill_0 = false;
    }

    IEnumerator StartSkill_0()
    {
        state = 2;
        while (state == 3)
            yield return null; // Skill_1 코루틴 발동함
        if (state == 3)
            yield return new WaitForSeconds(2f);

        state = 2;
        isSkill_0 = true;
        isMove = false;
        animator.SetBool("isMove", false);
        animator.SetBool("isSkill_0", true);
        StopCoroutine("StartMove");

        int x = Mathf.RoundToInt(PlayerScript.instance.transform.position.x);
        int y = Mathf.RoundToInt(PlayerScript.instance.transform.position.y);
        Vector3Int vec = new Vector3Int(0, 10, 0);
        for (int j = y; j > y - 10; j--)
        {
            if (MapData.instance.GetTileMap(new Vector3Int(x, j, 0), 0).GetTile(new Vector3Int(x, j, 0)) != null)
            {
                vec = new Vector3Int(x, j + 1, 0);
                break;
            }
        }

        skill0 = ObjectPool.GetObject<D_1_Midboss_Skill0>(29, ObjectPool.instance.dungeon_1_room_objectTr, vec);
        skill0.damage = damage;
        skill0.type = type;

        yield return new WaitForSeconds(coolTime_skill_0);
        
        isSkill_0 = false;
    }

    IEnumerator StartSkill_1()
    {
        state = 3;
        while (state == 2)
            yield return null; // Skill_1 코루틴 발동함
        if (state == 2)
            yield return new WaitForSeconds(2f);

        state = 3;
        isSkill_1 = true;
        isMove = false;
        animator.SetBool("isMove", false);
        animator.SetBool("isSkill_1", true);
        StopCoroutine("StartMove");
        StopCoroutine("StartSkill_0");

        int x = Mathf.RoundToInt(PlayerScript.instance.transform.position.x);
        int y = Mathf.RoundToInt(PlayerScript.instance.transform.position.y);
        Vector3Int vec = new Vector3Int(0, 10, 0);
        for (int j = y; j > y - 10; j--)
        {
            if (MapData.instance.GetTileMap(new Vector3Int(x, j, 0), 0).GetTile(new Vector3Int(x, j, 0)) != null)
            {
                vec = new Vector3Int(x, j + 1, 0);
                break;
            }
        }

        skill1 = ObjectPool.GetObject<D_1_Midboss_Skill1>(30, ObjectPool.instance.dungeon_1_room_objectTr, vec);
        skill1.damage = damage;
        skill1.type = type;

        yield return new WaitForSeconds(coolTime_skill_1);

        isSkill_1 = false;
    }

    private void GotoPlayer()
    {
        if (isGotoRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
            {
                isGotoRight = false;
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
            {
                isGotoRight = true;
                transform.localScale = new Vector3(-scale, scale, scale);
            }
        }
    }

    public void End_Skill_0()
    {
        state = 0;
        animator.SetBool("isMove", false);
        animator.SetBool("isSkill_0", false);
        StartCoroutine("StartMove");
    }

    public void End_Skill_1()
    {
        state = 0;
        animator.SetBool("isMove", false);
        animator.SetBool("isSkill_1", false);
        StartCoroutine("StartMove");
        StartCoroutine("LinkSkill");
    }

    public void ReturnObject()
    {
        ObjectPool.ReturnObject<D_1_Midboss>(28, this);
    }
}
