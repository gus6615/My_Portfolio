using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Boss : MonoBehaviour
{
    static public float[] kingSlime_bufItemForces = { 0.3f, 0.7f, 0.3f, 0.7f, 0.3f, 0.7f, 0.5f, 0.75f, 1f, 1.25f, 2f, 3f, 4f, 5f, 7f };
    static public int[] kingSlime_bufItemTypes = { 0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

    static public long[] kingSlime_damages = { 70, 130, 200, 350, 500, 750, 1500, 2500, 6000, 20000, 150000, 1000000, 10000000, 30000000, 400000000 };
    static public long[] d_1_midboss_damages = { 0, 0, 0, 0, 0, 0, 2000, 3500, 10000, 50000, 300000, 1500000, 15000000, 50000000, 800000000 };
    static public long[] d_1_boss_damages = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    static public long[] kingSlime_hps = { 200, 500, 800, 1200, 2000, 3000, 6000, 12000, 25000, 70000, 500000, 2000000, 20000000, 50000000, 400000000 };
    static public long[] d_1_midboss_hps = { 0, 0, 0, 0, 0, 0, 5000, 10000, 40000, 200000, 2000000, 10000000, 30000000, 70000000, 800000000 };
    static public long[] d_1_boss_hps = { 0, 0, 0, 0, 0, 0, 10000, 20000, 80000, 400000, 4000000, 20000000, 60000000, 140000000, 1600000000 };

    static public int[] kingSlime_exps = { 30, 50, 70, 100, 150, 250, 400, 600, 900, 1300, 1800, 2500, 3000, 3500, 4000 };
    static public int[] d_1_midboss_exps = { 0, 0, 0, 0, 0, 0, 500, 700, 1000, 1500, 2000, 3000, 3500, 4000, 4500 };
    static public int[] d_1_boss_exps = { 0, 0, 0, 0, 0, 0, 500, 700, 1000, 1500, 2000, 3000, 3500, 4000, 4500 };

    static public int[] kingSlime_manaOres = { 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 110 };
    static public int[] d_1_midboss_manaOres = { 0, 0, 0, 0, 0, 0, 100, 130, 160, 200, 270, 400, 650, 900, 1200 };
    static public int[] d_1_boss_manaOres = { 0, 0, 0, 0, 0, 0, 100, 130, 160, 200, 270, 400, 650, 900, 1200 };

    static public int[] kingSlime_cashes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    static public int[] d_1_midboss_cashes = { 0, 0, 0, 0, 0, 0, 10, 12, 14, 16, 18, 20, 22, 25, 28 };
    static public int[] d_1_boss_cashes = { 0, 0, 0, 0, 0, 0, 10, 12, 14, 16, 18, 20, 22, 25, 28 };

    static public float[] itemPercentAsType = { 0f, 0f, 0f, 0f, 0f, 0f, 15f, 35f, 50f, 80f, 150f, 1000f, 5000f, 10000f, 20000f };
    static public float[] petPercentsAsType = { 5f, 8f, 12f, 20f, 50f, 80f, 100f, 200f, 400f, 800f, 1500f, 10000f, 15000f, 20000f, 30000f };
    static public float[][] petPercents =
    {
        new float[] { 0f, 0f, 0f, 0.6f, 0.35f, 0.05f, 0f, 0f, 0f, 0f }, // 7층 - A
        new float[] { 0f, 0f, 0f, 0f, 0.85f, 0.125f, 0.025f, 0f, 0f, 0f }, // 8층 - S
        new float[] { 0f, 0f, 0f, 0f, 0.7f, 0.25f, 0.05f, 0.0001f, 0f, 0f }, // 9층 - S ~ SS
        new float[] { 0f, 0f, 0f, 0f, 0.6f, 0.325f, 0.075f, 0.00025f, 0.0001f, 0f }, // 10층 - SS
        new float[] { 0f, 0f, 0f, 0f, 0.4f, 0.5f, 0.1f, 0.001f, 0.0001f, 0.00001f }, // 11층 - SS ~ SSS
        new float[] { 0f, 0f, 0f, 0f, 0.2f, 0.65f, 0.15f, 0.0015f, 0.00015f, 0.000015f  }, // 12층 - SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0.8f, 0.2f, 0.0025f, 0.00025f, 0.000025f }, // 13층 - SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0.3f, 0.7f, 0.005f, 0.0005f, 0.00005f }, // 14층 - SSS
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0.99f, 0.01f, 0.00075f, 0.000075f }, // 15층 - SSS
    };

    public Animator animator;
    public new AudioSource audio;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected BoxCollider2D[] cols;

    public bool isDead, isActive;
    public long HP, maxHP;
    public long damage;
    public int exp;
    public long manaOre, cash;
    public int type; // 깊이에 따른 강함
    public int kind; // 0 = 킹슬라임, 1 = D_1_Midboss, 2 = D_1_Boss
    public int max_createMonsters, cur_createMonsters;

    public void ParentInit()
    {
        isDead = isActive = false;
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = true;
        sprite.transform.localScale = new Vector3(1f, 1f, 1f);
        sprite.color = new Color(1f, 1f, 1f, 0f);
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public virtual void Dead()
    {
        isDead = true;
        animator.SetBool("isDead", true);

        StartCoroutine(FadeOut());
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = false;

        AchievementCtrl.instance.SetAchievementAmount(12, 1);
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 2 });
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 86, 87, 88 });
        QuestCtrl.instance.SetSubQuestAmount(1);

        // 콤보 시스템
        GameFuction.AddComboGauge(4);
    }

    public virtual void DropItem()
    {
        PrintUI.instance.ExpInfo(exp, true);
    }

    public IEnumerator FadeIn()
    {
        while (sprite.color.a < 1f)
        {
            Color color = sprite.color;
            color.a += Time.deltaTime;
            sprite.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator FadeOut()
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
