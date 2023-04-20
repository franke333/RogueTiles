using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCActionBase : MonoBehaviour
{

    /// <summary>
    /// Returns true if the action makes sense and can be done.
    /// </summary>
    public abstract bool CheckPlayability(NPCUnit caster);

    /// <summary>
    /// Perform the action.
    /// </summary>
    public abstract void PerformAction(NPCUnit caster);
}
