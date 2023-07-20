using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper class for Item generation
/// </summary>
public class ItemGenerator : SingletonClass<ItemGenerator>
{
    [SerializeField]
    private GridItem gridItemGO;

    [SerializeField]
    private List<Sprite> _spritesMelee, _spritesRanged, _spritesMagic, _spritesArmor, _spritesTrinket;

    [SerializeField]
    private ConsumableItem _foodItem, _bandageItem, _potionItem;

    /// <summary>
    /// Get item sprite by its type
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <returns>Sprite that corrsponds to the type of the item</returns>
    public Sprite GetSrpite(ItemType type)
    {
        switch(type)
        {
            case ItemType.Ranged:
                return _spritesRanged[MyRandom.Int(0, _spritesRanged.Count)];
            case ItemType.Melee:
                return _spritesMelee[MyRandom.Int(0, _spritesMelee.Count)];
            case ItemType.Magic:
                return _spritesMagic[MyRandom.Int(0, _spritesMagic.Count)];
            case ItemType.Armor:
                return _spritesArmor[MyRandom.Int(0, _spritesArmor.Count)];
            case ItemType.Trinket:
                return _spritesTrinket[MyRandom.Int(0, _spritesTrinket.Count)];
            default:
                return null;
        }
    }

    /// <summary>
    /// Choose which item should be created
    /// </summary>
    public Item GenerateItem()
    {
        int random = MyRandom.Int(0, 100);
        if(random < 50)
        {
            return Item.GenerateEquip(5); //50%
        }
        else if(random < 75)
        {
            return _foodItem; //25%
        }
        else if(random < 90)
        {
            return _bandageItem; //15%
        }
        else
        {
            return _potionItem; //10%
        }
        
        
    }

    /// <summary>
    /// Wrap an item in a GridItem Object
    /// </summary>
    /// <param name="item">Item to be wrapped</param>
    /// <returns>GridItem Object that cna be placed on tile</returns>
    public GridItem WrapItem(Item item)
    {
        var gi = Instantiate(gridItemGO);
        gi.GetComponent<SpriteRenderer>().sprite = item.sprite;
        gi.AssignedItem = item;
        return gi;
    }
}
