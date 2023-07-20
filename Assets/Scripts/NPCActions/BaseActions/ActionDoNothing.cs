using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// skips turn
/// </summary>
public class ActionDoNothing : NPCActionBase
{
    public override bool CheckPlayability(NPCUnit caster)
    {
        return true;
    }

    public override void PerformAction(NPCUnit caster)
    {
        return;
    }
}
