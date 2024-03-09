using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Monster
{
    static private Sprite[] spriteData; // 0, 1, 2 = Reinforce | 3, 4, 5 = ManaOre

    public int species;
    public float moveSpeed;
    public bool isMad, isAttack, isMove;
    private float scale;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Sleep", -1, 0f);
        animator.SetBool("isDead", false);
        animator.SetBool("isAttack", false);
        if (spriteData == null)
            spriteData = Resources.LoadAll<Sprite>("Images/Monster/Golem");

        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sprite = spriteData[i + 3 * species];
            sprites[i].color = new Color(1f - 0.3f * type, 1f - 0.3f * type, 1f - 0.3f * type, 0f);
        }
        scale = 0.8f + 0.25f * type;
        transform.localScale = Vector3.one * scale;

        damage = (species == 0) ?  golem_damages[type] : manaGolem_damages[type];
        maxHP = (species == 0) ? golem_hps[type] : manaGolem_hps[type];
        exp = (species == 0) ? golem_exps[type] : manaGolem_exps[type];


        switch (type)
        {
            case 0:
                rigidbody.mass = 1.9f;
                height = 0.75f;
                break;
            case 1:
                rigidbody.mass = 5f;
                height = 0.9f;
                break;
            case 2:
                rigidbody.mass = 10f;
                height = 1.15f;
                break;
        }

        HP = maxHP;
        isMad = isAttack = isMove = false;
        moveSpeed = 0.5f;
        turnDis = 0.5f;
        kind = 5;
        StartCoroutine("FadeIn");

        if (Random.Range(0, 2) == 0)
        {
            isGotoRight = false;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            isGotoRight = true;
            transform.localScale = new Vector3(-scale, scale, scale);
        }
    }

    public override void Dead()
    {
        base.Dead();
        DropItem();

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 50 });
        QuestCtrl.instance.SetSubQuestAmount(13);
    }

    public override void DropItem()
    {
        base.DropItem();

        long reinforceNum = 0, manaNum = 0;
        float totalNum = 0f;
        float count = 0f;

        if (species == 0)
        {
            // 강화석 데이터 생성
            reinforceNum = golem_reinforceOres[type];
            reinforceNum = GameFuction.GetNumOreByRound(reinforceNum, totalNum, out totalNum);
        }
        else
        {
            // 마나석 데이터 생성
            manaNum = manaGolem_manaOres[type];
            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
                manaNum *= 2;
            manaNum = GameFuction.GetNumOreByRound(manaNum, totalNum, out totalNum);
        }
        count = -(totalNum / 2);
        GameFuction.SetDropForce(count, false);

        // 강화석 드랍
        GameFuction.CreateDropReinforceOre(this.transform.position, reinforceNum, count, out count);

        // 마나석 드랍
        GameFuction.CreateDropManaOre(this.transform.position, manaNum, count, out count);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            if (!isMad)
            {
                // 잠에서 깨어남
                if(HP < maxHP)
                {
                    animator.SetBool("isMad", true);
                    StartCoroutine(Start_Mad());
                }
            }
            else
            {
                if (!isAttack && Vector2.Distance(this.transform.position, PlayerScript.instance.transform.position) < 8f) StartCoroutine(Start_Attack());
                if (isMove && Mathf.Abs(this.transform.position.x - PlayerScript.instance.transform.position.x) < 20f) GotoPlayer();
            }

            if (rigidbody.velocity.y < -7.5f)
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, -7.5f);
        }
    }

    public void GotoPlayer()
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

    IEnumerator Start_Mad()
    {
        isMad = isAttack = true;
        yield return new WaitForSeconds(Random.Range(4f, 8f));
        isAttack = false;
    }

    IEnumerator Start_Attack()
    {
        isAttack = true;
        isMove = false;
        animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(Random.Range(6f, 8f));
        isAttack = false;
    }

    public void CheckAttack()
    {
        if (rigidbody.velocity.y == 0f)
        {
            audio.clip = SaveScript.SEs[18];
            audio.Play();

            // 플레이어 감지
            if (PlayerScript.instance.rigidbody.velocity.y == 0 && Mathf.Abs(this.transform.position.x - PlayerScript.instance.transform.position.x) < 10f)
            {
                PlayerScript.instance.DamageToPlayer((long)(damage * 1.5f), true);
            }
        }
    }

    public void EndAttack()
    {
        isMove = true;
        animator.SetBool("isAttack", false);
    }

    public void EndAwake()
    {
        isMove = true;
        animator.SetBool("isMad", false);
    }
}
