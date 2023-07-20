using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class that represents a unit that is controlled by computer
/// </summary>
public class NPCUnit : GridUnit
{
    [System.Serializable]
    public struct ActionEntry
    {
        public int weight;
        public NPCActionBase actionObj;
    }

    [SerializeField]
    List<ActionEntry> _actions = new List<ActionEntry>();
    public ITile homeTile;

    public override void Init(int maxHp, bool enemy)
    {
        base.Init(maxHp, enemy);
        homeTile = CurrentTile;
    }

    public List<ActionEntry> ActionList { get => _actions; }
    protected override bool PlayTurn()
    {
        // implementation of utility system
        int weightSum = 0;
        // filter legal actions
        List<ActionEntry> performableActions = new List<ActionEntry>();
        foreach(var a in _actions)
        {
            if (a.actionObj.CheckPlayability(this))
            {
                weightSum += a.weight;
                performableActions.Add(a);
            }
        }
        if (weightSum == 0)
            return true;

        // perform one of legal actions
        int roll = MyRandom.Int(0, weightSum + 1);

        foreach(var a in performableActions)
        {
            roll -= a.weight;
            if(roll <= 0)
            {
                a.actionObj.PerformAction(this);
                return true;
            }
        }

        return true;
    }

    protected override void Die()
    {
        ITile tile = _currentTile;
        base.Die();
        //drop item
        tile.Occupy(ItemGenerator.Instance.WrapItem(ItemGenerator.Instance.GenerateItem()));
    }
}
