using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    Empty = 0,
    Dirt = 1,



    Wall = 10,
    Cobblestone = 11
}

public class LevelDesignManager : SingletonClass<LevelDesignManager>
{
    public Tile tilePrefab;


    public Tile GetTilePrefab(byte index)
    {
        //TODO
        return tilePrefab;
    }
}
