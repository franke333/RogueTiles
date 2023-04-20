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

    string _description;

    RectTransform rectTransform;

    public void DisplayCard(Card card)
    {
        gameObject.SetActive(true);
        _cardName.text = card.name;
        _description = card.description;

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 mousePos = rectTransform.InverseTransformPoint(Input.mousePosition);
        if (rectTransform.rect.Contains(mousePos))
        {
           UIManager.Instance.CardInfoDisplayer.DisplayInfoFor(_cardName.text, _description, Time.deltaTime*2);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        displayerClicked.Invoke();
    }
}
