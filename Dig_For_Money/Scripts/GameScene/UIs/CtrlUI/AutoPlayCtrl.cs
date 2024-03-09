using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AutoPlayCtrl : MonoBehaviour
{
    static public AutoPlayCtrl instance;

    public Image image;
    public Button button;
    public Text buttonText;
    public GameObject info;
    public bool isAutoOn;

    private Color downColor, upColor;
    private Vector2Int currentPos; // 플레이어 최근 위치
    private Vector2 jemVec, oreVec, digVec; // 플레이어 채광 방향
    private float stopMaxTime; // 플레이어가 멈춘 시간
    public float breakTime;
    private int detectDis; // 감지 범위
    private int sign; // 움직이는 방향 부호
    private bool isStop, isDetect, isBlock, isJumpStart;
    private bool isUp, isDown, isEnd; // jem을 먹는데 그곳에 블럭이 있어 사용되는 dig
    private int count, maxCount; // Up & Down 카운트
    private float upPosY;
    private int startPosY, endPosY;

    [SerializeField]
    private LayerMask detectObject;

    Tile tempTile;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        downColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        upColor = Color.white;
        breakTime = SaveScript.picks[SaveScript.saveData.equipPick].breakTime;
        stopMaxTime = 1f;
        detectDis = 2;
        sign = 1;
        endPosY = 15;
        image.color = upColor;
        info.SetActive(false);
        DefaultData();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAutoOn)
        {
            Vector2 playerPos = PlayerScript.instance.transform.position;
            Vector2Int playerPos_int = Vector2Int.RoundToInt(playerPos);
            if (!isDetect) StartCoroutine("Detect");

            // 아래 넷 중 하나만 발동
            // 아무 것도 없음 + 빈 공간 존재
            if (jemVec == Vector2.zero && oreVec == Vector2.zero && !isBlock && !isUp && !isDown)
            {
                // -> 이동
                MoveCtrl.isMoveStart = true;
                PlayerScript.instance.moveData = sign;
            }
            // 현재 땅을 파는 방향에 블럭이 존재 -> 파괴
            if (isBlock)
            {
                if(CheckCanBreak(new Vector3Int(playerPos_int.x + sign, playerPos_int.y, 0)))
                {
                    MoveCtrl.isMoveStart = false;
                    Interaction.instance.Auto_Dig(digVec);
                }
                else
                {
                    if (GameFuction.GetRandFlag(GetPercentAsPosY())) SetDown(Random.Range(1, 3));
                    else SetUp(Random.Range(1, 3));
                }
            }
            // 근처에 광석 발견
            if (oreVec != Vector2.zero)
            {
                MoveCtrl.isMoveStart = false;
                Interaction.instance.Auto_Dig(oreVec);
            }
            // 떨어진 광물 발견
            if (jemVec != Vector2.zero)
            {
                MoveCtrl.isMoveStart = true;
                PlayerScript.instance.moveData = Mathf.Sign(jemVec.x);
            }
            // 계단형 파기 + 올라가기
            if (isUp)
            {
                if(!isJumpStart) 
                    StartCoroutine("JumpStart");
                MoveCtrl.isMoveStart = true;
                PlayerScript.instance.moveData = sign;
                if (playerPos.y > upPosY + 0.5f)
                {
                    if (CheckCanBreak(new Vector3Int(playerPos_int.x + sign, playerPos_int.y, 0)))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(playerPos_int, Vector2.right * sign, 0.5f, 256);
                        if (hit) Interaction.instance.Auto_Dig(Vector2.right * sign);
                    }
                    else 
                        isUp = false;
                }
                else
                {
                    if (CheckCanBreak(new Vector3Int(playerPos_int.x, playerPos_int.y + 1, 0)))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(playerPos_int, Vector2.up, 0.5f, 256);
                        if (hit) Interaction.instance.Auto_Dig(Vector2.up);
                    }
                    else
                        isUp = false;
                }
            }
            // 밑으로 파기 + 내려가기
            if (isDown)
            {
                if (CheckCanBreak(new Vector3Int(playerPos_int.x, playerPos_int.y - 1, 0)))
                {
                    float gap_x = playerPos.x - currentPos.x;
                    if (Mathf.Abs(gap_x) > 0f)
                        PlayerScript.instance.moveData = -Mathf.Sign(gap_x);
                    else
                        PlayerScript.instance.moveData = Mathf.Sign(gap_x);
                    MoveCtrl.isMoveStart = true;
                    Interaction.instance.Auto_Dig(Vector2.down);
                }
                else
                {
                    Debug.Log("아래로 가기 멈춤 - 위로 올라가기");
                    if (isEnd)
                        StartCoroutine("SetDefaultEnd");
                    else
                        SetUp(Random.Range(1, 3));
                }
            }

            // 플레이어가 위치가 변함
            if (currentPos != playerPos_int)
            {
                if (isUp || isDown)
                {
                    if (count < maxCount)
                    {
                        if (isUp && currentPos.x != playerPos_int.x && upPosY != playerPos_int.y)
                        {
                            upPosY = playerPos_int.y;
                            count++;
                        }
                        if (isDown && currentPos.y != playerPos_int.y)
                        {
                            count++;
                        }
                    }
                    if (count >= maxCount)
                    {
                        if (isUp) isUp = false;
                        if (isDown) 
                        {
                            isDown = false;
                            if (isEnd) StartCoroutine("SetDefaultEnd");
                        }
                    }
                }
                if (isStop)
                {
                    if(!isUp || (isUp && currentPos.x != playerPos_int.x))
                    {
                        isStop = false;
                        StopCoroutine("CheckStop");
                    }
                }
                currentPos = playerPos_int;
            }
            else 
            {
                // 플레이어가 멈춤
                if (!isEnd && !isStop) StartCoroutine("CheckStop");
                // 맵의 끝
                if (!isEnd && Mathf.Abs(currentPos.x) > MapData.instance.mapEndLength - 1.5)
                {
                    Debug.Log("플레이어 멈춤 - 맵의 끝");
                    isEnd = true;
                    isStop = false;
                    StopCoroutine("CheckStop");
                    sign = -sign;
                    PlayerScript.instance.moveData = sign;
                    SetDown(endPosY - (startPosY - currentPos.y));
                }
            }
        }
    }

    IEnumerator CheckStop()
    {
        isStop = true;

        yield return new WaitForSeconds(stopMaxTime * 2);

        Debug.Log("플레이어 멈춤 - 위로 올라가기");
        SetUp(Random.Range(1, 3));

        yield return new WaitForSeconds(stopMaxTime * 2);
        Debug.Log("플레이어 멈춤 - 재설정");
        SetAutoData();
        sign = -sign;
    }

    IEnumerator Detect()
    {
        isDetect = true;
        jemVec = oreVec = digVec = Vector2.zero;
        isBlock = false;

        oreVec = DetectOres();
        if (oreVec == Vector2.zero) jemVec = DetectJemObjects();
        if (jemVec == Vector2.zero && oreVec == Vector2.zero) isBlock = DetectBlock();
        if (currentPos.y < -178f && currentPos.y > -190f) SetUp(Random.Range(2, 4));
        if (currentPos.y < -220f && currentPos.y > -236f) SetDown(Random.Range(2, 4));
        if (isUp || isDown) Interaction.instance.SetInit();
        if ((jemVec == Vector2.zero && oreVec == Vector2.zero) && !isUp && !isDown && !isEnd)
        {
            if (GameFuction.GetRandFlag(breakTime))
            {
                if (GameFuction.GetRandFlag(GetPercentAsPosY()))
                {
                    Debug.Log("아래로 내려가기");
                    SetDown(Random.Range(1, 3));
                }
                else
                {
                    Debug.Log("위로 올라가기");
                    SetUp(Random.Range(1, 3));
                }
            }
        }

        yield return new WaitForSeconds(breakTime);

        isDetect = false;
    }

    IEnumerator JumpStart()
    {
        JumpCtrl.isJumpStart = true;
        isJumpStart = true;
        yield return new WaitForSeconds(0.15f);
        isJumpStart = false;
    }

    IEnumerator SetDefaultEnd()
    {
        yield return new WaitForSeconds(3f);
        isEnd = false;
        startPosY = (int)PlayerScript.instance.transform.position.y;
    }

    public Vector2 DetectJemObjects()
    {
        Vector2 data = Vector2.zero;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(PlayerScript.instance.transform.position, 2f, Vector2.zero, 0f, detectObject);
        if (hits.Length != 0) data = hits[0].transform.position - PlayerScript.instance.transform.position + new Vector3(sign * 0.1f, -0.1f, 0);

        return data;
    }

    public Vector2 DetectOres()
    {
        Vector2 data = Vector2.zero;

        for (int j = detectDis; j >= -detectDis; j--)
        {
            if (new Vector2(0, j).magnitude < Mathf.Sqrt(2 * detectDis * detectDis) && CheckCanBreak(new Vector3Int(currentPos.x, currentPos.y + j, 0)))
            {
                // 사이 블럭도 체크
                if (Mathf.Abs(j) == 2 && !CheckCanBreak(new Vector3Int(currentPos.x, currentPos.y + j - (int)Mathf.Sign(j), 0)))
                    continue;

                int jemIndex = MapData.instance.jemTiles.IndexOf(MapData.instance.GetTileMap(new Vector3Int(currentPos.x, currentPos.y + j, 0), 1).GetTile(new Vector3Int(currentPos.x, currentPos.y + j, 0)) as Tile);
                if (jemIndex == SaveScript.totalItemNum || jemIndex == SaveScript.totalItemNum + 1 || (jemIndex != -1 && (SaveScript.jems[jemIndex].quality == 5 || SaveScript.jems[jemIndex].quality == 6 ||
                    SaveScript.jems[jemIndex].quality <= SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick] / SaveScript.reinforceNumAsQulity)))
                {
                    data = new Vector2(0, j) * Interaction.instance.radius;
                    break;
                }
            }
        }

        return data;
    }

    public bool CheckCanBreak(Vector3Int pos)
    {
        tempTile = MapData.instance.GetTileMap(pos, 0).GetTile(pos) as Tile;
        int blockIndex = MapData.instance.blockTiles.IndexOf(tempTile);
        int jemIndex = MapData.instance.jemTiles.IndexOf(tempTile);
        int mapIndex = MapData.instance.mapBlockTiles.IndexOf(tempTile);
        int dungeon_0_index = MapData.instance.dungeon_0_blockTiles.IndexOf(tempTile);
        int dungeon_1_index = MapData.instance.dungeon_1_blockTiles.IndexOf(tempTile);
        int kingSlimeIndex = MapData.instance.kingSlimeBlocks.IndexOf(tempTile);

        if (kingSlimeIndex != -1 || dungeon_0_index != -1 || dungeon_1_index != -1 || blockIndex == 0) return false;
        if (mapIndex != -1)
        {
            if (mapIndex < SaveScript.saveData.equipPick + 3) return true;
            else return false;
        }

        if (jemIndex == SaveScript.totalItemNum || jemIndex == SaveScript.totalItemNum + 1 ||
            (blockIndex < SaveScript.saveData.equipPick + 2 && (jemIndex == -1 || SaveScript.jems[jemIndex].quality == 5 || SaveScript.jems[jemIndex].quality == 6
            || SaveScript.jems[jemIndex].quality <= SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick] / SaveScript.reinforceNumAsQulity)))
            return true;
        else
            return false;
    }

    public bool RayForward()
    {
        return Physics2D.BoxCast(this.transform.position, new Vector2(0.5f, 0.5f), 0f, Vector2.right * Mathf.Sign(this.transform.localScale.x), 0.1f, 256);
    }

    public bool DetectBlock()
    {
        bool isBlock = false;

        RaycastHit2D hit = Physics2D.Raycast(PlayerScript.instance.transform.position, Vector2.right * sign, 0.5f, 256);
        if (hit)
        {
            isBlock = true;
            digVec = Vector2.right * sign;
        }

        return isBlock;
    }

    public void SetUp(int _maxCount)
    {
        isUp = true;
        isDown = false;
        upPosY = Mathf.RoundToInt(PlayerScript.instance.transform.position.y);
        count = 0;
        if (_maxCount > 10) _maxCount = endPosY;
        maxCount = _maxCount;
    }

    public void SetDown(int _maxCount)
    {
        isUp = false;
        isDown = true;
        count = 0;
        if (_maxCount > 10) _maxCount = endPosY;
        maxCount = _maxCount;
    }

    private float GetPercentAsPosY()
    {
        return 0.5f + (currentPos.y - startPosY) / 2f * endPosY;
    }

    public void AutoPlayButton()
    {
        if (PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isAutoOn = !isAutoOn;
        PrintUI.instance.AudioPlay(0);
        if (isAutoOn)
        {
            SetAutoData();
            image.color = downColor;
            info.SetActive(true);
        }
        else
        {
            SetInit();
        }
    }

    public void SetAutoData()
    {
        DefaultData();
        MoveCtrl.isMoveStart = true;
        sign = (int)Mathf.Sign(PlayerScript.instance.transform.localScale.x);
        stopMaxTime = 1f + breakTime * 10f;
        currentPos = Vector2Int.RoundToInt(PlayerScript.instance.transform.position);
        startPosY = (int)PlayerScript.instance.transform.position.y;
    }

    public void SetInit()
    {
        MoveCtrl.isMoveStart = false;
        PlayerScript.instance.moveData = 0f;
        Interaction.instance.SetInit();
        DefaultData();

        isAutoOn = false;
        image.color = upColor;
        info.SetActive(false);
    }

    private void DefaultData()
    {
        isStop = isDetect = isBlock = isJumpStart = false;
        isUp = isDown = isEnd = false;
        count = maxCount = 0;
        upPosY = 0f;
        jemVec = oreVec = digVec = Vector2.zero;
        startPosY = 0;
        StopCoroutine("CheckStop");
    }

    public void SetButtonEnable(bool isEnable)
    {
        button.enabled = isEnable;
        buttonText.color = (isEnable) ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(0.3f, 0.3f, 0.3f, 0f);
        image.color = (isEnable) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
    }
}
