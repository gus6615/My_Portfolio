using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class EventMap : MonoBehaviour
{
    public static EventMap instance;
    public readonly Color timerColor = new Color(1f, 1f, 1f, 0.8f);
    public readonly Color countColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
    private const int deleteTime = 30;
    private const int createTime = 6;
    private const int maxCreateTime = 60 * 8;
    private const int bodyLength = 50;
    private const float createPercent = 0.0125f;
    private const float mysticPercent = 0.0075f;
    private const float ultimatePercent = 0.05f;
    private const float manaOrePercent = 0.125f;

    public GameObject portal_prefab;
    public Image timerInfo, countInfo;
    private Text countInfoText;
    private Text[] timerInfoTexts;

    private bool isCreate; // 현재 이벤트맵이 생성되었는가?
    private bool isCheckCreate; // 주기적으로 이벤트 상황 체크
    private bool isCheckMustCreate;
    private bool isStartCount; // 게임 시작 3초 카운트
    private bool isCountDown;
    private bool isDone;
    private int startCount;
    private float playTime;
    private float currentTime;

    public Vector3Int bodyStartVec;
    public bool isDungeon_0;

    List<Vector3Int> positions = new List<Vector3Int>();
    GameObject portal;
    EventBlock eventBlock;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        countInfoText = countInfo.GetComponentInChildren<Text>();
        timerInfoTexts = timerInfo.GetComponentsInChildren<Text>();

        bodyStartVec = new Vector3Int(-400, 100, 0);
        playTime = 20f + SaveScript.stat.eventMap;
        countInfo.color = timerInfo.color = new Color(0f, 0f, 0f, 0f);
        countInfoText.color = new Color(0f, 0f, 0f, 0f);
        for (int i = 0; i < timerInfoTexts.Length; i++)
            timerInfoTexts[i].color = new Color(0f, 0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.saveData.pickLevel < 2) return;

        if (!isCreate)
        {
            if (!isCheckCreate)
                StartCoroutine("CheckCreate");
            if (!isCheckMustCreate)
                StartCoroutine("CheckMustCreate");
        }
        else
        {
            if (isStartCount)
            {
                if (isCountDown)
                    StartCoroutine("CountDown");
            }
            else
            {
                if (PlayerScript.instance.isEventMap_On)
                {
                    currentTime -= Time.deltaTime;
                    if (currentTime > 5f)
                        SetTimerText(string.Format("{0:N2}", currentTime), Color.white);
                    else if (currentTime > 0f)
                        SetTimerText(string.Format("{0:N2}", currentTime), new Color(1f, 0.4f, 0.4f, 1f));
                    else
                    {
                        if (!isDone)
                        {
                            isDone = true;
                            SetTimerText("0:00", new Color(1f, 0.4f, 0.4f, 1f), 0f, 1f);
                        }
                    }
                }
            }
        }
    }

    IEnumerator StartPlay()
    {
        yield return new WaitForSeconds(playTime);

        PlayerScript.instance.isCanCtrl = false;
        PlayerScript.instance.SetButtonMode(false, false, false, false);
        SetCountInfo("종료", Color.white, 0f, 1f);

        yield return new WaitForSeconds(1f);

        EventButton.mainType = 8;
        isCreate = isCheckCreate = isCheckMustCreate = isStartCount = isCountDown = isDone = false;
        StartCoroutine(BlindScript.instance.switchPos(PlayerScript.instance.eventMap_startPos, BlindScript.instance.GetStageBGM(), 1.5f, 1.5f));
    }

    IEnumerator CountDown()
    {
        isCountDown = false;
        if (startCount == 0)
        {
            SetCountInfo("시작", Color.white, 0f, 1f);
            PlayerScript.instance.isCanCtrl = true;
            PlayerScript.instance.SetButtonMode(false, true, false, true);
        }
        else if (startCount < 0)
        {
            StartCoroutine("StartPlay");
            isStartCount = false;
            currentTime = playTime;
        }
        else
            SetCountInfo(startCount.ToString(), Color.white, 0f, 1f);
        startCount--;

        yield return new WaitForSeconds(1f);
        isCountDown = true;
    }

    public void StartCountDown()
    {
        isStartCount = true;
        isCountDown = true;
        startCount = 4;
    }

    private Vector3 GetPortalPos()
    {
        Vector3 pos;

        positions.Clear();
        for (int i = 0; i < DungeonCreater.instance.responVecList.Count; i++)
            if (Vector3.Distance(PlayerScript.instance.transform.position, DungeonCreater.instance.responVecList[i]) < 3f)
                positions.Add(DungeonCreater.instance.responVecList[i]);
        if (positions.Count > 0)
            pos = positions[Random.Range(0, positions.Count)];
        else
            pos = PlayerScript.instance.transform.position;

        return pos;
    }

    public void CreateMap()
    {
        Vector3Int pos;

        for (int i = -bodyLength; i < bodyLength; i++)
        {
            for (int j = bodyLength; j > -bodyLength; j--)
            {
                pos = bodyStartVec + new Vector3Int(i, j, 0);

                if (j == bodyLength || j == -bodyLength + 1)
                {
                    // 맵 맨 위 & 맨 아래
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                }
                else
                {
                    if (i == -bodyLength || i == bodyLength - 1)
                    {
                        // 맵 양끝
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                    }
                    else
                    {
                        // 내부
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[SaveScript.saveData.pickLevel + 1]);
                        MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.backgroundBlockTiles[SaveScript.saveData.pickLevel]);
                        if (Mathf.Abs(pos.x) % 4 == 0 && Mathf.Abs(pos.y) % 4 == 0)
                            CreateJem(pos);
                    }
                }
            }
        }

        // 빈 공간 생성
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                pos = bodyStartVec + new Vector3Int(i, j, 0);
                MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
            }
        }
    }

    public void CreateJem(Vector3Int pos)
    {
        int createJemIndex;
        float createMystic = mysticPercent;
        float createUltimate = ultimatePercent;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 3)
        {
            createMystic *= 2;
            createUltimate *= 2;
        }

        // 광물 설정 (이전 층 ~ 현재 층만, 얼티밋 제외)
        if (GameFuction.GetPlayerGrade() > 1 && GameFuction.GetRandFlag(createMystic))
            createJemIndex = Jem.GetMysticCode(SaveScript.saveData.pickLevel);
        else if (GameFuction.GetRandFlag(createUltimate))
            createJemIndex = Jem.GetUItimateCode(SaveScript.saveData.pickLevel);
        else if (GameFuction.GetRandFlag(manaOrePercent))
            createJemIndex = -1;
        else
            return; // 생성 X

        // 광물 생성
        Vector3Int randVec = Vector3Int.zero;
        Vector3Int realPos;
        int num = Random.Range(SaveScript.minCreateJemNuM, SaveScript.maxCreateJemNum + 1);

        for (int i = 0; i < num; i++)
        {
            realPos = pos + randVec;
            if (Mathf.Abs(bodyStartVec.x - realPos.x) >= bodyLength - 1 || Mathf.Abs(bodyStartVec.y - realPos.y) >= bodyLength - 1)
                return;

            if (MapData.instance.GetTileMap(realPos, 1).GetTile(realPos) == null)
            {
                if (createJemIndex == -1)
                {
                    // 마나석 광물
                    MapData.instance.GetTileMap(realPos, 1).SetTile(realPos, MapData.instance.mana_oreTile);
                    MapData.instance.GetTileMap(realPos, 10).SetTile(realPos, MapData.instance.jemLightTiles[MapData.instance.jemLightTiles.Count - 3]);
                }
                else
                {
                    // 미스틱 & 얼티밋 광물
                    MapData.instance.GetTileMap(realPos, 1).SetTile(realPos, MapData.instance.jemTiles[createJemIndex]);
                    MapData.instance.GetTileMap(realPos, 10).SetTile(realPos, MapData.instance.jemLightTiles[SaveScript.jems[createJemIndex].quality - 1]);
                }
            }

            switch (Random.Range(0, 4))
            {
                case 0: randVec += Vector3Int.right; break;
                case 1: randVec += Vector3Int.left; break;
                case 2: randVec += Vector3Int.up; break;
                case 3: randVec += Vector3Int.down; break;
            }
        }
    }

    public void DeleteMap()
    {
        Vector3Int pos;
        StopCoroutine("CheckDelete");
        Destroy(portal.gameObject);

        for (int i = -bodyLength; i < bodyLength; i++)
        {
            for (int j = bodyLength; j > -bodyLength; j--)
            {
                pos = bodyStartVec + new Vector3Int(i, j, 0);

                MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 2).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 4).SetTile(pos, null);
                MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
            }
        }
    }

    public void CreatePortal()
    {
        if (PlayerScript.instance.isDungeon_0_On || PlayerScript.instance.isDungeon_1_On)
        {
            Debug.Log("이벤트 포탈 생성 안됨");
            return;
        }

        // 이벤트 포탈 & 맵 생성
        isCreate = true;
        StopCoroutine("CheckCreate");
        StopCoroutine("CheckMustCreate");
        StartCoroutine("CheckDelete");
        portal = Instantiate(portal_prefab, GetPortalPos(), Quaternion.identity, ObjectPool.instance.objectTr);
        eventBlock = portal.GetComponentInChildren<EventBlock>();
        eventBlock.eventMainType = 7;
        portal.GetComponentInChildren<SpriteRenderer>().color = SaveScript.stageColors[SaveScript.saveData.pickLevel];
        PrintUI.instance.AudioPlay(35);
    }

    IEnumerator CheckCreate()
    {
        isCheckCreate = true;
        yield return new WaitForSeconds(createTime);
        isCheckCreate = false;
        if (GameFuction.GetRandFlag(createPercent))
        {
            Debug.Log("이벤트 포탈 생성 - 일반");
            CreatePortal();
        }
    }

    IEnumerator CheckMustCreate()
    {
        isCheckMustCreate = true;
        yield return new WaitForSeconds(maxCreateTime);
        isCheckMustCreate = false;

        while (PlayerScript.instance.isDungeon_0_On || PlayerScript.instance.isDungeon_1_On)
            yield return null;
        yield return new WaitForSeconds(Random.Range(0f, 60f));
        CreatePortal();
        Debug.Log("이벤트 포탈 생성 - 특수");
    }

    IEnumerator CheckDelete()
    {
        yield return new WaitForSeconds(deleteTime);
        Destroy(portal.gameObject);
        isCreate = false;
    }

    public void StopDelete()
    {
        StopCoroutine("CheckDelete");
    }

    public void SetTimerText(string text, Color color, float fadeStart, float fadeTime)
    {
        SetTimerText(text, color);
        timerInfo.GetComponent<FadeUI>().SetFadeValues(0f, fadeStart, fadeTime);
    }

    public void SetTimerText(string text, Color color)
    {
        timerInfoTexts[0].text = text;
        for (int i = 0; i < timerInfoTexts.Length; i++)
            timerInfoTexts[i].color = color;
    }

    public void SetCountInfo(string text, Color color, float fadeStart, float fadeTime)
    {
        countInfoText.text = text;
        countInfoText.color = color;
        countInfo.color = countColor;
        countInfo.GetComponent<FadeUI>().SetFadeValues(0.25f, fadeStart, fadeTime);
    }

    public void InfoInit()
    {
        timerInfoTexts[0].text = string.Format("{0:N2}", playTime);
        countInfo.color = countColor;
        timerInfo.color = timerColor;
        countInfoText.color = Color.white;
        for (int i = 0; i < timerInfoTexts.Length; i++)
            timerInfoTexts[i].color = Color.white;
    }
}