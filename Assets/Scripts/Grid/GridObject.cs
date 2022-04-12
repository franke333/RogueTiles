using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GridObject : MonoBehaviour
{
    public string alias;

    [SerializeField]
    protected bool _walkable;
    [SerializeField]
    protected bool _targetable;

    protected Tile _currentTile; 

    public Tile CurrentTile { get => _currentTile; set => _currentTile = value; }

    public abstract bool TakeDamage(int dmg);

    public bool IsWalkable { get => _walkable; }
    public bool IsTargetable { get => _targetable; }

    public abstract void SetVisible(bool value);
}
