using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_1_Midboss_Skill0 : MonsterAttack
{
    public Animator animator;
    public SpriteRenderer attack_sprite;
    public BoxCollider2D attack_col;
    private float fadeSpeed_start = 5f;
    private float fadeSpeed_end = 5f;
    private float fadeHoldTime_start = 0.5f;
    private float fadeHoldTime_end = 1f;

    private void OnEnable()
    {
        animator.SetBool("isAttack", false);

        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        Color color = SaveScript.monsterColors[type];
        attack_sprite.color = new Color(color.r, color.g, color.b, 0f);
        attack_col.enabled = false;
        isSkillDamage = true;
        skillDamage = 0.075f;

        StartCoroutine(FadeIn_sprite());
    }

    IEnumerator FadeIn_sprite()
    {
        while (sprite.color.a < 1f)
        {
            Color color = sprite.color;
            color.a += Time.deltaTime * fadeSpeed_start;
            sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(fadeHoldTime_start);
        StartCoroutine(FadeOut_sprite());
    }

    IEnumerator FadeOut_sprite()
    {
        while (sprite.color.a > 0f)
        {
            Color color = sprite.color;
            color.a -= Time.deltaTime * fadeSpeed_start;
            sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Start_Attack();
    }

    public void Start_Attack()
    {
        animator.SetBool("isAttack", true);
        attack_sprite.color = new Color(attack_sprite.color.r, attack_sprite.color.g, attack_sprite.color.b);
        attack_col.enabled = true;
    }

    public void End_Attack()
    {
        isSkillDamage = false;
        StartCoroutine(FadeOut_Attack());
    }

    IEnumerator FadeOut_Attack()
    {
        yield return new WaitForSeconds(fadeHoldTime_end);
        attack_col.enabled = false;
        while (attack_sprite.color.a > 0f)
        {
            Color color = attack_sprite.color;
            color.a -= Time.deltaTime * fadeSpeed_end;
            attack_sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ObjectPool.ReturnObject<D_1_Midboss_Skill0>(29, this);
    }
}
