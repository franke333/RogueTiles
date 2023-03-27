using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buffs and Debuffs for characters that react to EventType events
/// </summary>
public abstract class LingeringEffect 
{
    protected string _tag;
    protected bool _discard;

    public bool discard { get => _discard; }

    /// <summary>
    /// Units have only single effect of the same tag (except tags=="")
    /// </summary>
    public string tag { get => _tag; }

    /// <summary>
    /// Checks if correct event happened, and if yes, applies the effect
    /// </summary>
    /// <param name="info"> Holds information about the invoking effect </param>
    public abstract void DoEffect(EventInfo info);

}
