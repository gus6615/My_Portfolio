using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreater : MonoBehaviour
{
    private const float JEM_FADE_LOOP_SPEED = 2.0f;
    private const int JEM_CREATE_MINNUM = 2;
    private const int JEM_CREATE_MAXNUM = 8;
    private const int MAP_MAX_HEIGHT = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (!SaveScript.saveData.isTutorial)
            InitMap();

        StartCoroutine(LoopJemLightFade());
    }

    // Update is called once per frame
    void Update()
    {
        if (!SaveScript.saveData.isTutorial)
        {
            ExpendMap();
        }
    }

    /// <summary>
    /// 맵 입장 시 카메라 영역에 보이는 만큼 맵을 초기화하는 함수
    /// </summary>
    public void InitMap()
    {
        // 카메라 영역 만큼 꽉찬 맵 생성
        int maxWidth = MapManager.Data.mapWidth;
        int maxHeight = MapManager.Data.mapHeight;

        for (int i = -maxWidth; i <= maxWidth; i++)
        {
            for (int j = -maxHeight; j < maxHeight; j++)
            {
                if (j <= CameraManager.instance.cameraHeight)
                {
                    Vector3Int pos = MapManager.Data.GetTileMap(new Vector3Int(i, j, 0), 0).WorldToCell(new Vector3(i, j, 0));
                    MapManager.Data.GetTileMap(pos, TilemapType.Block).SetTile(pos, MapManager.Data.blockTiles[1]);
                    MapManager.Data.GetTileMap(pos, TilemapType.Background).SetTile(pos, MapManager.Data.backgroundBlockTiles[0]);

                    // 광물 생성
                    if (Mathf.Abs(pos.x) % 5 == 0 && Mathf.Abs(pos.y) % 5 == 0)
                        CreateJem(pos);

                    if (pos.y == CameraManager.instance.cameraHeight)
                        MapManager.Data.GetTileMap(pos, TilemapType.Block).SetTile(pos, MapManager.Data.blockTiles[0]);
                }
            }
        }

        // 맵 중앙에 플레이어가 놓일 수 있는 빈 공간(3 * 3) 생성
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3Int pos = MapManager.Data.GetTileMap(new Vector3Int(i, j, 0), 0).WorldToCell(new Vector3(i, j, 0));
                MapManager.Data.GetTileMap(pos, TilemapType.Block).SetTile(pos, null);
                MapManager.Data.GetTileMap(pos, TilemapType.Jem).SetTile(pos, null);
                MapManager.Data.GetTileMap(pos, TilemapType.JemLight).SetTile(pos, null);
                MapManager.Data.GetTileMap(pos, TilemapType.Broken).SetTile(pos, MapManager.Data.brokenTiles[0]);
                MapManager.Data.GetTileMap(pos, TilemapType.Background).SetTile(pos, MapManager.Data.backgroundBlockTiles[0]);
            }
        }
    }

    /// <summary>
    /// 플레이어가 정수 좌표만큼 이동할 경우 맵을 확장하는 함수
    /// </summary>
    public void ExpendMap()
    {
        Vector2Int currentPlayerPos = new Vector2Int(Mathf.RoundToInt(PlayerScript.instance.transform.position.x), Mathf.RoundToInt(PlayerScript.instance.transform.position.y));
        
        if(currentPlayerPos != PlayerScript.instance.playerPos)
        {
            // (좌로 한칸 이동 감지)
            if (currentPlayerPos.x < PlayerScript.instance.playerPos.x)
            {
                for (int i = currentPlayerPos.y - MapManager.Data.mapHeight; i < currentPlayerPos.y + MapManager.Data.mapHeight; i++)
                {
                    Vector3Int currentPos = new Vector3Int(currentPlayerPos.x - MapManager.Data.mapWidth, i, 0);
                    BlockTileType blockTileType = MapManager.Data.GetBlockTileType(i);
                    CreateMapBlock(currentPos, blockTileType);
                }
            }
            // (우로 한칸 이동)
            else if(currentPlayerPos.x > PlayerScript.instance.playerPos.x)
            {
                for (int i = currentPlayerPos.y - MapManager.Data.mapHeight; i < currentPlayerPos.y + MapManager.Data.mapHeight; i++)
                {
                    Vector3Int currentPos = new Vector3Int(currentPlayerPos.x + MapManager.Data.mapWidth, i, 0);
                    BlockTileType blockTileType = MapManager.Data.GetBlockTileType(i);
                    CreateMapBlock(currentPos, blockTileType);
                }
            }
            // (하로 한칸 이동)
            else if(currentPlayerPos.y < PlayerScript.instance.playerPos.y)
            {
                for (int i = currentPlayerPos.x - MapManager.Data.mapWidth; i <= currentPlayerPos.x + MapManager.Data.mapWidth; i++)
                {
                    Vector3Int currentPos = new Vector3Int(i, currentPlayerPos.y - MapManager.Data.mapHeight + 1, 0);
                    BlockTileType blockTileType = MapManager.Data.GetBlockTileType(i);
                    CreateMapBlock(currentPos, blockTileType);
                }
            }
            // (상으로 한칸 이동)
            else if (currentPlayerPos.y > PlayerScript.instance.playerPos.y)
            {
                for (int i = currentPlayerPos.x - MapManager.Data.mapWidth; i <= currentPlayerPos.x + MapManager.Data.mapWidth; i++)
                {
                    Vector3Int currentPos = new Vector3Int(i, currentPlayerPos.y + MapManager.Data.mapHeight - 1, 0);
                    BlockTileType blockTileType = MapManager.Data.GetBlockTileType(i);
                    CreateMapBlock(currentPos, blockTileType);
                }
            }

            PlayerScript.instance.playerPos = currentPlayerPos;
        }
    }

    private void CreateMapBlock(Vector3Int currentPos, BlockTileType blockType)
    {
        if (currentPos.y > MAP_MAX_HEIGHT)
            return; // 지상은 생성하지 않음

        if (MapManager.Data.GetTileMap(currentPos, TilemapType.Block).GetTile(currentPos) == null && MapManager.Data.GetTileMap(currentPos, TilemapType.Broken).GetTile(currentPos) == null)
        {
            MapManager.Data.GetTileMap(currentPos, TilemapType.Background).SetTile(currentPos, MapManager.Data.GetBackgroundTile(currentPos.y));
            MapManager.Data.GetTileMap(currentPos, TilemapType.Block).SetTile(currentPos, MapManager.Data.GetBlockTile(currentPos.y));

            Vector3Int randPos = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2), 0);

            // 폐광 생성
            if (currentPos.x % 4 == 0 && currentPos.y % 4 == 0 && GameFuction.GetRandFlag(SaveScript.createMinePercent))
            {
                DungeonCreater.instance.CreateMine(currentPos, true);
            }

            // 광물 생성
            if (currentPos.x % 5 == 0 && currentPos.y % 5 == 0)
            {
                CreateJem(currentPos + randPos);
            }
        }
    }

    /// <summary>
    /// 광물을 생성하는 함수
    /// </summary>
    public void CreateJem(Vector3Int pos)
    {
        int stage = MapManager.Data.GetStage(pos.y);
        if (stage == -1) 
            return; // 지상은 생성하지 않음

        int createJemIndex = -1;
        int jemMinIndex = 0;
        int jemMaxIndex = 0;
        bool isCreateJem = false;

        // 생성 가능 광물 설정 (이전 층 ~ 현재 층에 해당하는 광물만)
        for (int i = 0; i < stage; i++)
            jemMinIndex += SaveScript.stageItemNums[i];
        jemMaxIndex = jemMinIndex + SaveScript.stageItemNums[stage];

        // 생성 여부 설정
        for (int i = jemMinIndex; i < jemMaxIndex; i++)
        {
            float createPercent = SaveScript.jems[i].createPercent;

            // 생성 여부 시도 (ex. createPercent = 0.3 이라면 30% 확률로 아래 수행)
            if (GameFuction.GetRandFlag(createPercent))
            {
                isCreateJem = true;
                createJemIndex = SaveScript.jems[i].itemCode;
                break;
            }
        }

        // 맵에 광물 배치
        if (isCreateJem)
        {
            Vector3Int randVec = Vector3Int.zero;
            int num = Random.Range(JEM_CREATE_MINNUM, JEM_CREATE_MAXNUM + 1);

            for (int i = 0; i < num; i++)
            {
                if ((pos + randVec).y < CameraManager.instance.cameraHeight)
                {
                    if (MapManager.Data.GetStage((pos + randVec).y) == -1) return;
                    int blockIndex = MapManager.Data.blockTiles.IndexOf(MapManager.Data.GetTileMap(pos + randVec, TilemapType.Block).GetTile(pos + randVec) as Tile);
                    int brokenIndex = MapManager.Data.brokenTiles.IndexOf(MapManager.Data.GetTileMap(pos + randVec, TilemapType.Broken).GetTile(pos + randVec) as Tile);

                    // 파괴된 벽이 아니고, 광물이 없는 경우에만 생성
                    if (brokenIndex == -1 && MapManager.Data.GetTileMap(pos + randVec, TilemapType.Jem).GetTile(pos + randVec) == null)
                    {
                        MapManager.Data.GetTileMap(pos + randVec, 1).SetTile(pos + randVec, MapManager.Data.jemTiles[createJemIndex]);
                        if (SaveScript.jems[createJemIndex].quality > JemQuality.None)
                            MapManager.Data.GetTileMap(pos + randVec, TilemapType.JemLight).SetTile(pos + randVec, MapManager.Data.jemLightTiles[(int)SaveScript.jems[createJemIndex].quality]);
                    }
                }
                
                // 다음 생성할 위치를 위한 랜덤 벡터 설정
                switch (Random.Range(0, 4))
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

    /// <summary>
    /// 맵에 존재하는 모든 광물이 일정 주기로 빛나게 하기 위한 코루틴 함수
    /// </summary>
    IEnumerator LoopJemLightFade()
    {
        Color color = new Color(1f, 1f, 1f, 0f);

        // 서서히 밝아짐
        while (color.a < 1f)
        {
            color.a += Time.deltaTime * JEM_FADE_LOOP_SPEED;
            SetJemLight(color);
            yield return null;
        }

        color.a = 1f;
        SetJemLight(color);

        // 서서히 어두워짐
        while (color.a > 1f)
        {
            color.a -= Time.deltaTime * JEM_FADE_LOOP_SPEED;
            SetJemLight(color);
            yield return null;
        }

        color.a = 0f;
        SetJemLight(color);

        // 반복
        StartCoroutine(LoopJemLightFade());
    }

    private void SetJemLight(Color color)
    {
        foreach (var tileMap in MapManager.Data.jemLightTileMap)
            tileMap.color = color;
    }
}
