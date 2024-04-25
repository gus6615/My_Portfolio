using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChunk : MonoBehaviour
{
    public Tilemap Tilemap;
    public int ID;

    public TilemapChunk(Tilemap tilemap, int iD)
    {
        Tilemap = tilemap;
        ID = iD;
    }
}