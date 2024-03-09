using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Monster_Boss
{
    private int page; // 0 = sleep, 1 = awake, 2 = mad
    private bool isAttack, isSkill, isCreateMonsters;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Sleep", -1, 0f);
        animator.SetBool("isDead", false);
        animator.SetBool("isSkill", false);
        animator.SetBool("isOnPage", false);
        animator.SetBool("isCreateMonsters", false);
        animator.SetInteger("Page", 0);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprite.color = SaveScript.monsterColors[type];
        damage = kingSlime_damages[type];
        maxHP = kingSlime_hps[type];
        exp = kingSlime_exps[type];
        manaOre = (long)(kingSlime_manaOres[type] * Random.Range(0.8f, 1.2f));
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOre *= 2;
        cash = 0;

        HP = maxHP;
        kind = 0;
        page = 0;
        isAttack = isCreateMonsters = isSkill = false;
        max_createMonsters = 6;
        cur_createMonsters = 0;

        StartCoroutine("FadeIn");
    }

    private void Update()
    {
        if (isDead && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 20f)
        {
            ObjectPool.ReturnObject<KingSlime>(16, this);
        }

        if (!isDead && Vector3.Distance(PlayerScript.instance.transform.position, this.transform.position) < 8f)
        {
            if (page == 0)
            {
                // 잠에서 깨기
                if (HP < maxHP)
                {
                    animator.SetInteger("Page", 0);
                    animator.SetBool("isOnPage", true);
                }
            }
            else if (page == 1)
            {
                // 화난 상태 돌입
                if (HP < maxHP * 0.5f)
                {
                    animator.SetInteger("Page", 1);
                    animator.SetBool("isOnPage", true);
                }
                // 일반 공격 및 소환술
                if (!isAttack) StartCoroutine(Attack());
                if (!isCreateMonsters && cur_createMonsters < max_createMonsters) StartCoroutine(CreateMonsters());
            }
            else if (page == 2)
            {
                // 일반 공격 및 스킬 공격
                if (!isAttack) StartCoroutine(Attack());
                if (!isSkill) StartCoroutine(Skill());
            }
        }
    }

    public override void Dead()
    {
        base.Dead();
 
        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 31 });
        QuestCtrl.instance.SetSubQuestAmount(5);

        audio.clip = SaveScript.SEs[7];
        audio.Play();
        DropItem();
    }

    public override void DropItem()
    {
        base.DropItem();

        float totalNum = 0f;
        float count = 0f;

        manaOre = (long)(manaOre * SaveScript.stat.bosMana);
        manaOre = GameFuction.GetNumOreByRound(manaOre, totalNum, out totalNum);
        totalNum += kingSlime_bufItemForces[type] + 1;
        count = -(totalNum / 2);
        GameFuction.SetDropForce(count, true);

        // 마나석 생성
        GameFuction.CreateDropManaOre(this.transform.position, manaOre, count, out count);

        // 아이템 생성
        if (type >= 6)
            GameFuction.CreateElixirItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, kingSlime_bufItemForces[type], count, out count);
        else GameFuction.CreateBufItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, kingSlime_bufItemForces[type], kingSlime_bufItemTypes[type], count, out count);

        // 펫 생성
        GameFuction.CreateDropPet(this.transform.position, ObjectPool.instance.objectTr, (100 * petPercentsAsType[type]), count, out count);
    }

    IEnumerator Attack()
    {
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

        isAttack = true;
        KingSlime_Attack attack = ObjectPool.GetObject<KingSlime_Attack>(17, ObjectPool.instance.objectTr, vec);
        attack.damage = damage;
        attack.type = type;

        yield return new WaitForSeconds(4f - page);
        isAttack = false;
    }

    IEnumerator CreateMonsters()
    {
        isCreateMonsters = true;
        animator.SetBool("isCreateMonsters", true);
        int num = Random.Range(1, 3);
        if (cur_createMonsters + num > max_createMonsters)
            num = max_createMonsters - cur_createMonsters;
        for (int i = 0; i < num; i++)
        {
            Monster monster = null;
            int monster_type = 0;
            if(type > 1)
                monster_type = Random.Range(2, 5);

            switch (monster_type)
            {
                case 0: monster = ObjectPool.GetObject<SoilSlime>(6, ObjectPool.instance.objectTr, this.transform.position + Vector3.right * Random.Range(-3f, 3f)); break;
                case 2: monster = ObjectPool.GetObject<SpeedSlime>(9, ObjectPool.instance.objectTr, this.transform.position + Vector3.right * Random.Range(-3f, 3f)); break;
                case 3: monster = ObjectPool.GetObject<BigSlime>(10, ObjectPool.instance.objectTr, this.transform.position + Vector3.right * Random.Range(-3f, 3f)); break;
                case 4: monster = ObjectPool.GetObject<MushroomSlime>(11, ObjectPool.instance.objectTr, this.transform.position + Vector3.right * Random.Range(-3f, 3f)); break;
            }
            monster.type = type;
            monster.boss = this;
            cur_createMonsters++;
        }

        yield return new WaitForSeconds(Random.Range(12f, 16f));

        isCreateMonsters = false;
    }

    IEnumerator Skill()
    {
        isSkill = true;
        animator.SetBool("isSkill", true);
        int num = Random.Range(6, 9);
        int x = Mathf.RoundToInt(this.transform.position.x);
        int y = Mathf.RoundToInt(this.transform.position.y);
        Vector3Int vec = new Vector3Int(0, 10, 0);
        List<int> emptyList = new List<int>();
        for (int i = -5; i <= 5; i++)
            emptyList.Add(i);

        for (int i = 0; i < num; i++)
        {
            int order = Random.Range(0, emptyList.Count);
            int data = emptyList[order];
            emptyList.RemoveAt(order);
            for (int j = y; j > y - 10; j--)
            {
                if (MapData.instance.GetTileMap(new Vector3Int(x + data, j, 0), 0).GetTile(new Vector3Int(x + data, j, 0)) != null)
                {
                    vec = new Vector3Int(x + data, j + 1, 0);
                    break;
                }
            }

            KingSlime_Attack attack = ObjectPool.GetObject<KingSlime_Attack>(17, ObjectPool.instance.objectTr, vec);
            attack.damage = damage;
            attack.type = type;
        }

        yield return new WaitForSeconds(Random.Range(4f, 6f));
        isSkill = false;
    }

    public void NextPage()
    {
        page++;
        isAttack = false;
        animator.SetBool("isOnPage", false);
    }

    public void End_CreateMonsters()
    {
        animator.SetBool("isCreateMonsters", false);
    }

    public void End_Skill()
    {
        animator.SetBool("isSkill", false);
    }

    public void ReturnObject()
    {
        ObjectPool.ReturnObject<KingSlime>(16, this);
    }
}
