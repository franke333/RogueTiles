using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : SingletonClass<GridManager>
{

    Dictionary<Vector2Int, ITile> _map;
    List<Room> _rooms;
    ITile _selectedTile;
    ITile _selectedTileInRange;
    [SerializeField]
    Tilemap _tilemap;

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

    public void RegisterTiles(List<ITile> tiles)
    {
        foreach (var t in tiles)
        {
            _map.Add(new Vector2Int(t.x, t.y), t);
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

    public void DisplayRange(Card card,ITile startTile)
    {
        var (x, y) = (startTile.x, startTile.y);
        CleanDisplayInRange();
        _tilesDisplayedInRange.Add(startTile);
        switch (card.shape) 
        {
            case Card.AreaShape.Line:
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
            case Card.AreaShape.Circle:
                //BFS
                void BFS(ITile start, int range)
                {
                    Queue<ITile> q = new Queue<ITile>();
                    q.Enqueue(start);
                    while (q.Count > 0)
                    {
                        var t = q.Dequeue();
                        if (t == null || t.IsWall || _tilesDisplayedInRange.Contains(t) || t.ManhattanDistance(start) > range)
                            continue;
                        _tilesDisplayedInRange.Add(t);
                        if(t.ManhattanDistance(start) == range)
                            continue;
                        foreach(ITile adjTile in GetAdjecentTiles(t))
                            q.Enqueue(adjTile);
                    }
                }
                BFS(startTile, card.range);
                break;
            case Card.AreaShape.None:
            default:
                break;
        }
        foreach(var t in _tilesDisplayedInRange)
            t.InRange = true;
    }

    public void CleanDisplayInRange()
    {
        foreach (var t in _tilesDisplayedInRange)
            t.InRange = false;
        _tilesDisplayedInRange.Clear();
    }

    public ITile CreateTile(TileType tileType,Vector3 pos,Transform parent= null)
    {
        if (_useStandalone)
        {
            TileStandalone tile = Instantiate(LevelDesignManager.Instance.GetTileStandalonePrefab(tileType) , pos, Quaternion.identity);
            if (parent != null)
                tile.transform.SetParent(parent);
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


    public ITile GetTile(Vector2Int pos)
    {
        if (_map.TryGetValue(pos, out ITile tile))
            return tile;
        return null;
    }

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

    public ITile GetSelectedTile() => _selectedTile;
    public ITile SelectedTileInRange
    {
        set => _selectedTileInRange=value;
        get => _selectedTileInRange;
    }

    public void SetSelectedTile(ITile tile)
    {
        _selectedTile = tile;
    }

    public void UpdateFog()
    {
        List<GridUnit> lightGivingUnits = new List<GridUnit>();
        foreach(GridUnit unit in GameManager.Instance.GetUnits())
        {
            if (unit.visibleRange > 0)
                lightGivingUnits.Add(unit);
        }
        foreach (ITile tile in _tilesLightUp)
            tile.Visible = false;
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
            LightUp(unit.CurrentTile, unit.visibleRange);
    }
}
