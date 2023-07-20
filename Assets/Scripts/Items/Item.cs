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

/// <summary>
/// An Item that can be equipped by the player and hold cards
/// </summary>
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

    /// <summary>
    /// Generate an equipable item using the coins as a guide how strong the item should be
    /// </summary>
    /// <param name="coins">coins to spend on cards</param>
    /// <returns>generated item</returns>
    public static Item GenerateEquip(int coins)
    {
        //TODO create better names using prepared words (f.e. in .txt file)

        Item item = (Item)ScriptableObject.CreateInstance(typeof(Item));
        item.itemType = (ItemType)MyRandom.Int(1, 6);
        item.itemName = MyRandom.String(3, 6) + " " + item.itemType.ToString();

        item.sprite = ItemGenerator.Instance.GetSrpite(item.itemType);
       
        // Load cards from /Resources/ and filter by type
        var cardPool = Resources.LoadAll<Card>("Player Cards").Where(c => c.itemType == item.itemType || c.itemType == ItemType.Any).ToList();
        while (coins > 0)
        {
            // filter cards that can be bought with the remaining coins and choose one at random
            item.cards.Add(MyRandom.Choice(cardPool.Where(c => coins - c.cost >= 0).ToList()));
            // pay for card
            coins -= item.cards[item.cards.Count - 1].cost;
            // for the last card, buy the most expensive card that can be bought
            if(item.cards.Count + 1 == maxCardsPerItem && coins > 0)
            {
                //try to maximize the value of the last card
                var cardsWithoutGoingOver = cardPool.Where(c => coins - c.cost >= 0);
                var maxCost = cardsWithoutGoingOver.Max(c => c.cost);
                item.cards.Add(MyRandom.Choice(cardsWithoutGoingOver.Where(c => c.cost == maxCost).ToList()));
                break;
            }
        }

        return item;
    }
}