using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages tiles and rooms
/// </summary>
public class GridManager : SingletonClass<GridManager>
{

    Dictionary<Vector2Int, ITile> _map;
    List<Room> _rooms;
    ITile _selectedTile;
    ITile _selectedTileInRange;
    [SerializeField]
    Tilemap _tilemap;

    private int _minXCoord = 0, _maxXCoord = 0,
        _minYCoord = 0, _maxYCoord = 0;

    public int Width { get => _maxXCoord - _minXCoord; }
    public int Height { get => _maxYCoord - _minYCoord; }

    /// <summary>
    /// Get tilemap in case we are using its implementation of ITile
    /// </summary>
    public Tilemap Tilemap
    {
        get
        {
            if (_tilemap == null)
                _tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
            return _tilemap;
        }
        set => _tilemap = value;
    }

    public bool _useStandalone;

    HashSet<ITile> _tilesDisplayedInRange = new HashSet<ITile>();
    HashSet<ITile> _tilesLightUp = new HashSet<ITile>();

    /// <summary>
    /// Add tiles to the map
    /// </summary>
    /// <param name="tiles">List of tiles to be added</param>
    public void RegisterTiles(List<ITile> tiles)
    {
        foreach (var t in tiles)
        {
            _map.Add(new Vector2Int(t.x, t.y), t);
            if (t.x < _minXCoord)
                _minXCoord = t.x;
            if (t.x > _maxXCoord)
                _maxXCoord = t.x;
            if (t.y < _minYCoord)
                _minYCoord = t.y;
            if (t.y > _maxYCoord)
                _maxYCoord = t.y;
        }
    }


    protected override void Awake()
    {
        base.Awake();
        _map = new Dictionary<Vector2Int, ITile>();
        _rooms = new List<Room>();
        if(_useStandalone)
            this.Tilemap.gameObject.SetActive(false);
        else
            this.Tilemap.gameObject.SetActive(true);
        
    }

    /// <summary>
    /// Highlight tiles that can be reached by the played card
    /// </summary>
    /// <param name="card">Card with info about its range</param>
    /// <param name="startTile">Tile the player is standing on</param>
    public void DisplayRange(Card card,ITile startTile)
    {
        var (x, y) = (startTile.x, startTile.y);
        CleanDisplayInRange();
        
        switch (card.shape) 
        {
            // the target of the card must be on the same row or column as the player
            case Card.AreaShape.Line:
                _tilesDisplayedInRange.Add(startTile);
                foreach (var dir in new int[] { 1, -1 })
                {
                    ITile t;

                    for (int i = 1; i <= card.range; i++)
                        if (_map.TryGetValue(new Vector2Int(x + dir * i, y), out t) && !t.IsWall)
                            _tilesDisplayedInRange.Add(t);
                        else
                            break;
                    for (int i = 1; i <= card.range; i++)
                        if (_map.TryGetValue(new Vector2Int(x, y + dir * i), out t) && !t.IsWall)
                            _tilesDisplayedInRange.Add(t);
                        else
                            break;
                }
                break;
            // the target of the card must be in range of the card
            case Card.AreaShape.Circle:
                //BFS
                Queue<ITile> q = new Queue<ITile>();
                q.Enqueue(startTile);
                int range = card.range;
                while (q.Count > 0)
                {
                    var tile = q.Dequeue();
                    if (tile == null || tile.IsWall || _tilesDisplayedInRange.Contains(tile) || tile.ManhattanDistance(startTile) > range)
                        continue;
                    _tilesDisplayedInRange.Add(tile);
                    if(tile.ManhattanDistance(startTile) == range)
                        continue;
                    foreach(ITile adjTile in GetAdjecentTiles(tile))
                        q.Enqueue(adjTile);
                }
                break;
            case Card.AreaShape.None:
            default:
                break;
        }
        foreach(var t in _tilesDisplayedInRange)
            t.InRange = true;
    }

    /// <summary>
    /// Remove highlights from tiles
    /// </summary>
    public void CleanDisplayInRange()
    {
        foreach (var t in _tilesDisplayedInRange)
            t.InRange = false;
        _tilesDisplayedInRange.Clear();
    }

    /// <summary>
    /// Create an tile using the tilemap or the standalone version
    /// </summary>
    /// <param name="tileType">type of tile</param>
    /// <param name="pos">position of the tile in world</param>
    /// <param name="parent"> assign parent to clean scene hierarchy </param>
    /// <returns></returns>
    public ITile CreateTile(TileType tileType,Vector3 pos,Transform parent= null)
    {
        if (_useStandalone)
        {
            TileStandalone tile = Instantiate(LevelDesignManager.Instance.GetTileStandalonePrefab(tileType) , pos, Quaternion.identity);
            if (parent != null)
                tile.transform.SetParent(parent);
            //reduce memory usage of the standalone version
            tile.gameObject.name = "";
            return tile;
        }
        else
        {
            this.Tilemap.SetTile(this.Tilemap.WorldToCell(pos), LevelDesignManager.Instance.GetTile2DTM(tileType));
            this.Tilemap.SetTileFlags(this.Tilemap.WorldToCell(pos), TileFlags.None);
            var tile =  this.Tilemap.GetTile<Tile2DTM>(this.Tilemap.WorldToCell(pos));
            return tile;
        }
    }

    /// <summary>
    /// Generate a level using the given layout generator and room functions
    /// </summary>
    /// <param name="layoutGenerator">Function to Generate layout which return list of rooms</param>
    /// <param name="roomFuncs">Action that processes list of rooms</param>
    public void GenerateLevel(Func<List<Room>> layoutGenerator, Action<List<Room>> roomFuncs)
    {
        // dispose of old level
        if (_rooms != null)
        {
            foreach (var room in _rooms)
                Destroy(room.gameObject);
        }

        // clean values
        _map = new Dictionary<Vector2Int, ITile>();
        _rooms = new List<Room>();


        // generate and porcess rooms
        _rooms = layoutGenerator.Invoke();
        if(roomFuncs != null)
            roomFuncs.Invoke(_rooms);


        // register all tiles
        foreach (var room in _rooms)
            RegisterTiles(room.GetRoomTiles);

        // clean up editor
        GameObject enviroemntGO = GameObject.Find("Enviroment");
        this.Tilemap.RefreshAllTiles();
        foreach (var room in _rooms)
        {
            room.transform.SetParent(enviroemntGO.transform);
            foreach (var tile in room.GetRoomTiles)
                tile.Refresh();
        }
        
    }

    /// <summary>
    /// Get tile at position
    /// </summary>
    /// <param name="pos">position</param>
    /// <returns>The tile at position pos</returns>
    public ITile GetTile(Vector2Int pos)
    {
        if (_map.TryGetValue(pos, out ITile tile))
            return tile;
        return null;
    }

    /// <summary>
    /// Get all tiles adjacent to the given tile
    /// </summary>
    /// <param name="tile">tile to get neighbours of</param>
    /// <returns>List of adjacent tiles</returns>
    public List<ITile> GetAdjecentTiles(ITile tile)
    {
        List<ITile> list = new List<ITile>();
        foreach(var d in new int[] { -1, 1 })
        {
            if (_map.TryGetValue(new Vector2Int(tile.x + d, tile.y), out ITile outTile))
                list.Add(outTile);
            if (_map.TryGetValue(new Vector2Int(tile.x, tile.y + d), out ITile outTile2))
                list.Add(outTile2);
        }
        return list;
    }

    /// <summary>
    /// Get tile that is currently selected by user
    /// </summary>
    /// <returns> selected tile </returns>
    public ITile GetSelectedTile() => _selectedTile;

    /// <summary>
    /// Tile that is currently selected by user and is also highlighted
    /// </summary>
    public ITile SelectedTileInRange
    {
        set => _selectedTileInRange=value;
        get => _selectedTileInRange;
    }

    /// <summary>
    /// Set the tile that is currently selected by user
    /// </summary>
    /// <param name="tile">tile that is currently selected by user</param>
    public void SetSelectedTile(ITile tile)
    {
        _selectedTile = tile;
    }

    /// <summary>
    /// Update the fog of war
    /// </summary>
    public void UpdateFog()
    {
        // find all units that give light
        List<GridUnit> lightGivingUnits = new List<GridUnit>();
        foreach(GridUnit unit in GameManager.Instance.GetUnits())
        {
            if (unit.VisibleRange > 0)
                lightGivingUnits.Add(unit);
        }

        // apply fog to all previously lit tiles
        foreach (ITile tile in _tilesLightUp)
            tile.Visible = false;

        // light up all tiles that are in range of a light giving unit
        Dictionary<Vector2, int> lightLevel = new Dictionary<Vector2, int>();
        void LightUp(ITile t,int intensity)
        {
            if (intensity == 0)
                return;
            if (t.Visible && intensity <= lightLevel[new Vector2(t.x, t.y)])
                return;
            _tilesLightUp.Add(t);
            t.Visible = true;
            lightLevel[new Vector2(t.x, t.y)] = intensity;
            if(!t.IsWall)
                foreach (var at in GetAdjecentTiles(t))
                    LightUp(at, intensity - 1);
        }

        foreach (var unit in lightGivingUnits)
            LightUp(unit.CurrentTile, unit.VisibleRange);
    }
}
