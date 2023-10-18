using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforceOre : MonoBehaviour
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

    private static ReinforceOre reinforceOre;

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
        if (!SaveScript.saveData.isTutorial) isStartDelete = true;
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
            ObjectPool.ReturnObject<ReinforceOre>(1, this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            int num = (int)Mathf.Pow(10, type);
            audio.clip = SaveScript.SEs[12];
            audio.Play();
            PlayerScript.instance.reinforceOre += num;
            SaveScript.saveData.hasReinforceOre += num;
            AchievementCtrl.instance.SetAchievementAmount(22, num);

            // 퀘스트
            QuestCtrl.instance.SetSubQuestAmount(4, num);

            ObjectPool.ReturnObject<ReinforceOre>(1, this);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }

    /// <summary>
    /// 강화석을 생성하는 함수
    /// </summary>
    /// <param name="pos">생성 위치</param>
    /// <param name="forceVec">받는 힘, zero일 경우 랜덤 벡터</param>
    /// <param name="num">생성 개수</param>
    static public void CreateReinforceObject(Vector3 pos, Vector2 forceVec, long num)
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

            reinforceOre = ObjectPool.GetObject<ReinforceOre>(1, ObjectPool.instance.objectTr, pos);
            reinforceOre.type = unit;
            reinforceOre.touchTime = 0.25f;
            if (forceVec != Vector2.zero)
                reinforceOre.initForceVec = forceVec;
            else
                reinforceOre.initForceVec = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 1.5f));
            num -= standard;
        }
    }
}
