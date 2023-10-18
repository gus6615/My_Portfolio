using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Block
{
    public long HP, maxHP;

    public Block(int _index)
    {
        this.maxHP = SaveScript.special_block_hps[_index];
        this.HP = this.maxHP;
    }
}

public class RoomData : MonoBehaviour
{
    public int type;
    public EventBlock eventBlock;
    public Vector3Int pos;
    public int[][] maps;
    public Block[][] blocks;

    public RoomData(int _type, EventBlock _eventBlock, Vector3Int _pos, int[][] _maps, Block[][] _blocks)
    {
        SetData(_type, _eventBlock, _pos, _maps, _blocks);
    }

    public void SetData(int _type, EventBlock _eventBlock, Vector3Int _pos, int[][] _maps, Block[][] _blocks)
    {
        this.type = _type;
        this.pos = _pos;
        this.eventBlock = _eventBlock;
        this.maps = (int[][])_maps.Clone();
        if (_blocks != null)
            this.blocks = (Block[][])_blocks.Clone();
    }
};

public class DungeonCreater : MonoBehaviour
{
    public static DungeonCreater instance;
    static private float batSlimePercent = 0.03f;
    static private int maxSoilSlimeNum = 12;
    public int currentSoilSlimeNum;

    // 일반 던전 관련
    public List<Vector3Int> responVecList;
    private bool isMonsterCreating;
    private bool isBatSlimeCreating;

    // 유적 던전 관련 
    static public Vector3Int dungeon_0_startPos; // 현재 플레이어가 들어간 특수 던전 입구 위치
    static public GameObject dungeon_0_entranceEvent; // 현재 플레이어가 들어간 특수 던전 입구 이벤트블럭
    static public Vector3Int dungeon_0_portalPos; // 현재 플레이어가 들어간 특수 던전 포탈 입구 위치
    static public int dungeon_0_type; // 유적 던전 타입, 0 = 3층
    static public int dungeon_0_floor; // 유적 던전 층

    // 고대 던전
    static public Vector3Int dungeon_1_startPos; // 현재 플레이어가 들어간 던전 입구 위치
    static public GameObject dungeon_1_entranceEvent; // 현재 플레이어가 들어간 던전 입구 이벤트블럭
    static public int dungeon_1_type; // 고대 던전 타입, 0 = 7층
    static public RoomData dungeon_1_roomData; // 현재 방의 데이터

    [SerializeField] private GameObject dungeon_0_NPC;
    public Vector3Int dungeon_0_bodyStartVec, dungeon_1_bodyStartVec;
    public Vector3Int dungeon_0_entrance_startVec;
    public Vector3Int dungeon_1_entrance_startVec, dungeon_1_room_startVec;
    private Vector3Int kingSlime_startVec;
    public int dungeon_0_width, dungeon_0_height, dungeon_1_width, dungeon_1_height;
    private static int[] dungeon_1_room_widths = { 28, 12, 24, 12 };
    private static int dungeon_1_room_height;
    public int kingSlimeRoom_width, kingSlimeRoom_height;

    // Temp
    EventBlock eventBlock;
    SpeedSlime speedSlime;
    BigSlime bigSlime;
    MushroomSlime mushroomSlime;
    D_1_HoleSlime d_1_HoleSlime;
    D_1_MimicSlime D_1_MimicSlime;
    NormalBox normalBox;
    RuinBox ruinBox;
    AncientBox d_1_box;
    AncientTresure ancientTresure;
    D_1_Midboss D_1_Midboss;
    D_1_Boss D_1_Boss;
    D_1_Torch D_1_Torch;
    List<Vector2Int> randVecs = new List<Vector2Int>();


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        responVecList = new List<Vector3Int>();
        dungeon_0_bodyStartVec = new Vector3Int(200, 200, 0);
        dungeon_1_bodyStartVec = new Vector3Int(400, 200, 0);
        dungeon_1_room_startVec = new Vector3Int(400, 214, 0);
        dungeon_0_entrance_startVec = new Vector3Int(0, 23, 0);
        dungeon_1_entrance_startVec = new Vector3Int(0, 33, 0);
        kingSlime_startVec = new Vector3Int(7 ,13, 0);
        dungeon_0_width = 14;
        dungeon_0_height = 7;
        dungeon_1_width = 14;
        dungeon_1_height = 8;
        dungeon_1_room_height = dungeon_1_room_startVec.y - dungeon_1_bodyStartVec.y;
        kingSlimeRoom_width = 14;
        kingSlimeRoom_height = 6;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SaveScript.saveData.isTutorial) // 튜토리얼 X
        {
            if (!PlayerScript.instance.isEnd)
            {
                if (!PlayerScript.instance.isDungeon_0_On && !PlayerScript.instance.isDungeon_1_On && !PlayerScript.instance.isEventMap_On)
                {
                    if (!isMonsterCreating)
                        StartCoroutine(CreateMonster());
                    if (GameFuction.GetPlayerGrade() > 0 && !isBatSlimeCreating)
                        StartCoroutine(CreateBatSlime());
                }

            }
        }
    }

    // 몬스터가 생성될 수 있는 RandResponVec을 반환 
    public Vector3Int SetRandResponVec()
    {
        if (responVecList.Count == 0) return Vector3Int.up;
        return responVecList[Random.Range(0, responVecList.Count - 1)];
    }

    // 몬스터 생성
    IEnumerator CreateMonster()
    {
        isMonsterCreating = true;
        while (maxSoilSlimeNum < currentSoilSlimeNum)
            yield return new WaitForSeconds(10f);

        Vector3Int randVec = SetRandResponVec();
        while(randVec == Vector3Int.up || Vector3Int.Distance(Vector3Int.RoundToInt(PlayerScript.instance.transform.position), randVec) < CameraCtrl.instance.cameraWidth * 1.2f)
        {
            randVec = SetRandResponVec();
            yield return new WaitForSeconds(0.1f);
        }

        int type = MapData.instance.GetBlockTileType(randVec.y) - 1;
        if (type > -1)
        {
            SoilSlime data = ObjectPool.GetObject<SoilSlime>(6, ObjectPool.instance.objectTr, randVec);
            data.type = type;
            currentSoilSlimeNum++;
        }

        yield return new WaitForSeconds(Random.Range(18f - type, 24f - type));
        isMonsterCreating = false;
    }

    // 몬스터 생성
    IEnumerator CreateBatSlime()
    {
        isBatSlimeCreating = true;

        yield return new WaitForSeconds(6f);

        isBatSlimeCreating = false;
        if (GameFuction.GetRandFlag(batSlimePercent))
        {
            Vector3Int randVec;
            Vector3 playerPos = PlayerScript.instance.transform.position;
            float distance_X = CameraCtrl.instance.cameraWidth * 1.2f * Mathf.Sign(Random.Range(-1f, 1f));
            float distance_Y = CameraCtrl.instance.cameraHeight * 1.2f * Mathf.Sign(Random.Range(-1f, 1f));

            do
            {
                randVec = new Vector3Int((int)(Random.Range(playerPos.x + distance_X * 3, playerPos.x + distance_X * 2)),
                    (int)(Random.Range(playerPos.y + distance_Y * 3, playerPos.y + distance_Y * 2)), 0);
            } while (Vector3Int.Distance(Vector3Int.RoundToInt(playerPos), randVec) < distance_X);

            ObjectPool.GetObject<BatSlime>(21, ObjectPool.instance.objectTr, randVec);
        }
    }

    /// <summary>
    /// 플레이어 기준 카메라의 약 1.2배 반경에서 빈 공간 탐색
    /// </summary>
    public void SetResponVecList()
    {
        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(PlayerScript.instance.transform.position.x), Mathf.RoundToInt(PlayerScript.instance.transform.position.y));
        responVecList.Clear();

        for (int i = (int)(playerPos.x - MapData.instance.mapWidth * 1.2f); i < (int)(playerPos.x + MapData.instance.mapWidth * 1.2f); i++)
        {
            for (int j = playerPos.y - MapData.instance.mapHeight; j < playerPos.y + MapData.instance.mapHeight; j++)
            {
                if (MapData.instance.GetTileMap(new Vector3Int(i, j, 0), 2).GetTile(new Vector3Int(i, j, 0)) == MapData.instance.brokenTiles[0])
                {
                    responVecList.Add(new Vector3Int(i, j, 0));
                    break;
                }
            }
        }
    }

    // 폐광 생성 (pos를 기준점으로 아래, 왼쪽 및 오른쪽으로 생성)
    public void CreateMine(Vector3Int _pos, bool isLeft)
    {
        // 변수 초기화 및 할당
        Vector3Int pos = _pos;
        bool isPlus = false; // 한 층을 추가할 것인가?
        int blocktype = MapData.instance.GetBlockTileType(pos.y) + 1; // 벽의 타입
        int height = 4; // 높이
        int length = 0; // 길이
        int stairLength = 0; // 계단 길이
        int sign = 1; // 왼쪽, 오른쪽으로 생성되는지에 대한 절대값
        int count = 1; // 층의 갯수

        // 하나의 줄을 만드는 함수
        void CreateLine(int[] blockTypes)
        {
            for (int j = 0; j < blockTypes.Length; j++)
            {
                Vector3Int point = pos + new Vector3Int(0, -j, 0);
                int mapIndex = MapData.instance.mapBlockTiles.IndexOf(MapData.instance.GetTileMap(point, 0).GetTile(point) as Tile);
                // 블럭 안의 몬스터 및 박스 제거
                MapData.instance.DeleteObject(point);
                if (mapIndex > blocktype) continue; // 깊은 층의 일반 던전과 겹치는 경우 생성 안함
                if ((point.y < MapData.depth[5] && point.y >= MapData.depth[6]) || (point.y < MapData.depth[10] && point.y >= MapData.depth[11])) continue; // 빈 공간일 경우 생성 안함
                if (Mathf.Abs(point.x) >= MapData.instance.mapEndLength || point.y < MapData.depth[MapData.depth.Length - 1]) continue; // 맵의 끝일 경우 생성 안함

                int dungeon_0 = MapData.instance.dungeon_0_DecoX32Tiles.IndexOf(MapData.instance.GetTileMap(point, 4).GetTile(point) as Tile);
                int dungeon_1 = MapData.instance.dungeon_1_blockTiles.IndexOf(MapData.instance.GetTileMap(point, 4).GetTile(point) as Tile);
                int kingBlockIndex = MapData.instance.kingSlimeBlocks.IndexOf(MapData.instance.GetTileMap(point, 4).GetTile(point) as Tile);

                // 특수 던전 OR 킹슬라임 방은 PASS
                if (dungeon_0 == -1 && dungeon_1 == -1 && kingBlockIndex == -1)
                {
                    if (point.y != CameraCtrl.instance.cameraHeight)
                    {
                        MapData.instance.GetTileMap(point, 0).SetTile(point, null);
                        MapData.instance.GetTileMap(point, 1).SetTile(point, null);
                        MapData.instance.GetTileMap(point, 2).SetTile(point, null);
                        MapData.instance.GetTileMap(point, 10).SetTile(point, null);

                        MapData.instance.GetTileMap(point, 4).SetTile(point, MapData.instance.backgroundBlockTiles[blocktype - 2]);
                        if (blockTypes[j] == 0) // 벽
                            MapData.instance.GetTileMap(point, 0).SetTile(point, MapData.instance.mapBlockTiles[blocktype]);
                        else // 빈 공간
                            MapData.instance.GetTileMap(point, 2).SetTile(point, MapData.instance.brokenTiles[0]);
                    }
                    else
                    {
                        MapData.instance.GetTileMap(point, 0).SetTile(point, MapData.instance.blockTiles[0]);
                    }
                }
            }
        }
        if (isLeft) sign = -1;
        else sign = 1;

        // 생성 시작 (첫번째 줄)
        CreateLine(new int[] { 0, 0, 0, 0 });
        pos += new Vector3Int(sign * 1, 0, 0);

        // 두번째 줄 시작
        do
        {
            length = Random.Range(5, 9);

            for (int i = 0; i < length; i++)
            {
                CreateLine(new int[] { 0, 1, 1, 0 });

                // 상자 생성
                if (Mathf.Abs(pos.x) < MapData.instance.mapEndLength && GameFuction.GetRandFlag(0.1f)
                    && MapData.instance.GetTileMap(pos + new Vector3Int(0, 2 - height, 0), 0).GetTile(pos + new Vector3Int(0, 2 - height, 0)) == null)
                {
                    normalBox = ObjectPool.GetObject<NormalBox>(2, ObjectPool.instance.objectTr, pos + new Vector3Int(0, 2 - height, 0));
                    normalBox.boxType = blocktype - 2;
                }

                // 던전 몬스터 생성
                if (Mathf.Abs(pos.x) < MapData.instance.mapEndLength && GameFuction.GetRandFlag(0.1f)
                    && MapData.instance.GetTileMap(pos + new Vector3Int(0, 2 - height, 0), 0).GetTile(pos + new Vector3Int(0, 2 - height, 0)) == null)
                {
                    ADSlime monster = ObjectPool.GetObject<ADSlime>(7, ObjectPool.instance.objectTr, pos + new Vector3Int(0, 2 - height, 0));
                    monster.type = blocktype - 2;
                }

                pos += new Vector3Int(sign * 1, 0, 0);
            }

            // 계단 생성
            if (GameFuction.GetRandFlag(0.7f - 0.1f * count))
            {
                int rand = Random.Range(1, 3);
                count++;
                isPlus = true;
                stairLength = Random.Range(2, 5);

                if (rand == 1) // 위로
                {
                    pos += new Vector3Int(0, 1, 0);
                    for (int i = 0; i < stairLength; i++)
                    {
                        if (i == 0)
                            CreateLine(new int[] { 0, 0, 1, 1, 0 });
                        else if (i == 1)
                            CreateLine(new int[] { 0, 0, 1, 1, 1, 0 });
                        else
                            CreateLine(new int[] { 0, 0, 1, 1, 1, 0, 0 });

                        pos += new Vector3Int(sign * 1, 1, 0);
                    }

                    pos += new Vector3Int(0, -1, 0);

                    CreateLine(new int[] { 0, 1, 1, 1, 0, 0 });
                    pos += new Vector3Int(sign * 1, 0, 0);

                    CreateLine(new int[] { 0, 1, 1, 0, 0 });
                    pos += new Vector3Int(sign * 1, 0, 0);
                }
                else // 아래로
                {
                    for (int i = 0; i < stairLength; i++)
                    {
                        if (i == 0)
                        {
                            CreateLine(new int[] { 0, 1, 1, 0, 0 });
                            pos += new Vector3Int(sign * 1, 0, 0);
                        }
                        else if (i == 1)
                        {
                            CreateLine(new int[] { 0, 1, 1, 1, 0, 0 });
                            pos += new Vector3Int(sign * 1, 0, 0);
                        }
                        else
                        {
                            CreateLine(new int[] { 0, 0, 1, 1, 1, 0, 0 });
                            pos += new Vector3Int(sign * 1, -1, 0);
                        }
                    }

                    CreateLine(new int[] { 0, 0, 1, 1, 1, 0 });
                    pos += new Vector3Int(sign * 1, -1, 0);

                    CreateLine(new int[] { 0, 0, 1, 1, 0 });
                    pos += new Vector3Int(sign * 1, -1, 0);
                }
            }
            else
            {
                isPlus = false;
            }

        } while (isPlus);

        CreateLine(new int[] { 0, 0, 0, 0 });

        SetResponVecList();
    }

    // 던전 생성 (pos를 기준점으로 type 형태의 던전을 아래, 왼쪽 및 오른쪽으로 생성), type은 0(3번째 층)부터 시작
    public void CreateDungeon_Entrance(int mode, Vector3Int _pos, int type, bool isLeft)
    {
        // Mode Setting
        List<Tile> dungeon_blockTiles, dungeon_wallTiles, dungeon_decoX32, dungeon_decoX64;
        int width, height;

        // 입구 관련 변수
        Vector3Int render_startVec; // 랜더링 시작점
        Vector3Int startPos;
        if (isLeft) startPos = _pos + Vector3Int.left * 15;
        else startPos = _pos;

        switch (mode)
        {
            case 0:
                dungeon_blockTiles = MapData.instance.dungeon_0_blockTiles;
                dungeon_wallTiles = MapData.instance.dungeon_0_DecoX32Tiles;
                dungeon_decoX32 = MapData.instance.dungeon_0_DecoX32Tiles;
                dungeon_decoX64 = MapData.instance.dungeon_0_DecoX64Tiles;
                width = dungeon_0_width;
                height = dungeon_0_height;
                render_startVec = dungeon_0_entrance_startVec + Vector3Int.right * 15 * type; // 랜더링 시작점
                Debug.Log("유적 던전 생성");
                break;
            default:
                dungeon_blockTiles = MapData.instance.dungeon_1_blockTiles;
                dungeon_wallTiles = MapData.instance.dungeon_1_blockTiles;
                dungeon_decoX32 = MapData.instance.dungeon_1_DecoX32Tiles;
                dungeon_decoX64 = MapData.instance.dungeon_1_DecoX64Tiles;
                width = dungeon_1_width;
                height = dungeon_1_height;
                render_startVec = dungeon_1_entrance_startVec + Vector3Int.right * 15 * type; // 랜더링 시작점
                Debug.Log("고대 던전 생성");
                break;
        }

        // 던전 입구 블럭 하나를 랜더링 하는 함수
        void RenderBlock(Vector3Int render_pos, Vector3Int real_pos)
        {
            int blockIndex = dungeon_blockTiles.IndexOf(MapData.instance.GetTileMap(render_pos, 0).GetTile(render_pos) as Tile);
            int wallIndex = dungeon_wallTiles.IndexOf(MapData.instance.GetTileMap(render_pos, 4).GetTile(render_pos) as Tile);
            bool isNotWall = false;
            int decoX32Index = dungeon_decoX32.IndexOf(MapData.instance.GetTileMap(render_pos, 5).GetTile(render_pos) as Tile);
            int decoX32PosType = 0; // 0 = behind_nonblock, 1 = behind_block, 2 = forward_nonblock, 3 = forward_block
            int decoX64Index = dungeon_decoX64.IndexOf(MapData.instance.GetTileMap(render_pos, 5).GetTile(render_pos) as Tile);
            int decoX64PosType = 0; // 0 = behind_nonblock, 1 = behind_block, 2 = forward_nonblock, 3 = forward_block
            bool isEmptySpace = false; // 구조물이 있는가?

            // 블러 안의 몬스터 및 박스 제거
            MapData.instance.DeleteObject(real_pos);

            // wallIndex 탐색
            if (wallIndex == -1)
            {
                wallIndex = MapData.instance.backgroundBlockTiles.IndexOf(MapData.instance.GetTileMap(render_pos, 4).GetTile(render_pos) as Tile);
                isNotWall = true;
            }
            else
            {
                MapData.instance.GetTileMap(real_pos, 1).SetTile(real_pos, null);
                MapData.instance.GetTileMap(real_pos, 10).SetTile(real_pos, null);
            }

            // decoX32 탐색
            if (decoX32Index == -1)
            {
                decoX32Index = dungeon_decoX32.IndexOf(MapData.instance.GetTileMap(render_pos, 6).GetTile(render_pos) as Tile);
                decoX32PosType = 1;

                if (decoX32Index == -1)
                {
                    decoX32Index = dungeon_decoX32.IndexOf(MapData.instance.GetTileMap(render_pos, 8).GetTile(render_pos) as Tile);
                    decoX32PosType = 2;

                    if (decoX32Index == -1)
                    {
                        decoX32Index = dungeon_decoX32.IndexOf(MapData.instance.GetTileMap(render_pos, 9).GetTile(render_pos) as Tile);
                        decoX32PosType = 3;
                    }
                }
            }

            // decoX64 탐색
            if (decoX64Index == -1)
            {
                decoX64Index = dungeon_decoX64.IndexOf(MapData.instance.GetTileMap(render_pos, 6).GetTile(render_pos) as Tile);
                decoX64PosType = 1;

                if (decoX64Index == -1)
                {
                    decoX64Index = dungeon_decoX64.IndexOf(MapData.instance.GetTileMap(render_pos, 8).GetTile(render_pos) as Tile);
                    decoX64PosType = 2;

                    if (decoX64Index == -1)
                    {
                        decoX64Index = dungeon_decoX64.IndexOf(MapData.instance.GetTileMap(render_pos, 9).GetTile(render_pos) as Tile);
                        decoX64PosType = 3;
                    }
                }
            }

            if (blockIndex == -1 && isNotWall && decoX32Index == -1 && decoX64Index == -1)
                isEmptySpace = true;

            // 해당 위치에 건축물이 있을 경우 (ex. 일반 던전)
            if (MapData.instance.GetTileMap(real_pos, 0).GetTile(real_pos) != null || MapData.instance.GetTileMap(real_pos, 2).GetTile(real_pos) != null)
            {
                // 해당 위치에 특수 던전 구조물이 들어오는 경우
                if (!isEmptySpace)
                {
                    MapData.instance.GetTileMap(real_pos, 0).SetTile(real_pos, null);
                    MapData.instance.GetTileMap(real_pos, 2).SetTile(real_pos, null);
                    MapData.instance.GetTileMap(real_pos, 4).SetTile(real_pos, null);
                }
            }
            else
            {
                // 해당 위치에 어떤 건축물이 없을 경우 ( 빈 블럭 )
                if (isEmptySpace)
                    MapData.instance.GetTileMap(real_pos, 0).SetTile(real_pos, MapData.instance.blockTiles[MapData.instance.GetBlockTileType(real_pos.y)]);
            }

            // 랜더링 시작
            if (blockIndex > -1)
            {
                MapData.instance.GetTileMap(real_pos, 0).SetTile(real_pos, dungeon_blockTiles[blockIndex]);
            }

            if (wallIndex > -1)
            {
                if (isNotWall)
                {
                    if (MapData.instance.kingSlimeBlocks.IndexOf(MapData.instance.GetTileMap(real_pos, 4).GetTile(real_pos) as Tile) == -1)
                        MapData.instance.GetTileMap(real_pos, 4).SetTile(real_pos, MapData.instance.backgroundBlockTiles[wallIndex]);
                }
                else
                {
                    MapData.instance.GetTileMap(real_pos, 4).SetTile(real_pos, dungeon_wallTiles[wallIndex]);
                    if (MapData.instance.GetTileMap(real_pos, 0).GetTile(real_pos) == null)
                        MapData.instance.GetTileMap(real_pos, 2).SetTile(real_pos, MapData.instance.brokenTiles[0]);
                }
            }

            if (decoX32Index > -1)
            {
                switch (decoX32PosType)
                {
                    case 0:
                        if (mode == 0 && (decoX32Index == 0 || decoX32Index == 1))
                            ObjectPool.GetObject<D_0_Torch>(27, ObjectPool.instance.objectTr, real_pos + Vector3.one * 0.5f);
                        else
                            MapData.instance.GetTileMap(real_pos, 5).SetTile(real_pos, dungeon_decoX32[decoX32Index]);
                        break;
                    case 1: MapData.instance.GetTileMap(real_pos, 6).SetTile(real_pos, dungeon_decoX32[decoX32Index]); break;
                    case 2: MapData.instance.GetTileMap(real_pos, 8).SetTile(real_pos, dungeon_decoX32[decoX32Index]); break;
                    case 3: MapData.instance.GetTileMap(real_pos, 9).SetTile(real_pos, dungeon_decoX32[decoX32Index]); break;
                }
            }

            if (decoX64Index > -1)
            {
                switch (decoX64PosType)
                {
                    case 0:
                        if (mode == 1 && (decoX64Index % 7 == 5 || decoX64Index % 7 == 6))
                        {
                            D_1_Torch = ObjectPool.GetObject<D_1_Torch>(26, ObjectPool.instance.objectTr, real_pos + Vector3.right * 0.5f);
                            D_1_Torch.type = decoX64Index / 7;
                        }
                        else
                            MapData.instance.GetTileMap(real_pos, 5).SetTile(real_pos, dungeon_decoX64[decoX64Index]);
                        break;
                    case 1: MapData.instance.GetTileMap(real_pos, 6).SetTile(real_pos, dungeon_decoX64[decoX64Index]); break;
                    case 2: MapData.instance.GetTileMap(real_pos, 8).SetTile(real_pos, dungeon_decoX64[decoX64Index]); break;
                    case 3: MapData.instance.GetTileMap(real_pos, 9).SetTile(real_pos, dungeon_decoX64[decoX64Index]); break;
                }
            }
        }

        // 랜더링
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Mathf.Abs(startPos.x + i) < MapData.instance.mapEndLength)
                    RenderBlock(render_startVec + new Vector3Int(i, -j, 0), startPos + new Vector3Int(i, -j, 0));
            }
        }

        switch (mode)
        {
            case 0:
                if (isLeft)
                {
                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.objectTr, _pos + new Vector3(-8.5f, -5f, 0));
                    eventBlock.portal_vec = _pos + new Vector3(-8.5f, -5f, 0);
                }
                else
                {
                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.objectTr, _pos + new Vector3(6.5f, -5f, 0));
                    eventBlock.portal_vec = _pos + new Vector3(6.5f, -5f, 0);
                }
                eventBlock.eventMainType = 0;
                eventBlock.eventSubType = type;
                break;
            default:
                if (isLeft)
                {
                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.objectTr, _pos + new Vector3(-8.5f, -4f, 0));
                    eventBlock.portal_vec = _pos + new Vector3(-8.5f, -4f, 0);
                }
                else
                {
                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.objectTr, _pos + new Vector3(6.5f, -4f, 0));
                    eventBlock.portal_vec = _pos + new Vector3(6.5f, -4f, 0);
                }
                eventBlock.eventMainType = 9;
                eventBlock.eventSubType = type;
                CashEquipmentCtrl.instance.eventBlocks.Add(eventBlock);
                break;
        }
    }

    public void CreateDungeon_0_Body(int type)
    {
        // 특수 던전 본체 제작 변수
        Vector3Int leftStartVec = dungeon_0_bodyStartVec; // 왼쪽 기준점
        Vector3Int rightStartVec = leftStartVec + Vector3Int.right; // 오른쪽 기준점
        Vector3Int currentPos = leftStartVec; // 현재 위치
        Vector3Int leftEndVec = leftStartVec; // 왼쪽 끝 점, 포탈 위치
        Vector3Int rightEndVec = rightStartVec; // 오른쪽 끝 점, 포탈 위치
        List<Vector3Int> responVec = new List<Vector3Int>(); // 특수 던전 상자 위치
        int lobby_width = 10; // 로비 가로 길이
        int all_height = 7; // 로비 및 방 기본 높이
        int room_width = Random.Range(10, 15);
        int r_count = 0; // 방의 갯수
        int f_count = 0; // 층의 갯수
        bool r_isPlus = false; // 방 추가?
        bool isPlusFloor = false;
        float room_1_percent = 0.15f; // 도서관 리스폰 확률
        float room_2_percent = 0.1f; // NPC 상인 리스폰 확률

        // 던전 본체 한 라인 생성 함수 
        void CreateLine(Vector3Int point, int[] blockTypes, bool _isLeft)
        {
            for (int j = 0; j < blockTypes.Length; j++)
            {
                Vector3Int tempPos = point + Vector3Int.down * j;

                switch (blockTypes[j])
                {
                    case -1: // 빈 공간 + 상자
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_0_DecoX32Tiles[8 + type]);
                        responVec.Add(tempPos);
                        break;
                    case 0: // 빈 공간
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_0_DecoX32Tiles[8 + type]);
                        break;
                    case 1: // 벽
                        MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.dungeon_0_blockTiles[type]);
                        break;
                    case 2: // 책장
                        break;
                }
            }

            if (_isLeft)
                currentPos += Vector3Int.left;
            else
                currentPos += Vector3Int.right;
        }

        // 던전 방 생성
        void CreateRoom(int _type, bool isLeft)
        {
            int sign = 1;
            if (isLeft)
                sign = -1;

            if (_type == 1)
            {
                // 도서관
                Vector3Int leftTop = currentPos + Vector3Int.down;
                Vector3Int rightBot = currentPos + new Vector3Int((int)(room_width * 1.5f) * sign, -(all_height - 2), 0);

                // 공간 생성
                for (int i = 0; i < (int)(room_width * 1.5f); i++)
                    CreateLine(currentPos, new int[] { 1, 0, 0, 0, 0, 0, 1 }, isLeft);

                // 책장 생성
                int height = Random.Range(all_height - 4, all_height - 2); // 높이 3 ~ 4
                int width = Random.Range(3, 5); // 가로 3 ~ 4
                bool isNext = false;
                Vector3Int start = new Vector3Int(Random.Range(leftTop.x, leftTop.x + 2 * sign), rightBot.y + height - 1, 0);

                do
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Vector3Int dataPos = start + new Vector3Int(i * sign, -j, 0);

                            if (i == 0)
                            {
                                if (isLeft)
                                    MapData.instance.GetTileMap(dataPos, 6).SetTile(dataPos, MapData.instance.dungeon_0_DecoX32Tiles[6 + Random.Range(0, 2)]);
                                else
                                    MapData.instance.GetTileMap(dataPos, 6).SetTile(dataPos, MapData.instance.dungeon_0_DecoX32Tiles[2 + Random.Range(0, 2)]);
                            }
                            else if (i == width - 1)
                            {
                                if (isLeft)
                                    MapData.instance.GetTileMap(dataPos, 6).SetTile(dataPos, MapData.instance.dungeon_0_DecoX32Tiles[2 + Random.Range(0, 2)]);
                                else
                                    MapData.instance.GetTileMap(dataPos, 6).SetTile(dataPos, MapData.instance.dungeon_0_DecoX32Tiles[6 + Random.Range(0, 2)]);
                            }
                            else
                            {
                                MapData.instance.GetTileMap(dataPos, 6).SetTile(dataPos, MapData.instance.dungeon_0_DecoX32Tiles[4 + Random.Range(0, 2)]);
                            }

                            if (GameFuction.GetRandFlag(0.1f))
                            {
                                MapData.instance.GetTileMap(dataPos, 10).SetTile(dataPos, MapData.instance.jemLightTiles[MapData.instance.jemLightTiles.Count - 1]);
                                eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, dataPos);
                                eventBlock.eventMainType = 4;
                            }
                        }
                    }

                    start += Vector3Int.right * sign * (width + Random.Range(0, 2));
                    width = Random.Range(3, 5); // 가로 3 ~ 4

                    if (isLeft)
                    {
                        if (start.x - width >= rightBot.x)
                            isNext = true;
                        else
                            isNext = false;
                    }
                    else
                    {
                        if (start.x + width <= rightBot.x)
                            isNext = true;
                        else
                            isNext = false;
                    }
                } while (isNext);
            }
            else if (_type == 2)
            {
                Vector3Int leftTop = currentPos + Vector3Int.down;
                Vector3Int rightBot = currentPos + new Vector3Int(room_width * sign, -(all_height - 2), 0);

                // NPC 상인 
                for (int i = 0; i < room_width; i++)
                    CreateLine(currentPos, new int[] { 1, 0, 0, 0, 0, 0, 1 }, isLeft);

                // NPC 생성
                Dungeon_0_NPC data = ObjectPool.GetObject<Dungeon_0_NPC>(5, ObjectPool.instance.dungeon_0_objectTr, (leftTop + rightBot) / 2);
                data.GetComponentInChildren<EventBlock>().eventMainType = 5;
                data.type = type;
            }
            else
            {
                Vector3Int leftTop = currentPos + Vector3Int.down;
                Vector3Int rightBot = currentPos + new Vector3Int(room_width * sign, -(all_height - 2), 0);
                Vector3Int start = leftTop + Vector3Int.right * Random.Range(1, 4) * sign;
                bool isPlusPillar = false; // 기둥 추가
                bool isPlusBlock = false; // 발판이 있는가?
                int width = 4; // 기둥 사이의 간격
                responVec.Clear();

                // 일반 벽
                for (int i = 0; i < room_width; i++)
                    CreateLine(currentPos, new int[] { 1, 0, 0, 0, 0, -1, 1 }, isLeft);

                // 기둥 생성
                do
                {
                    for (int i = 0; i < all_height - 2; i++)
                    {
                        Vector3Int pos = start + Vector3Int.down * i;

                        if (i == 0) // 맨 위
                            MapData.instance.GetTileMap(pos, 6).SetTile(pos, MapData.instance.dungeon_0_DecoX32Tiles[21 + type * 3]);
                        else if (i == all_height - 3) // 맨 아래
                            MapData.instance.GetTileMap(pos, 6).SetTile(pos, MapData.instance.dungeon_0_DecoX32Tiles[23 + type * 3]);
                        else // 기둥 본체
                            MapData.instance.GetTileMap(pos, 6).SetTile(pos, MapData.instance.dungeon_0_DecoX32Tiles[22 + type * 3]);
                    }

                    start += Vector3Int.right * width * sign;

                    if (isLeft)
                    {
                        if (start.x - width - 1 > rightBot.x)
                            isPlusPillar = true;
                        else
                            isPlusPillar = false;
                    }
                    else
                    {
                        if (start.x + width + 1 < rightBot.x)
                            isPlusPillar = true;
                        else
                            isPlusPillar = false;
                    }
                } while (isPlusPillar);

                // 발판 생성
                isPlusBlock = GameFuction.GetRandFlag(0.5f);
                if (isPlusBlock)
                {
                    bool isPlus = true;
                    int count = 0;
                    start = leftTop + new Vector3Int(Random.Range(1, 4) * sign, -(all_height / 2 - 1), 0);

                    do
                    {
                        MapData.instance.GetTileMap(start, 7).SetTile(start, MapData.instance.dungeon_0_blockTiles[type]);
                        responVec.Add(start + Vector3Int.up);
                        start += Vector3Int.right * sign;

                        if (isLeft)
                        {
                            if (start.x - 1 > rightBot.x)
                                isPlus = GameFuction.GetRandFlag(1f - (++count - 3) * 0.2f);
                            else
                                isPlus = false;
                        }
                        else
                        {
                            if (start.x + 1 < rightBot.x)
                                isPlus = GameFuction.GetRandFlag(1f - (++count - 3) * 0.2f);
                            else
                                isPlus = false;
                        }
                    } while (isPlus);
                }

                // 상자 생성 및 몬스터 생성
                if (responVec.Count != 0)
                {
                    for (int i = 0; i < responVec.Count; i++)
                    {
                        // 상자 생성
                        if (GameFuction.GetRandFlag(0.1f))
                        {
                            normalBox = ObjectPool.GetObject<NormalBox>(2, ObjectPool.instance.dungeon_0_objectTr, responVec[i]);
                            normalBox.boxType = 2 + type;
                        }
                        else if (GameFuction.GetRandFlag(0.05f))
                        {
                            ruinBox = ObjectPool.GetObject<RuinBox>(3, ObjectPool.instance.dungeon_0_objectTr, responVec[i] + Vector3.up * 0.5f);
                            ruinBox.boxType = 2 + type;
                        }

                        // 몬스터 생성
                        if (GameFuction.GetRandFlag(0.05f))
                        {
                            speedSlime = ObjectPool.GetObject<SpeedSlime>(9, ObjectPool.instance.dungeon_0_objectTr, responVec[i]);
                            speedSlime.type = 2 + type;
                        }
                        if (!isPlusBlock && GameFuction.GetRandFlag(0.02f))
                        {
                            bigSlime = ObjectPool.GetObject<BigSlime>(10, ObjectPool.instance.dungeon_0_objectTr, responVec[i]);
                            bigSlime.type = 2 + type;
                        }
                        if (GameFuction.GetRandFlag(0.03f))
                        {
                            mushroomSlime = ObjectPool.GetObject<MushroomSlime>(11, ObjectPool.instance.dungeon_0_objectTr, responVec[i]);
                            mushroomSlime.type = 2 + type;
                        }
                    }
                }
            }
        }

        do
        {
            isPlusFloor = GameFuction.GetRandFlag(1f - f_count * 0.25f);
            room_width = Random.Range(8, 13);
            r_count = 0;

            // 왼쪽 본체 중심 제작
            currentPos = leftStartVec + Vector3Int.down * all_height * f_count;
            Vector3Int pos = currentPos + new Vector3Int(-1, -(all_height - 2), 0);
            if (f_count == 0)
            {
                MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_0_DecoX64Tiles[3 + type * 3]);
                eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, currentPos + new Vector3(-0.5f, -(all_height - 2), 0));
                eventBlock.eventMainType = 1;
            }
            else
            {
                MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_0_DecoX64Tiles[2]);
                eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, currentPos + new Vector3(-0.5f, -(all_height - 2), 0));
                eventBlock.eventMainType = 3;
            }

            for (int i = 0; i < lobby_width * 0.5 - 1; i++)
                CreateLine(currentPos, new int[] { 1, 0, 0, 0, 0, 0, 1 }, true);
            CreateLine(currentPos, new int[] { 1, 1, 1, 1, 1, 0, 1 }, true);

            // 왼쪽 본체 방 제작
            do
            {
                float[] percents = { 0f, room_1_percent, room_2_percent };
                percents[0] = 1f - (percents[1] + percents[2]);
                int roomType = GameFuction.GetRandFlag(percents); // 0 = 일반 방, 1 = 도서관, 2 = NPC 방

                CreateRoom(roomType, true);

                r_isPlus = GameFuction.GetRandFlag(0.5f - r_count * 0.15f);
                if (r_isPlus)
                    CreateLine(currentPos, new int[] { 1, 1, 1, 1, 1, 0, 1 }, true);
                else
                    CreateLine(currentPos, new int[] { 1, 1, 1, 1, 1, 1, 1 }, true);

                r_count++;
                room_width = Random.Range(8, 13);
            } while (r_isPlus);
            leftEndVec = currentPos + new Vector3Int(2, -(all_height - 2), 0);

            // 오른쪽 본체 중심 제작
            currentPos = rightStartVec + Vector3Int.down * all_height * f_count;
            pos = currentPos + new Vector3Int(1, -(all_height - 2), 0);
            if (isPlusFloor)
            {
                MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_0_DecoX64Tiles[2]);
                eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, currentPos + new Vector3(1.5f, -(all_height - 2), 0));
                eventBlock.eventMainType = 2;
            }
            else
            {
                MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_0_DecoX64Tiles[3 + type * 3]);
                eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, currentPos + new Vector3(1.5f, -(all_height - 2), 0));
                eventBlock.eventMainType = 1;
            }

            for (int i = 0; i < lobby_width * 0.5; i++)
                CreateLine(currentPos, new int[] { 1, 0, 0, 0, 0, 0, 1 }, false);
            CreateLine(currentPos, new int[] { 1, 1, 1, 1, 1, 0, 1 }, false);

            // 오른쪽 본체 방 제작
            do
            {
                float[] percents = { 0f, room_1_percent, room_2_percent };
                percents[0] = 1f - (percents[1] + percents[2]);
                int roomType = GameFuction.GetRandFlag(percents); // 0 = 일반 방, 1 = 도서관, 2 = NPC 방

                CreateRoom(roomType, false);

                r_isPlus = GameFuction.GetRandFlag(0.5f - r_count * 0.15f);
                if (r_isPlus)
                    CreateLine(currentPos, new int[] { 1, 1, 1, 1, 1, 0, 1 }, false);
                else
                    CreateLine(currentPos, new int[] { 1, 1, 1, 1, 1, 1, 1 }, false);

                r_count++;
                room_width = Random.Range(8, 13);
            } while (r_isPlus);
            rightEndVec = currentPos + new Vector3Int(-3, -(all_height - 2), 0);

            // 왼쪽 끝과 오른쪽 끝 연결
            MapData.instance.GetTileMap(leftEndVec, 5).SetTile(leftEndVec, MapData.instance.dungeon_0_DecoX64Tiles[2]);
            MapData.instance.GetTileMap(rightEndVec, 5).SetTile(rightEndVec, MapData.instance.dungeon_0_DecoX64Tiles[2]);
            eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, leftEndVec);
            eventBlock.eventMainType = 6;
            eventBlock.portal_vec = rightEndVec;
            eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_0_objectTr, rightEndVec);
            eventBlock.eventMainType = 6;
            eventBlock.portal_vec = leftEndVec;

            f_count++;
        } while (isPlusFloor);
    }

    public void CreateDungeon_1_Body(int type)
    {
        Vector3Int startVec = dungeon_1_bodyStartVec; // 왼쪽 기준점
        Vector3Int currentPos = startVec; // 현재 위치
        int lobby_width = 10; // 로비 가로 길이
        int all_height = 12; // 로비 및 방 기본 높이 (EventButton에도 수정)
        int room_width = 9;
        int f_count = 0; // 층의 갯수
        bool isPlusFloor = false;
        float[] roomPercents = { 0.5f, 0.25f, 0.25f };
        float mysticRoomPercent = 0.1f;
        float manaRoomPercent = 0.6f;
        int maxTreasureNum = 4;
        float treasurePercent = 0.4f;

        // 던전 방 데이터 설정하는 함수
        int[][] SetMapData(int _type, out Block[][] _block_hps)
        {
            int length = dungeon_1_room_widths[_type];
            int height = dungeon_1_room_height;
            int[][] maps = new int[length][];
            Block[][] block_hps = null;
            if (_type == 2) 
                block_hps = new Block[length][];

            // 공통 맵 설정
            for (int i = 0; i < length; i++)
            {
                maps[i] = new int[height];
                if (_type == 2)
                    block_hps[i] = new Block[height];

                if (i == 0 || i == length - 1)
                {
                    for (int j = 0; j < height; j++)
                        maps[i][j] = 0;
                }
                else
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (j == 0 || j == height - 1)
                            maps[i][j] = 0;
                        else if (j == height - 2)
                            maps[i][j] = 1;
                        else if (j == height - 3)
                            maps[i][j] = 3;
                        else
                            maps[i][j] = 2;
                    }
                }
            }

            // 맵 세부 설정
            switch (_type)
            {
                case 0: // 일반 방
                    int maxLength = 10; // 최대 발판 길이
                    int curLength = 0;
                    int goalLength = 0;
                    int noCount = 0;
                    bool isCreating = false;

                    for (int j = height - 5; j >= 3; j -= 3)
                    {
                        for (int i = 5; i < length; i++)
                        {
                            if (j == height - 5 && i != length - 1)
                            {
                                if (GameFuction.GetRandFlag(0.05f))
                                    maps[i][j + 1] = -2; // 검 몬스터 생성
                                else if (GameFuction.GetRandFlag(0.075f))
                                    maps[i][j + 1] = -4; // 상자 생성
                            }

                            if (i == 5)
                                isCreating = false; // 첫번째 자리로 이동

                            if (!isCreating)
                            {
                                if (GameFuction.GetRandFlag(0.5f + 0.1f * (noCount + 1)))
                                {
                                    // 블록 시작점
                                    curLength = 0;
                                    noCount = 0;
                                    goalLength = Random.Range(4, maxLength);
                                    isCreating = true;
                                }
                                else
                                    noCount++;
                            }
                            else
                            {
                                // 블록 생성
                                if (curLength < goalLength)
                                {
                                    maps[i][j] = 0;
                                    curLength++;
                                    // 몬스터 & 상자 생성
                                    if (i != length - 1)
                                    {
                                        if (GameFuction.GetRandFlag(0.05f))
                                            maps[i][j - 1] = -3; // 상자 몬스터 생성
                                        else if (GameFuction.GetRandFlag(0.075f))
                                            maps[i][j - 1] = -4; // 상자 생성
                                    }
                                }
                                else if (isCreating)
                                    isCreating = false;
                            }
                        }
                    }
                    break;
                case 1: // 얼티밋 & 마나석 방
                    bool isManaRoom = GameFuction.GetRandFlag(manaRoomPercent);
                    bool isMysticRoom = false;
                    if (dungeon_1_type >= 4)
                        isMysticRoom = GameFuction.GetRandFlag(mysticRoomPercent);

                    for (int i = 7; i < length - 1; i++)
                    {
                        for (int j = 1; j < height - 2; j++)
                        { 
                            if (j % 3 == 0)
                                maps[i][j] = 0;
                            else
                            {
                                if (isMysticRoom) maps[i][j] = 12;
                                else if (isManaRoom) maps[i][j] = 9;
                                else maps[i][j] = 8;
                            }
                        }
                    }
                    break;
                case 2: // 랜덤 보물 방
                    int tresureNum = Random.Range(1, maxTreasureNum + 1);
                    // 전체 블록 생성
                    for (int i = 6; i < length - 1; i++)
                    {
                        for (int j = 1; j < height - 2; j++)
                        {
                            block_hps[i][j] = new Block(dungeon_1_type);
                            maps[i][j] = 10;
                        }
                    }
                    // 보물 공간 생성
                    randVecs.Clear();
                    for (int i = 0; i < tresureNum; i++)
                    {
                        int pos_x = Random.Range(7, length - 3);
                        int pos_y = Random.Range(3, height - 3);
                        bool isEqual = false;

                        // 중복 검사
                        for (int j = 0; j < randVecs.Count; j++)
                        {
                            if (Vector2.Distance(randVecs[j], new Vector2(pos_x, pos_y)) < 4f)
                            {
                                isEqual = true;
                                break;
                            }
                        }

                        if (isEqual) { i--; continue; }
                        randVecs.Add(new Vector2Int(pos_x, pos_y));

                        // 빈 공간 생성
                        for (int j = -1; j <= 1; j++)
                        {
                            for (int k = -2; k <= 1; k++)
                            {
                                // 3 X 3 공간 설정
                                if (k == 1) maps[pos_x + j][pos_y + k] = 0;
                                else maps[pos_x + j][pos_y + k] = 2;
                                if (GameFuction.GetRandFlag(treasurePercent))
                                    maps[pos_x][pos_y] = -5; // 보물 생성
                                else
                                    maps[pos_x][pos_y] = -6; // 횃불 생성
                            }
                        }
                    }
                    break;
                case 3: // 포탈
                    maps[8][height - 3] = 11; // 포탈 문
                    break;
            }

            _block_hps = block_hps;
            return maps;
        }

        do
        {
            isPlusFloor = GameFuction.GetRandFlag(1f - f_count * 0.25f);

            // 로비 제작
            currentPos = startVec + Vector3Int.down * all_height * f_count;

            for (int i = 0; i < lobby_width; i++)
            {
                switch (i)
                {
                    case 0: Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, out currentPos); break;
                    case 2: 
                    case 3: Dungeon_1_CreateLine(currentPos, new int[] { 0, 2, 2, 2, 2, 2, 2, 3, 1, 1, 1, 0 }, out currentPos); break;
                    case 5: // 횃불 생성
                        D_1_Torch = ObjectPool.GetObject<D_1_Torch>(26, ObjectPool.instance.dungeon_1_objectTr, currentPos + new Vector3(-0.5f, -(all_height - 5), 0));
                        D_1_Torch.type = dungeon_1_type;
                        Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 3, 1, 1, 1, 0 }, out currentPos); break;
                    case 6: // 탈출 문 생성
                        Vector3Int pos = currentPos + new Vector3Int(0, -(all_height - 5), 0);
                        MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_1_DecoX64Tiles[7 + 7 * dungeon_1_type]);
                        eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_1_objectTr, currentPos + new Vector3(0.5f, -(all_height - 5), 0));
                        eventBlock.eventMainType = 10;
                        break;
                    default: Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 3, 1, 1, 1, 0 }, out currentPos); break;
                }
            }
            Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 4, 7, 1, 1, 0 }, out currentPos);
            Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 5, 7, 1, 0 }, out currentPos);
            Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 6, 1, 0 }, out currentPos);

            // 복도 제작
            int roomCount = GameFuction.GetRandFlag(new float[] { 0, 0, 0, 0.6f, 0.3f, 0.1f });
            int portal = Random.Range(0, roomCount);

            for (int i = 0; i < roomCount; i++)
            {
                int roomType = GameFuction.GetRandFlag(roomPercents);
                if (i == portal) // 포탈 방 생성
                    roomType = 3;
                Block[][] blocks;
                int[][] maps = SetMapData(roomType, out blocks);

                for (int j = 0; j < room_width; j++)
                {
                    Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 3, 1, 0 }, out currentPos);
                    // 복도 슬라임 생성
                    if (GameFuction.GetRandFlag(0.05f))
                    {
                        d_1_HoleSlime = ObjectPool.GetObject<D_1_HoleSlime>(22, ObjectPool.instance.dungeon_1_objectTr, currentPos + Vector3Int.down * 6);
                        d_1_HoleSlime.type = 6 + type;
                    }

                    if (j == room_width / 2)
                    {
                        // 문 생성 
                        Vector3Int pos = currentPos + new Vector3Int(0, -(all_height - 3), 0);
                        MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_1_DecoX64Tiles[9 + 7 * dungeon_1_type]);
                        eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_1_objectTr, currentPos + new Vector3(0.5f, -(all_height - 3), 0));
                        RoomData roomData = new RoomData(roomType, eventBlock, pos, maps, blocks);
                        eventBlock.eventMainType = 11;
                        eventBlock.eventSubType = roomType;
                        eventBlock.portal_vec = pos;
                        eventBlock.roomData = roomData;
                    }
                }
            }

            Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 3, 1, 0 }, out currentPos);
            Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, out currentPos);

            f_count++;
        } while (isPlusFloor);

        // 마지막 보스 층 제작
        currentPos = startVec + Vector3Int.down * all_height * f_count;

        for (int i = 0; i < lobby_width; i++)
        {
            switch (i)
            {
                case 0: Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, out currentPos); break;
                case 2:
                case 3: Dungeon_1_CreateLine(currentPos, new int[] { 0, 2, 2, 2, 2, 2, 2, 3, 1, 1, 1, 0 }, out currentPos); break;
                case 5: // 횃불 생성
                    D_1_Torch = ObjectPool.GetObject<D_1_Torch>(26, ObjectPool.instance.dungeon_1_objectTr, currentPos + new Vector3(-0.5f, -(all_height - 5), 0));
                    D_1_Torch.type = dungeon_1_type;
                    Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 3, 1, 1, 1, 0 }, out currentPos); break;
                case 6: // 탈출 문 생성
                    Vector3Int pos = currentPos + new Vector3Int(0, -(all_height - 5), 0);
                    MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_1_DecoX64Tiles[7 + 7 * dungeon_1_type]);
                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_1_objectTr, currentPos + new Vector3(0.5f, -(all_height - 5), 0));
                    eventBlock.eventMainType = 10;
                    break;
                default: Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 3, 1, 1, 1, 0 }, out currentPos); break;
            }
        }
        Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 4, 7, 1, 1, 0 }, out currentPos);
        Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 5, 7, 1, 0 }, out currentPos);
        Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 6, 1, 0 }, out currentPos);

        // 복도 제작
        for (int j = 0; j < room_width * 2; j++)
        {
            Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 3, 1, 0 }, out currentPos);

            if (j == room_width)
            {
                if (GameFuction.GetRandFlag(0.5f))
                {
                    // [레드 다이아 골렘] 보스 생성
                    D_1_Midboss = ObjectPool.GetObject<D_1_Midboss>(28, ObjectPool.instance.dungeon_1_objectTr, currentPos + Vector3Int.down * 6);
                    D_1_Midboss.type = 6 + dungeon_1_type;
                }
                else
                {
                    // [악마 슬라임 골렘] 보스 생성
                    D_1_Boss = ObjectPool.GetObject<D_1_Boss>(32, ObjectPool.instance.dungeon_1_objectTr, currentPos + Vector3Int.down * 6);
                    D_1_Boss.type = 6 + dungeon_1_type;
                }
            }
            else if (j == room_width * 2 - 4)
            {
                // 다시 1층으로 이동하는 문
                Vector3Int pos = currentPos + new Vector3Int(0, -(all_height - 3), 0);
                MapData.instance.GetTileMap(pos, 5).SetTile(pos, MapData.instance.dungeon_1_DecoX64Tiles[7 + 7 * dungeon_1_type]);
                eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_1_objectTr, currentPos + new Vector3(0.5f, -(all_height - 3), 0));
                eventBlock.eventMainType = 15;
            }
        }

        Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 3, 1, 0 }, out currentPos);
        Dungeon_1_CreateLine(currentPos, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, out currentPos);
    }

    // 던전 본체 한 라인 생성 함수 
    void Dungeon_1_CreateLine(Vector3Int point, int[] blockTypes, out Vector3Int currentPos)
    {
        for (int i = 0; i < blockTypes.Length; i++)
        {
            Vector3Int tempPos = point + Vector3Int.down * i;
            switch (blockTypes[i])
            {
                case -6: // 횃불
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    D_1_Torch = ObjectPool.GetObject<D_1_Torch>(26, ObjectPool.instance.dungeon_1_room_objectTr, tempPos);
                    D_1_Torch.type = dungeon_1_type;
                    break;
                case -5: // 보물 상자
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    ancientTresure = ObjectPool.GetObject<AncientTresure>(25, ObjectPool.instance.dungeon_1_room_objectTr, tempPos);
                    ancientTresure.boxType = 6 + dungeon_1_type;
                    break;
                case -4: // 고대 상자
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    d_1_box = ObjectPool.GetObject<AncientBox>(24, ObjectPool.instance.dungeon_1_room_objectTr, tempPos);
                    d_1_box.boxType = 6 + dungeon_1_type;
                    break;
                case -3: // 미믹 슬라임
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    D_1_MimicSlime = ObjectPool.GetObject<D_1_MimicSlime>(23, ObjectPool.instance.dungeon_1_room_objectTr, tempPos);
                    D_1_MimicSlime.type = 6 + dungeon_1_type;
                    break;
                case -2: // 검 슬라임 (Room)
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    d_1_HoleSlime = ObjectPool.GetObject<D_1_HoleSlime>(22, ObjectPool.instance.dungeon_1_room_objectTr, tempPos);
                    d_1_HoleSlime.type = 6 + dungeon_1_type;
                    break;
                case -1: // 검 슬라임 (Hole)
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    d_1_HoleSlime = ObjectPool.GetObject<D_1_HoleSlime>(22, ObjectPool.instance.dungeon_1_objectTr, tempPos);
                    d_1_HoleSlime.type = 6 + dungeon_1_type;
                    break;
                case 0: // 테두리 블록
                    MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[0 + 3 * dungeon_1_type]);
                    break;
                case 1: // 바닥 블록
                    MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[2 + 3 * dungeon_1_type]);
                    break;
                case 2: // 위쪽 벽지
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    break;
                case 3: // 아래쪽 벽지
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[3 + 5 * dungeon_1_type]);
                    break;
                case 4: // 기울기 start 벽지
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[4 + 5 * dungeon_1_type]);
                    break;
                case 5: // 기울기 ing 벽지
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[5 + 5 * dungeon_1_type]);
                    break;
                case 6: // 기울기 end 벽지
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[6 + 5 * dungeon_1_type]);
                    break;
                case 7: // 기울기 아래쪽 벽지
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[7 + 5 * dungeon_1_type]);
                    break;
                case 8: // 얼터밋 광물
                    int ultimate = Jem.GetUItimateCode(dungeon_1_type + 6);
                    MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.blockTiles[dungeon_1_type + 7]);
                    if (i == blockTypes.Length - 3)
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[3 + 5 * dungeon_1_type]);
                    else
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    MapData.instance.GetTileMap(tempPos, 1).SetTile(tempPos, MapData.instance.jemTiles[ultimate]);
                    MapData.instance.GetTileMap(tempPos, 10).SetTile(tempPos, MapData.instance.jemLightTiles[SaveScript.jems[ultimate].quality - 1]);
                    break;
                case 9: // 마나석 광물
                    MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.blockTiles[dungeon_1_type + 7]);
                    if (i == blockTypes.Length - 3)
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[3 + 5 * dungeon_1_type]);
                    else
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    MapData.instance.GetTileMap(tempPos, 1).SetTile(tempPos, MapData.instance.mana_oreTile);
                    MapData.instance.GetTileMap(tempPos, 10).SetTile(tempPos, MapData.instance.jemLightTiles[MapData.instance.jemLightTiles.Count - 3]);
                    break;
                case 10: // 특수 블록
                    MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.dungeon_1_specialBlockTiles[dungeon_1_type]);
                    if (i == blockTypes.Length - 3)
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[3 + 5 * dungeon_1_type]);
                    else
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    break;
                case 11: // 다음 층으로 이동하는 포탈
                    MapData.instance.GetTileMap(tempPos, 5).SetTile(tempPos, MapData.instance.dungeon_1_DecoX64Tiles[7 + 7 * dungeon_1_type]);
                    MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[3 + 5 * dungeon_1_type]);
                    EventBlock data = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_1_room_objectTr, tempPos + Vector3.right * 0.5f);
                    data.eventMainType = 13;
                    data.eventSubType = PlayerScript.instance.d_1_currentFloor;
                    break;
                case 12: // 미스틱 광물
                    int mystic = Jem.GetMysticCode(dungeon_1_type + 6);
                    MapData.instance.GetTileMap(tempPos, 0).SetTile(tempPos, MapData.instance.blockTiles[dungeon_1_type + 7]);
                    if (i == blockTypes.Length - 3)
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_DecoX32Tiles[3 + 5 * dungeon_1_type]);
                    else
                        MapData.instance.GetTileMap(tempPos, 4).SetTile(tempPos, MapData.instance.dungeon_1_blockTiles[1 + 3 * dungeon_1_type]);
                    MapData.instance.GetTileMap(tempPos, 1).SetTile(tempPos, MapData.instance.jemTiles[mystic]);
                    MapData.instance.GetTileMap(tempPos, 10).SetTile(tempPos, MapData.instance.jemLightTiles[SaveScript.jems[mystic].quality - 1]);
                    break;
            }
        }
        currentPos = point + Vector3Int.right;
    }

    public void Dungeon_1_CreateRoom(RoomData roomData)
    {
        Vector3Int startVec = dungeon_1_room_startVec; // 왼쪽 기준점
        Vector3Int currentPos = startVec; // 현재 위치
        Vector3Int doorPos = dungeon_1_room_startVec + new Vector3Int(3, -11, 0);

        // 맵 생성
        for (int i = 0; i < roomData.maps.Length; i++)
            Dungeon_1_CreateLine(currentPos, roomData.maps[i], out currentPos);

        // 문 생성 
        MapData.instance.GetTileMap(doorPos, 5).SetTile(doorPos, MapData.instance.dungeon_1_DecoX64Tiles[9 + 7 * dungeon_1_type]);
        eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.dungeon_1_room_objectTr, doorPos + Vector3.right * 0.5f);
        eventBlock.eventMainType = 12;
    }

    public void Dungeon_1_DeleteRoom()
    {
        // 타일맵 정리
        Vector3Int pos;

        for (int i = 0; i < dungeon_1_room_widths[0] * 2; i++)
        {
            for (int j = 0; j < dungeon_1_room_height; j++)
            {
                pos = dungeon_1_room_startVec + new Vector3Int(i, -j, 0);
                for (int k = 0; k < 11; k++)
                    MapData.instance.GetTileMap(pos, k).SetTile(pos, null);
            }
        }

        // 몬스터 및 NPC, 이벤트 블럭 정리
        Transform[] datas = ObjectPool.instance.dungeon_1_room_objectTr.GetComponentsInChildren<Transform>();
        for (int i = 1; i < datas.Length; i++)
        {
            if (datas[i].tag.Equals("EventBlock")) ObjectPool.ReturnObject<EventBlock>(12, datas[i].transform.GetComponent<EventBlock>());
            else if (datas[i].tag.Equals("D_1_HoleSlime")) ObjectPool.ReturnObject<D_1_HoleSlime>(22, datas[i].transform.GetComponent<D_1_HoleSlime>());
            else if (datas[i].tag.Equals("D_1_MimicSlime")) ObjectPool.ReturnObject<D_1_MimicSlime>(23, datas[i].transform.GetComponent<D_1_MimicSlime>());
            else if (datas[i].tag.Equals("UniqueBox")) ObjectPool.ReturnObject<AncientBox>(24, datas[i].transform.GetComponent<AncientBox>());
            else if (datas[i].tag.Equals("AncientTresure")) ObjectPool.ReturnObject<AncientTresure>(25, datas[i].transform.GetComponent<AncientTresure>());
            else if (datas[i].tag.Equals("D_1_Torch")) ObjectPool.ReturnObject<D_1_Torch>(26, datas[i].transform.GetComponentInParent<D_1_Torch>());
            else if (datas[i].tag.Equals("ItemObject")) ObjectPool.ReturnObject<ItemObject>(4, datas[i].transform.GetComponentInParent<ItemObject>());
        }

        datas = ObjectPool.instance.objectTr.GetComponentsInChildren<Transform>();
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].tag.Equals("JemObject")) ObjectPool.ReturnObject<JemObject>(0, datas[i].transform.GetComponentInParent<JemObject>());
            else if (datas[i].tag.Equals("ItemObject")) ObjectPool.ReturnObject<ItemObject>(4, datas[i].transform.GetComponentInParent<ItemObject>());
            else if (datas[i].tag.Equals("DropPet")) Destroy(datas[i].gameObject);
            else if (datas[i].tag.Equals("ManaOre")) ObjectPool.ReturnObject<ManaOre>(19, datas[i].transform.GetComponentInParent<ManaOre>());
            else if (datas[i].tag.Equals("CardObject")) ObjectPool.ReturnObject<CardObject>(20, datas[i].transform.GetComponentInParent<CardObject>());
        }
    }

    public void DeleteDungeon_0()
    {
        // 타일맵 정리
        for (int i = 100; i < 300; i++)
        {
            for (int j = 200; j > 100; j--)
            {
                Vector3Int data = new Vector3Int(i, j, 0);
                for (int k = 0; k < 11; k++)
                    MapData.instance.GetTileMap(data, k).SetTile(data, null);
            }
        }

        // 몬스터 및 NPC, 이벤트 블럭 정리
        Transform[] datas = ObjectPool.instance.dungeon_0_objectTr.GetComponentsInChildren<Transform>();
        for (int i = 1; i < datas.Length; i++)
        {
            if (datas[i].transform.tag.Equals("NormalBox")) ObjectPool.ReturnObject<NormalBox>(2, datas[i].transform.GetComponent<NormalBox>());
            else if (datas[i].tag.Equals("SpecialBox")) ObjectPool.ReturnObject<RuinBox>(3, datas[i].transform.GetComponent<RuinBox>());
            else if (datas[i].tag.Equals("Dungeon_0_NPC")) ObjectPool.ReturnObject<Dungeon_0_NPC>(5, datas[i].transform.GetComponent<Dungeon_0_NPC>());
            else if (datas[i].tag.Equals("SoilSlime")) ObjectPool.ReturnObject<SoilSlime>(6, datas[i].transform.GetComponent<SoilSlime>());
            else if (datas[i].tag.Equals("SpeedSlime")) ObjectPool.ReturnObject<SpeedSlime>(9, datas[i].transform.GetComponent<SpeedSlime>());
            else if (datas[i].tag.Equals("BigSlime")) ObjectPool.ReturnObject<BigSlime>(10, datas[i].transform.GetComponent<BigSlime>());
            else if (datas[i].tag.Equals("MushroomSlime")) ObjectPool.ReturnObject<MushroomSlime>(11, datas[i].transform.GetComponent<MushroomSlime>());
            else if (datas[i].tag.Equals("EventBlock") && !datas[i].parent.tag.Equals("Dungeon_0_NPC")) ObjectPool.ReturnObject<EventBlock>(12, datas[i].transform.GetComponent<EventBlock>());
        }
    }

    public void DeleteDungeon_1()
    {
        // 타일맵 정리
        for (int i = 400; i < 500; i++)
        {
            for (int j = 200; j > 100; j--)
            {
                Vector3Int data = new Vector3Int(i, j, 0);
                for (int k = 0; k < 11; k++)
                    MapData.instance.GetTileMap(data, k).SetTile(data, null);
            }
        }

        // 몬스터 및 NPC, 이벤트 블럭 정리
        Transform[] datas = ObjectPool.instance.dungeon_1_objectTr.GetComponentsInChildren<Transform>();
        for (int i = 1; i < datas.Length; i++)
        {
            if (datas[i].tag.Equals("EventBlock")) ObjectPool.ReturnObject<EventBlock>(12, datas[i].transform.GetComponent<EventBlock>());
            else if (datas[i].tag.Equals("D_1_HoleSlime")) ObjectPool.ReturnObject<D_1_HoleSlime>(22, datas[i].transform.GetComponent<D_1_HoleSlime>());
            else if (datas[i].tag.Equals("D_1_Torch")) ObjectPool.ReturnObject<D_1_Torch>(26, datas[i].transform.GetComponent<D_1_Torch>());
            else if (datas[i].tag.Equals("D_1_Midboss")) ObjectPool.ReturnObject<D_1_Midboss>(28, datas[i].transform.GetComponent<D_1_Midboss>());
            else if (datas[i].tag.Equals("D_1_Midboss_Skill0")) ObjectPool.ReturnObject<D_1_Midboss_Skill0>(29, datas[i].transform.GetComponent<D_1_Midboss_Skill0>());
            else if (datas[i].tag.Equals("D_1_Midboss_Skill1")) ObjectPool.ReturnObject<D_1_Midboss_Skill1>(30, datas[i].transform.GetComponent<D_1_Midboss_Skill1>());
            else if (datas[i].tag.Equals("D_1_Boss")) ObjectPool.ReturnObject<D_1_Boss>(32, datas[i].transform.GetComponent<D_1_Boss>());
        }
    }

    public void CreateKingSlimeRoom(Vector3Int _pos, int type, bool isLeft)
    {
        // 입구 관련 변수
        Vector3Int render_startVec = kingSlime_startVec + Vector3Int.right * 15 * type; // 랜더링 시작점
        Vector3Int startPos;

        if (isLeft)
            startPos = _pos + Vector3Int.left * 15;
        else
            startPos = _pos;

        // 던전 블럭 하나를 랜더링 하는 함수
        void RenderBlock(Vector3Int render_pos, Vector3Int real_pos)
        {
            int blockIndex_kingSlime = MapData.instance.kingSlimeBlocks.IndexOf(MapData.instance.GetTileMap(render_pos, 0).GetTile(render_pos) as Tile);
            int blockIndex_map = MapData.instance.mapBlockTiles.IndexOf(MapData.instance.GetTileMap(render_pos, 0).GetTile(render_pos) as Tile);
            int wallIndex = MapData.instance.kingSlimeBlocks.IndexOf(MapData.instance.GetTileMap(render_pos, 4).GetTile(render_pos) as Tile);

            RaycastHit2D[] hit = Physics2D.BoxCastAll(new Vector2(real_pos.x - 0.01f, real_pos.y), Vector2.one * 0.5f, 0f, Vector2.right, 0.02f, 1024);

            // 블러 안의 몬스터 및 박스 제거
            MapData.instance.DeleteObject(real_pos);

            // 해당 위치에 특수 던전이 있을 경우 -> PASS
            if (MapData.instance.dungeon_0_DecoX32Tiles.IndexOf(MapData.instance.GetTileMap(real_pos, 4).GetTile(real_pos) as Tile) != -1 ||
                MapData.instance.dungeon_1_blockTiles.IndexOf(MapData.instance.GetTileMap(real_pos, 4).GetTile(real_pos) as Tile) != -1)
                return;

            // 해당 위치에 건축물이 있을 경우 (ex. 일반 던전, 일반 벽) -> 덮어쓰기
            if (MapData.instance.GetTileMap(real_pos, 0).GetTile(real_pos) != null || MapData.instance.GetTileMap(real_pos, 2).GetTile(real_pos) != null)
            {
                MapData.instance.GetTileMap(real_pos, 0).SetTile(real_pos, null);
                MapData.instance.GetTileMap(real_pos, 2).SetTile(real_pos, null);
                MapData.instance.GetTileMap(real_pos, 4).SetTile(real_pos, null);
            }

            // 랜더링 시작
            if (blockIndex_kingSlime > -1)
            {
                MapData.instance.GetTileMap(real_pos, 0).SetTile(real_pos, MapData.instance.kingSlimeBlocks[blockIndex_kingSlime]);
            }
            if (blockIndex_map > -1)
            {
                MapData.instance.GetTileMap(real_pos, 0).SetTile(real_pos, MapData.instance.mapBlockTiles[blockIndex_map]);
            }
            if (wallIndex > -1)
            {
                MapData.instance.GetTileMap(real_pos, 4).SetTile(real_pos, MapData.instance.kingSlimeBlocks[wallIndex]);
                MapData.instance.GetTileMap(real_pos, 1).SetTile(real_pos, null);
                MapData.instance.GetTileMap(real_pos, 10).SetTile(real_pos, null);
                if (MapData.instance.GetTileMap(real_pos, 0).GetTile(real_pos) == null)
                    MapData.instance.GetTileMap(real_pos, 2).SetTile(real_pos, MapData.instance.brokenTiles[0]);
            }
        }

        Debug.Log("여왕 슬라임 던전 생성");

        // 랜더링
        for (int i = 0; i < kingSlimeRoom_width; i++)
        {
            for (int j = 0; j < kingSlimeRoom_height; j++)
            {
                if (Mathf.Abs(startPos.x + i) < MapData.instance.mapEndLength)
                    RenderBlock(render_startVec + new Vector3Int(i, -j, 0), startPos + new Vector3Int(i, -j, 0));
            }
        }
        // 여왕 생성
        KingSlime data;
        if(isLeft)
            data = ObjectPool.GetObject<KingSlime>(16, ObjectPool.instance.objectTr, _pos + new Vector3(-8.5f, -3f, 0));
        else
            data = ObjectPool.GetObject<KingSlime>(16, ObjectPool.instance.objectTr, _pos + new Vector3(6.5f, -3f, 0));
        data.type = type;
    }
}
