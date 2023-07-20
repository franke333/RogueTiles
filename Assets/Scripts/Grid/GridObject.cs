using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// abstract class to represent an object on the grid.
/// Any object that can be placed on the grid should inherit from this class.
/// </summary>
abstract public class GridObject : MonoBehaviour
{
    /// <summary>
    /// Name of the object.
    /// </summary>
    public string Alias;

    [SerializeField]
    protected bool _walkable;
    [SerializeField]
    protected bool _targetable;

    protected ITile _currentTile; 

    /// <summary>
    /// Tile this object is currently on.
    /// </summary>
    public ITile CurrentTile { get => _currentTile; set => _currentTile = value; }

    /// <summary>
    /// The object will take an amount of damage.
    /// </summary>
    /// <param name="dmg">The amount of damage to take</param>
    /// <returns>True if the damage was taken</returns>
    public abstract bool TakeDamage(int dmg);

    /// <summary>
    /// Wheter the object can be walked on by another object.
    /// </summary>
    public bool IsWalkable { get => _walkable; }

    /// <summary>
    /// Wheter the object can be targeted by card or effect.
    /// </summary>
    public bool IsTargetable { get => _targetable; }

    /// <summary>
    /// Toggle rendering of the object.
    /// </summary>
    /// <param name="value">Whether the object should be visible</param>
    public virtual void SetVisible(bool value)
    {
        gameObject.SetActive(value);
    }

    /// <summary>
    /// Returns the manhattan distance between this object (the tile the object is on) and given tile.
    /// </summary>
    /// <param name="other">Other tile to calculate distance from/to</param>
    /// <returns>Manhattan distance</returns>
    public int ManhattanDistance(ITile other)
    {
        return CurrentTile.ManhattanDistance(other);
    }
}
