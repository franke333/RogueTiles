using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonClass<UIManager>
{
    [SerializeField]
    GameObject _enemyTurnMessage;

    public EndScreenScript EndScreen;

    public HealthBarScript PlayerHealthBar;

    public void ToggleEnemyTurnMessage(bool value)
    {
        _enemyTurnMessage?.SetActive(value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            HandDisplayer.Instance.ToggleVisibility();
    }
}
