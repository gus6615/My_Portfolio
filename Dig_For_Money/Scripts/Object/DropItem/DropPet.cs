using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPet : MonoBehaviour
{
    public static Sprite[] miner_dropSprites, adventurer_dropSprites;
    public static float[] dropPercents = new float[] { 0.001f, 0.0001f, 0.00001f, 0.000001f, 0f, 0f, 0f };
    public static float TypePercent = 0.4f; // 모험가 펫이 뜰 확률
    public static bool isInit;

    [SerializeField] private new Rigidbody2D rigidbody;
    private new AudioSource audio;
    public SpriteRenderer sprite;
    public new BoxCollider2D collider;
    public GameObject detectCol;

    public Vector2 initForceVec;
    public int type, code;
    public bool isCanTouch, isStartDelete;
    public float touchTime;

    private void OnEnable()
    {
        StartCoroutine("Init");
        if (!isInit)
        {
            isInit = true;
            miner_dropSprites = Resources.LoadAll<Sprite>("Images/Item/MinerImage");
            adventurer_dropSprites = Resources.LoadAll<Sprite>("Images/Item/AdventurerImage");
        }
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
        collider.enabled = true;
        rigidbody.gravityScale = 1f;

        isCanTouch = false;
        isStartDelete = false;
        sprite.color = Color.white;
        switch (type)
        {
            case 0: sprite.sprite = miner_dropSprites[code]; break;
            case 1: sprite.sprite = adventurer_dropSprites[code]; break;
        }
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
        if (sprite.color.a > 0f)
        {
            Color color = sprite.color;
            color.a -= Time.deltaTime;
            sprite.color = color;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isCanTouch && col.gameObject.tag == "Player")
        {
            int index;
            AchievementCtrl.instance.SetAchievementAmount(19, 1);
            audio.clip = SaveScript.SEs[12];
            audio.Play();

            switch (type)
            {
                case 0:
                    index = MinerSlime.FindEmptyPetInven();
                    if (index == -1)
                    {
                        PetInven.instance.ShowPetInven(type, code);
                    }
                    else
                    {
                        SaveScript.saveData.hasMiners[index] = code;
                        SaveScript.saveData.hasMinerLevels[index] = 1;
                        SaveScript.saveData.hasMinerExps[index] = 0;
                        MinerSlime.SortPetInven();
                        DropInfoUI.instance.SetPetInfo(type, code);

                        QuestCtrl.instance.SetMainQuestAmount(new int[] { 32 });
                        switch (code)
                        {
                            case 7: QuestCtrl.instance.SetMainQuestAmount(new int[] { 106 }); break;
                            case 8: QuestCtrl.instance.SetMainQuestAmount(new int[] { 107 }); break;
                            case 9: QuestCtrl.instance.SetMainQuestAmount(new int[] { 108 }); break;
                        }
                    }
                    break;
                case 1:
                    index = AdventurerSlime.FindEmptyPetInven();
                    if (index == -1)
                    {
                        PetInven.instance.ShowPetInven(type, code);
                    }
                    else
                    {
                        SaveScript.saveData.hasAdventurers[index] = code;
                        SaveScript.saveData.hasAdventurerLevels[index] = 1;
                        SaveScript.saveData.hasAdventurerExps[index] = 0;
                        AdventurerSlime.SortPetInven();
                        DropInfoUI.instance.SetPetInfo(type, code);

                        QuestCtrl.instance.SetMainQuestAmount(new int[] { 32 });
                        switch (code)
                        {
                            case 7: QuestCtrl.instance.SetMainQuestAmount(new int[] { 106 }); break;
                            case 8: QuestCtrl.instance.SetMainQuestAmount(new int[] { 107 }); break;
                            case 9: QuestCtrl.instance.SetMainQuestAmount(new int[] { 108 }); break;
                        }
                    }
                    break;
            }

            Destroy(this.gameObject);
        }
    }

    private void CanTouch()
    {
        isCanTouch = true;
    }
}
