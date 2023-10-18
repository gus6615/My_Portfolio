using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    private new AudioSource audio;
    public SpriteRenderer sprite;
    public new CircleCollider2D collider;

    public int itemCode;
    public int itemType; // 0 = 버프 아이템, 1 = 강화 아이템, 2 = 영약
    public Vector2 initForceVec;
    public bool isCanTouch, isStartDelete;
    public float touchTime;

    private void OnEnable()
    {
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

        rigidbody.AddForce(initForceVec, ForceMode2D.Impulse);
        audio.mute = !SaveScript.saveData.isSEOn;
        Invoke("CanTouch", touchTime);
        StartCoroutine("AutoDelete");
    }

    private IEnumerator AutoDelete()
    {
        yield return new WaitForSeconds(15f);
        if (!SaveScript.saveData.isTutorial) 
            isStartDelete = true;
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
            ObjectPool.ReturnObject<ItemObject>(4, this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            audio.clip = SaveScript.SEs[8];
            audio.Play();

            switch (itemType)
            {
                case 0:
                    AchievementCtrl.instance.SetAchievementAmount(17, 1);
                    DropInfoUI.instance.SetItemInfo(SaveScript.bufItems[itemCode]);
                    SaveScript.saveData.hasBufItems[itemCode]++;
                    break;
                case 1:
                    AchievementCtrl.instance.SetAchievementAmount(18, 1);
                    DropInfoUI.instance.SetItemInfo(SaveScript.reinforceItems[itemCode]);
                    SaveScript.saveData.hasReinforceItems[itemCode]++;
                    break;
                case 2:
                    AchievementCtrl.instance.SetAchievementAmount(17, 1);
                    DropInfoUI.instance.SetItemInfo(SaveScript.elixirs[itemCode]);
                    SaveScript.saveData.hasElixirs[itemCode]++;
                    break;
                case 3:
                    AchievementCtrl.instance.SetAchievementAmount(18, 1);
                    DropInfoUI.instance.SetItemInfo(SaveScript.reinforceItems2[itemCode]);
                    SaveScript.saveData.hasReinforceItems2[itemCode]++;
                    if (itemCode > 4)
                    {
                        // 10000성 이상 제련석 채팅 알림
                        Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이 <" + SaveScript.reinforceItems2[itemCode].name + "> 을 얻었습니다!", 2);
                    }
                    break;
            }

            ObjectPool.ReturnObject<ItemObject>(4, this);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }
}
