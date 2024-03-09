using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_1_HoleSlime : Monster
{
    private float moveSpeed;
    private float attackTime;
    private float size;
    private bool isAttack, isAttackStart;
    private bool isIdle;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Idle", -1, 0f);
        animator.SetBool("isMove", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isDead", false);

        StartCoroutine(Init());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            // 가까이 있으면 false, 멀리 있으면 true;
            isIdle = !(Vector3.Distance(PlayerScript.instance.transform.position, this.transform.position) < 8f);
            if (!isIdle)
            {
                if (CheckPlayer())
                {
                    if (!isAttackStart)
                        StartCoroutine("Attack");
                }
                if (!isAttack)
                    GotoPlayer();
            }
            else
            {
                animator.SetBool("isMove", false);
                animator.SetBool("isAttack", false);
            }
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        audio = GetComponent<AudioSource>();
        damage = d_1_holeSlime_damages[type];
        maxHP = d_1_holeSlime_hps[type];
        exp = d_1_holeSlime_exps[type];

        HP = maxHP;
        sprites[0].color = SaveScript.monsterColors[type];
        kind = 7;
        moveSpeed = 0.75f;
        attackTime = 1.5f;
        percentDamage = 0.125f;
        size = 1.25f;
        turnDis = 0.5f;
        height = 1.5f;
        isAttack = isAttackStart = false;
        isIdle = true;

        audio.mute = !SaveScript.saveData.isSEOn;
        if (transform.position.x >= PlayerScript.instance.transform.position.x)
        {
            isGotoRight = false;
            transform.localScale = new Vector3(size, size, size);
        }
        else
        {
            isGotoRight = true;
            transform.localScale = new Vector3(-size, size, size);
        }

        StartCoroutine("FadeIn");
    }

    public override void Dead()
    {
        base.Dead();

        DropItem();
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 54 });
        QuestCtrl.instance.SetSubQuestAmount(15);
    }

    public override void DropItem()
    {
        base.DropItem();

        long manaNum = 0;
        float count = 0f;

        // 마나석 생성 여부 결정
        if (GameFuction.GetRandFlag(0.3f + type * 0.05f))
        {
            manaNum = (int)(all_manaOres[type] * Random.Range(0.8f, 1.2f));
            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
                manaNum *= 2;
            manaNum = GameFuction.GetNumOreByRound(manaNum, count, out count);
            count = -(count / 2);
        }

        // 마나석 드랍
        GameFuction.SetDropForce(count, false);
        GameFuction.CreateDropManaOre(this.transform.position, manaNum, count, out count);

        // 아이템 생성
        GameFuction.CreateElixirItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce_ancient[type], count, out count);
    }

    public void GotoPlayer()
    {
        animator.SetBool("isMove", true);
        animator.SetBool("isAttack", false);
        if (isGotoRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
            {
                isGotoRight = false;
                transform.localScale = new Vector3(size, size, size);
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
            {
                isGotoRight = true;
                transform.localScale = new Vector3(-size, size, size);
            }
        }
    }

    public bool CheckPlayer()
    {
        RaycastHit2D hitRight = Physics2D.BoxCast(this.transform.position, new Vector2(1f, 1f), 0f, Vector2.right, 1f, 512);
        RaycastHit2D hitLeft = Physics2D.BoxCast(this.transform.position, new Vector2(1f, 1f), 0f, Vector2.left, 1f, 512);

        if (hitRight || hitLeft)
            return true;
        else
            return false;
    }

    IEnumerator Attack()
    {
        isAttackStart = true;
        isAttack = true;
        animator.SetBool("isMove", false);
        animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(attackTime);

        isAttackStart = false;
    }

    public void EndAttack()
    {
        isAttack = false;
    }

    public void CheckAttackDamage()
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position + new Vector3(0.5f * -Mathf.Sign(this.transform.localScale.x), 0.15f), new Vector2(2f, 2.5f)
            , 0f, Vector2.right * -Mathf.Sign(this.transform.localScale.x), 0f, 512);
        audio.clip = SaveScript.SEs[38];
        audio.Play();

        if (hit)
        {
            // 퍼센트 데미지로 적용
            long realDamage = (long)(PlayerScript.instance.pickFullHP * percentDamage);
            if (realDamage < damage)
                realDamage = damage;
            PlayerScript.instance.DamageToPlayer(realDamage, true);
        }
    }
}
