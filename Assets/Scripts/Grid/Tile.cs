using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    Color _oddColor;
    [SerializeField]
    SpriteRenderer _sr;
    [SerializeField]
    GameObject _selectionTile;
    [SerializeField]
    GameObject _fogOfWarTile;
    [SerializeField]
    GameObject _rangeTile;
    [SerializeField]
    GridObject _occupiedObject;

    public Room Room { get => _room; set => _room = value; }
    Room _room;


    static HashSet<Tile> tilesDisplayedInRange = new HashSet<Tile>();
    bool _visible = false;

    public int x, y;

    public GridObject GetObject { get => _occupiedObject; }
    public void Init(bool isOdd,int x,int y, Room room)
    {
        if(isOdd)
            _sr.color = _oddColor;
        this.x = x;
        this.y = y;
        Visible = false;
        _room = room;
    }

    public bool Visible { 
        get => _visible;
        set
        {
            _visible = value;
            _fogOfWarTile.SetActive(!value);
            if (_occupiedObject != null)
                _occupiedObject.GetComponent<SpriteRenderer>().enabled = value;
        }
    }

    public static void CleanDisplayInRange()
    {
        foreach (var t in tilesDisplayedInRange)
            t._rangeTile.SetActive(false);
        tilesDisplayedInRange.Clear();
    }

    public void DisplayInRange()
    {
        _rangeTile.SetActive(true);
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
        get => _occupiedObject != null ? _occupiedObject.IsWalkable : true;
    }

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
        _selectionTile.SetActive(true);
    }

    private void OnMouseExit()
    {
        _selectionTile.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!CheckSelectability())
            return;
        if (_rangeTile.activeSelf)
            GridManager.Instance.SelectedTileInRange = this;
        GridManager.Instance.SetSelectedTile(this);
    }
}
