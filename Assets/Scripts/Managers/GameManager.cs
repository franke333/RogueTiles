using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///  Manages the game state and the units
/// </summary>
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

    /// <summary>
    /// Register a unit to the game manager
    /// </summary>
    /// <param name="gu">Unit to be registered</param>
    public void RegisterUnit(GridUnit gu)
    {
        Log.Info($"Registered {gu}", gameObject);
        _units.Add(gu);
    }

    /// <summary>
    /// Unregister a unit from the game manager
    /// </summary>
    /// <param name="gu">Unit to be unregistered</param>
    public void UnregisterUnit(GridUnit gu)
    {
        
        Log.Info($"UNregistered {gu}", gameObject);
        // active untis managed by index. update it if needed
        if (_units.IndexOf(gu) <= _currentUnitIndex)
            _currentUnitIndex--;
        _units.Remove(gu);
    }


    private void ProcessUnitTurn()
    {
        // wait until animation is finished
        if (Time.time < _waitUntil)
            return;

        // start game if necessary
        if (State == GameState.StartGame)
            ChangeState(GameState.StartGame);

        // set camera focus to current unit
        if (State == GameState.PlayerTurn)
            CameraManager.Instance.SetFocusAt(_units[_currentUnitIndex].gameObject);

        // we are waiting on turn (animation or player)
        if (!_units[_currentUnitIndex].TakeTurn())
            return;

        // dont wait invisible units movement
        _waitUntil = Time.time +
            (_units[_currentUnitIndex].CurrentTile.Visible ? _waitTime : 0);

        // next unit
        var lastTurnUnit = _units[_currentUnitIndex];
        _currentUnitIndex = (_currentUnitIndex + 1) % _units.Count;

        var newTurnUnit = _units[_currentUnitIndex];

        //a friendly unit might moved -> update fog
        if (!lastTurnUnit.IsEnemy)
            GridManager.Instance.UpdateFog();

        // change gamestate if necessary
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
        // Process an unit turn each frame
        // NOTE: we could play turns of multiple (hidden) units at single frame,
        //      but there is no need due to size of our game

        switch (State)
        {
            case GameState.EnemyTurn:
                while (State == GameState.EnemyTurn)
                {
                    if (Time.time < _waitUntil)
                        break;
                    ProcessUnitTurn();
                }
                break;
            case GameState.PlayerTurn:
                ProcessUnitTurn();
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

    /// <summary>
    /// Change game state and handle it
    /// </summary>
    /// <param name="newState">New State</param>
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
            default:
                break;
        }
    }

    private void GenerateLayout()
    {
        // while generation fails, try again
        // fail is quite rare -> it is faster to generate new world
        int fails = 0;
        while (!LevelDesignManager.Instance.GenerateWorld())
        {
            //clean units
            foreach (var unit in _units)
                UnregisterUnit(unit);
            if(++fails >= 5)
            {
                Log.Error("Failed to generate world", gameObject);
                return;
            }
        }
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

    /// <summary>
    /// Clear the list of units
    /// </summary>
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
        // we need new intance when returning back to main menu
        // otherwise we lose component connections and settings in main menu would not work
        LevelDesignManager.Instance.RemoveManagerInstance();
    }

    /// <summary>
    /// Announce the end of the game
    /// </summary>
    /// <param name="isWin">Wheter the game ended with Victory for the player</param>
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
        EndGame
    }

    /// <summary>
    /// Get list of all units
    /// </summary>
    /// <returns>List of all units</returns>
    public List<GridUnit> GetUnits() => _units;

    /// <summary>
    /// Get the current unit that is taking a turn
    /// </summary>
    public GridUnit currentUnit { get => _units[_currentUnitIndex];}
}
