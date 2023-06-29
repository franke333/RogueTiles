using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDisplayer : SingletonClass<HandDisplayer>, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerUnit _activeUnit;

    public bool MouseOver { get; private set; }

    [SerializeField]
    private List<CardDisplayer> cardDisplayers;


    public void DisplayUnitCards(PlayerUnit unit)
    {
        _activeUnit = unit;
        // display cards here
        // TODO: make pick logic inside player.. not here :/
        var hand = unit.GetCards().PickN(cardDisplayers.Count);
        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            cardDisplayers[i].DisplayCard(card);
            cardDisplayers[i].displayerClicked = () => unit.SelectCard(card);
        }
        for (int i = hand.Count; i < cardDisplayers.Count; i++)
        {
            cardDisplayers[i].Hide();
        }
        gameObject.SetActive(true);
    }

    public void DeseceltCard()
    {
        _activeUnit.DeselectCard();
    }

    public void ToggleVisibility()
    {
        if (_activeUnit != null)
            gameObject.SetActive(!gameObject.activeSelf);
        if (!gameObject.activeSelf)
            MouseOver = false;
        CardInfoDisplayer.Instance.HideInfo();
    }

    public void Hide()
    {
        _activeUnit = null;
        gameObject.SetActive(false);
        CardInfoDisplayer.Instance.HideInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOver = true;
        GridManager.Instance.SetSelectedTile(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOver = false;
    }
}
