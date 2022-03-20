using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCActionBase : MonoBehaviour
{
    [SerializeField]
    protected int value;


    /// <summary>
    /// Returns true if the action makes sense and can be done.
    /// </summary>
    public virtual bool CheckPlayability(GridUnit caster)
    {
        Log.Error("CheckPlayablity was called in NPCActionBase", gameObject);
        return false;
    }

    /// <summary>
    /// Perform the action.
    /// </summary>
    public virtual void PerformAction(GridUnit caster)
    {
        Log.Error("PerformAction was called in NPCActionBase", gameObject);
    }
}
