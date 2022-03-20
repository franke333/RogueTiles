using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerUnit : GridUnit
{

    [SerializeField]
    private int _visibleRange;

    public List<Card> cards;

    // if card is selected, then we are looking at possible placements,
    // otherwise player is looking to where to walk
    private Card _selectedCard;
    public override int visibleRange { get => _visibleRange; }

    private Tile GetKeyDownTile()
    {
        var adjTiles = GridManager.Instance.GetAdjecentTiles(CurrentTile);
        Vector2 pos = new Vector2(CurrentTile.x, CurrentTile.y);
        if (Input.GetKeyDown(KeyCode.W))
            pos.y++;
        else if (Input.GetKeyDown(KeyCode.A))
            pos.x--;
        else if (Input.GetKeyDown(KeyCode.S))
            pos.y--;
        else if (Input.GetKeyDown(KeyCode.D))
            pos.x++;
        var tile = adjTiles.Where(t => (t.x == pos.x) && (t.y == pos.y));
        return tile.Any() ? tile.First() : null;
    }

    public void SelectCard(int index)
    {
        Tile.CleanDisplayInRange();
        Log.Debug($"{index} selctedCard Index", gameObject);
        HandDisplayer.Instance.ToggleVisibility();
        _selectedCard = cards[index];
        if (_selectedCard.needsTarget)
            GridManager.Instance.DisplayRange(_selectedCard, CurrentTile);
    }

    public void DeselectCard()
    {
        _selectedCard = null;
        Tile.CleanDisplayInRange();
    }



    protected override bool PlayTurn()
    {
        // player has no card selected, clicking a tile means movement
        if (_selectedCard == null)
        {
            Tile selectedTile;
            if ((selectedTile = GridManager.Instance.GetSelectedTile()) == null)
                if ((selectedTile = GetKeyDownTile()) == null)
                    return false;
            GridManager.Instance.SetSelectedTile(null);
            if (!selectedTile.IsWalkable)
                return false;
            if (!GridManager.Instance.GetAdjecentTiles(CurrentTile).Contains(selectedTile))
                return false;
            selectedTile.Occupy(this);
            return true;
        }
        // else player selected card
        else
        {
            if (_selectedCard.needsTarget)
            {
                Tile selectedTile;
                if ((selectedTile = GridManager.Instance.SelectedTileInRange) == null)
                    return false;
                if(selectedTile.GetObject == null)
                {
                    selectedTile = null;
                    return false;
                }
                if (!selectedTile.GetObject.IsTargetable)
                    return false;

                Card.currentTarget = selectedTile.GetObject;

                //clear selection
                GridManager.Instance.SelectedTileInRange = null;
            }
            Card.currentUnit = this;
            Card.playedCard = _selectedCard;

            _selectedCard.cardEffect.Invoke();


            //clean
            Card.ClearData();
            DeselectCard();
            GridManager.Instance.SetSelectedTile(null);

            return true;
        }
    }
}
