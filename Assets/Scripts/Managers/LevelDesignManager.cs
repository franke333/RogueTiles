using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyEntry
{
    public int difficulty;
    public GridUnit enemy;
    bool appearOutside, appearInside;
}

public enum TileType
{
    Empty,
    Dirt,
    Wall,
    Cobblestone
}

public static class TileTypeExtensions
{
    private static HashSet<TileType> _isWalkable =
        new HashSet<TileType>() { TileType.Dirt, TileType.Cobblestone };
    public static bool IsWalkable(this TileType tileType) => _isWalkable.Contains(tileType);
}

public class LevelDesignManager : SingletonClass<LevelDesignManager>
{

    public Tile DirtTilePrefab;
    public Tile WallTilePrefab;
    public Tile CobblestoneTilePrefab;
    public Tile WaterTilePrefab;

    public Tile GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.Dirt:
                return DirtTilePrefab;
            case TileType.Wall:
                return WallTilePrefab;
            case TileType.Cobblestone:
                return CobblestoneTilePrefab;
            default:
                return WaterTilePrefab;
        }
    }

    public Tile GetTilePrefab(byte index) => GetTilePrefab((TileType)index);


}
