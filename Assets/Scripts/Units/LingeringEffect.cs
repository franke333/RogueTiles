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
    protected int _duration;
    protected bool _infinite = false;

    public bool discard { get => _discard; }

    /// <summary>
    /// Units have only single effect of the same tag (except tags=="")
    /// </summary>
    public string tag { get => _tag; }

    /// <summary>
    /// Checks if correct event happened, and if yes, applies the effect
    /// </summary>
    /// <param name="info"> Holds information about the invoking effect </param>
    public virtual void DoEffect(EventInfo info)
    {
        if(infinite)
            return;
        _duration--;
        if (_duration <= 0)
            _discard = true;
    }

    /// <summary>
    /// Tells the remaining duration in turns of the effect
    /// </summary>
    public int duration { get => _duration; }

    /// <summary>
    /// Tells if the effect is infinite
    /// </summary>
    public bool infinite { get => _infinite; }


}
