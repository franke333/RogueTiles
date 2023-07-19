using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonClass<UIManager>
{
    [SerializeField]
    GameObject _enemyTurnMessage;

    [SerializeField]
    GameObject _gameMenu;

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
        if (Input.GetKeyDown(KeyCode.Escape))
            _gameMenu.SetActive(!_gameMenu.activeSelf);
    }
}
