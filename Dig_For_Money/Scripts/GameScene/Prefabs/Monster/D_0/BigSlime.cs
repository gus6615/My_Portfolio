using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSlime : Monster
{
    private Vector3 moveVec;
    private float moveSpeed;
    private float jumpPower;
    private float changeIdleTime;
    private bool isIdle, isIdleChange;
    private bool isStartJump, isJump, isCheckLand, isJumpDown;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Idle", -1, 0f);
        animator.SetBool("isMove", false);
        animator.SetBool("isStartJump", false);
        animator.SetBool("isJump", false);
        animator.SetBool("isJumpEnd", false);
        animator.SetBool("isDead", false);

        StartCoroutine(Init());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            if (!isStartJump)
            {
                if (CheckPlayer())
                {
                    isStartJump= true;
                    animator.SetBool("isStartJump", true);
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
                if (isJump)
                {
                    rigidbody.velocity = new Vector2(0f, 0.1f);
                    rigidbody.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
                    isJump = false;
                    isCheckLand = true;
                }
                else
                {
                    if (rigidbody.velocity.y < 0f)
                        isJumpDown = true;

                    if(isJumpDown && isCheckLand)
                        CheckLanding();
                }
            }
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        audio = GetComponent<AudioSource>();
        damage = bigSlime_damages[type];
        maxHP = bigSlime_hps[type];
        exp = bigSlime_exps[type];

        HP = maxHP;
        sprites[0].color = SaveScript.monsterColors[type];
        isIdle = isIdleChange = false;
        isStartJump = isJump = isCheckLand = isJumpDown = false;
        kind = 3;
        height = 1f;
        moveSpeed = 0.75f;
        jumpPower = 8f;

        audio.mute = !SaveScript.saveData.isSEOn;
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

    public bool CheckPlayer()
    {
        RaycastHit2D hitRight = Physics2D.BoxCast(this.transform.position, new Vector2(1f, 1f), 0f, Vector2.right, 4f, 512);
        RaycastHit2D hitLeft = Physics2D.BoxCast(this.transform.position, new Vector2(1f, 1f), 0f, Vector2.left, 4f, 512);

        if (hitRight)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (hitLeft)
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (hitRight || hitLeft)
            return true;
        else
            return false;
    }

    public void Jump()
    {
        isJump = true;
        animator.SetBool("isJump", true);
        animator.SetBool("isStartJump", false);
    }

    public void CheckLanding()
    {
        if(rigidbody.velocity.y == 0f)
        {
            animator.SetBool("isJump", false);
            animator.SetBool("isJumpEnd", true);
            isJump = false;
            isCheckLand = false;
            isJumpDown = false;
            audio.clip = SaveScript.SEs[18];
            audio.Play();

            // 플레이어 감지
            if (Mathf.Abs(this.transform.position.y - PlayerScript.instance.transform.position.y) < 1f && PlayerScript.instance.rigidbody.velocity.y == 0)
            {
                PlayerScript.instance.DamageToPlayer((long)(damage * 1.5f), true);
            }
        }
    }

    public void CheckIdle()
    {
        animator.SetBool("isJumpEnd", false);
        isStartJump = false;
    }

    public void DeadToMiniSlime()
    {
        ObjectPool.ReturnObject<BigSlime>(10, this); ;
        for (int i = 0; i < 4; i++)
        {
            SoilSlime data;
            if(boss == null) data = ObjectPool.GetObject<SoilSlime>(6, ObjectPool.instance.dungeon_0_objectTr, this.transform.position);
            else data = ObjectPool.GetObject<SoilSlime>(6, ObjectPool.instance.objectTr, this.transform.position);
            data.type = type;
            data.rigidbody.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 3.5f)), ForceMode2D.Impulse);
        }
    }
}

