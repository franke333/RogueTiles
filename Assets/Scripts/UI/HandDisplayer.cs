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

    private void Start()
    {
        for (int i = 0; i < cardDisplayers.Count; i++)
        {
            int iCopy = i;
            cardDisplayers[i].displayerClicked = () =>
            {
                _activeUnit.SelectCard(iCopy);
                MouseOver = false;
            };
        }
    }

    public void DisplayUnitCards(PlayerUnit unit)
    {
        _activeUnit = unit;
        // display cards here
        var hand = unit.cards.PickN(cardDisplayers.Count);
        for (int i = 0; i < hand.Count; i++)
        {
            cardDisplayers[i].DisplayCard(hand[i]);
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
    }

    public void Hide()
    {
        _activeUnit = null;
        gameObject.SetActive(false);
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
