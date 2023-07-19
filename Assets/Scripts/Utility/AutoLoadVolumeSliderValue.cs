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
        //clean up old listeners and make sure new one is created
        _slider.onValueChanged = new UnityEngine.UI.Slider.SliderEvent();
        _slider.onValueChanged.AddListener((float value) => AudioManager.Instance.SFXVolume = value);
    }
}
