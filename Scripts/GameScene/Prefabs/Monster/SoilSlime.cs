using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilSlime : Monster
{
    public float moveSpeed;
    public bool isJump;
    private float detectBlockDis;
    private Vector2 jumpVec;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Move", -1, 0f);
        animator.SetBool("isJump", false);
        animator.SetBool("isHighJump", false);
        animator.SetBool("isDead", false);

        StartCoroutine("Init");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!SaveScript.saveData.isTutorial && isDelete && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 50f)
        {
            ObjectPool.ReturnObject<SoilSlime>(6, this);
            DungeonCreater.instance.currentSoilSlimeNum--;
        }

        if (!isDead)
        {
            GotoPlayer();
            Jump();

            if (rigidbody.velocity.y < -7.5f)
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, -7.5f);
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprites[0].color = SaveScript.monsterColors[type];
        damage = soilSlime_damages[type];
        maxHP = soilSlime_hps[type];
        exp = soilSlime_exps[type];

        HP = maxHP;
        isJump = false;
        moveSpeed = 0.75f;
        turnDis = 0.5f;
        height = 0.5f;
        kind = 0;
        detectBlockDis = 0.5f;
        jumpVec = Vector2.up * 4f;
        StartCoroutine("Delete");
        StartCoroutine("FadeIn");

        if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
        {
            isGotoRight = false;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
        {
            isGotoRight = true;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public override void Dead()
    {
        base.Dead();
        DropItem();
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
        if (type >= 10) GameFuction.CreateElixirItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce_normal[type], count, out count);
        else GameFuction.CreateBufItem(this.gameObject.transform.position, ObjectPool.instance.objectTr, bufItemForce_normal[type], bufItemType_normal[type], count, out count);
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(60f);
        isDelete = true;
    }

    public void GotoPlayer()
    {
        if (isGotoRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
            {
                isGotoRight = false;
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
            {
                isGotoRight = true;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    public void Jump()
    {
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, detectBlockDis, 256);

        if (hitDown)
        {
            if (!isJump)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(transform.localScale.x), detectBlockDis, 256);
                RaycastHit2D higtHit = Physics2D.Raycast(transform.position + Vector3.up, Vector2.right * Mathf.Sign(transform.localScale.x), detectBlockDis, 256);

                if (hit)
                { 
                    if (higtHit)
                    {
                        animator.SetBool("isHighJump", true);

                    }
                    else
                    {
                        isJump = true;
                        rigidbody.AddForce(jumpVec, ForceMode2D.Impulse);
                        animator.SetBool("isJump", true);
                    }
                }
            }
            else
            {
                if (rigidbody.velocity.y == 0f)
                {
                    isJump = false;
                    animator.SetBool("isHighJump", false);
                    animator.SetBool("isJump", false);
                }
            }
        }
    }

    public void HighJump()
    {
        isJump = true;
        rigidbody.AddForce(jumpVec * 1.5f, ForceMode2D.Impulse);
    }
}
