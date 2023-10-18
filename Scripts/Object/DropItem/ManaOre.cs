using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaOre : MonoBehaviour
{
    public static int unitNum = 6;

    [SerializeField] private new Rigidbody2D rigidbody;
    private new AudioSource audio;
    public SpriteRenderer[] sprites;
    public CircleCollider2D[] colliders;

    public Vector2 initForceVec;
    public bool isCanTouch, isStartDelete;
    public float touchTime;
    public int type; // 0 : 1개, 1 : 10개, 2 : 100개, 3 : 1k개, 4 : 10k개, 5 : 100k개

    private static ManaOre manaOre;

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
            ObjectPool.ReturnObject<ManaOre>(19, this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            int num = (int)Mathf.Pow(10, type);
            audio.clip = SaveScript.SEs[12];
            audio.Play();
            PlayerScript.instance.manaOre += num;
            SaveScript.saveData.manaOre += num;
            AchievementCtrl.instance.SetAchievementAmount(23, num);
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 46, 66, 68, 74, 84, 94, 104, 114, 124 }, SaveScript.saveData.mainQuest_goal + num);
            QuestCtrl.instance.SetSubQuestAmount(6, num);
            ObjectPool.ReturnObject<ManaOre>(19, this);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }

    static public void CreateManaObject(Vector3 pos, Vector2 forceVec, long num)
    {
        int unit = unitNum - 1;
        long standard = (long)Mathf.Pow(10, unit);

        while (num > 0)
        {
            while (num < standard)
            {
                unit--;
                standard = (long)Mathf.Pow(10, unit);
            }

            manaOre = ObjectPool.GetObject<ManaOre>(19, ObjectPool.instance.objectTr, pos);
            manaOre.type = unit;
            manaOre.touchTime = 0.25f;
            manaOre.initForceVec = forceVec;
            num -= standard;
        }
    }
}
