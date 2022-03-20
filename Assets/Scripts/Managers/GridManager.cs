using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Tile TilePrefab;

    Dictionary<Vector2, Tile> _map;
    List<Room> _rooms;
    Tile _selectedTile;
    Tile _selectedTileInRange;

    static GridManager _instance;

    public static GridManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("GridManager").GetComponent<GridManager>();
            return _instance;
        }
    }

    public void RegisterTiles(List<Tile> tiles)
    {
        foreach (var t in tiles)
        {
            _map.Add(new Vector2(t.x, t.y), t);
        }
    }


    public void Awake()
    {
        _instance = this;
        _map = new Dictionary<Vector2, Tile>();
        _rooms = new List<Room>();
    }

    public void DisplayRange(Card card,Tile startTile)
    {
        var (x, y) = (startTile.x, startTile.y);
        startTile.DisplayInRange();
        switch (card.shape) 
        {
            case Card.AreaShape.Line:
                foreach(var dir in new int[] { 1,-1})
                    for (int i = 1; i <= card.range; i++)
                    {
                        Tile t;
                        if(_map.TryGetValue(new Vector2(x + dir * i, y),out t))
                            t.DisplayInRange();
                        if(_map.TryGetValue(new Vector2(x, y + dir * i), out t))
                            t.DisplayInRange();
                    }
                break;
            case Card.AreaShape.Circle:
                Log.Error("NOT IMPLEMENTED");
                break;
            case Card.AreaShape.Square:
                Log.Error("NOT IMPLEMENTED");
                break;
            case Card.AreaShape.None:
            default:
                break;
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
        _map = new Dictionary<Vector2, Tile>();
        _rooms = new List<Room>();

        Log.Debug("1", gameObject);

        // generate and porcess rooms
        _rooms = layoutGenerator.Invoke();
        if(roomFuncs != null)
            roomFuncs.Invoke(_rooms);

        Log.Debug("2", gameObject);

        // register all tiles
        foreach (var room in _rooms)
            RegisterTiles(room.GetRoomTiles);


        // clean up editor
        GameObject enviroemntGO = GameObject.Find("Enviroment");
        foreach (var room in _rooms)
        {
            room.transform.SetParent(enviroemntGO.transform);
        }
    }


    public Tile GetTile(Vector2 pos)
    {
        if (_map.TryGetValue(pos, out Tile tile))
            return tile;
        return null;
    }

    public List<Tile> GetAdjecentTiles(Tile tile)
    {
        List<Tile> list = new List<Tile>();
        foreach(var d in new int[] { -1, 1 })
        {
            if (_map.TryGetValue(new Vector2(tile.x + d, tile.y), out Tile outTile))
                list.Add(outTile);
            if (_map.TryGetValue(new Vector2(tile.x, tile.y + d), out Tile outTile2))
                list.Add(outTile2);
        }
        return list;
    }

    public Tile GetSelectedTile() => _selectedTile;
    public Tile SelectedTileInRange
    {
        set => _selectedTileInRange=value;
        get => _selectedTileInRange;
    }

    public void SetSelectedTile(Tile tile)
    {
        _selectedTile = tile;
    }

    public void UpdateFog()
    {
        List<GridUnit> lightGivingUnits = new List<GridUnit>();
        foreach(Tile tile in _map.Values)
        {
            tile.Visible = false;
            GridObject obj;
            if ((obj = tile.GetObject) != null){
                if (obj is GridUnit unit)
                    if (unit.visibleRange > 0)
                        lightGivingUnits.Add(unit);
            }
        }
        Dictionary<Vector2, int> lightLevel = new Dictionary<Vector2, int>();
        void LightUp(Tile t,int intensity)
        {
            if (intensity == 0)
                return;
            if (t.Visible && intensity <= lightLevel[new Vector2(t.x, t.y)])
                return;
            t.Visible = true;
            lightLevel[new Vector2(t.x, t.y)] = intensity;
            foreach (var at in GetAdjecentTiles(t))
                LightUp(at, intensity - 1);
        }

        foreach (var unit in lightGivingUnits)
            LightUp(unit.CurrentTile, unit.visibleRange);
    }
}
