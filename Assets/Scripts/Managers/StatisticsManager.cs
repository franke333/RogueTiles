using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Counts the statistics of the game to be displayed at the end
/// </summary>
public class StatisticsManager : SingletonClass<StatisticsManager>
{
    public int TilesMove { get; private set; }
    public int TurnsTaken { get; private set; }
    public int DamageDealt { get; private set; }
    public int DamageTaken { get; private set; }
    public int HealthRestored { get; private set; }

    public int EnemiesKilled { get; private set; }

    private float timeStarted;

    private void Start()
    {
        timeStarted = Time.time;
        StartCoroutine(GameManager.WaitForStart(SetUpUnitListeners));
    }

    /// <summary>
    /// lingering effect sitting on the player unit to count the moves and turns taken by the player unit
    /// </summary>
    class StatisticalCaptureLingEff : LingeringEffect
    {
        public StatisticalCaptureLingEff()
        {
            _infinite = true;
        }
        public override void DoEffect(EventInfo info)
        {
            if (info.eventType == EventType.Move)
            {
                Instance.TilesMove++;
            }
            else if (info.eventType == EventType.EndTurn)
            {
                Instance.TurnsTaken++;
            }
            base.DoEffect(info);
        }
    }

    // Sets the listeners to count damage dealt (and other) to all units
    private void SetUpUnitListeners()
    {
        foreach (var unit in GameManager.Instance.GetUnits())
        {
            var copyOfUnit = unit;
            UnityAction callbackHpChange = () =>
            {
                if (copyOfUnit.IsEnemy)
                {
                    if (copyOfUnit.HpChange < 0)
                        DamageDealt -= copyOfUnit.HpChange;
                    if(copyOfUnit.HP <= 0)
                        EnemiesKilled++;
                }
                else
                {
                    if(copyOfUnit.HpChange < 0)
                        DamageTaken -= copyOfUnit.HpChange;
                    else
                        HealthRestored += copyOfUnit.HpChange;
                }
            };

            unit.ChangeHealthEvent.AddListener(callbackHpChange);

            //player unit
            if (!unit.IsEnemy)
                unit.ApplyEffect(new StatisticalCaptureLingEff());

        }
    }

    /// <summary>
    /// Returns the statistics of the game
    /// </summary>
    /// <returns>List of Tuples (name,value) of the recorded statistics</returns>
    public List<Tuple<string, int>> GetStats()
    {
        var stats = new List<Tuple<string, int>>();
        stats.Add(new Tuple<string, int>("Tiles moved", TilesMove));
        stats.Add(new Tuple<string, int>("Turns taken", TurnsTaken));
        stats.Add(new Tuple<string, int>("Damage dealt", DamageDealt));
        stats.Add(new Tuple<string, int>("Damage taken", DamageTaken));
        stats.Add(new Tuple<string, int>("Health restored", HealthRestored));
        stats.Add(new Tuple<string, int>("Enemies killed", EnemiesKilled));
        stats.Add(new Tuple<string, int>("Time taken", (int)(Time.time - timeStarted)));
        return stats;
    }

}
