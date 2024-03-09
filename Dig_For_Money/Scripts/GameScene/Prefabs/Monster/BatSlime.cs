using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSlime : Monster
{
    public float moveSpeed;
    public bool isJump;

    private void OnEnable()
    {
        ParentInit();
        animator.Play("Idle", -1, 0f);
        animator.SetBool("isDead", false);

        StartCoroutine("Init");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDelete)
        {
            StartCoroutine("FadeOut");
        }

        if (!isDead)
        {
            GotoPlayer();
            if (!isJump)
                StartCoroutine("Jump");

            if (rigidbody.velocity.y < -5f)
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, -5f);
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        type = SaveScript.saveData.pickLevel;
        sprites[0].color = SaveScript.monsterColors[type];
        damage = batSlime_damages[type];
        maxHP = batSlime_hps[type];
        exp = batSlime_exps[type];

        HP = maxHP;
        isJump = false;
        moveSpeed = 1.5f;
        turnDis = 1.5f;
        height = 0.5f;
        kind = 6;
        StartCoroutine("Delete");
        StartCoroutine("FadeIn");

        if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
        {
            isGotoRight = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
        {
            isGotoRight = true;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public override void Dead()
    {
        base.Dead();
        DropItem();

        QuestCtrl.instance.SetMainQuestAmount(new int[] { 52 });
        QuestCtrl.instance.SetSubQuestAmount(14);
    }

    public override void DropItem()
    {
        base.DropItem();
        int manaOre = batSlime_manaOres[type];
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
            manaOre *= 2;

        // 마나석 바로 드랍
        PlayerScript.instance.manaOre += manaOre;
        SaveScript.saveData.manaOre += manaOre;
        DropInfoUI.instance.SetManaInfo(manaOre);
        // 업적
        AchievementCtrl.instance.SetAchievementAmount(23, manaOre);
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(30f);
        isDelete = true;
    }

    public void GotoPlayer()
    {
        float gap_x = Mathf.Abs(PlayerScript.instance.transform.position.x - transform.position.x);
        if (gap_x < 2f) gap_x = 2f;

        if (isGotoRight)
        {
            transform.position += Vector3.right * gap_x * moveSpeed * Time.deltaTime;

            if (transform.position.x >= PlayerScript.instance.transform.position.x + turnDis)
            {
                isGotoRight = false;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            transform.position += Vector3.left * gap_x * moveSpeed * Time.deltaTime;

            if (transform.position.x + turnDis <= PlayerScript.instance.transform.position.x)
            {
                isGotoRight = true;
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    IEnumerator Jump()
    {
        isJump = true;
        if (PlayerScript.instance.transform.position.y > this.transform.position.y)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(Vector2.up * Random.Range(3f, 4f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);

        isJump = false;
    }
}
