using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime_Attack : MonsterAttack
{
    public Animator animator;
    public SpriteRenderer attack_sprite;
    public BoxCollider2D attack_col;
    private float fadeSpeed = 5f;
    private float fadeHoldTime = 0.75f;
    
    private void OnEnable()
    {
        animator.SetBool("isAttack", false);

        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprite.color = new Color(1f, 1f, 1f, 0f);
        attack_sprite.color = SaveScript.monsterColors[type];

        StartCoroutine(FadeIn_sprite());
    }

    IEnumerator FadeIn_sprite()
    {
        while (sprite.color.a < 1f)
        {
            Color color = sprite.color;
            color.a += Time.deltaTime * fadeSpeed;
            sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(fadeHoldTime);
        StartCoroutine(FadeOut_sprite());
    }

    IEnumerator FadeOut_sprite()
    {
        while (sprite.color.a > 0f)
        {
            Color color = sprite.color;
            color.a -= Time.deltaTime * fadeSpeed;
            sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Start_Attack();
    }

    public void Start_Attack()
    {
        animator.SetBool("isAttack", true);
        attack_sprite.color = new Color(attack_sprite.color.r, attack_sprite.color.g, attack_sprite.color.b);
    }

    public void End_Attack()
    {
        StartCoroutine(FadeOut_Attack());
    }

    IEnumerator FadeOut_Attack()
    {
        yield return new WaitForSeconds(fadeHoldTime);
        while (attack_sprite.color.a > 0f)
        {
            Color color = attack_sprite.color;
            color.a -= Time.deltaTime * fadeSpeed;
            attack_sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ObjectPool.ReturnObject<KingSlime_Attack>(17, this);
    }
}
