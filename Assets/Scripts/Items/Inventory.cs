using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory class that holds a list of ItemSlots
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<ItemSlot> slots;
    private void Start()
    {
        // init slots
        for (int i = 0; i < slots.Count; i++)
        {
            InventoryDisplayer.Instance.slotDisplayers[i].Link(slots[i]);
        }
    }
}
