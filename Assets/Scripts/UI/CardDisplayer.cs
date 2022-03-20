using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardDisplayer : MonoBehaviour, IPointerDownHandler
{

    public Action displayerClicked;

    [SerializeField]
    Text _cardName;

    public void DisplayCard(Card card)
    {
        gameObject.SetActive(true);
        _cardName.text = card.name;

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        displayerClicked.Invoke();
    }
}
