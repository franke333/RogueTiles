using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FadeColorVFX : MonoBehaviour
{
    SpriteRenderer _bodyRenderer;
    Color _bodyColor;
    Color _fadeColor;

    const float _durationBase = 0.35f;
    float _duration;
    bool _colorSet,_running;

    public void SetColor(Color fadeColor)
    {
        _fadeColor = fadeColor;
        _colorSet = true;
    }

    // Start is called before the first frame update
    void Init()
    {
        if (!_colorSet)
        {
            Log.Warning("FadeColorVFX: color not set. Waiting on Color.", gameObject);
            return;
        }
        _bodyRenderer = transform.Find("body")?.GetComponent<SpriteRenderer>();
        
        
        if (_bodyRenderer == null)
        {
            Log.Error("FadeColorVFX: body renderer not found", gameObject);
            Destroy(this);
            return;
        }
        //make sure to copy correct base color (renderer might be different if other VFXs are active)
        _bodyColor = _bodyRenderer.color;
        var activeVFXs = GetComponents<FadeColorVFX>().Where(vfx => vfx._colorSet).ToList();
        if (activeVFXs.Count > 0)
            _bodyColor = activeVFXs[0]._bodyColor;

        _bodyRenderer.color = _fadeColor;
        _duration = _durationBase;
        _running = true;
    }

    private void Update()
    {
        if (!_running)
        {
            Init();
            return;
        }
        _duration -= Time.deltaTime;
        _bodyRenderer.color = Color.Lerp(_fadeColor, _bodyColor, 1 - _duration / _durationBase);
        if (_duration <= 0)
        {
            _bodyRenderer.color = _bodyColor;
            Destroy(this);
        }
    }


}
