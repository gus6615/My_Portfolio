using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSlimeAttack : MonsterAttack
{
    private float moveSpeed;
    public bool isRight;
    public Vector3 moveVec;

    private void OnEnable()
    {
        StartCoroutine("Init");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        DetectMap();
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        Color color = SaveScript.monsterColors[type];
        sprite.color = new Color(color.r, color.g, color.b, 1f);
        damage = ADSlime_damages[type];

        if (isRight)
            moveVec = Vector3.right;
        else
            moveVec = Vector3.left;

        moveSpeed = 3f;
        kind = 0;
    }

    private void Move()
    {
        this.transform.position += moveVec * moveSpeed * Time.deltaTime;
    }

    private void DetectMap()
    {
        if(Physics2D.Raycast(this.transform.position, moveVec, sprite.bounds.extents.x * 1.5f, 256))
        {
            ObjectPool.ReturnObject<ADSlimeAttack>(8, this);
        }
    }
}
