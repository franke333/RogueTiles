using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Hand displayer that displays cards in hand
/// </summary>
public class HandDisplayer : SingletonClass<HandDisplayer>, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerUnit _activeUnit;

    public bool MouseOver { get; private set; }

    [SerializeField]
    private List<CardDisplayer> cardDisplayers;

    /// <summary>
    /// Draw cards from playerUnit and display them
    /// </summary>
    /// <param name="playerUnit">Player Unit containg the cards</param>
    public void DisplayUnitCards(PlayerUnit playerUnit)
    {
        _activeUnit = playerUnit;
        // display cards here
        // NOTE: would be more logical to have a list of cards in playerUnit
        var hand = playerUnit.GetCards().PickN(cardDisplayers.Count);
        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            cardDisplayers[i].DisplayCard(card);
            cardDisplayers[i].DisplayerClicked = () => playerUnit.SelectCard(card);
        }
        for (int i = hand.Count; i < cardDisplayers.Count; i++)
        {
            cardDisplayers[i].Hide();
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Cancels the card selection
    /// </summary>
    public void DeseceltCard()
    {
        _activeUnit.DeselectCard();
    }

    /// <summary>
    /// Toggles the visibility of the hand
    /// </summary>
    public void ToggleVisibility()
    {
        if (_activeUnit != null)
            gameObject.SetActive(!gameObject.activeSelf);
        if (!gameObject.activeSelf)
            MouseOver = false;
        PopupInfoDisplayer.Instance.HideInfo();
    }

    /// <summary>
    /// Hides the hand
    /// </summary>
    public void Hide()
    {
        _activeUnit = null;
        gameObject.SetActive(false);
        PopupInfoDisplayer.Instance.HideInfo();
    }

    // ------------------- UI Events -------------------

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
