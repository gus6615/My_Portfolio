using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSlime : Monster
{
    [SerializeField] private GameObject shotPoint;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Idle", -1, 0f);
        animator.SetBool("isAttack", false);
        animator.SetBool("isDead", false);

        StartCoroutine("Init");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!SaveScript.saveData.isTutorial && isDelete && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 50f)
        {
            ObjectPool.ReturnObject<ADSlime>(7, this);
        }

        if (!isDead)
        {
            if(CheckActive())
                AimPlayer();
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprites[0].color = SaveScript.monsterColors[type];
        damage = ADSlime_damages[type];
        maxHP = ADSlime_hps[type];
        exp = ADSlime_exps[type];

        HP = maxHP;
        turnDis = 0.5f;
        height = 0.75f;
        kind = 1;
        StartCoroutine("Delete");
        StartCoroutine("FadeIn");

        if (transform.position.x >= PlayerScript.instance.transform.position.x)
        {
            isGotoRight = false;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
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

    public void AimPlayer()
    {
        if (isGotoRight)
        {
            if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
            {
                isGotoRight = false;
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        else
        {
            if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
            {
                isGotoRight = true;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    private bool CheckActive()
    {
        if (Vector3.Distance(PlayerScript.instance.transform.position, this.transform.position) > 6f)
        {
            isActive = false;
            animator.SetBool("isAttack", false);
        }
        else
        {
            isActive = true;
            animator.SetBool("isAttack", true);
        }

        return isActive;
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(60f);
        isDelete = true;
    }

    public void CreateAttack()
    {
        ADSlimeAttack data = ObjectPool.GetObject<ADSlimeAttack>(8, ObjectPool.instance.objectTr, shotPoint.transform.position);
        data.type = type;

        if (isGotoRight)
            data.isRight = true;
        else
            data.isRight = false;
    }
}
