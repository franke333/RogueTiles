using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class ListExtensions
{
    /// <summary>
    /// Shuffles elements in a list
    /// </summary>
    /// <param name="list">List to be shuffled</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = MyRandom.Int(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Pick n distinct elements from a list.
    /// Warning. the list is going to be shuffled.
    /// Pass a copy of the list if you want to keep the original order.
    /// </summary>
    /// <returns>picked elements</returns>
    public static List<T> PickN<T>(this IList<T> list,int n)
    {

        List<T> picks = new List<T>();
        if (n >= list.Count) {
            foreach (T item in list)
                picks.Add(item);
            return picks;
        }
        list.Shuffle();
        for (int i = 0; i < n; i++)
        {
            picks.Add(list[i]);
        }
        return picks;
    } 
}

/// <summary>
/// Class that represents a card
/// </summary>
[CreateAssetMenu(fileName = "New Card",menuName = "Scriptables/Card")]
public class Card : ScriptableObject
{
    public enum AreaShape
    {
        Line, Circle, None
    }
    // ---------------------------
    // static values and methods for cardEffects
    public static Card playedCard;
    public static GridUnit currentUnit;
    public static GridObject currentTarget;

    public static void ClearData()
    {
        playedCard = null; currentTarget = null; currentUnit = null;
    }
    // ---------------------------
    public new string name;
    public Sprite sprite;
    [TextArea(3, 10)]
    public string description;

    public AreaShape shape;
    public int range;

    // item generation
    public int cost;
    public ItemType itemType;

    public bool needsTarget;
    public UnityEvent cardEffect;

    public AudioManager.SFXType audioType;
}
