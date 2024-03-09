using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapData : MonoBehaviour
{
    public static MapData instance;

    public GameObject[] tileObjects;
    public GameObject[] tilePrefabs;

    public List<List<Tilemap>> blockTileMap, brokenTileMap, jemTileMap, jemLightTileMap, blockDetectTileMap, secondBlockTileMap,
        decoWallTileMap, decoTileMap_behind_nonBlock, decoTileMap_behind_block, decoTileMap_forward_nonBlock, decoTileMap_forward_block;
    public List<Tile> blockTiles, brokenTiles, backgroundBlockTiles;
    public List<Tile> jemTiles, jemLightTiles;
    public List<Tile> mapBlockTiles;
    public List<Tile> dungeon_0_blockTiles, dungeon_0_DecoX32Tiles, dungeon_0_DecoX64Tiles;
    public List<Tile> dungeon_1_specialBlockTiles, dungeon_1_blockTiles, dungeon_1_DecoX32Tiles, dungeon_1_DecoX64Tiles;
    public List<Tile> blockTiles_6th, blockTiles_10th;
    public List<Tile> kingSlimeBlocks;
    public Tile blockDetectTile;
    public Tile mana_oreTile, growth_oreTile;

    public int mapWidth, mapHeight; // 플레이어로부터 맵이 생성되기 시작하는 길이 
    public int mapEndLength; // 맵의 최대 가로
    static public int[] depth = { -10, -30, -60, -95, -137, -180, -235, -285, -350, -412, -483, -560, -623, -689, -754, -819, - 900 }; // 맵의 깊이 (블록 변경 지점) 
    static public int chunk_size = 100; // Chunk의 크기 (가로)

    GameObject object_temp;
    Order order;
    EventBlock eventBlock;
    Tilemap tilemap;
    TileObject[] tileObjectArr;
    List<Tilemap> tilemapList = new List<Tilemap>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        blockTileMap = new List<List<Tilemap>>();
        brokenTileMap = new List<List<Tilemap>>();
        jemTileMap = new List<List<Tilemap>>();
        jemLightTileMap = new List<List<Tilemap>>();
        blockDetectTileMap = new List<List<Tilemap>>();
        secondBlockTileMap = new List<List<Tilemap>>();
        decoWallTileMap = new List<List<Tilemap>>();
        decoTileMap_behind_nonBlock = new List<List<Tilemap>>();
        decoTileMap_behind_block = new List<List<Tilemap>>();
        decoTileMap_forward_nonBlock = new List<List<Tilemap>>();
        decoTileMap_forward_block = new List<List<Tilemap>>();

        for (int i = 0; i < tileObjects.Length; i++)
        {
            blockTileMap.Add(new List<Tilemap>());
            brokenTileMap.Add(new List<Tilemap>());
            jemTileMap.Add(new List<Tilemap>());
            jemLightTileMap.Add(new List<Tilemap>());
            blockDetectTileMap.Add(new List<Tilemap>());
            secondBlockTileMap.Add(new List<Tilemap>());
            decoWallTileMap.Add(new List<Tilemap>());
            decoTileMap_behind_nonBlock.Add(new List<Tilemap>());
            decoTileMap_behind_block.Add(new List<Tilemap>());
            decoTileMap_forward_nonBlock.Add(new List<Tilemap>());
            decoTileMap_forward_block.Add(new List<Tilemap>());
        }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene")
        {
            Tilemap[] tilemaps = tileObjects[1].GetComponentsInChildren<Tilemap>();
            blockTileMap[1].Add(tilemaps[0]);
            jemTileMap[1].Add(tilemaps[1]);
            brokenTileMap[1].Add(tilemaps[2]);
            blockDetectTileMap[1].Add(tilemaps[3]);
            decoWallTileMap[1].Add(tilemaps[4]);
            decoTileMap_behind_nonBlock[1].Add(tilemaps[5]);
            decoTileMap_behind_block[1].Add(tilemaps[6]);
            secondBlockTileMap[1].Add(tilemaps[7]);
            decoTileMap_forward_nonBlock[1].Add(tilemaps[8]);
            decoTileMap_forward_block[1].Add(tilemaps[9]);
            jemLightTileMap[1].Add(tilemaps[10]);
        }
        jemLightTileMap[0].Add(tileObjects[0].GetComponentsInChildren<Tilemap>()[10]);

        blockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Blocks"));
        brokenTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/BrokenBlocks"));
        backgroundBlockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/BackgroundBlocks"));
        jemTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/JemBlocks"));
        jemLightTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/JemLights"));
        mapBlockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/MapBlocks"));
        dungeon_0_blockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_0_Block"));
        dungeon_0_DecoX32Tiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_0_DecoX32"));
        dungeon_0_DecoX64Tiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_0_DecoX64"));
        dungeon_1_specialBlockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_1_SpecialBlock"));
        dungeon_1_blockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_1_Block"));
        dungeon_1_DecoX32Tiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_1_DecoX32"));
        dungeon_1_DecoX64Tiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Dungeons/Dungeon_1_DecoX64"));
        blockTiles_6th.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Blocks_6th"));
        blockTiles_10th.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Blocks_10th"));
        kingSlimeBlocks.AddRange(Resources.LoadAll<Tile>("Images/Blocks/KingSlimeBlocks"));

        blockDetectTile = ScriptableObject.CreateInstance<Tile>();
        blockDetectTile.sprite = Resources.Load<Sprite>("Images/Blocks/BlockDetect");

        mapWidth = Mathf.CeilToInt(CameraCtrl.instance.cameraWidth) + (int)CameraCtrl.instance.cameraWidth;
        mapHeight = Mathf.CeilToInt(CameraCtrl.instance.cameraHeight) + (int)CameraCtrl.instance.cameraHeight;
        mapEndLength = 805;
    }

    /// <summary>
    /// 'point' 위치에 존재하는 모든 오브젝트를 삭제하는 함수
    /// </summary>
    /// <param name="point">Tilemap 위치</param>
    public void DeleteObject(Vector3Int point)
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(new Vector2(point.x - 0.01f, point.y), Vector2.one * 0.5f, 0f, Vector2.right, 0.02f);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform.tag.Equals("SoilSlime")) ObjectPool.ReturnObject<SoilSlime>(6, hit[i].transform.GetComponent<SoilSlime>());
            else if (hit[i].transform.tag.Equals("ADSlime")) ObjectPool.ReturnObject<ADSlime>(7, hit[i].transform.GetComponent<ADSlime>());
            else if (hit[i].transform.tag.Equals("NormalBox")) ObjectPool.ReturnObject<NormalBox>(2, hit[i].transform.GetComponent<NormalBox>());
            else if (hit[i].transform.tag.Equals("EventBlock"))
            {
                eventBlock = hit[i].transform.GetComponent<EventBlock>();
                if (eventBlock.CheckIsOre())
                {
                    CashEquipmentCtrl.instance.eventBlocks.Remove(eventBlock);
                    ObjectPool.ReturnObject<EventBlock>(12, eventBlock);
                }
            }
        }
    }

    /// <summary>
    /// 땅 깊이에 따른 공통 Block 타입을 리턴. 가공이 필요하다.
    /// </summary>
    /// <param name="_depth">땅 깊이</param>
    /// <returns></returns>
    public int GetBlockTileType(int _depth)
    {
        int type;

        if (_depth >= CameraCtrl.instance.cameraHeight) type = 0;
        else if (_depth >= depth[0]) type = 1;
        else if (_depth >= depth[1]) type = 2;
        else if (_depth >= depth[2]) type = 3;
        else if (_depth >= depth[3]) type = 4;
        else if (_depth >= depth[4]) type = 5;
        else if (_depth >= depth[5]) type = 6;
        else if (_depth >= depth[6]) type = -1;
        else if (_depth >= depth[7]) type = 7;
        else if (_depth >= depth[8]) type = 8;
        else if (_depth >= depth[9]) type = 9;
        else if (_depth >= depth[10]) type = 10;
        else if (_depth >= depth[11]) type = -2;
        else if (_depth >= depth[12]) type = 11;
        else if (_depth >= depth[13]) type = 12;
        else if (_depth >= depth[14]) type = 13;
        else if (_depth >= depth[15]) type = 14;
        else if (_depth >= depth[16]) type = 15;
        else type = 0;

        return type;
    }

    
    /// <summary>
    /// 땅 깊이에 따른 배경색 타일 반환
    /// </summary>
    /// <param name="_depth">땅 깊이</param>
    /// <returns></returns>
    public Tile GetBackgroundTile(int _depth)
    {
        Tile tile = null;
        int type = GetBlockTileType(_depth);
        if (type == 0) return null;

        if (type == -1)
        {
            // 6 ~ 7층 사이
            int gap = (depth[5] - depth[6]) / (blockTiles_6th.Count - 2);
            for (int i = 0; i < blockTiles_6th.Count - 2; i++)
            {
                if (_depth >= depth[5] - gap * i)
                {
                    tile = blockTiles_6th[i + 2];
                    break;
                }
                if (i == blockTiles_6th.Count - 3)
                    tile = blockTiles_6th[blockTiles_6th.Count - 1];
            }
        }
        else if (type == -2)
        {
            // 10 ~ 11층 사이
            int gap = (depth[10] - depth[11]) / (blockTiles_10th.Count - 3);
            for (int i = 0; i < blockTiles_10th.Count - 3; i++)
            {
                if (_depth >= depth[10] - gap * i)
                {
                    tile = blockTiles_10th[i + 3];
                    break;
                }
                if (i == blockTiles_10th.Count - 4)
                    tile = blockTiles_10th[blockTiles_10th.Count - 1];
            }
        }
        else tile = backgroundBlockTiles[type - 1];

        return tile;
    }

    public Tile GetBlockTile(int _depth)
    {
        Tile tile = null;
        int type = GetBlockTileType(_depth);

        if(type >= 0)
            tile = blockTiles[type];

        return tile;
    }

    /// <summary>
    /// 0 = block, 1 = jem, 2 = broken, 3 = detect, 4 = deco_Wall, 5 = deco_b_nb, 6 = deco_b_b, 7 = secondBlock, 8 = deco_f_nb, 9 = deco_f_b, 10 = light
    /// </summary>
    public Tilemap GetTileMap(Vector3Int _pos, int type)
    {
        int floor; // 땅의 깊이 (층)

        if (_pos.y >= CameraCtrl.instance.cameraHeight) floor = 0;
        else if (_pos.y >= depth[0]) floor = 1;
        else if (_pos.y >= depth[1]) floor = 2;
        else if (_pos.y >= depth[2]) floor = 3;
        else if (_pos.y >= depth[3]) floor = 4;
        else if (_pos.y >= depth[4]) floor = 5;
        else if (_pos.y >= depth[5]) floor = 6;
        else if (_pos.y >= depth[6]) floor = 7;
        else if (_pos.y >= depth[7]) floor = 8;
        else if (_pos.y >= depth[8]) floor = 9;
        else if (_pos.y >= depth[9]) floor = 10;
        else if (_pos.y >= depth[10]) floor = 11;
        else if (_pos.y >= depth[11]) floor = 12;
        else if (_pos.y >= depth[12]) floor = 13;
        else if (_pos.y >= depth[13]) floor = 14;
        else if (_pos.y >= depth[14]) floor = 15;
        else if (_pos.y >= depth[15]) floor = 16;
        else if (_pos.y >= depth[16]) floor = 17;
        else floor = 0;

        return FindTileMap(_pos.x, floor, type);
    }

    public int GetStage(int _depth)
    {
        int stage;

        if (_depth >= depth[0]) stage = 0;
        else if (_depth >= depth[1]) stage = 1;
        else if (_depth >= depth[2]) stage = 2;
        else if (_depth >= depth[3]) stage = 3;
        else if (_depth >= depth[4]) stage = 4;
        else if (_depth >= depth[5]) stage = 5;
        else if (_depth >= depth[6]) stage = -1;
        else if (_depth >= depth[7]) stage = 6;
        else if (_depth >= depth[8]) stage = 7;
        else if (_depth >= depth[9]) stage = 8;
        else if (_depth >= depth[10]) stage = 9;
        else if (_depth >= depth[11]) stage = -1;
        else if (_depth >= depth[12]) stage = 10;
        else if (_depth >= depth[13]) stage = 11;
        else if (_depth >= depth[14]) stage = 12;
        else if (_depth >= depth[15]) stage = 13;
        else if (_depth >= depth[16]) stage = 14;
        else stage = -1;

        return stage;
    }

    public Tilemap FindTileMap(int x, int _floor, int type)
    {
        // x 위치에 해당하는 Chunk 번호
        int chunkNum = x / chunk_size;
        tilemap = null;
        switch (type)
        {
            case 0: tilemapList = blockTileMap[_floor]; break;
            case 1: tilemapList = jemTileMap[_floor]; break;
            case 2: tilemapList = brokenTileMap[_floor]; break;
            case 3: tilemapList = blockDetectTileMap[_floor]; break;
            case 4: tilemapList = decoWallTileMap[_floor]; break;
            case 5: tilemapList = decoTileMap_behind_nonBlock[_floor]; break;
            case 6: tilemapList = decoTileMap_behind_block[_floor]; break;
            case 7: tilemapList = secondBlockTileMap[_floor]; break;
            case 8: tilemapList = decoTileMap_forward_nonBlock[_floor]; break;
            case 9: tilemapList = decoTileMap_forward_block[_floor]; break;
            case 10: tilemapList = jemLightTileMap[_floor]; break;
        }

        // 탐색
        if(_floor != 0)
        {
            // N-way Chunk
            for (int i = 0; i < tilemapList.Count; i++)
            {
                if(chunkNum == tilemapList[i].GetComponent<Order>().order)
                {
                    // Chunk 발견
                    tilemap = tilemapList[i];
                    break;
                }
            }
        }
        else
        {
            // One-way Chunk
            tilemap = tileObjects[_floor].GetComponentsInChildren<Tilemap>()[type];
        }

        // 탐색 실패
        if(tilemap == null)
        {
            // 새로운 Chunk 생성
            tileObjectArr = tileObjects[_floor].GetComponentsInChildren<TileObject>();
            for (int i = 0; i < tilePrefabs.Length; i++)
            {
                object_temp = Instantiate(tilePrefabs[i], tileObjectArr[i].transform);
                tilemap = object_temp.GetComponent<Tilemap>();
                order = object_temp.GetComponent<Order>();
                order.order = chunkNum;

                switch (i)
                {
                    case 0: blockTileMap[_floor].Add(tilemap); break;
                    case 1: jemTileMap[_floor].Add(tilemap); break;
                    case 2: brokenTileMap[_floor].Add(tilemap); break;
                    case 3: blockDetectTileMap[_floor].Add(tilemap); break;
                    case 4: decoWallTileMap[_floor].Add(tilemap); break;
                    case 5: decoTileMap_behind_nonBlock[_floor].Add(tilemap); break;
                    case 6: decoTileMap_behind_block[_floor].Add(tilemap); break;
                    case 7: secondBlockTileMap[_floor].Add(tilemap); break;
                    case 8: decoTileMap_forward_nonBlock[_floor].Add(tilemap); break;
                    case 9: decoTileMap_forward_block[_floor].Add(tilemap); break;
                    case 10: jemLightTileMap[_floor].Add(tilemap); break;
                }
            }
        }

        return tilemap;
    }
}
