using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardDisplayer : MonoBehaviour, IPointerDownHandler , IPointerEnterHandler, IPointerExitHandler
{

    public Action displayerClicked;

    [SerializeField]
    Text _cardName;

    string _description;
    [SerializeField]
    private bool _clickable = true;

    public void DisplayCard(Card card)
    {
        gameObject.SetActive(true);
        _cardName.text = card.name;
        _description = card.description;

    }

    public void Hide()
    {
        gameObject.SetActive(false);
        CardInfoDisplayer.Instance.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_clickable)
            displayerClicked.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        CardInfoDisplayer.Instance.HideInfo();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(CardInfoDisplayer.Instance.timeToWaitBeforeDisplayingInfo);
        //show message
        CardInfoDisplayer.Instance.DisplayInfoFor(_description, Input.mousePosition);
    }
}
