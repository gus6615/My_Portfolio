using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSlime : Monster
{
    private Vector3 moveVec;
    private float moveSpeed;
    private float changeIdleTime, changeFormTime;
    private bool isIdle, isIdleChange;
    private bool isStartDefend, isDefend;
    private bool isChangeForm, isChangeFormTime, isChanging;
    public bool isDefending; // 현재 방어 상태인가?

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Idle", -1, 0f);
        animator.SetBool("isMove", false);
        animator.SetBool("isStartDefend", false);
        animator.SetBool("isDefend", false);
        animator.SetBool("isDefending", false);
        animator.SetBool("isDead", false);

        StartCoroutine("Init");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDelete && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 50f)
        {
            ObjectPool.ReturnObject<MushroomSlime>(11, this);
        }

        if (!isDead)
        {
            if(isChangeFormTime)
                StartCoroutine("SetForm");

            if (!isStartDefend)
            {
                if (isChangeForm)
                {
                    ChangeForm();
                }
                else if(!isChanging)
                {
                    if (!isIdleChange)
                        StartCoroutine("Idle");

                    if (isIdle)
                    {
                        animator.SetBool("isMove", false);
                    }
                    else
                    {
                        animator.SetBool("isMove", true);
                        this.transform.position += moveVec * moveSpeed * Time.deltaTime;
                    }
                }
            }
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprites[0].color = SaveScript.monsterColors[type];
        damage = mushroomSlime_damages[type];
        maxHP = mushroomSlime_hps[type];
        exp = mushroomSlime_exps[type];

        HP = maxHP;
        kind = 4;
        moveSpeed = 0.5f;
        height = 0.5f;
        isChangeFormTime = true;
        isIdle = isIdleChange = false;
        isStartDefend = isDefend = false;
        isChangeForm = isChanging = false;
        isDefending = false;
        StartCoroutine("Delete");
        StartCoroutine("FadeIn");
    }

    public override void Dead()
    {
        base.Dead();
        DropItem();

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 19 });
        QuestCtrl.instance.SetSubQuestAmount(8);
    }

    public override void DropItem()
    {
        base.DropItem();

        long reinforceNum = 0;
        float totalNum = 0f;
        float count = 0f;

        // 강화석 생성 여부 결정
        if (GameFuction.GetRandFlag(0.1f + type * 0.05f))
        {
            reinforceNum = Random.Range(1 + (type + kind) * 2, 2 + (type + kind) * 5);
            reinforceNum = GameFuction.GetNumOreByRound(reinforceNum, totalNum, out totalNum);
        }
        count = -(totalNum / 2);

        // 강화석 드랍
        GameFuction.SetDropForce(count, false);
        GameFuction.CreateDropReinforceOre(this.transform.position, reinforceNum, count, out count);

        // 아이템 생성
        if (type >= 6) GameFuction.CreateElixirItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce_ruin[type], count, out count);
        else GameFuction.CreateBufItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce_ruin[type], bufItemType_ruin[type], count, out count);
    }

    IEnumerator Idle()
    {
        isIdleChange = true;
        isIdle = !isIdle;
        changeIdleTime = Random.Range(1f, 2f);
        if (Random.Range(1, 3) == 1)
        {
            moveVec = Vector3.right;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            moveVec = Vector3.left;
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        yield return new WaitForSeconds(changeIdleTime);

        isIdleChange = false;
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(60f);
        isDelete = true;
    }

    IEnumerator SetForm()
    {
        isChangeFormTime = false;
        changeFormTime = Random.Range(5f, 8f);

        yield return new WaitForSeconds(changeFormTime);

        isChangeForm = true;
        isChangeFormTime = true;
    }

    public void ChangeForm()
    {
        isChangeForm = false;
        isChanging = true;
        animator.SetBool("isStartDefend", true);
    }

    public void Defend()
    {
        animator.SetBool("isStartDefend", false);
        animator.SetBool("isDefend", true);
    }

    public void DefendEnd()
    {
        isChanging = false;
        isDefending = !isDefending;
        animator.SetBool("isDefending", isDefending);
        animator.SetBool("isDefend", false);
    }
}
