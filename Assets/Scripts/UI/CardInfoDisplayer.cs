using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoDisplayer : SingletonClass<CardInfoDisplayer>
{
    [SerializeField]
    RectTransform _infoPopUpWindow;
    [SerializeField]
    Text _descriptionText;
    [SerializeField]
    Vector2 _extraBorderSize;

    public float timeToWaitBeforeDisplayingInfo = 0.33f;

    public void DisplayInfoFor(string cardDescription, Vector2 position)
    {
        _descriptionText.text = cardDescription;
        _infoPopUpWindow.gameObject.SetActive(true);
        _infoPopUpWindow.sizeDelta = new Vector2(math.max(200, _descriptionText.preferredWidth), _descriptionText.preferredHeight) + _extraBorderSize;
        _infoPopUpWindow.transform.position = position + new Vector2(_infoPopUpWindow.sizeDelta.x/2,0);
        //the display may run out of screen bounds. Push it back
        //Bottom left corner of Screen is (0,0)
        if (_infoPopUpWindow.transform.position.x + _infoPopUpWindow.sizeDelta.x > Screen.width)
        {
            _infoPopUpWindow.transform.position = new Vector2(Screen.width - _infoPopUpWindow.sizeDelta.x/2, _infoPopUpWindow.transform.position.y);
        }
        if(_infoPopUpWindow.transform.position.y - _infoPopUpWindow.sizeDelta.y < 0)
        {
            _infoPopUpWindow.transform.position = new Vector2(_infoPopUpWindow.transform.position.x, _infoPopUpWindow.sizeDelta.y/2);
        }
    }

    private void Start()
    {
        HideInfo();
    }

    public void HideInfo()
    {
        _descriptionText.text = default;
        _infoPopUpWindow.gameObject.SetActive(false);
    }



}
