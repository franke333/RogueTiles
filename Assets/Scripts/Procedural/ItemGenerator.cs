using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : SingletonClass<ItemGenerator>
{
    [SerializeField]
    private GridItem gridItemGO;

    [SerializeField]
    private List<Sprite> _spritesMelee, _spritesRanged, _spritesMagic, _spritesArmor, _spritesTrinket;

    [SerializeField]
    private ConsumableItem _foodItem, _bandageItem, _potionItem;

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

    public GridItem WrapItem(Item item)
    {
        var gi = Instantiate(gridItemGO);
        gi.GetComponent<SpriteRenderer>().sprite = item.sprite;
        gi.AssignedItem = item;
        return gi;
    }
}
