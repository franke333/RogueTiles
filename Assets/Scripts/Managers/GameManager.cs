using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingletonClass<GameManager>
{
    List<GridUnit> _units;
    int _currentUnitIndex;

    [SerializeField]
    float _waitTime = 0.2f;
    float _waitUntil;

    [SerializeField]
    public GameState State { get; private set; }

    /// <summary>
    /// Coroutine to wait for the game to start
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator WaitForStart(Action action)
    {
        while (GameManager.Instance == null)
            yield return null;
        while (GameManager.Instance.State != GameManager.GameState.StartGame &&
            GameManager.Instance.State != GameManager.GameState.PlayerTurn)
            yield return null;
        action.Invoke();
    }

    protected override void Awake()
    {
        base.Awake();
        _currentUnitIndex = 0;
        _units = new List<GridUnit>();
        
    }

    private void Start()
    {
        ChangeState(GameState.PrepareLevel);
    }

    public void RegisterUnit(GridUnit gu)
    {
        Log.Info($"Registered {gu}", gameObject);
        _units.Add(gu);
    }

    public void UnregisterUnit(GridUnit gu)
    {
        // active untis managed by index.. needs to check for this
        Log.Info($"UNregistered {gu}", gameObject);
        if (_units.IndexOf(gu) <= _currentUnitIndex)
            _currentUnitIndex--;
        _units.Remove(gu);
    }

    private void ProcessUnitTurn()
    {
        if (Time.time < _waitUntil)
            return;

        if (State == GameState.StartGame)
            ChangeState(GameState.StartGame);

        if (State == GameState.PlayerTurn)
            CameraManager.Instance.SetFocusAt(_units[_currentUnitIndex].gameObject);

        // we are waiting on turn (animation or player)
        if (!_units[_currentUnitIndex].TakeTurn())
            return;

        // dont wait invisible units movement
        _waitUntil = Time.time +
            (_units[_currentUnitIndex].CurrentTile.Visible ? _waitTime : 0);

        var lastTurnUnit = _units[_currentUnitIndex];
        _currentUnitIndex = (_currentUnitIndex + 1) % _units.Count;

        var newTurnUnit = _units[_currentUnitIndex];

        //a friendly unit has moved -> update fog
        if (!lastTurnUnit.IsEnemy)
            GridManager.Instance.UpdateFog();

        if (lastTurnUnit.IsEnemy != newTurnUnit.IsEnemy)
        {
            if (newTurnUnit.IsEnemy)
                ChangeState(GameState.EnemyTurn);
            else
                ChangeState(GameState.PlayerTurn);
        }
    }

    private void Update()
    {

        switch (State)
        {
            case GameState.PrepareLevel:
                break;
            case GameState.StartGame:
                break;
            case GameState.EnemyTurn:
                while (State == GameState.EnemyTurn)
                {
                    if (Time.time < _waitUntil)
                        break;
                    ProcessUnitTurn();
                }
                break;
            case GameState.PlayerTurn:
            case GameState.PlayerChooseTarget:
                ProcessUnitTurn();
                break;
            case GameState.EndGame:
                break;
            default:
                break;
        }
    }

    public BossUnit Boss { get; set; }

    private PlayerUnit _player;

    public PlayerUnit Player
    {
        get
        {
            if (_player == null)
                _player = (PlayerUnit)_units.Where(unit => !unit.IsEnemy).FirstOrDefault();
            return _player;
        }
    }

    public void ChangeState(GameState newState)
    {
        State = newState;
        Log.Debug($"GameState changed to {State}", gameObject);
        switch (newState)
        {
            case GameState.PrepareLevel:
                GenerateLayout();
                break;
            case GameState.StartGame:
                HandleStart();
                break;
            case GameState.PlayerTurn:
                StartPlayerTurn();
                break;
            case GameState.EnemyTurn:
                StartEnemyTurn();
                break;
            case GameState.EndGame:
                break;
            case GameState.PlayerChooseTarget:
                break;
            default:
                break;
        }
    }

    private void GenerateLayout()
    {
        LevelDesignManager.Instance.GenerateWorld();
    }

    private void StartEnemyTurn()
    {
        UIManager.Instance.ToggleEnemyTurnMessage(true);
        HandDisplayer.Instance.Hide();
    }

    private void StartPlayerTurn()
    {
        UIManager.Instance.ToggleEnemyTurnMessage(false);
        HandDisplayer.Instance.DisplayUnitCards((PlayerUnit)_units[_currentUnitIndex]);
    }

    public void ClearUnits()
    {
        //clean
        foreach (var unit in _units)
            Destroy(unit.gameObject);
        _units.Clear();
    }

    private void HandleStart()
    {
        //start with player unit
        _currentUnitIndex = 0;
        while (_units[_currentUnitIndex].IsEnemy)
            _currentUnitIndex++;
        ChangeState(GameState.PlayerTurn);
        GridManager.Instance.UpdateFog();
    }

    public void EndGame(bool isWin)
    {
        UIManager.Instance.EndScreen.Show(isWin,StatisticsManager.Instance.GetStats());
        ChangeState(GameState.EndGame);
    }

    public enum GameState
    {
        PrepareLevel,
        StartGame,
        PlayerTurn,
        EnemyTurn,
        EndGame,
        PlayerChooseTarget
    }

    public List<GridUnit> GetUnits() => _units;

    public GridUnit currentUnit { get => _units[_currentUnitIndex];}
}
