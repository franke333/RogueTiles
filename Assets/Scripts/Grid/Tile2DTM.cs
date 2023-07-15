using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile 2DTM", menuName = "Scriptables/Tile2DTM")]
public class Tile2DTM : TileBase, ITile
{
    static private Dictionary<Vector2Int, Tile2DTM> _map = new Dictionary<Vector2Int, Tile2DTM>();

    [SerializeField]
    bool _isWall, _isWalkable;
    [SerializeField]
    Sprite _sprite;
    [SerializeField]
    Color _baseColor, _oddTileDarkening, _rangeColor, _selectionColor, _fogOfWarColor;
    Color _color;
    bool _inRange, _selected, _visible;

    public Room Room { get; set; }
    public int x { get; set; }
    public int y { get; set; }

    GridObject _gridObject;

    Room _room;

    public GridObject GetObject => _gridObject;

    public bool Visible { get => _visible; set { if (value != _visible) { _visible = value; Refresh(); } } }

    public bool IsWall => _isWall;

    public bool IsWalkable => _isWalkable;

    public bool InRange { get => _inRange; set { if (value != _inRange) { _inRange = value; Refresh(); }  } }
    public bool Selected { get => _selected; set { if (value != _selected) { _selected = value; Refresh(); } } }

    public static Tile2DTM GetTile(int x, int y)
    {
        if (_map.TryGetValue(new Vector2Int(x, y), out Tile2DTM tile))
            return tile;
        return null;
    }

    public ITile Init(bool isOdd, int x, int y, Room room)
    {
        Tile2DTM tile = CreateInstance<Tile2DTM>();
        //copy
        tile._sprite = _sprite;
        tile._isWall = _isWall;
        tile._isWalkable = _isWalkable;
        tile._baseColor = _baseColor;
        tile._oddTileDarkening = _oddTileDarkening;
        tile._rangeColor = _rangeColor;
        tile._selectionColor = _selectionColor;
        tile._fogOfWarColor = _fogOfWarColor;

        tile.x = x;
        tile.y = y;
        tile._baseColor = _baseColor;
        if (isOdd)
            tile._baseColor *= _oddTileDarkening;
        tile._room = room;
        tile._color = _baseColor;
        _map.Add(new Vector2Int(x, y), tile);
        return tile;
    }

    public void Occupy(GridObject obj)
    {
        if (obj != null && _gridObject != null)
        {
            if (_gridObject is GridItem)
            {
                if (obj is PlayerUnit)
                    InventoryDisplayer.Instance.DisplayPickUpDialog(((GridItem)_gridObject).item);
                _gridObject.gameObject.SetActive(false);
            }
            else
            {
                Log.Error($"{obj} is trying to occupy an occupied tile by {_gridObject}", null);
                return;
            }
        }

        _gridObject = obj;
        if (obj == null)
            return;

        if (obj.CurrentTile != null)
        {
            obj.CurrentTile.Occupy(null);
        }
        obj.CurrentTile = this;
        obj.gameObject.transform.position = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(x, y, 0)) + GridManager.Instance.Tilemap.cellSize/2;
        obj.SetVisible(Visible);
    }

    public void Refresh()
    {
        _color = _baseColor;
        if (!Visible)
            _color = Color.Lerp(_color, _fogOfWarColor, _fogOfWarColor.a);
        if (InRange)
            _color = Color.Lerp(_color, _rangeColor, _rangeColor.a);
        if (Selected)
            _color = Color.Lerp(_color, _selectionColor, _selectionColor.a);
        _color.a = _baseColor.a;
        GridManager.Instance.Tilemap.SetColor(new Vector3Int(x,y,0), _color);
    }

    public void TintBaseColor(Color color, float by)
    {
        _baseColor = Color.Lerp(_baseColor, color, by);
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (_map.TryGetValue(new Vector2Int(position.x, position.y), out Tile2DTM tile))
        {
            tileData.flags = TileFlags.LockTransform;
            tileData.sprite = tile._sprite;
            tileData.color = tile._color;
        }
    }

    public bool CheckSelectability()
    {
        if (GameManager.Instance.State != GameManager.GameState.PlayerTurn)
            return false;
        if (HandDisplayer.Instance.MouseOver)
            return false;
        if (InventoryDisplayer.Instance.InventoryWindowActive)
            return false;
        return true;
    }

    public Color GetColor() => _baseColor;
}
