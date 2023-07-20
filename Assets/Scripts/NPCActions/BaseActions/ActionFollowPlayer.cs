using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Move towards player unit
/// </summary>
public class ActionFollowPlayer : NPCActionBase
{
    public override bool CheckPlayability(NPCUnit caster)
    {
        var player = GameManager.Instance.GetUnits().Where(unit => !unit.IsEnemy).First();
        if (player == null)
            return false;
        if (caster.ManhattanDistance(player.CurrentTile) > TribesManager.Instance.NPCDetectionRange)
            return false;
        if (caster.ManhattanDistance(player.CurrentTile) == 1)
            return false;
        return GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.GetObject == null).
            Where(t => t.IsWalkable).Count() > 0;

    }

    public override void PerformAction(NPCUnit caster)
    {
        var player = GameManager.Instance.GetUnits().Where(unit => !unit.IsEnemy).First();
        if (player == null)
            return;
        var newTile = GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.GetObject == null).
            Where(t => t.IsWalkable).OrderBy(t=>t.ManhattanDistance(player.CurrentTile)).First();
        newTile?.Occupy(caster);
    }
}
