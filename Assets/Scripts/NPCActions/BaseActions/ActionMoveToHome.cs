using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ActionMoveToHome : NPCActionBase
{
    public override bool CheckPlayability(NPCUnit caster)
    {
        if(caster.homeTile == null)
            return false;
        if(math.abs(caster.homeTile.x - caster.CurrentTile.x) < 10 && math.abs(caster.homeTile.y - caster.CurrentTile.y) < 10)
            return false;
        return GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.GetObject == null).
            Where(t => t.IsWalkable).Count() > 0;
    }

    public override void PerformAction(NPCUnit caster)
    {
        var newTile = GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).
            Where(t => t.GetObject == null).Where(t => t.IsWalkable).OrderBy(t => t.ManhattanDistance(caster.homeTile)).First();
        newTile?.Occupy(caster);
    }
}
