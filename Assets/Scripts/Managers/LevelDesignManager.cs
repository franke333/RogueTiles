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

public static class TileTypeExtensions
{
    private static HashSet<TileType> _isWalkable =
        new HashSet<TileType>() { TileType.Dirt, TileType.Cobblestone };
    public static bool IsWalkable(this TileType tileType) => _isWalkable.Contains(tileType);
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
