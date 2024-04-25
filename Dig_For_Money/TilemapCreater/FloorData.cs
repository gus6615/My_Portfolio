using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorData : MonoBehaviour
{
    [SerializeField] private List<Transform> tilemapTrList;
    public List<Transform> TilemapTrList => tilemapTrList;

    public List<List<TilemapChunk>> TileChunks;

    public void Init()
    {
        TileChunks = new List<List<TilemapChunk>>();
        for (int i = 0; i < 11;  i++)
            TileChunks.Add(new List<TilemapChunk>());
    }
}
