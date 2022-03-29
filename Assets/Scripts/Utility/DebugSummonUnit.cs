using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSummonUnit : MonoBehaviour
{

    public GridUnit characterPrefab;
    public Vector2 pos;

    public bool Summon()
    {
        Log.Debug($"Debug summon of {characterPrefab} at {pos}", gameObject);
        var unit = Instantiate(characterPrefab);
        GridManager.Instance.GetTile(pos)?.Occupy(unit);
        if (unit.CurrentTile == null)
        {
            GameManager.Instance.UnregisterUnit(unit);
            Destroy(unit.gameObject);
            return false;
        }
        return true;

    }

    
}
