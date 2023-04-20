using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
