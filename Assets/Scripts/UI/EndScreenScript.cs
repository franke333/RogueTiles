using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenScript : MonoBehaviour
{
    [SerializeField]
    Text _result, _stats;

    public void Show(bool victory, List<Tuple<string,int>> stats)
    {
        gameObject.SetActive(true);
        if (victory)
        {
            _result.text = "Victory!";
            _result.color = Color.green;
        }
        else
        {
            _result.text = "Defeat!";
            _result.color = Color.red;
        }
        _stats.text = "";
        foreach (var (name,value) in stats)
        {
            _stats.text += $"{name}: {value}\n";
        }
    }
}
