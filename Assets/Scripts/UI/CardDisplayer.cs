using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardDisplayer : MonoBehaviour, IPointerDownHandler , IPointerEnterHandler, IPointerExitHandler
{

    public Action DisplayerClicked;

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
        PopupInfoDisplayer.Instance.HideInfo();
    }

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

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(PopupInfoDisplayer.Instance.timeToWaitBeforeDisplayingInfo);
        //show hoverbox
        PopupInfoDisplayer.Instance.DisplayInfoFor(_description, Input.mousePosition);
    }
}
