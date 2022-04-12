using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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


    public List<ActionEntry> ActionList { get => _actions; }
    protected override bool PlayTurn()
    {

        int weightSum = 0;
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
}
