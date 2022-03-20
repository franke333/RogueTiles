using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    TakeDamage, DealDamage, StartTurn, EndTurn, Move
}

public static class Vector2Extensions
{
    public static float ManhattanDistance(this Vector2 v1, Vector2 v2)
        => Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
}


/// <summary>
/// Info about the event occuring to be passed to effects
/// </summary>
public class EventInfo
{
    public readonly EventType eventType;

    public readonly int baseDamage;
    public int finalDamage;

    public EventInfo(EventType eventType,int damage=0)
    {
        this.eventType = eventType;
        baseDamage = damage;
        finalDamage = damage;
    }
}