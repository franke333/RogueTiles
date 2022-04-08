using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
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


    static HashSet<Tile> tilesDisplayedInRange = new HashSet<Tile>();
    bool _visible,_inRange,_selected;

    public int x, y;

    public GridObject GetObject { get => _occupiedObject; }
    public void Init(bool isOdd,int x,int y, Room room)
    {
        _baseColor = _sr.color;
        if(isOdd && IsWalkable)
            _baseColor *= _oddColor;
        this.x = x;
        this.y = y;
        Visible = false;
        _room = room;
        RecalculateColor();
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
                _occupiedObject.GetComponent<SpriteRenderer>().enabled = value;
        }
    }

    public static void CleanDisplayInRange()
    {
        foreach (var t in tilesDisplayedInRange)
        {
            t._inRange = false;
            t.RecalculateColor();
        }
        tilesDisplayedInRange.Clear();
    }

    public void DisplayInRange()
    {
        _inRange = true;
        RecalculateColor();
        tilesDisplayedInRange.Add(this);
    }

    public void Deoccupy() => Occupy(null);
    public void Occupy(GridObject obj)
    {
        if(obj!= null && _occupiedObject != null)
        {
            Log.Error($"{obj} is trying to occupy an occupied tile by {_occupiedObject}", gameObject);
        }

        _occupiedObject = obj;
        if (obj == null)
            return;

        if (obj.CurrentTile != null)
        {
            obj.CurrentTile._occupiedObject = null;
        }
        obj.CurrentTile = this;
        obj.gameObject.transform.position = transform.position;
        obj.GetComponent<SpriteRenderer>().enabled = Visible;
    }

    public bool IsWalkable
    {
        get => !_wall && _walkable && (_occupiedObject != null ? _occupiedObject.IsWalkable : true);
    }

    public bool IsWall { get => _wall; }

    private bool CheckSelectability()
    {
        if (GameManager.Instance.State != GameManager.GameState.PlayerTurn &&
            GameManager.Instance.State != GameManager.GameState.PlayerChooseTarget)
            return false;
        if (HandDisplayer.Instance.MouseOver)
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
}
