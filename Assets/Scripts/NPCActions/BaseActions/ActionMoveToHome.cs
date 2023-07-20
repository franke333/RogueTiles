using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Move towards home tile if far away
/// </summary>
public class ActionMoveToHome : NPCActionBase
{
    public override bool CheckPlayability(NPCUnit caster)
    {
        if(caster.homeTile == null)
            return false;
        if(caster.CurrentTile.ManhattanDistance(caster.homeTile) < 10)
            return false;
        return GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.GetObject == null || t.GetObject.IsWalkable).
            Where(t => t.IsWalkable).Count() > 0;
    }

    public override void PerformAction(NPCUnit caster)
    {
        var newTile = GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).
            Where(t => t.GetObject == null || t.GetObject.IsWalkable).Where(t => t.IsWalkable).OrderBy(t => t.ManhattanDistance(caster.homeTile)).First();
        newTile?.Occupy(caster);
    }
}
