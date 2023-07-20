using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Interface for a implementation of a tile
/// </summary>
public interface ITile
{
    /// <summary>
    /// Room this tile is part of
    /// </summary>
    public Room Room { get; set; }

    public int x { get; set; }
    public int y { get; set; }

    /// <summary>
    /// Get the object on this tile (null if there is none)
    /// </summary>
    public GridObject GetObject { get; }

    /// <summary>
    /// Initialize the tile
    /// </summary>
    /// <param name="isOdd">Apply a checker pattern</param>
    /// <param name="x">the x coord of the tile</param>
    /// <param name="y">the y coord of the tile</param>
    /// <param name="room">room which the tile is part of</param>
    /// <returns>The refence of this tile</returns>
    public ITile Init(bool isOdd, int x, int y, Room room);

    /// <summary>
    /// Tint the base color of the tile
    /// </summary>
    /// <param name="color">color of the tint</param>
    /// <param name="by"> a value between 0 and 1 of how much should the basecolor be tinted</param>
    public void TintBaseColor(Color color, float by);

    public bool Visible { get; set; }

    public bool IsWall { get; }

    public bool IsWalkable { get; }

    /// <summary>
    /// Whether the tile is highlighted as in range
    /// </summary>
    public bool InRange { get; set; }

    /// <summary>
    /// Whether the tile is highlighted as selected (hovered over by cursor)
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    /// Sets the object on this tile
    /// </summary>
    /// <param name="obj">Object that will occupy this tile</param>
    public void Occupy(GridObject obj);

    /// <summary>
    /// Refresh color of the tile
    /// </summary>
    public void Refresh();

    /// <summary>
    /// Get the color of the tile
    /// </summary>
    /// <returns>Color of the tile</returns>
    public Color GetColor();
}

public static class ITileExtensions
{
    /// <summary>
    /// Get the manhattan distance between two tiles
    /// </summary>
    /// <param name="a">first tile</param>
    /// <param name="b">second tile</param>
    /// <returns>the manhattan distance</returns>
    public static int ManhattanDistance(this ITile a, ITile b)
    {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y);
    }
}
