using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoadSeed : MonoBehaviour
{
    [SerializeField]
    InputField _inputField;

    /// <summary>
    ///  Load seed from input field and play
    /// </summary>
    public void TryLoadSeedAndPlay()
    {
        if(_inputField.text != "" && int.TryParse(_inputField.text,out int seed))
        {
            MyRandom.SetState(seed);
        }
        MenuManager.Instance.Play();
    }
}
