using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Interaction : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static Interaction instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform mainCirle;
    [SerializeField] private RectTransform pointCircle;
    public new AudioSource audio;

    public Image mainCircleImage;
    public Image pointCircleImage;

    private Vector2 zoomVec;
    private Vector3Int savedDetectVec;
    private Color pointDetectColor;
    public float radius;
    private float breakTime, refeshBlockHPTime;
    private float detectDis;
    private bool isDetecting, isDetected, isStartDetect;
    private bool isBreakBlock, isRefeshBlockHP;
    public bool isCtrl;
    private bool isInfo;

    // Temp 변수
    RoomData roomData;
    EventBlock eventBlock;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        pointDetectColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        radius = mainCirle.rect.width * 0.35f;
        detectDis = 2.5f;
        refeshBlockHPTime = 0.1f;
        audio.mute = !SaveScript.saveData.isSEOn || !SaveScript.saveData.isBlockSoundOn;
        isCtrl = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerScript.instance.isEnd)
        {
            if (isDetecting)
                Detecting();

            if (isDetected && isStartDetect && !isBreakBlock)
                StartCoroutine(BreakBlock(savedDetectVec));
        }
    }

    public void OnDrag(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
        if (isCtrl && !PlayerScript.instance.isEnd)
        {
            zoomVec = (e.position - (Vector2)mainCirle.position) / canvas.transform.localScale.x;
            zoomVec = Vector2.ClampMagnitude(zoomVec, radius);
            pointCircle.localPosition = zoomVec;
            isDetecting = true;

            if (zoomVec.magnitude >= radius * 0.8f)
            {
                pointCircleImage.color = pointDetectColor;
                PlayerScript.instance.handsAnimator.SetBool("isBreakBlock", true);
                isStartDetect = true;
            }
            else
            {
                pointCircleImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                PlayerScript.instance.handsAnimator.SetBool("isBreakBlock", false);
                isStartDetect = false;
            }
        }
        else
        {
            zoomVec = Vector2.zero;
            pointCircle.localPosition = zoomVec;
            PlayerScript.instance.handsAnimator.SetBool("isBreakBlock", false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BossHPSlider.instance.CloseHPSlider();
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
        if (isCtrl) SetInit();
    }

    public void Detecting()
    {
        RaycastHit2D hit = Physics2D.Raycast(PlayerScript.instance.transform.position, zoomVec.normalized, detectDis, 256);

        if(hit)
        {
            bool standardX = false; // Hit이 x축을 기준으로 나타났는가?
            Vector3Int detectPos;
            isDetected = true;

            if (Mathf.Round((hit.point.x - 0.5f) * 10000f) / 10000f == Mathf.RoundToInt(hit.point.x - 0.5f))
                standardX = true;

            if (standardX)
            {
                if(hit.point.x < PlayerScript.instance.transform.position.x)
                    detectPos = new Vector3Int(Mathf.FloorToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), 0);
                else
                    detectPos = new Vector3Int(Mathf.CeilToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), 0);
            }
            else
            {
                if (hit.point.y < PlayerScript.instance.transform.position.y)
                    detectPos = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), 0);
                else
                    detectPos = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.CeilToInt(hit.point.y), 0);
            }
                

            if (savedDetectVec != null)
            {
                if (savedDetectVec == detectPos)
                    return;
                else
                {
                    MapData.instance.GetTileMap(savedDetectVec, 3).SetTile(savedDetectVec, null);
                    MapData.instance.GetTileMap(detectPos, 3).SetTile(detectPos, MapData.instance.blockDetectTile);
                    savedDetectVec = detectPos;
                }
            }
            else
            {
                savedDetectVec = detectPos;
                MapData.instance.GetTileMap(detectPos, 3).SetTile(detectPos, MapData.instance.blockDetectTile);
            }
        }
        else
        {
            MapData.instance.GetTileMap(savedDetectVec, 3).SetTile(savedDetectVec, null);
            savedDetectVec = Vector3Int.zero;
            isDetected = false;
        }
    }

    public void Auto_Dig(Vector2 digVec)
    {
        isDetecting = true;
        isStartDetect = true;
        zoomVec = Vector2.ClampMagnitude(digVec * radius, radius);
        pointCircle.localPosition = zoomVec;
        PlayerScript.instance.handsAnimator.SetBool("isBreakBlock", true);
    }

    IEnumerator InfoReset()
    {
        isInfo = true;
        yield return new WaitForSeconds(5f);
        isInfo = false;
    }

    IEnumerator RefreshBlockHP(Block block)
    {
        isRefeshBlockHP = true;
        BossHPSlider.instance.SetHPSlider(block);
        yield return new WaitForSeconds(refeshBlockHPTime);
        isRefeshBlockHP = false;
    }

    IEnumerator BreakBlock(Vector3Int pos)
    {
        Tile brokenBlock = MapData.instance.GetTileMap(pos, 2).GetTile(pos) as Tile;
        Tile block = MapData.instance.GetTileMap(pos, 0).GetTile(pos) as Tile;
        Tile jemTileData = MapData.instance.GetTileMap(pos, 1).GetTile(pos) as Tile; 

        int blockIndex = MapData.instance.blockTiles.IndexOf(block); // 0 = 부술수 없음, 1 ~ 5 : 벽
        int mapIndex = MapData.instance.mapBlockTiles.IndexOf(MapData.instance.GetTileMap(pos, 0).GetTile(pos) as Tile); // 일반 던전의 벽 인덱스, 0 ~ 1 : 상자, 2 ~ 6 : 벽
        int brokenBlockIndex = MapData.instance.brokenTiles.IndexOf(brokenBlock);
        int jemIndex = MapData.instance.jemTiles.IndexOf(jemTileData); // 보석 인덱스 (totalNum: 마나석, totalNum + 1: 성장하는 돌)
        int blockIndex_6th = MapData.instance.blockTiles_6th.IndexOf(block); // 0 : 6층 첫 시작, 1 : 5층 마지막
        int blockIndex_10th = MapData.instance.blockTiles_10th.IndexOf(block); // 0 : 11층 첫 시작, 1 : 10층 마지막
        int specialBlockIndex = MapData.instance.dungeon_1_specialBlockTiles.IndexOf(block);

        int count = 0; // brokenBlock의 부서짐을 나타냄
        int count_plus = 1;
        float plusTime = 1f - SaveScript.stat.pick02;

        audio.clip = SaveScript.SEs[13];
        isBreakBlock = true;

        // 일반 벽일 경우
        if (blockIndex > 0)
        {
            PlayerScript.instance.pickHP--;
            plusTime += SaveScript.resistences[blockIndex - 1];
            breakTime = SaveScript.picks[SaveScript.saveData.equipPick].breakTime * plusTime;
            if (breakTime < 0.03f)
            {
                count_plus++;
                breakTime = 0.05f;
            }

            if (brokenBlock != MapData.instance.brokenTiles[0])
            {
                if (blockIndex < SaveScript.saveData.equipPick + 2)
                {
                    if (jemIndex == -1 || jemIndex == SaveScript.totalItemNum || jemIndex == SaveScript.totalItemNum + 1 || SaveScript.jems[jemIndex].quality == 5 || SaveScript.jems[jemIndex].quality == 6 ||
                        SaveScript.jems[jemIndex].quality <= SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick] / SaveScript.reinforceNumAsQulity)
                    {
                        if (brokenBlock == null) count = count_plus;
                        else count = brokenBlockIndex + count_plus;

                        if (count >= MapData.instance.brokenTiles.Count)
                        {
                            // 블럭 완전히 파괴
                            audio.clip = SaveScript.SEs[14];
                            MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                            MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
                            DungeonCreater.instance.SetResponVecList();
                            AchievementCtrl.instance.SetAchievementAmount(0, 1);
                            PlayerScript.instance.pickHP -= SaveScript.digDamageAsFloor[blockIndex - 1];

                            // 튜토리얼 (땅파기)
                            if (SaveScript.saveData.isTutorial && Tutorial.instance.tutorialIndex == 7)
                                Tutorial.instance.value++;

                            // 퀘스트
                            QuestCtrl.instance.SetMainQuestAmount(new int[] { 0 });
                            QuestCtrl.instance.SetSubQuestAmount(3);

                            // 콤보 시스템
                            GameFuction.AddComboGauge(0);

                            // 보석이 존재하는 경우
                            if (jemTileData != null)
                            {
                                float plusPercent = SaveScript.stat.pendant02;
                                bool isFind = false;
                                if (jemIndex >= 0) isFind = true;

                                // 퀘스트
                                QuestCtrl.instance.SetSubQuestAmount(0);

                                if (jemIndex == SaveScript.totalItemNum + 1)
                                {
                                    // 성장하는 돌
                                    GrowthOre.CreateGrowthOreObject(pos, new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 1.5f)));
                                    MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                                    MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                                }
                                else if (jemIndex == SaveScript.totalItemNum)
                                {
                                    // 마나석
                                    int num = Mathf.RoundToInt(SaveScript.saveData.pickLevel + 1 + SaveScript.stat.manaOre);
                                    num *= (int)(1f + GameFuction.GetComboForce(2));
                                    if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 0)
                                        num *= 2;
                                    float temp = 0;
                                    GameFuction.CreateDropManaOre(pos, num, temp, out temp);
                                    MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                                    MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);

                                    // 콤보 시스템
                                    GameFuction.AddComboGauge(3);
                                }
                                else
                                {
                                    AchievementCtrl.instance.SetAchievementAmount(SaveScript.jems[jemIndex].quality + 1, 1);
                                    // 일반 광물
                                    PlayerScript.instance.pickHP -= SaveScript.OreDamageAsQuality[SaveScript.jems[jemIndex].quality];

                                    if (isFind)
                                    {
                                        long num = 1;
                                        // 목걸이 능력
                                        if (SaveScript.saveData.equipPendant != -1)
                                        {
                                            if (SaveScript.jems[jemIndex].quality == 6 || SaveScript.jems[jemIndex].quality <= GameFuction.GetQualityOfEquipment(SaveScript.saveData.pendantReinforces[SaveScript.saveData.equipPendant]))
                                            {
                                                if (GameFuction.GetRandFlag(plusPercent))
                                                    num = GameFuction.GetOreNum();
                                            }
                                            else
                                            {
                                                // 목걸이 등급 안됨
                                                if (!isInfo)
                                                {
                                                    PlayerScript.instance.SetInfoText("목걸이의 등급이 낮아 효과를 받지 못했습니다!", 1f, 2f);
                                                    StartCoroutine(InfoReset());
                                                }
                                            }
                                        }

                                        // 주말 이벤트
                                        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 4)
                                            num *= 2;

                                        // 광물 생성
                                        JemObject.CreateJemObject(pos, jemIndex, num);

                                        // 카드 생성
                                        if (!SaveScript.saveData.isTutorial && !PlayerScript.instance.isEventMap_On
                                            && GameFuction.GetRandFlag(SaveScript.cardDropPercents[SaveScript.jems[jemIndex].quality] * SaveScript.stat.collection))
                                            CardObject.CreateCardObject(pos, jemIndex);

                                        // 퀘스트
                                        switch (SaveScript.jems[jemIndex].quality)
                                        {
                                            case 0: QuestCtrl.instance.SetMainQuestAmount(new int[] { 1 }); break; // 노멀
                                            case 1: QuestCtrl.instance.SetMainQuestAmount(new int[] { 6 }); break; // 레어
                                            case 2: QuestCtrl.instance.SetMainQuestAmount(new int[] { 15 }); break; // 에픽
                                            case 4: QuestCtrl.instance.SetMainQuestAmount(new int[] { 27, 45, 59, 64, 72, 82, 92, 102, 112 }); break; // 레전드리
                                            case 5: 
                                                QuestCtrl.instance.SetMainQuestAmount(new int[] { 60, 65, 73, 83, 93, 103, 113, 122, 134 });
                                                QuestCtrl.instance.SetSubQuestAmount(18);
                                                break; // 얼티밋
                                            case 6: 
                                                QuestCtrl.instance.SetMainQuestAmount(new int[] { 96, 97, 98, 116, 117, 118, 123, 135 });
                                                QuestCtrl.instance.SetSubQuestAmount(19);
                                                break; // 미스틱
                                        }

                                        // 얼티밋 & 미라클 광물 EventBlock 삭제
                                        if (SaveScript.jems[jemIndex].quality == 5 || SaveScript.jems[jemIndex].quality == 6)
                                        {
                                            RaycastHit2D[] rays = Physics2D.RaycastAll(new Vector2(pos.x, pos.y), Vector2.right, 0.1f);
                                            for (int i = 0; i < rays.Length; i++)
                                            {
                                                eventBlock = rays[i].transform.GetComponent<EventBlock>();
                                                if (eventBlock != null && eventBlock.CheckIsOre())
                                                {
                                                    CashEquipmentCtrl.instance.eventBlocks.Remove(eventBlock);
                                                    Destroy(eventBlock.gameObject);
                                                }    
                                            }
                                        }

                                        MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                                        MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[count]);
                        }
                    }
                    else
                    {
                        // 곡괭이 등급이 안됨
                        if (!isInfo)
                        {
                            PlayerScript.instance.SetInfoText("곡괭이의 등급이 낮아 캘 수 없습니다!", 1f, 2f);
                            StartCoroutine(InfoReset());
                        }
                    }
                }
            }
        }
        
        // 던전 벽일 경우
        if(mapIndex > 1)
        {
            PlayerScript.instance.pickHP--;
            plusTime += SaveScript.resistences[mapIndex - 2];
            breakTime = SaveScript.picks[SaveScript.saveData.equipPick].breakTime * plusTime;
            if (breakTime < 0.03f)
            {
                count_plus++;
                breakTime = 0.05f;
            }

            if (brokenBlock != MapData.instance.brokenTiles[0])
            {
                if (mapIndex < SaveScript.saveData.equipPick + 3)
                {
                    if (brokenBlock == null) count = count_plus;
                    else count = brokenBlockIndex + count_plus;

                    if (count >= MapData.instance.brokenTiles.Count)
                    {
                        // 블럭 완전히 파괴
                        audio.clip = SaveScript.SEs[14];
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                        MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
                        DungeonCreater.instance.SetResponVecList();

                        // 퀘스트
                        QuestCtrl.instance.SetMainQuestAmount(new int[] { 0 });
                        QuestCtrl.instance.SetSubQuestAmount(3);

                        // 콤보 시스템
                        GameFuction.AddComboGauge(0);
                    }
                    else
                    {
                        MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[count]);
                    }
                }
            }
        }

        // 6층, 10층 전용 벽일 경우
        if(blockIndex_6th > -1 || blockIndex_10th > -1)
        {
            PlayerScript.instance.pickHP--;
            breakTime = SaveScript.picks[SaveScript.saveData.equipPick].breakTime * plusTime;
            if (breakTime < 0.03f)
            {
                count_plus++;
                breakTime = 0.05f;
            }

            if (brokenBlock != MapData.instance.brokenTiles[0])
            {
                if (brokenBlock == null) count = count_plus;
                else count = brokenBlockIndex + count_plus;

                if (count >= MapData.instance.brokenTiles.Count)
                {
                    // 블럭 완전히 파괴
                    audio.clip = SaveScript.SEs[14];
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                    MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
                    DungeonCreater.instance.SetResponVecList();

                    // 퀘스트
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { 0 });
                    QuestCtrl.instance.SetSubQuestAmount(3);

                    // 콤보 시스템
                    GameFuction.AddComboGauge(0);
                }
                else
                    MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[count]);
            }
        }

        // 특수 벽일 경우
        if (specialBlockIndex > -1)
        {
            Vector3Int block_pos = pos - DungeonCreater.instance.dungeon_1_room_startVec;
            long block_HP = 0;
            long block_maxHP = 0;
            roomData = DungeonCreater.dungeon_1_roomData;

            roomData.blocks[block_pos.x][-block_pos.y].HP -= Mathf.RoundToInt(SaveScript.stat.pick02 * 100);
            block_HP = roomData.blocks[block_pos.x][-block_pos.y].HP;
            block_maxHP = roomData.blocks[block_pos.x][-block_pos.y].maxHP;
            breakTime = SaveScript.picks[SaveScript.saveData.equipPick].breakTime * plusTime;
            PlayerScript.instance.pickHP -= SaveScript.digDamageAsFloor[specialBlockIndex];

            if (block_HP < 0)
            {
                // 블럭 파괴
                audio.clip = SaveScript.SEs[14];
                MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
                BossHPSlider.instance.CloseHPSlider();

                // 퀘스트
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 0 });
                QuestCtrl.instance.SetSubQuestAmount(3);

                // 콤보 시스템
                GameFuction.AddComboGauge(0);
            }
            else
            {
                int phaze = MapData.instance.brokenTiles.Count - (int)((float)block_HP / block_maxHP * 5) - 1;
                if (phaze == -1) phaze = 0;
                MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[phaze]);
                if (!isRefeshBlockHP)
                    StartCoroutine(RefreshBlockHP(roomData.blocks[block_pos.x][-block_pos.y]));
            }
        }

        if (AutoPlayCtrl.instance != null)
            AutoPlayCtrl.instance.breakTime = breakTime;
        PickStateUI.instance.ShowPickState();
        audio.Play();

        yield return new WaitForSeconds(breakTime);
        isBreakBlock = false;
    }

    public void SetInit()
    {
        zoomVec = Vector2.zero;
        pointCircle.localPosition = zoomVec;
        pointCircleImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        mainCircleImage.color = Color.white;
        isDetecting = false;
        isDetected = false;
        isStartDetect = false;
        MapData.instance.GetTileMap(savedDetectVec, 3).SetTile(savedDetectVec, null);
        savedDetectVec = Vector3Int.zero;
        PlayerScript.instance.handsAnimator.SetBool("isBreakBlock", false);
    }

    public void SetButtonEnable(bool isEnable)
    {
        isCtrl = isEnable;
        mainCircleImage.color = (isEnable) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
        pointCircleImage.color = (isEnable) ? new Color(0.8f, 0.8f, 0.8f, 1f) : new Color(1f, 1f, 1f, 0f);
        mainCircleImage.raycastTarget = pointCircleImage.raycastTarget = isEnable;
        if (isEnable) 
            SetInit();
        else
        {
            isDetecting = isDetected = isStartDetect = false;
            MapData.instance.GetTileMap(savedDetectVec, 3).SetTile(savedDetectVec, null);
        }
    }
}
