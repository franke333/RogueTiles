using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionMove : NPCActionBase
{
    public override bool CheckPlayability(NPCUnit caster)
    {
        return GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.GetObject == null).
            Where(t => t.IsWalkable).Count() > 0;
    }

    public override void PerformAction(NPCUnit caster)
    {
        var newTile = GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.GetObject == null).Where(t => t.IsWalkable)
            .OrderBy(x => MyRandom.Int()).First();
        newTile?.Occupy(caster);
    }
}
