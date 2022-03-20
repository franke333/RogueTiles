using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSummonUnit : MonoBehaviour
{

    public GridUnit characterPrefab;
    public Vector2 pos;

    public void Summon()
    {
        Log.Debug($"Debug summon of {characterPrefab} at {pos}", gameObject);
        var unit = Instantiate(characterPrefab);
        GridManager.Instance.GetTile(pos).Occupy(unit);
    }

    
}
