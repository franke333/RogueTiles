using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnit : NPCUnit
{
    protected override void Die()
    {
        base.Die();
        GameManager.Instance.EndGame(true);
    }

}
