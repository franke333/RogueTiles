using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ItemType slotType;
    public Item item { get; private set; }

    public void SetItem(Item item)
    {
        this.item = item;
    }
}
