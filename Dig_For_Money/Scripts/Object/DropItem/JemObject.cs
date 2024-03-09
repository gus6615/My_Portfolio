using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JemObject : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private CircleCollider2D[] colliders;
    private new AudioSource audio;
    public SpriteRenderer[] sprites;

    public int jemIndex, jemType;
    public Vector2 initForceVec;
    public bool isCanTouch, isStartDelete;
    public float touchTime;

    private static JemObject jemObject;
    private static Sprite sprite;

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

        audio = GetComponentInParent<AudioSource>();

        isCanTouch = false;
        isStartDelete = false;
        sprites[jemType].gameObject.SetActive(true);
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

        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(initForceVec, ForceMode2D.Impulse);
        audio.mute = !SaveScript.saveData.isSEOn;
        Invoke("CanTouch", touchTime);
        StartCoroutine("AutoDelete");
    }

    private IEnumerator AutoDelete()
    {
        yield return new WaitForSeconds(15f);
        if(!SaveScript.saveData.isTutorial) isStartDelete = true;
    }

    private void Delete()
    {
        if(sprites[jemType].color.a > 0f)
        {
            Color color = sprites[jemType].color;
            color.a -= Time.deltaTime;
            sprites[jemType].color = color;
        }
        else
        {
            ObjectPool.ReturnObject<JemObject>(0, this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            long num = (long)Mathf.Pow(10, jemType);
            audio.clip = SaveScript.SEs[8];
            audio.Play();
            DropInfoUI.instance.SetJemInfo(SaveScript.jems[jemIndex], jemType);
            PlayerScript.instance.jems[SaveScript.jems[jemIndex].itemCode] += num;
            SaveScript.saveData.hasItemNums[SaveScript.jems[jemIndex].itemCode] += num;
            // 튜토리얼 (광물 획득)
            if (SaveScript.saveData.isTutorial)
            {
                if (Tutorial.instance.tutorialIndex == 8 || (Tutorial.instance.tutorialIndex == 11 && SaveScript.jems[jemIndex].quality > 0))
                    Tutorial.instance.value++;
            }

            ObjectPool.ReturnObject<JemObject>(0, this);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }

    static public void CreateJemObject(Vector3 pos, int jemIndex, long num)
    {
        if (num.ToString().Length > 2) 
            num -= num % (long)Mathf.Pow(10, num.ToString().Length - 2);
        if (num < 0) 
            num = 1;

        int unit = Jem.unitNum - 1;
        long standard = (long)Mathf.Pow(10, unit);
        while (num > 0)
        {
            while (num < standard)
            {
                unit--;
                standard = (long)Mathf.Pow(10, unit);
            }

            switch (unit)
            {
                case 0: sprite = SaveScript.jems[jemIndex].jemSprite; break;
                case 1: sprite = SaveScript.jems[jemIndex].jem_unitSprite; break;
                case 2: sprite = SaveScript.jems[jemIndex].jem_unit2Sprite; break;
                case 3: sprite = SaveScript.jems[jemIndex].jem_unit3Sprite; break;
                case 4: sprite = SaveScript.jems[jemIndex].jem_unit4Sprite; break;
                case 5: sprite = SaveScript.jems[jemIndex].jem_unit5Sprite; break;
                case 6: sprite = SaveScript.jems[jemIndex].jem_unit6Sprite; break;
                case 7: sprite = SaveScript.jems[jemIndex].jem_unit7Sprite; break;
                case 8: sprite = SaveScript.jems[jemIndex].jem_unit8Sprite; break;
                case 9: sprite = SaveScript.jems[jemIndex].jem_unit9Sprite; break;
            }

            jemObject = ObjectPool.GetObject<JemObject>(0, ObjectPool.instance.objectTr, pos);
            jemObject.sprites[unit].sprite = sprite;
            jemObject.jemIndex = jemIndex;
            jemObject.jemType = unit;
            jemObject.touchTime = 0f;
            jemObject.initForceVec = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 1.5f));
            num -= standard;
        }
    }
}
