using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent an item on the grid.
/// </summary>
public class GridItem : GridObject
{
    public Item AssignedItem;
    public override bool TakeDamage(int dmg)
    {
        return false;
    }

    /// <summary>
    /// Toggle rendering of the item icon atop of tile
    /// </summary>
    /// <param name="value">Visibility of the item</param>
    public override void SetVisible(bool value)
    {
        if(AssignedItem == null)
        {
            Log.Error("GridItem is missing an item", gameObject);
            return;
        }
        GetComponent<SpriteRenderer>().sprite = AssignedItem.sprite;
        base.SetVisible(value);
    }
}
