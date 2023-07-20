using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Class that displays a card on the user interface
/// </summary>
public class CardDisplayer : MonoBehaviour, IPointerDownHandler , IPointerEnterHandler, IPointerExitHandler
{

    public Action DisplayerClicked;

    [SerializeField]
    Text _cardName;

    string _description;
    [SerializeField]
    private bool _clickable = true;

    /// <summary>
    /// Display information about a card
    /// </summary>
    public void DisplayCard(Card card)
    {
        gameObject.SetActive(true);
        _cardName.text = card.name;
        _description = card.description;

    }

    /// <summary>
    /// Hide card
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        PopupInfoDisplayer.Instance.HideInfo();
    }

    // ------------------- UI EVENTS -------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_clickable)
            DisplayerClicked.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        PopupInfoDisplayer.Instance.HideInfo();
    }

    
    // method for displaying hoverbox
    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(PopupInfoDisplayer.Instance.timeToWaitBeforeDisplayingInfo);
        //show hoverbox
        PopupInfoDisplayer.Instance.DisplayInfoFor(_description, Input.mousePosition);
    }
}
