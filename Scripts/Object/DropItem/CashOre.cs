using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashOre : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    private new AudioSource audio;
    public SpriteRenderer[] sprites;
    public CircleCollider2D[] colliders;

    public Vector2 initForceVec;
    public bool isCanTouch, isStartDelete;
    public float touchTime;
    public int type; // 0 : 1개, 1 : 10개, 2 : 100개, 3 : 1000개

    private static CashOre cashOre;

    private void OnEnable()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].gameObject.SetActive(false);
            sprites[i].color = Color.white;
        }
        StartCoroutine("Init");
    }

    private void Update()
    {
        if (rigidbody.velocity.y < -10f)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -10f);

        if (isStartDelete)
            Delete();

        // 자석 아이템
        if (SaveScript.saveData.isCashEquipmentOn[0])
        {
            float speed = Mathf.Lerp(7f, 10f, Vector3.Distance(PlayerScript.instance.transform.position, this.transform.position) / 10f)
                + Mathf.Abs(PlayerScript.instance.moveData) * PlayerScript.instance.moveSpeed;
            this.transform.position += (PlayerScript.instance.transform.position - this.transform.position).normalized * Time.deltaTime * speed;
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        sprites[type].gameObject.SetActive(true);
        audio = GetComponentInParent<AudioSource>();
        if (SaveScript.saveData.isCashEquipmentOn[0])
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = false;
            rigidbody.gravityScale = 0f;
            touchTime = 0f;
        }
        else
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = true;
            rigidbody.gravityScale = 1f;
        }

        isCanTouch = false;
        isStartDelete = false;
        rigidbody.AddForce(initForceVec, ForceMode2D.Impulse);
        audio.mute = !SaveScript.saveData.isSEOn;
        Invoke("CanTouch", touchTime);
        StartCoroutine("AutoDelete");
    }

    private IEnumerator AutoDelete()
    {
        yield return new WaitForSeconds(15f);
        isStartDelete = true;
    }

    private void Delete()
    {
        if (sprites[type].color.a > 0f)
        {
            Color color = sprites[type].color;
            color.a -= Time.deltaTime;
            sprites[type].color = color;
        }
        else
        {
            ObjectPool.ReturnObject<CashOre>(31, this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            int num = (int)Mathf.Pow(10, type);
            audio.clip = SaveScript.SEs[12];
            audio.Play();
            SaveScript.saveData.cash += num;
            DropInfoUI.instance.SetCashInfo(type);
            AchievementCtrl.instance.SetAchievementAmount(24, num);
            ObjectPool.ReturnObject<CashOre>(31, this);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }

    static public void CreateCashObject(Vector3 pos, Vector2 forceVec, int num)
    {
        while (num > 0)
        {
            cashOre = ObjectPool.GetObject<CashOre>(31, ObjectPool.instance.dungeon_1_objectTr, pos);
            cashOre.touchTime = 0.25f;
            cashOre.initForceVec = forceVec;

            if (num >= 1000)
            {
                cashOre.type = 3;
                num -= 1000;
            }
            else if (num >= 100)
            {
                cashOre.type = 2;
                num -= 100;
            }
            else if (num >= 10)
            {
                cashOre.type = 1;
                num -= 10;
            }
            else
            {
                cashOre.type = 0;
                num--;
            }
        }
    }
}
