using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_1_MimicSlime : Monster
{
    private float attackTime;
    private float size;
    private bool isAttack, isAttackStart;

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
            if (CheckPlayer())
            {
                if (!isAttackStart && CheckAttack())
                    StartCoroutine("Attack");
            }
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        audio = GetComponent<AudioSource>();
        damage = d_1_mimicSlime_damages[type];
        maxHP = d_1_mimicSlime_hps[type];
        exp = d_1_mimicSlime_exps[type];

        HP = maxHP;
        sprites[0].color = SaveScript.monsterColors[type];
        kind = 7;
        attackTime = 2.5f;
        percentDamage = 0.2f;
        size = 1f;
        height = 0.25f;
        isAttack = isAttackStart = false;

        audio.mute = !SaveScript.saveData.isSEOn;
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

    public bool CheckPlayer()
    {
        if (isAttack) return false; // 공격 도중엔 체크 X
        RaycastHit2D hitRight = Physics2D.BoxCast(this.transform.position + new Vector3(1f, -0.5f, 0f), new Vector2(1.25f, 1f), 0f, Vector2.right, 1f, 512);
        RaycastHit2D hitLeft = Physics2D.BoxCast(this.transform.position + new Vector3(-1f, -0.5f, 0f), new Vector2(1.25f, 1f), 0f, Vector2.left, 1f, 512);

        if (hitRight || hitLeft)
        {
            if (hitRight)
                transform.localScale = new Vector3(-size, size, size);
            else if (hitLeft)
                transform.localScale = new Vector3(size, size, size);
            return true;
        }
        else
            return false;
    }

    public bool CheckAttack()
    {
        return Physics2D.Raycast(this.transform.position - Vector3.up * 0.5f, Vector2.right * -Mathf.Sign(this.transform.localScale.x), 2f, 512);
    }

    IEnumerator Attack()
    {
        isAttackStart = true;
        isAttack = true;
        animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(attackTime);

        isAttackStart = false;
    }

    public void EndAttack()
    {
        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    public void CheckAttackDamage()
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position + new Vector3(-Mathf.Sign(this.transform.localScale.x) * 0.75f, -0.5f, 0f), new Vector2(2.5f, 1f)
            , 0f, Vector2.right * -Mathf.Sign(this.transform.localScale.x), 0f, 512);
        audio.clip = SaveScript.SEs[40];
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
