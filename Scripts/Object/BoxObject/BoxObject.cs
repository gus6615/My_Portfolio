using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObject : MonoBehaviour
{
    [SerializeField] private GameObject expInfo;
    [SerializeField] protected new AudioSource audio;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected BoxCollider2D detectCol;
    protected bool isDelete, isOpen;
    public int boxType;
    public int boxKind; // 박스 종류 (0 = 일반 상자, 1 = D_0_상자, 2 = D_1_상자)

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        audio.mute = !SaveScript.saveData.isSEOn;

        isDelete = false;
        isOpen = false;
        detectCol.enabled = true;
        switch (boxKind)
        {
            case 0: sprite.sprite = MapData.instance.mapBlockTiles[0].sprite; break;
            case 1: sprite.sprite = MapData.instance.dungeon_0_DecoX64Tiles[0].sprite; break;
            case 2: sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[0].sprite; break;
        }
        sprite.color = SaveScript.monsterColors[boxType];
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);

        if (!SaveScript.saveData.isTutorial) StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(60f);
        isDelete = true;
    }

    private void Update()
    {
        if(boxKind == 0)
            if ((isDelete && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 50f) || (isOpen && Vector3.Distance(this.transform.position, PlayerScript.instance.transform.position) > 15f))
                ObjectPool.ReturnObject<BoxObject>(2, this);
    }

    public virtual List<long> GetJems(float totalNum, out float out_totalNum)
    {
        out_totalNum = 0;
        return null;
    }

    public virtual void BoxOpen()
    {
        if (isOpen) return;

        detectCol.enabled = false;
        isOpen = true;

        // 업적
        AchievementCtrl.instance.SetAchievementAmount(8 + boxKind, 1);
        
        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 3 });

        // 콤보 시스템
        GameFuction.AddComboGauge(2);
    }
}