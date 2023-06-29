using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStandalone : MonoBehaviour, ITile
{
    [SerializeField]
    Color _oddColor;
    [SerializeField]
    bool _wall;
    [SerializeField]
    bool _walkable;
    [SerializeField]
    SpriteRenderer _sr;
    [SerializeField]
    Color _rangeColor, _selectionColor, _fogOfWarColor;
    Color _baseColor;
    [SerializeField]
    GridObject _occupiedObject;

    

    public Room Room { get => _room; set => _room = value; }
    Room _room;

    bool _visible,_inRange,_selected;

    public int x { get; set; }
    public int y { get; set; }

    public GridObject GetObject { get => _occupiedObject; }
    public ITile Init(bool isOdd,int x,int y, Room room)
    {
        _baseColor = _sr.color;
        if(isOdd && IsWalkable)
            _baseColor *= _oddColor;
        this.x = x;
        this.y = y;
        Visible = false;
        _room = room;
        RecalculateColor();
        return this;
    }

    public void TintBaseColor(Color color,float by)
    {
        _baseColor = Color.Lerp(_baseColor,color,by);
    }

    private void RecalculateColor()
    {
        Color c = _baseColor;
        if (!Visible)
            c = Color.Lerp(c, _fogOfWarColor, _fogOfWarColor.a);
        if (_inRange)
            c = Color.Lerp(c, _rangeColor, _rangeColor.a);
        if (_selected)
            c = Color.Lerp(c, _selectionColor, _selectionColor.a);
        c.a = _baseColor.a;
        _sr.color = c;
    }

    public bool Visible { 
        get => _visible;
        set
        {
            if(_visible==value)
                return;
            _visible = value;
            RecalculateColor();
            if (_occupiedObject != null)
                _occupiedObject.SetVisible(value);
        }
    }

    public void Occupy(GridObject obj)
    {
        if(obj!= null && _occupiedObject != null)
        {
            if (_occupiedObject is GridItem)
            {
                if (obj is PlayerUnit)
                    InventoryDisplayer.Instance.DisplayPickUpDialog(((GridItem)_occupiedObject).item);
                _occupiedObject.gameObject.SetActive(false);
            }
            else
            {
                Log.Error($"{obj} is trying to occupy an occupied tile by {_occupiedObject}", gameObject);
                return;
            }
        }

        _occupiedObject = obj;
        if (obj == null)
            return;

        if (obj.CurrentTile != null)
        {
            obj.CurrentTile.Occupy(null);
        }
        obj.CurrentTile = this;
        obj.gameObject.transform.position = transform.position;
        obj.SetVisible(Visible);
    }

    public bool IsWalkable
    {
        get => !_wall && _walkable && (_occupiedObject != null ? _occupiedObject.IsWalkable : true);
    }

    public bool IsWall { get => _wall; }
    public bool InRange { get => _inRange; set { _inRange = value; RecalculateColor(); } }
    public bool Selected { get => _selected; set{ _selected = value; RecalculateColor(); } }

    private bool CheckSelectability()
    {
        if (GameManager.Instance.State != GameManager.GameState.PlayerTurn &&
            GameManager.Instance.State != GameManager.GameState.PlayerChooseTarget)
            return false;
        if (HandDisplayer.Instance.MouseOver)
            return false;
        if (InventoryDisplayer.Instance.InventoryWindowActive)
            return false;
        return true;
    }

    private void OnMouseOver()
    {
        if (!CheckSelectability())
            return;
        _selected = true;
        RecalculateColor();
    }

    private void OnMouseExit()
    {
        _selected = false;
        RecalculateColor();
    }

    private void OnMouseDown()
    {
        if (!CheckSelectability())
            return;
        if (_inRange)
            GridManager.Instance.SelectedTileInRange = this;
        GridManager.Instance.SetSelectedTile(this);
    }

    public void Refresh()
    {
        RecalculateColor();
    }

    public Color GetColor() => _baseColor;
}
