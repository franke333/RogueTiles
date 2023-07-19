using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShakeVFX : MonoBehaviour
{
    Vector3 _originalBodyPos;
    const float _shakeIntensity=0.2f;

    const float _durationBase = 0.35f;
    float _duration;
    bool  _running;

    // Start is called before the first frame update
    private void Start()
    {
        _duration = _durationBase;
        _originalBodyPos = transform.position;
        _running = true;
    }

    private void Update()
    {
        if (!_running)
        {
            return;
        }
        _duration -= Time.deltaTime;
        transform.position = _originalBodyPos + new Vector3(Mathf.Sin((_duration/_durationBase)*4*Mathf.PI)*_shakeIntensity, 0, 0);
        if (_duration <= 0)
        {
            transform.position = _originalBodyPos;
            Destroy(this);
        }
    }

}
