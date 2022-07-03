using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : SingletonClass<ItemGenerator>
{
    public List<Item> predefinedItems;
    [SerializeField]
    private GridItem gridItemGO;

    public Item GenerateItem()
    {
        //TODO: proc gen
        return MyRandom.Choice(predefinedItems);
    }

    public GridItem WrapItem(Item item)
    {
        var gi = Instantiate(gridItemGO);
        gi.item = item;
        return gi;
    }
}
