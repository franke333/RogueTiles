using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionMove : NPCActionBase
{

    /*
    var tileEnum = GridManager.Instance.GetAdjecentTiles(CurrentTile).Where(tile => tile.IsWalkable).OrderBy(x => MyRandom.Int());
    if (tileEnum.Count()>0)
        tileEnum.First().Occupy(this);
    return true;
    */
    // Start is called before the first frame update

    public override bool CheckPlayability(GridUnit caster)
    {
        return GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.IsWalkable).Count() > 0;
    }

    public override void PerformAction(GridUnit caster)
    {
        var newTile = GridManager.Instance.GetAdjecentTiles(caster.CurrentTile).Where(t => t.IsWalkable)
            .OrderBy(x => MyRandom.Int()).First();
        newTile.Occupy(caster);
    }
}
