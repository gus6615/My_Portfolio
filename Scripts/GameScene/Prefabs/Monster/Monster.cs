using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // 드랍 관련 데이터
    static public float[] bufItemForce_normal = { 0.01f, 0.015f, 0.02f, 0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f, 0.045f, 0.05f, 0.055f, 0.06f, 0.07f };
    static public float[] bufItemForce_ruin = { 0f, 0f, 0.05f, 0.1f, 0.05f, 0.1f, 0.025f, 0.05f, 0.075f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.4f };
    static public float[] bufItemForce_ancient = { 0f, 0f, 0f, 0f, 0f, 0f, 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f };
    static public int[] bufItemType_normal = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3 };
    static public int[] bufItemType_ruin = { 0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3 };

    static public long[] soilSlime_damages = { 50, 70, 90, 130, 180, 250, 400, 600, 1000, 4000, 30000, 100000, 500000, 4000000, 30000000 };
    static public long[] ADSlime_damages = { 50, 70, 90, 130, 180, 250, 400, 600, 1000, 4000, 30000, 100000, 500000, 4000000, 30000000 };
    static public long[] speedSlime_damages = { 0, 0, 200, 300, 400, 550, 800, 1500, 4500, 15000, 100000, 600000, 6000000, 17500000, 120000000 };
    static public long[] bigSlime_damages = { 0, 0, 150, 230, 320, 450, 600, 1200, 3500, 12000, 70000, 500000, 5000000, 15000000, 100000000 };
    static public long[] mushroomSlime_damages = { 0, 0, 130, 200, 280, 400, 520, 1100, 3200, 9000, 50000, 400000, 4000000, 12500000, 80000000 };
    static public long[] batSlime_damages = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    static public long[] golem_damages = { 500, 2000, 20000 };
    static public long[] manaGolem_damages = { 1000000, 3000000, 50000000 };
    static public long[] d_1_holeSlime_damages = { 0, 0, 0, 0, 0, 0, 1500, 2500, 6000, 30000, 150000, 1000000, 10000000, 30000000, 200000000 };
    static public long[] d_1_mimicSlime_damages = { 0, 0, 0, 0, 0, 0, 2000, 3200, 7500, 40000, 175000, 1200000, 12000000, 35000000, 200000000 };

    static public long[] soilSlime_hps = { 25, 40, 70, 110, 160, 220, 300, 400, 600, 2500, 20000, 70000, 350000, 5000000, 30000000 };
    static public long[] ADSlime_hps = { 35, 60, 100, 140, 200, 300, 450, 600, 900, 3000, 30000, 85000, 500000, 7000000, 40000000 };
    static public long[] speedSlime_hps = { 0, 0, 100, 150, 250, 450, 750, 1250, 2500, 7500, 50000, 250000, 2500000, 7500000, 50000000 };
    static public long[] bigSlime_hps = { 0, 0, 200, 300, 500, 900, 1500, 2500, 5000, 15000, 100000, 500000, 5000000, 15000000, 100000000 };
    static public long[] mushroomSlime_hps = { 0, 0, 180, 260, 440, 820, 1400, 2300, 4600, 12000, 75000, 400000, 4000000, 12500000, 80000000 };
    static public long[] batSlime_hps = { 0, 0, 0, 0, 0, 0, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
    static public long[] golem_hps = { 500, 5000, 100000 };
    static public long[] manaGolem_hps = { 3000000, 30000000, 300000000 };
    static public long[] d_1_holeSlime_hps = { 0, 0, 0, 0, 0, 0, 1500, 2500, 5000, 15000, 100000, 500000, 5000000, 15000000, 200000000 };
    static public long[] d_1_mimicSlime_hps = { 0, 0, 0, 0, 0, 0, 1250, 2000, 4000, 13000, 80000, 350000, 3500000, 10000000, 180000000 };

    static public int[] soilSlime_exps = { 5, 6, 8, 11, 15, 20, 27, 38, 50, 70, 92, 110, 120, 130, 140 };
    static public int[] ADSlime_exps = { 6, 7, 10, 13, 18, 23, 30, 42, 55, 80, 104, 120, 132, 144, 156 };
    static public int[] speedSlime_exps = { 0, 0, 16, 22, 30, 40, 54, 76, 110, 130, 150, 170, 185, 200, 215 };
    static public int[] bigSlime_exps = { 0, 0, 24, 32, 42, 55, 80, 100, 130, 150, 180, 200, 220, 240, 260 };
    static public int[] mushroomSlime_exps = { 0, 0, 27, 34, 48, 72, 90, 116, 132, 160, 190, 210, 230, 250, 270 };
    static public int[] batSlime_exps = { 0, 0, 0, 0, 0, 0, 70, 150, 300, 700, 1500, 3000, 6000, 10000, 15000 };
    static public int[] golem_exps = { 50, 147, 994 };
    static public int[] manaGolem_exps = { 150, 497, 4994 };
    static public int[] d_1_holeSlime_exps = { 0, 0, 0, 0, 0, 0, 100, 120, 150, 180, 210, 250, 300, 350, 400 };
    static public int[] d_1_mimicSlime_exps = { 0, 0, 0, 0, 0, 0, 100, 120, 150, 180, 210, 250, 300, 350, 400 };

    static public int[] all_manaOres = { 0, 0, 0, 0, 0, 0, 10, 20, 30, 50, 100, 200, 400, 1000, 3000 };
    static public int[] golem_reinforceOres = { 100, 2000, 300000 };
    static public int[] manaGolem_manaOres = { 80, 800, 80000 };
    static public int[] batSlime_manaOres = { 0, 0, 0, 0, 0, 0, 100, 300, 500, 1000, 3000, 10000, 20000, 50000, 300000 };

    public Animator animator;
    public new Rigidbody2D rigidbody;
    public new AudioSource audio;
    [SerializeField] protected SpriteRenderer[] sprites;
    [SerializeField] protected BoxCollider2D[] cols;
    public DamageSlide HPSlider;
    public Monster_Boss boss; // 보스 몹에 의해 만들어진다면 null이 아니다.

    public bool isDead, isActive, isDelete;
    protected bool isGotoRight; // 몬스터의 이동 방향이 오른쪽인가
    protected float turnDis; // 몬스터가 이동 방향을 변경하는 거리 (플레이어를 기준)
    public float height;
    public long HP, maxHP;
    public long damage;
    public float percentDamage;
    public int exp;
    public int type; // 깊이에 따른 강함
    public int kind; // 0 = 일반 몬스터, 1 = AD슬라임, 2 = 스피드 슬ekd라임, 3 = 거대 슬라임, 4 = 버섯 슬라임

    public void ParentInit()
    {
        isDead = isDelete = isActive = isGotoRight = false;
        rigidbody.velocity = Vector2.zero;
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = true;
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].transform.localScale = new Vector3(1f, 1f, 1f);
            sprites[i].color = new Color(1f, 1f, 1f, 0f);
        }
        HPSlider = null;
        turnDis = 0f;
        type = kind = 0;
        boss = null;
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public virtual void Dead()
    {
        rigidbody.velocity = Vector2.zero;
        isDead = true;
        animator.SetBool("isDead", true);
        StartCoroutine("FadeOut");
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = false;
        ObjectPool.ReturnObject<DamageSlide>(14, HPSlider);
        AchievementCtrl.instance.SetAchievementAmount(11, 1);
        if (boss != null)
            boss.cur_createMonsters--;

        audio.clip = SaveScript.SEs[7];
        audio.Play();

        // 퀘스트 (모든 몬스터 적용)
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 2 });
        QuestCtrl.instance.SetSubQuestAmount(1);

        // 콤보 시스템
        GameFuction.AddComboGauge(1);
    }

    public virtual void DropItem()
    {
        PrintUI.instance.ExpInfo(exp, true);
    }

    public IEnumerator FadeIn()
    {
        while (sprites[0].color.a < 1f)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                Color color = sprites[i].color;
                color.a += Time.deltaTime;
                sprites[i].color = color;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator FadeOut()
    {
        StopCoroutine("FadeIn");
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

        switch (kind)
        {
            case 0: 
                ObjectPool.ReturnObject<SoilSlime>(6, this as SoilSlime);
                DungeonCreater.instance.currentSoilSlimeNum--;
                break;
            case 1: ObjectPool.ReturnObject<ADSlime>(7, this as ADSlime); break;
            case 2: ObjectPool.ReturnObject<SpeedSlime>(9, this as SpeedSlime); break;
            case 4: ObjectPool.ReturnObject<MushroomSlime>(11, this as MushroomSlime); break;
            case 5: ObjectPool.ReturnObject<Golem>(18, this as Golem); break;
            case 6: ObjectPool.ReturnObject<BatSlime>(21, this as BatSlime); break;
        }
    }
}
