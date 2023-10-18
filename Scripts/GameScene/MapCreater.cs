using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreater : MonoBehaviour
{
    public static MapCreater instance;

    public bool is_6th, is_10th;
    private bool isJemLightFadeOn;
    private float jemLightFadeSpeed;
    public int minMoveX, maxMoveX; // 왼, 오른쪽으로 간 최대 x 값
    private int left5thHeight, right5thHeight, left6thHeight, right6thHeight;
    private int left6thLength, right6thLength, right6thLength_cur, left6thLength_cur;
    private int left10thHeight, right10thHeight, left11thHeight, right11thHeight;
    private int left11thLength, right11thLength, right11thLength_cur, left11thLength_cur;

    List<Tilemap> tilemapList;
    ReinforceTree tree;
    Golem golem;
    EventBlock eventBlock;
    Color color = Color.white; // JemLight Color

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        jemLightFadeSpeed = 2f;

        if (!SaveScript.saveData.isTutorial)
            InitMap();
    }

    // Update is called once per frame
    void Update()
    {
        JemLightFade();

        if (!SaveScript.saveData.isTutorial) // 튜토리얼 X
        {
            if(!PlayerScript.instance.isDungeon_0_On && !PlayerScript.instance.isDungeon_1_On && !PlayerScript.instance.isEventMap_On)
            {
                ExpendMap();
            }
        }

        if(!is_6th && PlayerScript.instance.transform.position.y <= MapData.depth[5] - 10f)
        {
            is_6th = true;
            StartCoroutine(BlindScript.instance.switchBGM(6, 1f, 1f));
        }

        if (!is_10th && PlayerScript.instance.transform.position.y <= MapData.depth[10] - 10f)
        {
            is_10th = true;
            StartCoroutine(BlindScript.instance.switchBGM(8, 1f, 1f));
        }
    }

    // 맵 초기화
    public void InitMap()
    {
        Vector3Int pos;
        minMoveX = -MapData.instance.mapWidth;
        maxMoveX = MapData.instance.mapWidth;

        if(BlindScript.instance.spawnType == 0)
        {
            // 첫번째 스폰 생성
            for (int i = -MapData.instance.mapWidth; i <= MapData.instance.mapWidth; i++)
            {
                for (int j = -MapData.instance.mapHeight; j < MapData.instance.mapHeight; j++)
                {
                    if (j <= CameraCtrl.instance.cameraHeight)
                    {
                        pos = MapData.instance.GetTileMap(new Vector3Int(i, j, 0), 0).WorldToCell(new Vector3(i, j, 0));
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[1]);
                        MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.backgroundBlockTiles[0]);
                        if (Mathf.Abs(pos.x) % 5 == 0 && Mathf.Abs(pos.y) % 5 == 0)
                            CreateJem(pos);

                        if (pos.y == CameraCtrl.instance.cameraHeight)
                            MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                    }
                }
            }

            // 빈 공간 생성
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    pos = MapData.instance.GetTileMap(new Vector3Int(i, j, 0), 0).WorldToCell(new Vector3(i, j, 0));
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                    MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                    MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                    MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
                    MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.backgroundBlockTiles[0]);
                }
            }
        }
        else
        {
            // 2, 3번째 스폰 생성
            int depth = 0;
            switch (BlindScript.instance.spawnType)
            {
                case 1: depth = MapData.depth[6] + 15; break;
                case 2: depth = MapData.depth[11] + 15; break;
            }

            for (int i = -MapData.instance.mapWidth; i <= MapData.instance.mapWidth; i++)
            {
                for (int j = -MapData.instance.mapHeight + depth; j < MapData.instance.mapHeight + depth; j++)
                {
                    pos = MapData.instance.GetTileMap(new Vector3Int(i, j, 0), 0).WorldToCell(new Vector3(i, j, 0));
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, null);
                    MapData.instance.GetTileMap(pos, 2).SetTile(pos, MapData.instance.brokenTiles[0]);
                    MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                }
            }
        }

        // 초기 5층 바닥 생성
        int currentHeight = Random.Range(1, 10);
        left5thHeight = currentHeight;
        for (int i = -MapData.instance.mapWidth; i <= MapData.instance.mapWidth; i++)
        {
            for (int j = 0; j < currentHeight; j++)
            {
                pos = new Vector3Int(i, MapData.depth[5] - j, 0);
                MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                if (j == currentHeight - 1)
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles_6th[1]);
                else
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[6]);
                MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
            }

            currentHeight += Random.Range(-1, 2);
            if (currentHeight < 1) currentHeight = 1;
            else if (currentHeight > 10) currentHeight = 10;
        }
        right5thHeight = currentHeight;

        // 초기 6층 땅 생성
        currentHeight = Random.Range(1, 5);
        left6thHeight = currentHeight;
        left6thLength = Random.Range(1, 5);
        for (int i = -MapData.instance.mapWidth; i <= MapData.instance.mapWidth; i++)
        {
            for (int j = 0; j < currentHeight; j++)
            {
                pos = new Vector3Int(i, MapData.depth[6] + j, 0);
                MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                if (j == currentHeight - 1)
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles_6th[0]);
                else
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[7]);
            }

            // 강화석 나무 생성
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0]))
            {
                tree = ObjectPool.GetObject<ReinforceTree>(15, ObjectPool.instance.objectTr, new Vector3(i, MapData.depth[6] + 8, 0));
                tree.species = 0;
                if (Random.Range(1, 3) == 1) tree.transform.localScale = new Vector3(-1.5f, 1.5f, 1f);
                if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
                {
                    tree.type = 2;
                    golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(i + Random.Range(-3, 4), MapData.depth[6] + 8, 0));
                    golem.type = 2;
                    golem.species = 0;
                }
                else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                    tree.type = 1;
                else
                    tree.type = 0;
            }

            // 강화석 골렘 생성
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0] * 0.5f))
            {
                golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(i, MapData.depth[6] + 8, 0));
                golem.species = 0;
                if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
                    golem.type = 2;
                else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                    golem.type = 1;
                else
                    golem.type = 0;
            }

            if (left6thLength_cur > left6thLength)
            {
                currentHeight += Random.Range(-1, 2);
                left6thLength = Random.Range(1, 5);
                left6thLength_cur = 0;
            }
            else left6thLength_cur++;
            if (currentHeight < 1) currentHeight = 1;
            else if (currentHeight > 4) currentHeight = 4;
        }
        right6thHeight = currentHeight;

        // 초기 10층 바닥 생성
        currentHeight = Random.Range(1, 10);
        left10thHeight = currentHeight;
        for (int i = -MapData.instance.mapWidth; i <= MapData.instance.mapWidth; i++)
        {
            for (int j = 0; j < currentHeight; j++)
            {
                pos = new Vector3Int(i, MapData.depth[10] - j, 0);
                MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                if (j == currentHeight - 1)
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles_10th[2]);
                else
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[10]);
                MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
            }

            currentHeight += Random.Range(-1, 2);
            if (currentHeight < 1) currentHeight = 1;
            else if (currentHeight > 10) currentHeight = 10;
        }
        right10thHeight = currentHeight;

        // 초기 11층 땅 생성
        currentHeight = Random.Range(1, 5);
        left11thHeight = currentHeight;
        left11thLength = Random.Range(1, 5);
        for (int i = -MapData.instance.mapWidth; i <= MapData.instance.mapWidth; i++)
        {
            for (int j = 0; j < currentHeight + 2; j++)
            {
                pos = new Vector3Int(i, MapData.depth[11] + j, 0);
                MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                if (j == currentHeight + 1)
                    MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles_10th[0]);
                else
                {
                    if (j < currentHeight - 2) MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[11]);
                    else MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles_10th[1]); 
                }
            }

            // 마나석 나무 생성
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0]))
            {
                tree = ObjectPool.GetObject<ReinforceTree>(15, ObjectPool.instance.objectTr, new Vector3(i, MapData.depth[11] + 8, 0));
                tree.species = 1;
                if (Random.Range(1, 3) == 1) tree.transform.localScale = new Vector3(-1.5f, 1.5f, 1f);
                if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
                {
                    tree.type = 2;
                    golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(i + Random.Range(-3, 4), MapData.depth[11] + 8, 0));
                    golem.type = 2;
                    golem.species = 1;
                }
                else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                    tree.type = 1;
                else
                    tree.type = 0;
            }

            // 마나석 골렘 생성
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0] * 0.5f))
            {
                golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(i, MapData.depth[11] + 8, 0));
                golem.species = 1;
                if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
                    golem.type = 2;
                else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                    golem.type = 1;
                else
                    golem.type = 0;
            }

            if (left11thLength_cur > left11thLength)
            {
                currentHeight += Random.Range(-1, 2);
                left11thLength = Random.Range(1, 5);
                left11thLength_cur = 0;
            }
            else left11thLength_cur++;
            if (currentHeight < 1) currentHeight = 1;
            else if (currentHeight > 3) currentHeight = 3;
        }
        right11thHeight = currentHeight;
    }

    // 맵 확장
    public void ExpendMap()
    {
        Vector2Int data = new Vector2Int(Mathf.RoundToInt(PlayerScript.instance.transform.position.x), Mathf.RoundToInt(PlayerScript.instance.transform.position.y));
        
        if(data != PlayerScript.instance.playerPos)
        {
            Vector3Int pos;
            Vector3Int randPos;
            int blockTileType; // 생성될 블럭의 타입
            int abs_x, abs_y; // pos x, y 좌표의 절대값
            int endPosX; // 현재 플레이어 위치에서 랜더링되는 맵의 끝부분 x좌표 (변화 필요 부분)
            float createKingSlimeRoomPercent = SaveScript.createKingSlimeRoomPercent;
            float createDungeon_0_Percent = SaveScript.createDungeon_0_Percent;
            float createDungeon_1_Percent = SaveScript.createDungeon_1_Percent;
            if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 2)
            {
                createKingSlimeRoomPercent *= 2;
                createDungeon_0_Percent *= 2;
                createDungeon_1_Percent *= 2;
            }

            DungeonCreater.instance.SetResponVecList();

            // (좌)
            if (data.x < PlayerScript.instance.playerPos.x)
            {
                // 5층, 6층 설정
                endPosX = data.x - MapData.instance.mapWidth;
                if (endPosX < minMoveX)
                {
                    Create5thFloor(endPosX, true);
                    Create6thGround(endPosX, true);
                    Create10thFloor(endPosX, true);
                    Create11thGround(endPosX, true);
                    minMoveX = endPosX;
                }

                for (int i = data.y - MapData.instance.mapHeight; i < data.y + MapData.instance.mapHeight; i++)
                {
                    pos = new Vector3Int(data.x - MapData.instance.mapWidth, i, 0);
                    if (pos.y > CameraCtrl.instance.cameraHeight) continue;
                    abs_x = Mathf.Abs(pos.x);
                    abs_y = Mathf.Abs(pos.y);

                    // 검은 벽
                    if (i == CameraCtrl.instance.cameraHeight || pos.x <= -MapData.instance.mapEndLength)
                    {
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                        MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                        MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                    }
                    // 블럭 생성 (blockTile 과 brokenTile이 없어야 성립)
                    else if (MapData.instance.GetTileMap(pos, 0).GetTile(pos) == null && MapData.instance.GetTileMap(pos, 2).GetTile(pos) == null)
                    {
                        blockTileType = MapData.instance.GetBlockTileType(i);
                        MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                        if (blockTileType < 0) continue;
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.GetBlockTile(pos.y));
                        if (blockTileType == 0) continue;

                        // 마지막 벽과 구조물이 겹치지 않게 제한
                        if (pos.y + DungeonCreater.instance.dungeon_0_height > MapData.depth[MapData.depth.Length - 1])
                        {
                            randPos = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2), 0);

                            // 폐광 생성
                            if (abs_x % 4 == 0 && abs_y % 4 == 0 && GameFuction.GetRandFlag(SaveScript.createMinePercent))
                            {
                                DungeonCreater.instance.CreateMine(pos, true);
                            }
                            // 킹슬라임 둥지 생성 & 특수 던전 생성 
                            if (abs_x % (DungeonCreater.instance.dungeon_0_width + 4) == 0 && abs_y % (DungeonCreater.instance.dungeon_0_height + 4) == 0
                                && MapData.instance.GetBlockTileType((pos + randPos).y) > 0)
                            {
                                if (GameFuction.GetRandFlag(createKingSlimeRoomPercent))
                                    DungeonCreater.instance.CreateKingSlimeRoom(pos + randPos, blockTileType - 1, true);
                                else if (blockTileType >= 3 && GameFuction.GetRandFlag(createDungeon_0_Percent))
                                    DungeonCreater.instance.CreateDungeon_Entrance(0, pos + randPos, blockTileType - 3, true);
                                else if (blockTileType >= 7 && GameFuction.GetRandFlag(createDungeon_1_Percent))
                                    DungeonCreater.instance.CreateDungeon_Entrance(1, pos + randPos, blockTileType - 7, true);
                            }
                            // 일반 벽
                            if (abs_x % 5 == 0 && abs_y % 5 == 0)
                            {
                                CreateJem(pos);
                            }
                        }
                    }
                }
            }
            else if(data.x > PlayerScript.instance.playerPos.x)
            {
                endPosX = data.x + MapData.instance.mapWidth;
                if (endPosX > maxMoveX)
                {
                    Create5thFloor(endPosX, false);
                    Create6thGround(endPosX, false);
                    Create10thFloor(endPosX, false);
                    Create11thGround(endPosX, false);
                    maxMoveX = endPosX;
                }

                for (int i = data.y - MapData.instance.mapHeight; i < data.y + MapData.instance.mapHeight; i++)
                {
                    pos = new Vector3Int(data.x + MapData.instance.mapWidth, i, 0);
                    if (pos.y > CameraCtrl.instance.cameraHeight) continue;
                    abs_x = Mathf.Abs(pos.x);
                    abs_y = Mathf.Abs(pos.y);

                    if (i == CameraCtrl.instance.cameraHeight || pos.x >= MapData.instance.mapEndLength)
                    {
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                        MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                        MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                    }
                    else if (MapData.instance.GetTileMap(pos, 0).GetTile(pos) == null && MapData.instance.GetTileMap(pos, 2).GetTile(pos) == null)
                    {
                        blockTileType = MapData.instance.GetBlockTileType(i);
                        MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                        if (blockTileType < 0) continue;
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.GetBlockTile(pos.y));
                        if (blockTileType == 0) continue;

                        if (pos.y + 8 > MapData.depth[MapData.depth.Length - 1])
                        {
                            randPos = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2), 0);

                            if (abs_x % 4 == 0 && abs_y % 4 == 0 && GameFuction.GetRandFlag(SaveScript.createMinePercent))
                            {
                                DungeonCreater.instance.CreateMine(pos, false);
                            }
                            if (abs_x % (DungeonCreater.instance.dungeon_0_width + 4) == 0 && abs_y % (DungeonCreater.instance.dungeon_0_height + 4) == 0
                                && MapData.instance.GetBlockTileType((pos + randPos).y) > 0)
                            {
                                if (GameFuction.GetRandFlag(createKingSlimeRoomPercent))
                                    DungeonCreater.instance.CreateKingSlimeRoom(pos + randPos, blockTileType - 1, false);
                                else if (blockTileType >= 3 && GameFuction.GetRandFlag(createDungeon_0_Percent))
                                    DungeonCreater.instance.CreateDungeon_Entrance(0, pos + randPos, blockTileType - 3, false);
                                else if (blockTileType >= 7 && GameFuction.GetRandFlag(createDungeon_1_Percent))
                                    DungeonCreater.instance.CreateDungeon_Entrance(1, pos + randPos, blockTileType - 7, false);
                            }
                            if (abs_x % 5 == 0 && abs_y % 5 == 0)
                            {
                                CreateJem(pos);
                            }
                        }
                    }
                }
            }

            // 상하 확장
            if(data.y < PlayerScript.instance.playerPos.y)
            {
                for (int i = data.x - MapData.instance.mapWidth; i <= data.x + MapData.instance.mapWidth; i++)
                {
                    pos = new Vector3Int(i, data.y - MapData.instance.mapHeight + 1, 0);
                    if (pos.y > CameraCtrl.instance.cameraHeight) continue;
                    abs_x = Mathf.Abs(pos.x);
                    abs_y = Mathf.Abs(pos.y);

                    if (abs_x >= MapData.instance.mapEndLength)
                    {
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                        MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                        MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                    }
                    else if (MapData.instance.GetTileMap(pos, 0).GetTile(pos) == null && MapData.instance.GetTileMap(pos, 2).GetTile(pos) == null)
                    {
                        blockTileType = MapData.instance.GetBlockTileType(pos.y);
                        MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                        if (blockTileType < 0) continue;
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.GetBlockTile(pos.y));
                        if (blockTileType == 0) continue;

                        if (pos.y + 8 > MapData.depth[MapData.depth.Length - 1])
                        {
                            randPos = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2), 0);

                            if (abs_x % 4 == 0 && abs_y % 4 == 0 && GameFuction.GetRandFlag(SaveScript.createMinePercent))
                            {
                                DungeonCreater.instance.CreateMine(pos, false);
                            }
                            if (abs_x % (DungeonCreater.instance.dungeon_0_width + 4) == 0 && abs_y % (DungeonCreater.instance.dungeon_0_height + 4) == 0
                                && MapData.instance.GetBlockTileType((pos + randPos).y) > 0)
                            {
                                if (GameFuction.GetRandFlag(createKingSlimeRoomPercent))
                                    DungeonCreater.instance.CreateKingSlimeRoom(pos + randPos, blockTileType - 1, false);
                                else if (blockTileType >= 3 && GameFuction.GetRandFlag(createDungeon_0_Percent))
                                    DungeonCreater.instance.CreateDungeon_Entrance(0, pos + randPos, blockTileType - 3, false);
                                else if (blockTileType >= 7 && GameFuction.GetRandFlag(createDungeon_1_Percent))
                                    DungeonCreater.instance.CreateDungeon_Entrance(1, pos + randPos, blockTileType - 7, false);
                            }
                            if (abs_x % 5 == 0 && abs_y % 5 == 0)
                            {
                                CreateJem(pos);
                            }
                        }
                    }
                }
            }
            else if (data.y > PlayerScript.instance.playerPos.y)
            {
                for (int i = data.x - MapData.instance.mapWidth; i <= data.x + MapData.instance.mapWidth; i++)
                {
                    pos = new Vector3Int(i, data.y + MapData.instance.mapHeight - 1, 0);
                    if (pos.y > CameraCtrl.instance.cameraHeight) continue;
                    abs_x = Mathf.Abs(pos.x);
                    abs_y = Mathf.Abs(pos.y);

                    if (pos.y == CameraCtrl.instance.cameraHeight || abs_x >= MapData.instance.mapEndLength)
                    {
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.blockTiles[0]);
                        MapData.instance.GetTileMap(pos, 1).SetTile(pos, null);
                        MapData.instance.GetTileMap(pos, 10).SetTile(pos, null);
                    }
                    else if (MapData.instance.GetTileMap(pos, 0).GetTile(pos) == null && MapData.instance.GetTileMap(pos, 2).GetTile(pos) == null)
                    {
                        MapData.instance.GetTileMap(pos, 4).SetTile(pos, MapData.instance.GetBackgroundTile(pos.y));
                        if (pos.y < MapData.depth[5] && pos.y >= MapData.depth[6] ||
                            pos.y < MapData.depth[10] && pos.y >= MapData.depth[11]) continue;
                        MapData.instance.GetTileMap(pos, 0).SetTile(pos, MapData.instance.GetBlockTile(pos.y));

                        if (abs_x % 5 == 0 && abs_y % 5 == 0)
                            CreateJem(pos);
                    }
                }
            }

            PlayerScript.instance.playerPos = data;
        }
    }

    // 보석 생성
    public void CreateJem(Vector3Int pos)
    {
        int stage = MapData.instance.GetStage(pos.y); // 현재 스테이지
        if (stage == -1) return;
        int createJemIndex = -1;
        int jemMinIndex = 0;
        int jemMaxIndex = 0;
        bool isCreateJem = false;
        float growthOre_createPercent = SaveScript.growthOre_createPercent * SaveScript.stat.growthOre;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 7)
            growthOre_createPercent *= 2;

        if (GameFuction.GetRandFlag(growthOre_createPercent))
        {
            // 성장하는 돌 생성
            Debug.Log("성장하는 돌 등장!");
            isCreateJem = true;
            createJemIndex = -99;
        }
        else if (GameFuction.GetRandFlag(SaveScript.manaOreCreatePercent))
        {
            // 마나석 생성
            isCreateJem = true;
            createJemIndex = -1;
        }
        else
        {
            for (int i = 0; i < stage; i++)
                jemMinIndex += SaveScript.stageItemNums[i];
            jemMaxIndex = jemMinIndex + SaveScript.stageItemNums[stage];

            // 광물 설정 (이전 층 ~ 현재 층만, 얼티밋 제외)
            if (!isCreateJem)
            {
                for (int i = jemMinIndex; i < jemMaxIndex; i++)
                {
                    float percent = SaveScript.jems[i].createPercent;
                    if (SaveScript.jems[i].quality == 5)
                        percent *= SaveScript.stat.ultimateOre;
                    else if (SaveScript.jems[i].quality == 6)
                        percent *= SaveScript.stat.mysticOre;
                    if (SaveScript.jems[i].quality == 5 || SaveScript.jems[i].quality == 6)
                        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 3)
                            percent *= 2;

                    if (GameFuction.GetRandFlag(percent))
                    {
                        if (SaveScript.jems[i].quality == 5)
                            Debug.Log("얼티밋 등장 : " + SaveScript.jems[i].name + " / 확률 : " + percent * 100);
                        if (SaveScript.jems[i].quality == 6)
                            Debug.Log("미스틱 등장 : " + SaveScript.jems[i].name + " / 확률 : " + percent * 100);
                        isCreateJem = true;
                        createJemIndex = SaveScript.jems[i].itemCode;
                        break;
                    }
                }
            }
        }

        if (isCreateJem)
        {
            Vector3Int randVec = Vector3Int.zero;
            int num = Random.Range(SaveScript.minCreateJemNuM, SaveScript.maxCreateJemNum + 1);
            if (createJemIndex == -99)
                num = 1;

            for (int i = 0; i < num; i++)
            {
                if ((pos + randVec).y < CameraCtrl.instance.cameraHeight)
                {
                    if (MapData.instance.GetStage((pos + randVec).y) == -1) return;
                    int blockIndex = MapData.instance.blockTiles.IndexOf(MapData.instance.GetTileMap(pos + randVec, 0).GetTile(pos + randVec) as Tile);
                    int brokenIndex = MapData.instance.brokenTiles.IndexOf(MapData.instance.GetTileMap(pos + randVec, 2).GetTile(pos + randVec) as Tile);
                    int blockIndex_map = MapData.instance.mapBlockTiles.IndexOf(MapData.instance.GetTileMap(pos + randVec, 0).GetTile(pos + randVec) as Tile);
                    int blockIndex_kingSlime = MapData.instance.kingSlimeBlocks.IndexOf(MapData.instance.GetTileMap(pos + randVec, 4).GetTile(pos + randVec) as Tile);
                    int dungeon_0 = MapData.instance.dungeon_0_DecoX32Tiles.IndexOf(MapData.instance.GetTileMap(pos + randVec, 4).GetTile(pos + randVec) as Tile);
                    int dungeon_1 = MapData.instance.dungeon_1_blockTiles.IndexOf(MapData.instance.GetTileMap(pos + randVec, 4).GetTile(pos + randVec) as Tile);

                    if (brokenIndex == -1 && blockIndex_map == -1 && blockIndex_kingSlime == -1 && dungeon_0 == -1 && dungeon_1 == -1)
                    {
                        if (MapData.instance.GetTileMap(pos + randVec, 1).GetTile(pos + randVec) == null && blockIndex != 0)
                        {
                            if (createJemIndex >= 0)
                            {
                                MapData.instance.GetTileMap(pos + randVec, 1).SetTile(pos + randVec, MapData.instance.jemTiles[createJemIndex]);
                                if (SaveScript.jems[createJemIndex].quality > 0)
                                    MapData.instance.GetTileMap(pos + randVec, 10).SetTile(pos + randVec, MapData.instance.jemLightTiles[SaveScript.jems[createJemIndex].quality - 1]);

                                // 광물 탐지기용 EventBlock
                                if (SaveScript.jems[createJemIndex].quality == 5)
                                {
                                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.objectTr, pos + randVec);
                                    eventBlock.eventMainType = EventBlock.ULITMATE_CODE;
                                    CashEquipmentCtrl.instance.eventBlocks.Add(eventBlock);
                                }
                                if (SaveScript.jems[createJemIndex].quality == 6)
                                {
                                    eventBlock = ObjectPool.GetObject<EventBlock>(12, ObjectPool.instance.objectTr, pos + randVec);
                                    eventBlock.eventMainType = EventBlock.MYSTIC_CODE;
                                    CashEquipmentCtrl.instance.eventBlocks.Add(eventBlock);
                                }
                            }
                            else if (createJemIndex == -1)
                            {
                                MapData.instance.GetTileMap(pos + randVec, 1).SetTile(pos + randVec, MapData.instance.mana_oreTile);
                                MapData.instance.GetTileMap(pos + randVec, 10).SetTile(pos + randVec, MapData.instance.jemLightTiles[MapData.instance.jemLightTiles.Count - 3]);
                            }
                            else if (createJemIndex == -99)
                            {
                                MapData.instance.GetTileMap(pos + randVec, 1).SetTile(pos + randVec, MapData.instance.growth_oreTile);
                                MapData.instance.GetTileMap(pos + randVec, 10).SetTile(pos + randVec, MapData.instance.jemLightTiles[MapData.instance.jemLightTiles.Count - 2]);
                            }
                        }
                    }
                }
                
                switch(Random.Range(0, 4))
                {
                    case 0: randVec += Vector3Int.right; break;
                    case 1: randVec += Vector3Int.left; break;
                    case 2: randVec += Vector3Int.up; break;
                    case 3: randVec += Vector3Int.down; break;
                }
            }
        }
        else
            return;
    }

    // JemLightTile 컬러 변화
    public void JemLightFade()
    {
        if (isJemLightFadeOn)
        {
            if (color.a < 1f)
                color.a += Time.deltaTime * jemLightFadeSpeed;
            else
                isJemLightFadeOn = false;
        }
        else
        {
            if (color.a > 0f)
                color.a -= Time.deltaTime * jemLightFadeSpeed;
            else
                isJemLightFadeOn = true;
        }

        for (int i = 0; i < MapData.instance.jemLightTileMap.Count; i++)
        {
            tilemapList = MapData.instance.jemLightTileMap[i];
            for (int j = 0; j < tilemapList.Count; j++)
                MapData.instance.jemLightTileMap[i][j].color = color;
        }
    }

    // pos기준 5층 바닥 한 줄 생성
    private void Create5thFloor(int pos_x, bool isLeft)
    {
        if (Mathf.Abs(pos_x) >= MapData.instance.mapEndLength) 
            return; // 맵의 끝일 경우 생성 안함

        int currentHeight;
        Vector3Int temp;
        if (isLeft) currentHeight = left5thHeight;
        else currentHeight = right5thHeight;

        for (int j = 0; j < currentHeight; j++)
        {
            temp = new Vector3Int(pos_x, MapData.depth[5] - j, 0);
            MapData.instance.DeleteObject(temp);
            if (MapData.instance.GetTileMap(temp, 0).GetTile(temp) != null || MapData.instance.GetTileMap(temp, 2).GetTile(temp) != null)
                continue; // 해당 위치에 건축물 및 땅이 이미 존재

            MapData.instance.GetTileMap(temp, 4).SetTile(temp, MapData.instance.GetBackgroundTile(temp.y));
            if (j == currentHeight - 1)
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles_6th[1]);
            else
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles[6]);
        }

        currentHeight += Random.Range(-1, 2);
        if (currentHeight < 1) currentHeight = 1;
        else if (currentHeight > 10) currentHeight = 10;
        if (isLeft) left5thHeight = currentHeight;
        else right5thHeight = currentHeight;
    }

    // pos기준 6층 땅 한 줄 생성
    private void Create6thGround(int pos_x, bool isLeft)
    {
        if (Mathf.Abs(pos_x) >= MapData.instance.mapEndLength)
            return; // 맵의 끝일 경우 생성 안함

        int currentHeight;
        Vector3Int temp;
        if (isLeft) currentHeight = left6thHeight;
        else currentHeight = right6thHeight;

        for (int j = 0; j < currentHeight; j++)
        {
            temp = new Vector3Int(pos_x, MapData.depth[6] + j, 0);
            MapData.instance.DeleteObject(temp);
            if (MapData.instance.GetTileMap(temp, 0).GetTile(temp) != null || MapData.instance.GetTileMap(temp, 2).GetTile(temp) != null)
                continue; // 해당 위치에 건축물 및 땅이 이미 존재

            MapData.instance.GetTileMap(temp, 4).SetTile(temp, MapData.instance.GetBackgroundTile(temp.y));
            if (j == currentHeight - 1)
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles_6th[0]);
            else
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles[7]);
        }

        // 강화석 나무 생성
        if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0]))
        {
            tree = ObjectPool.GetObject<ReinforceTree>(15, ObjectPool.instance.objectTr, new Vector3(pos_x, MapData.depth[6] + 8, 0));
            tree.species = 0;
            if (Random.Range(1, 3) == 1) tree.transform.localScale = new Vector3(-1.5f, 1.5f, 1f);
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
            {
                tree.type = 2;
                golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(pos_x, MapData.depth[6] + 8, 0));
                golem.type = 2;
                golem.species = 0;
            }
            else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                tree.type = 1;
            else
                tree.type = 0;
        }

        // 골렘 생성
        if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0] * 0.5f))
        {
            golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(pos_x, MapData.depth[6] + 8, 0));
            golem.species = 0;
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
                golem.type = 2;
            else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                golem.type = 1;
            else
                golem.type = 0;
        }

        if (isLeft)
        {
            if (left6thLength_cur > left6thLength)
            {
                currentHeight += Random.Range(-1, 2);
                left6thLength = Random.Range(1, 5);
                left6thLength_cur = 0;
            }
            else left6thLength_cur++;
        }
        else
        {
            if (right6thLength_cur > right6thLength)
            {
                currentHeight += Random.Range(-1, 2);
                right6thLength = Random.Range(1, 5);
                right6thLength_cur = 0;
            }
            else right6thLength_cur++;
        }

        if (currentHeight < 1) currentHeight = 1;
        else if (currentHeight > 4) currentHeight = 4;
        if (isLeft) left6thHeight = currentHeight;
        else right6thHeight = currentHeight;
    }

    // pos기준 10층 바닥 한 줄 생성
    private void Create10thFloor(int pos_x, bool isLeft)
    {
        if (Mathf.Abs(pos_x) >= MapData.instance.mapEndLength)
            return; // 맵의 끝일 경우 생성 안함

        int currentHeight;
        Vector3Int temp;
        if (isLeft) currentHeight = left10thHeight;
        else currentHeight = right10thHeight;

        for (int j = 0; j < currentHeight; j++)
        {
            temp = new Vector3Int(pos_x, MapData.depth[10] - j, 0);
            MapData.instance.DeleteObject(temp);
            if (MapData.instance.GetTileMap(temp, 0).GetTile(temp) != null || MapData.instance.GetTileMap(temp, 2).GetTile(temp) != null)
                continue; // 해당 위치에 건축물 및 땅이 이미 존재

            MapData.instance.GetTileMap(temp, 4).SetTile(temp, MapData.instance.GetBackgroundTile(temp.y));
            if (j == currentHeight - 1)
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles_10th[2]);
            else
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles[10]);
        }

        currentHeight += Random.Range(-1, 2);
        if (currentHeight < 1) currentHeight = 1;
        else if (currentHeight > 10) currentHeight = 10;
        if (isLeft) left10thHeight = currentHeight;
        else right10thHeight = currentHeight;
    }

    // pos기준 11층 땅 한 줄 생성
    private void Create11thGround(int pos_x, bool isLeft)
    {
        if (Mathf.Abs(pos_x) >= MapData.instance.mapEndLength)
            return; // 맵의 끝일 경우 생성 안함

        int currentHeight;
        Vector3Int temp;
        if (isLeft) currentHeight = left11thHeight;
        else currentHeight = right11thHeight;

        for (int j = 0; j < currentHeight + 2; j++)
        {
            temp = new Vector3Int(pos_x, MapData.depth[11] + j, 0);
            MapData.instance.DeleteObject(temp);
            if (MapData.instance.GetTileMap(temp, 0).GetTile(temp) != null || MapData.instance.GetTileMap(temp, 2).GetTile(temp) != null)
                continue; // 해당 위치에 건축물 및 땅이 이미 존재

            MapData.instance.GetTileMap(temp, 4).SetTile(temp, MapData.instance.GetBackgroundTile(temp.y));
            if (j == currentHeight + 1)
                MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles_10th[0]);
            else
            {
                if (j < currentHeight - 2) MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles[11]);
                else MapData.instance.GetTileMap(temp, 0).SetTile(temp, MapData.instance.blockTiles_10th[1]); 
            }
        }

        // 마나석 나무 생성
        if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0]))
        {
            tree = ObjectPool.GetObject<ReinforceTree>(15, ObjectPool.instance.objectTr, new Vector3(pos_x, MapData.depth[11] + 8, 0));
            tree.species = 1;
            if (Random.Range(1, 3) == 1) tree.transform.localScale = new Vector3(-1.5f, 1.5f, 1f);
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
            {
                tree.type = 2;
                golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(pos_x, MapData.depth[11] + 8, 0));
                golem.type = 2;
                golem.species = 1;
            }    
            else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                tree.type = 1;
            else
                tree.type = 0;
        }

        // 마나석 골렘 생성
        if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[0] * 0.5f))
        {
            golem = ObjectPool.GetObject<Golem>(18, ObjectPool.instance.objectTr, new Vector3(pos_x, MapData.depth[11] + 8, 0));
            golem.species = 1;
            if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[2]))
                golem.type = 2;
            else if (GameFuction.GetRandFlag(SaveScript.reinforceTree_createPercents[1]))
                golem.type = 1;
            else
                golem.type = 0;
        }

        if (isLeft)
        {
            if (left11thLength_cur > left11thLength)
            {
                currentHeight += Random.Range(-1, 2);
                left11thLength = Random.Range(1, 5);
                left11thLength_cur = 0;
            }
            else left11thLength_cur++;
        }
        else
        {
            if (right11thLength_cur > right11thLength)
            {
                currentHeight += Random.Range(-1, 2);
                right11thLength = Random.Range(1, 5);
                right11thLength_cur = 0;
            }
            else right11thLength_cur++;
        }

        if (currentHeight < 1) currentHeight = 1;
        else if (currentHeight > 4) currentHeight = 4;
        if (isLeft) left11thHeight = currentHeight;
        else right11thHeight = currentHeight;
    }
}
