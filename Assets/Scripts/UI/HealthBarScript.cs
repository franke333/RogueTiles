using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField]
    GridUnit _assignedUnit;

    // is not shown when [SerializeField] is used
    public Gradient _gradient;

    Slider _slider;
    Image _fill;

    Text _text;

    [SerializeField]
    bool _isPlayerHealthBar = false;


    private void Start()
    {
        if (_isPlayerHealthBar)
        {
            // needs to be delayed
            _assignedUnit = GameManager.Instance.GetUnits().Where(u => !u.IsEnemy).FirstOrDefault();
        }
        if (_assignedUnit == null)
            _assignedUnit = GetComponentInParent<GridUnit>();
        if(_assignedUnit == null)
            Log.Error("HealthBarScript assigned to object with no GridUnit", gameObject);
        _assignedUnit.ChangeHealthEvent.AddListener(UpdateHealthBar);
        _slider = GetComponent<Slider>();
        _slider.minValue = 0;
        _text = GetComponentInChildren<Text>();
        _fill = _slider.fillRect.GetComponentInChildren<Image>();
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        _slider.maxValue= _assignedUnit.maxHp;
        _slider.value = _assignedUnit.hp;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
        _text.text = $"{_assignedUnit.hp}/{_assignedUnit.maxHp}";
    }
}
