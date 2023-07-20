using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for NPC actions.
/// </summary>
public abstract class NPCActionBase : MonoBehaviour
{
    [Header("Action Economy")]
    public int cost; // cost in coins
    public bool isBaseAction; // should be ignored in buy phase

    /// <summary>
    /// Returns true if the action makes sense and can be done.
    /// </summary>
    public abstract bool CheckPlayability(NPCUnit caster);

    /// <summary>
    /// Perform the action.
    /// </summary>
    public abstract void PerformAction(NPCUnit caster);

    
}
