using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthOre : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    private new AudioSource audio;
    public SpriteRenderer sprite;
    public new CircleCollider2D collider;

    public Vector2 initForceVec;
    public bool isCanTouch;
    public float touchTime;

    private static GrowthOre growthOre;

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

        // ¿⁄ºÆ æ∆¿Ã≈€
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

        sprite.gameObject.SetActive(true);
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
        rigidbody.AddForce(initForceVec, ForceMode2D.Impulse);
        audio.mute = !SaveScript.saveData.isSEOn;
        Invoke("CanTouch", touchTime);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            audio.clip = SaveScript.SEs[12];
            audio.Play();
            DropInfoUI.instance.SetGrowthOreInfo();
            SaveScript.saveData.growthOreNum += 1;
            PlayerScript.instance.growthOre += 1;
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 136, 137, 138 });
            ObjectPool.ReturnObject<GrowthOre>(33, this);
            Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'¥‘¿Ã 'º∫¿Â«œ¥¬ µπ'¿ª »πµÊ«œºÃΩ¿¥œ¥Ÿ!", 2);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }

    static public void CreateGrowthOreObject(Vector3 pos, Vector2 forceVec)
    {
        growthOre = ObjectPool.GetObject<GrowthOre>(33, ObjectPool.instance.objectTr, pos);
        growthOre.touchTime = 0.25f;
        growthOre.initForceVec = forceVec;
    }
}
