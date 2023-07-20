using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Item slot that holds an item in inventory (or pickup dialog)
/// </summary>
[Serializable]
public class ItemSlot
{
    public ItemType slotType;

    [SerializeField]
    private Item _item;
    public Item item { get => _item;}

    public void SetItem(Item item)
    {
        this._item = item;
    }

    public bool IsEmpty => item == null;
}
