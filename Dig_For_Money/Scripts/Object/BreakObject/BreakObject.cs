using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    private static long[] ancientTresure_HPs = { 0, 0, 0, 0, 0, 0, 5000, 10000, 25000, 100000, 500000, 2500000, 7500000, 35000000, 400000000 }; // = D_1_Midboss_HP / 2
    private static int[] ancientTresure_Exps = { 0, 0, 0, 0, 0, 0, 400, 600, 900, 1300, 1800, 2500, 3000, 3500, 4000 }; // = KingSlime_exp

    [SerializeField] private GameObject expInfo;
    [SerializeField] protected new AudioSource audio;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected Animator animator;
    [SerializeField] protected BoxCollider2D detectCol;
    protected bool isOpen;
    public int boxType;
    public int boxKind; // 0 = AncientTresure
    public long HP, maxHP;
    public int exp;

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        audio.mute = !SaveScript.saveData.isSEOn;

        detectCol.enabled = true;
        isOpen = false;
        switch (boxKind)
        {
            case 0: 
                sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[2].sprite;
                sprite.color = Color.white;
                maxHP = HP = ancientTresure_HPs[boxType];
                exp = ancientTresure_Exps[boxType];
                break;
            case 1: // 미사용
                sprite.color = SaveScript.monsterColors[boxType];
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
                break;
        }

    }
    
    public virtual void DamageToObject(long _damage)
    {
        HP -= _damage;
        animator.SetBool("isAttacked", true);
        BossHPSlider.instance.SetHPSlider(this);
    }

    public virtual void Reward()
    {
        detectCol.enabled = false;
        isOpen = true;

        StartCoroutine(FadeOut());
        PrintUI.instance.ExpInfo(exp, true);
        BossHPSlider.instance.CloseHPSlider();

        // 콤보 시스템
        GameFuction.AddComboGauge(2);
    }

    public void StopAttacked()
    {
        animator.SetBool("isAttacked", false);
    }

    IEnumerator FadeOut()
    {
        while (sprite.color.a > 0f)
        {
            Color color = sprite.color;
            color.a -= Time.deltaTime;
            sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
