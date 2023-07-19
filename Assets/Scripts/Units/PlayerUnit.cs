using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerUnit : GridUnit
{

    [SerializeField]
    private int _visibleRange;

    // base cards
    public List<Card> cards;
    public Inventory inventory;

    // if card is selected, then we are looking at possible placements,
    // otherwise player is looking to where to walk
    private Card _selectedCard;
    public override int visibleRange { get => _visibleRange; }

    public List<Card> GetCards()
    {
        List<Card> cards = new List<Card>();
        foreach (var card in this.cards) 
            cards.Add(card);
        foreach (var slot in inventory.slots)
            if (slot != null && !slot.IsEmpty)
                foreach (var card in slot.item.cards)
                    cards.Add(card);
        return cards;
    }

    private ITile GetKeyDownTile()
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

    public void SelectCard(Card card)
    {
        GridManager.Instance.CleanDisplayInRange();
        Log.Debug($"selcted Card: {card}", gameObject);
        HandDisplayer.Instance.ToggleVisibility();
        _selectedCard = card;
        if (_selectedCard.needsTarget)
            GridManager.Instance.DisplayRange(_selectedCard, CurrentTile);
    }

    public void DeselectCard()
    {
        _selectedCard = null;
        GridManager.Instance.CleanDisplayInRange();
    }

    protected override void RaiseEvent(EventInfo ei)
    {
        base.RaiseEvent(ei);
        foreach (var item in inventory.slots.Select(s => s.item).Where(i => i != null))
            foreach (var effect in item.effects)
                effect.DoEffect(ei);
    }


    protected override bool PlayTurn()
    {
        // player has no card selected, clicking a tile means movement
        if (_selectedCard == null)
        {
            ITile selectedTile;
            if ((selectedTile = GridManager.Instance.GetSelectedTile()) == null)
                if ((selectedTile = GetKeyDownTile()) == null)
                    return false;
            GridManager.Instance.SetSelectedTile(null);
            if (!selectedTile.IsWalkable)
                return false;
            if (!GridManager.Instance.GetAdjecentTiles(CurrentTile).Contains(selectedTile))
                return false;
            selectedTile.Occupy(this);
            RaiseEvent(new EventInfo(EventType.Move,this));
            return true;
        }
        // else player selected card
        else
        {
            if (_selectedCard.needsTarget)
            {
                ITile selectedTile;
                if ((selectedTile = GridManager.Instance.SelectedTileInRange) == null)
                    return false;
                if(selectedTile.GetObject == null)
                {
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
            AudioManager.Instance.PlaySFX(_selectedCard.audioType);

            //clean
            Card.ClearData();
            DeselectCard();
            GridManager.Instance.SetSelectedTile(null);

            gameObject.AddComponent<ShakeVFX>();
            return true;
        }
    }
    
    protected override void Die()
    {
        base.Die();
        GameManager.Instance.EndGame(false);
    }
}
