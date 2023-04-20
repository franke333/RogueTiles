using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoDisplayer : MonoBehaviour
{
    [SerializeField]
    GameObject _infoPopUpWindow;
    [SerializeField]
    Text _nameText;
    [SerializeField]
    Text _descriptionText;
    [SerializeField]
    Vector3 _offset;

    float _time= 0;

    public void DisplayInfoFor(string cardName,string cardDescription,float time)
    {
        Log.Debug("bug");
        _nameText.text = cardName;
        _descriptionText.text = cardDescription;
        _infoPopUpWindow.SetActive(true);
        time = Mathf.Max(time, 0.5f);
        _time = Mathf.Max(_time, time);
    }



    public void HideInfo()
    {
        _infoPopUpWindow.SetActive(false);
    }

    private void Update()
    {
        //follow mouse
        if(!_infoPopUpWindow.activeSelf)
            return;
        if (_time > 0)
        {
            _time -= Time.deltaTime;
            if (_time <= 0)
            {
                HideInfo();
                return;
            }
        }
        else
            return;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        
        _infoPopUpWindow.transform.position = mousePos+_offset;
    }
}
