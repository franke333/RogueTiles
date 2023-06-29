using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadVolumeSliderValue : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Slider _slider;
    private void OnEnable()
    {
        if(_slider == null)
            _slider = GetComponent<UnityEngine.UI.Slider>();
        _slider.value = AudioManager.Instance.SFXVolume;
    }
}
