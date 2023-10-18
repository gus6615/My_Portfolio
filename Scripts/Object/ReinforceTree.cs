using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforceTree : MonoBehaviour
{
    private static Sprite[] spriteData; // 0, 1, 2 = Mana | 3, 4, 5 = Reinforce
    private static long[] hps = { 500, 5000, 100000, 3000000, 30000000, 300000000 };
    private static int[] exps = { 30, 100, 800, 120, 400, 4000 };
    private static int[] reinforce_rewards = { 100, 2000, 300000 };
    private static int[] mana_rewards = { 80, 800, 80000 };
    private static float [] heights = { 0.75f, 1.25f, 2f };

    public Animator animator;
    public new AudioSource audio;
    public SpriteRenderer[] sprites;
    public DamageSlide HPSlider;
    public Collider2D[] cols;

    public int species;
    public bool isDead;
    public long HP, maxHP;
    public int exp;
    public int type; // 0 ~ 3: 나무의 종류
    public float height;
    public int reward; // 보상
    private int page; // 중간중간 아이템을 떨어뜨리는 횟수, 0이면 끝
    private int pageHP; // page가 발동되는 HP의 크기

    private void OnEnable()
    {
        StartCoroutine("Init");
        if (spriteData == null)
            spriteData = Resources.LoadAll<Sprite>("Images/Monster/ReinforceTrees");
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        isDead = false;
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = true;
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].gameObject.SetActive(false);
            sprites[i].sprite = spriteData[i + 3 * species];
            sprites[i].color = Color.white;
        }
        sprites[type].gameObject.SetActive(true);
        HPSlider = null;
        audio.mute = !SaveScript.saveData.isSEOn;

        maxHP = hps[type + 3 * species];
        exp = exps[type + 3 * species];
        reward = (species == 0) ? reinforce_rewards[type] : mana_rewards[type];
        height = heights[type];

        page = 5;
        pageHP = (int)(maxHP / page);
        HP = maxHP;
    }

    public void Dead()
    {
        long reinforceNum = 0, manaNum = 0;
        float totalNum = 0f;
        float count = 0f;

        // 강화석 데이터 생성
        if (species == 0)
        {
            reinforceNum = reward;
            reinforceNum = GameFuction.GetNumOreByRound(reinforceNum, totalNum, out totalNum);
        }
        else
        {
            manaNum = reward;
            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
                manaNum *= 2;
            manaNum = GameFuction.GetNumOreByRound(manaNum, totalNum, out totalNum);
        }

        count = -(totalNum / 2);
        GameFuction.SetDropForce(count, false);
        CheckHP();

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 51 });
        QuestCtrl.instance.SetSubQuestAmount(12);

        // 강화석 생성
        GameFuction.CreateDropReinforceOre(this.transform.position, reinforceNum, count, out count);

        // 마나석 드랍
        GameFuction.CreateDropManaOre(this.transform.position, manaNum, count, out count);

        PrintUI.instance.ExpInfo(exp, true);
        audio.clip = SaveScript.SEs[29];
        audio.Play();
        isDead = true;
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = false;

        ObjectPool.ReturnObject<DamageSlide>(14, HPSlider);
        StartCoroutine("FadeOut");
    }

    public void EndAttacked()
    {
        animator.SetBool("isAttacked", false);
    }

    public void CheckHP()
    {
        if(page != 0)
        {
            while (page > 0 && HP <= pageHP * (page - 1))
            {
                page--;
                if (species == 0)
                    ReinforceOre.CreateReinforceObject(this.transform.position, new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f)), reward / 10);
                else
                    ManaOre.CreateManaObject(this.transform.position, new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f)), reward / 10);
                audio.clip = SaveScript.SEs[29];
                audio.Play();
            }
        }
    }

    public IEnumerator FadeOut()
    {
        while (sprites[0].color.a > 0f)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                Color color = sprites[i].color;
                color.a -= Time.deltaTime;
                sprites[i].color = color;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ObjectPool.ReturnObject<ReinforceTree>(15, this);
    }
}
