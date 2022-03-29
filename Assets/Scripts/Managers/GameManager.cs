using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonClass<GameManager>
{
    List<GridUnit> _units;
    int _currentUnitIndex;

    [SerializeField]
    float _waitTime = 0.5f;
    float _waitUntil;

    [SerializeField]
    public GameState State { get; private set; }


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
            case GameState.PlayerTurn:
            case GameState.EnemyTurn:
            case GameState.PlayerChooseTarget:
                ProcessUnitTurn();
                break;
            case GameState.EndGame:
                break;
            default:
                break;
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
                HandleEnd();
                break;
            case GameState.PlayerChooseTarget:
                break;
            default:
                break;
        }
    }

    private void GenerateLayout()
    {
        //TODO

        //Graph g = Graph.Simple();
        //Graph g = Graph.Walk(new Vector2(4,4),0.43f,2);

        /*Func<List<Room>> func = 
            () => Graph.Walk(new Vector2(4, 4), 0.43f, 2).GenerateLayout(8,8,2,4,0.25f);

        GridManager.Instance.GenerateLevel(func, null);
        
        ChangeState(GameState.StartGame);
        */
    }

    private void StartEnemyTurn()
    {
        //TODO show message: "enemy is taking his turn" and start timer
        UIManager.Instance.ToggleEnemyTurnMessage(true);
        HandDisplayer.Instance.Hide();
    }

    private void StartPlayerTurn()
    {
        //TODO check if timer reached x seconds and start player turn
        UIManager.Instance.ToggleEnemyTurnMessage(false);
        HandDisplayer.Instance.DisplayUnitCards((PlayerUnit)_units[_currentUnitIndex]);
    }

    private void HandleStart()
    {
        //clean
        foreach (var unit in _units)
            Destroy(unit.gameObject);
        _units.Clear();
        


        foreach (DebugSummonUnit o in GameObject.FindObjectsOfType(typeof(DebugSummonUnit)))
            o.Summon();

        _currentUnitIndex = 0;
        if (_units[_currentUnitIndex].IsEnemy)
            ChangeState(GameState.EnemyTurn);
        else
            ChangeState(GameState.PlayerTurn);
        GridManager.Instance.UpdateFog();
    }

    private void HandleEnd()
    {
        //TODO
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
}
