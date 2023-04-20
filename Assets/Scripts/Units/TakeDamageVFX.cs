using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageVFX : MonoBehaviour
{
    SpriteRenderer _bodyRenderer;
    Color _bodyColor;

    const float _durationBase = 0.35f;
    float _duration;

    // Start is called before the first frame update
    void Start()
    {
        _bodyRenderer = transform.Find("body")?.GetComponent<SpriteRenderer>();
        if (_bodyRenderer == null)
        {
            Log.Error("TakeDamageVFX: body renderer not found", gameObject);
            Destroy(this);
            return;
        }
        _bodyColor = _bodyRenderer.color;
        _bodyRenderer.color = Color.red;
        _duration = _durationBase;
    }

    private void Update()
    {
        _duration -= Time.deltaTime;
        _bodyRenderer.color = Color.Lerp(Color.red, _bodyColor, 1 - _duration / _durationBase);
        if (_duration <= 0)
        {
            _bodyRenderer.color = _bodyColor;
            Destroy(this);
        }
    }


}
