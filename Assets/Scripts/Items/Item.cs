using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public enum ItemType
{
    Consumable,
    Armor,
    Melee,
    Ranged,
    Magic,
    Trinket,
    Any
}

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptables/Item")]
[Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public ItemType itemType;
    public List<Card> cards;
    public List<LingeringEffect> effects;
    
    const int maxCardsPerItem = 4;

    protected Item()
    {
        cards = new List<Card>();
        effects = new List<LingeringEffect>();
    }

    public static Item GenerateEquip(int value)
    {
        //TODO create better names in .txt file

        Item item = (Item)ScriptableObject.CreateInstance(typeof(Item));
        item.itemType = (ItemType)MyRandom.Int(1, 6);
        item.itemName = MyRandom.String(3, 6) + " " + item.itemType.ToString();

        item.sprite = ItemGenerator.Instance.GetSrpite(item.itemType);

        var cardPool = Resources.LoadAll<Card>("Player Cards").Where(c => c.itemType == item.itemType || c.itemType == ItemType.Any).ToList();
        while (value > 0)
        {
            item.cards.Add(MyRandom.Choice(cardPool.Where(c => value - c.cost >= 0).ToList()));
            value -= item.cards[item.cards.Count - 1].cost;
            if(item.cards.Count + 1 == maxCardsPerItem && value > 0)
            {
                //try to maximize the value of the last card
                var cardsWithoutGoingOver = cardPool.Where(c => value - c.cost >= 0);
                var maxCost = cardsWithoutGoingOver.Max(c => c.cost);
                item.cards.Add(MyRandom.Choice(cardsWithoutGoingOver.Where(c => c.cost == maxCost).ToList()));
                break;
            }
        }

        return item;
    }
}