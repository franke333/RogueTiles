using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cunsumable Items that heal the player on pickup
/// </summary>
[CreateAssetMenu(fileName = "New Consumable", menuName = "Scriptables/Consumable")]
[Serializable]
public class ConsumableItem : Item
{
    public int _healAmount;

    public void HealPlayer()
    {
        GameManager.Instance.Player.RestoreHealth(_healAmount);
    }
}
