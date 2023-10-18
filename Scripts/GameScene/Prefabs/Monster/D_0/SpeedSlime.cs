using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSlime : Monster
{
    private Vector3 moveVec;
    private float moveSpeed;
    private float rushSpeed;
    private float changeIdleTime;
    private bool isIdle, isIdleChange;
    private bool isStartRush, isRush;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Idle", -1, 0f);
        animator.SetBool("isMove", false);
        animator.SetBool("isRush", false);
        animator.SetBool("isStartRush", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isAferDead", false);

        StartCoroutine("Init");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDelete && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 50f)
        {
            ObjectPool.ReturnObject<SpeedSlime>(9, this);
        }

        if (!isDead)
        {
            if (!isStartRush)
            {
                if (CheckPlayer())
                {
                    isStartRush = true;
                    animator.SetBool("isStartRush", true);
                }
                else
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
            else
            {
                if (isRush)
                {
                    this.transform.position += moveVec * rushSpeed * Time.deltaTime;
                }
            }
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprites[0].color = SaveScript.monsterColors[type];
        damage = speedSlime_damages[type];
        maxHP = speedSlime_hps[type];
        exp = speedSlime_exps[type];

        HP = maxHP; 
        kind = 2;
        height = 0.5f;
        moveSpeed = 0.5f;
        rushSpeed = 5f;
        isIdle = isIdleChange = isStartRush = isRush = false;
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
        GameFuction.SetDropForce(count, false);

        // 강화석 드랍
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

    public bool CheckPlayer()
    {
        RaycastHit2D hitRight = Physics2D.BoxCast(this.transform.position, new Vector2(1f, 1f), 0f, Vector2.right, 5f, 512);
        RaycastHit2D hitLeft = Physics2D.BoxCast(this.transform.position, new Vector2(1f, 1f), 0f, Vector2.left, 5f, 512);

        if (hitRight)
        {
            moveVec = Vector3.right;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (hitLeft)
        {
            moveVec = Vector3.left;
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if(hitRight || hitLeft)
            return true;
        else
            return false;
    }

    public void Rush()
    {
        isRush = true;
        animator.SetBool("isRush", true);
        animator.SetBool("isStartRush", false);
        StartCoroutine("CheckRush");
    }

    IEnumerator CheckRush()
    {
        yield return new WaitForSeconds(1.5f);

        animator.SetBool("isRush", false);
        isRush = false;
        isStartRush = false;
    }
}
