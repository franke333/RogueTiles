using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : GridObject
{
    public Item item;
    public override bool TakeDamage(int dmg)
    {
        return false;
    }

    public override void SetVisible(bool value)
    {
        if(item == null)
        {
            Log.Error("GridItem is missing an item", gameObject);
            return;
        }
        GetComponent<SpriteRenderer>().sprite = item.sprite;
        base.SetVisible(value);
    }
}
