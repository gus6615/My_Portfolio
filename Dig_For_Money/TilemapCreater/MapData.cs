using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Linq;

public class MapData : MonoBehaviour
{
    private const int CHUNK_SIZE = 100;

    // 맵의 깊이 (블록 변경 지점) 
    private readonly int[] Depth = { -10, -30, -60, -95, -137, -180, -235, -285, -350, -412, -483, -560, -623, -689, -754, -819, -900, -965 };

    [SerializeField] private FloorData[] floorDatas;
    [SerializeField] private TilemapChunk[] chunkPrefabs;

    public List<Tile> blockTiles, brokenTiles, backgroundBlockTiles;
    public List<Tile> jemTiles, jemLightTiles;
    public List<Tile> mapBlockTiles;
    public Tile blockDetectTile;

    // Start is called before the first frame update
    void Start()
    {
        InitLoadData();

        foreach (FloorData floorData in floorDatas)
            floorData.Init();

        // 0 층은 1개의 청크만 존재하기 때문에 초기 할당 필요
        TilemapChunk[] chunks = floorDatas[0].GetComponentsInChildren<TilemapChunk>();
        for (int i = 0; i < chunks.Length; i++)
            floorDatas[0].TileChunks[i].Add(chunks[i]);

        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            // 튜토 전용 맵 데이터 처리
            chunks = floorDatas[1].GetComponentsInChildren<TilemapChunk>();
            for (int i = 0; i < chunks.Length; i++)
                floorDatas[1].TileChunks[i].Add(chunks[i]);
        }
    }

    private void InitLoadData()
    {
        blockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/Blocks"));
        brokenTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/BrokenBlocks"));
        backgroundBlockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/BackgroundBlocks"));
        jemTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/JemBlocks"));
        jemLightTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/JemLights"));
        mapBlockTiles.AddRange(Resources.LoadAll<Tile>("Images/Blocks/MapBlocks"));

        blockDetectTile = ScriptableObject.CreateInstance<Tile>();
        blockDetectTile.sprite = Resources.Load<Sprite>("Images/Blocks/BlockDetect");
    }

    /// <summary>
    /// 땅 깊이에 따른 공통 Block 타입을 리턴하는 함수
    /// </summary>
    public BlockTileType GetBlockTileType(int _depth)
    {
        BlockTileType blockType = 0;

        for (int i = 0; i < Depth.Length; i++)
        {
            if (_depth >= Depth[i])
            {
                blockType = (BlockTileType)i;
                break;
            }
        }
     
        return blockType;
    }

    public int GetStage(int _depth)
    {
        int stage = 0;

        for (int i = 0; i < Depth.Length; i++)
        {
            if (_depth >= Depth[i])
            {
                stage = i;
                break;
            }
        }

        return stage;
    }

    /// <summary>
    /// 땅 깊이에 따른 배경색 타일 반환
    /// </summary>
    public Tile GetBackgroundTile(int _depth)
    {
        BlockTileType blockType = GetBlockTileType(_depth);
        return backgroundBlockTiles[blockType];
    }

    public Tile GetBlockTile(int _depth)
    {
        BlockTileType blockType = GetBlockTileType(_depth);
        return blockTiles[blockType];
    }

    public Tilemap GetTileMap(Vector3Int _pos, TilemapType type)
    {
        int floor = GetStage(_pos.y); // 땅의 깊이 (층)
        return FindTileMap(_pos.x, floor, type);
    }

    private Tilemap FindTileMap(int x, int floor, TilemapType tilemapType)
    {
        // x 위치에 해당하는 Chunk 번호
        int type = (int)tilemapType;
        int chunkNum = x / CHUNK_SIZE;
        Tilemap tilemap = null;
        List<TilemapChunk> chunkList = floorDatas[floor].TileChunks[type];

        // 청크 탐색 시작
        if(floor != 0)
        {
            // N-way Chunk (지상 제외 모든 층)
            foreach (var chunk in chunkList)
            {
                if (chunkNum == chunk.ID)
                {
                    // Chunk 발견
                    tilemap = chunk.Tilemap;
                    break;
                }
            }
        }
        else
        {
            // One-way Chunk (지상 층 전용: 항상 하나가 존재)
            tilemap = chunkList[0].Tilemap;
        }

        // 탐색 실패 => 새로운 Chunk 생성
        if (tilemap == null)
        {
            TilemapChunk newChunk = Instantiate(chunkPrefabs[type], floorDatas[floor].TilemapTrList[type]).GetComponent<TilemapChunk>();
            newChunk.ID = chunkNum;
            tilemap = newChunk.Tilemap;
            floorDatas[floor].TileChunks[type].Add(newChunk);
        }

        if (tilemap == null)
        {
            Debug.LogError($"Tilemap is null!");
        }

        return tilemap;
    }
}
