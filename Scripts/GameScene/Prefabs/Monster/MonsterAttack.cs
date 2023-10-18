using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public static int[] ADSlime_damages = { 40, 55, 70, 100, 150, 200, 320, 500, 750, 3000, 20000, 80000, 400000, 4500000, 30000000 };

    [SerializeField] protected SpriteRenderer sprite;

    public long damage;
    public float skillDamage;
    public int type, kind;
    public bool isSkillDamage;
}
