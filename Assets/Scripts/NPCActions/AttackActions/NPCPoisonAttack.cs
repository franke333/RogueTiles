using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCPoisonAttack : NPCActionBase
{
    [Space]
    [Header("Poison Attack Stats")]
    [SerializeField]
    int range;
    [SerializeField]
    bool onlySameColumnRow;
    [SerializeField]
    int initialDamage,overTimeDamage,overTimeDuration;

    public override bool CheckPlayability(NPCUnit caster)
    {
        var player = GameManager.Instance.GetUnits().Where(unit => !unit.IsEnemy).FirstOrDefault();
        if (player == null)
            return false;
        if (caster.ManhattanDistance(player.CurrentTile) > range)
            return false;
        if (onlySameColumnRow && caster.CurrentTile.x != player.CurrentTile.x && caster.CurrentTile.y != player.CurrentTile.y)
            return false;
        return true;
    }

    public override void PerformAction(NPCUnit caster)
    {
        var player = GameManager.Instance.GetUnits().Where(unit => !unit.IsEnemy).First();
        if (player == null)
            return;
        if(initialDamage > 0)
            player.TakeDamage(initialDamage);
        player.ApplyEffect(new DOTEffect(overTimeDamage,overTimeDuration,"Poison","poison"));
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Debuff);

    }
}
