using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    private new AudioSource audio;
    public SpriteRenderer sprite;
    public new CircleCollider2D collider;

    public int jemIndex;
    public Vector2 initForceVec;
    public bool isCanTouch, isStartDelete;
    public float touchTime;

    private void OnEnable()
    {
        sprite.gameObject.SetActive(false);
        sprite.color = Color.white;
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

        audio = GetComponentInParent<AudioSource>();
        if (SaveScript.saveData.isCashEquipmentOn[0])
        {
            collider.enabled = false;
            rigidbody.gravityScale = 0f;
            touchTime = 0f;
        }
        else
        {
            collider.enabled = true;
            rigidbody.gravityScale = 1f;
        }

        isCanTouch = false;
        isStartDelete = false;
        sprite.gameObject.SetActive(true);
        sprite.sprite = SaveScript.collections[jemIndex].sprite;

        rigidbody.velocity = Vector2.zero;
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
        if (sprite.color.a > 0f)
        {
            Color color = sprite.color;
            color.a -= Time.deltaTime;
            sprite.color = color;
        }
        else
        {
            ObjectPool.ReturnObject<CardObject>(20, this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            audio.clip = SaveScript.SEs[4];
            audio.Play();
            SaveScript.saveData.collection_cards[jemIndex]++;
            if(SaveScript.saveData.collection_cards[jemIndex] >= GameFuction.GetGoalCardNum(jemIndex))
            {
                SaveScript.saveData.collection_cards[jemIndex] = 0;
                SaveScript.saveData.collection_levels[jemIndex]++;
                SaveScript.stat.SetStat();
            }

            // 퀘스트
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 11, 76, 77, 78 });
            ObjectPool.ReturnObject<CardObject>(20, this);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }

    static public void CreateCardObject(Vector3 pos, int jemIndex)
    {
        CardObject data = ObjectPool.GetObject<CardObject>(20, ObjectPool.instance.objectTr, pos);
        data.jemIndex = jemIndex;
        data.touchTime = 0f;
        data.initForceVec = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 1.5f));
    }
}
