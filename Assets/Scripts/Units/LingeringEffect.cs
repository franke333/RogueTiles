using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public string Name {  get; protected set;}
    public string Description { get; protected set; }
    public Sprite Sprite { get; protected set; }
    public bool IsToBeDisplayed { get; protected set; }

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
        UpdateDescription();
        if (infinite || info.eventType != EventType.EndTurn)
            return;
        //tick
        _duration--;
        if (_duration <= 0)
            _discard = true;
    }

    protected virtual void UpdateDescription()
    {
        Description = "Missing description";
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

public class TemporaryHPEffect : LingeringEffect
{
    int tempHP;
    public TemporaryHPEffect(int tempHP, int duration,string name)
    {
        this.tempHP = tempHP;
        this._duration = duration;
        _tag = "shield";
        Name = name;
        IsToBeDisplayed = true;
        //TODO add sprite
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Temporary {tempHP} HP for ";
        if (_duration == 1)
            Description += "1 turn";
        else
            Description += $"{_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            int tempHPDamage = Mathf.Min(tempHP, info.finalDamage);
            tempHP -= tempHPDamage;
            info.finalDamage -= tempHPDamage;
            if (tempHP == 0)
                this._discard = true;
        }
        base.DoEffect(info);
        UpdateDescription();
    }
}

//Damage over Time
public class DOTEffect : LingeringEffect
{
    int damage;
    public DOTEffect(int damage, int duration, string name, string tag)
    {
        //TODO sprite
        this.damage = damage;
        this._duration = duration;
        _tag = tag;
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Deals {damage} damage each turn for ";
        if (_duration == 1)
            Description += "1 turn";
        else
            Description += $"{_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.EndTurn)
        {
            info.hostOfEffect.TakeDamage(damage);
        }
        //ticking down
        base.DoEffect(info);
    }
}

// unit takes extra const damage from all sources
public class WeaknessEffect : LingeringEffect
{
    int damage;
    public WeaknessEffect(int damage, int duration, string name)
    {
        //TODO sprite
        this.damage = damage;
        this._duration = duration;
        _tag = "weakness";
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Takes {damage} extra damage from all sources for ";
        if (_duration == 1)
            Description += "1 turn";
        else
            Description += $"{_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            info.finalDamage += damage;
        }
        //ticking down
        base.DoEffect(info);
    }
}

//unit takes reduced damage from all sources
public class ResistanceEffect : LingeringEffect
{
    int damageReduction;
    public ResistanceEffect(int damageReduction, int duration, string name)
    {
        //TODO sprite
        this.damageReduction = damageReduction;
        this._duration = duration;
        _tag = "damage reduction";
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Takes {damageReduction} less damage from all sources for ";
        if (_duration == 1)
            Description += "1 turn";
        else
            Description += $"{_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            info.finalDamage -= damageReduction;
        }
        //ticking down
        base.DoEffect(info);
    }
}   

public class FireArmorEffect : LingeringEffect
{
    int damageReturn;
    int damageReduction;
    public FireArmorEffect(int damageReturn,int damageReduction, int duration, string name)
    {
        //TODO sprite
        this.damageReturn = damageReturn;
        this.damageReduction = damageReduction;
        this._duration = duration;
        _tag = "damage reduction";
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Deals {damageReturn} damage to attacker while taking reduced damage by {damageReduction} for ";
        if (_duration == 1)
            Description += "1 turn";
        else
            Description += $"{_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            info.finalDamage -= damageReduction;
            if (GameManager.Instance.currentUnit != info.hostOfEffect)
                GameManager.Instance.currentUnit.TakeDamage(damageReturn);
        }
        //ticking down
        base.DoEffect(info);
    }
}

public class DodgeEffect : LingeringEffect
{
    int remainingDodges;
    float dodgeChance;
    public DodgeEffect(float dodgeChance, int duration, int maxDodges, string name)
    {
        //TODO sprite
        this.dodgeChance = dodgeChance;
        this.remainingDodges = maxDodges;
        this._duration = duration;
        _tag = "parry";
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Has {dodgeChance}% chance to dodge up to ";
        if(remainingDodges == 1)
            Description += "1 attack ";
        else
            Description += $"{remainingDodges} attacks ";
        if (_duration == 1)
            Description += "for 1 turn";
        else
            Description += $"for {_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            if(MyRandom.Float(0,1) < dodgeChance)
            {
                remainingDodges--;
                if (remainingDodges == 0)
                    _discard = true;
                info.finalDamage = 0;
            }
        }
        //ticking down
        base.DoEffect(info);
    }
}

public class DeflectEffect : LingeringEffect
{
    float chanceToDeflect;

    public DeflectEffect(float chanceToDeflect, int duration, string name)
    {
        //TODO sprite
        this.chanceToDeflect = chanceToDeflect;
        this._duration = duration;
        _tag = "parry";
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Has {chanceToDeflect}% chance to deflect a damage and deal it to a random close enemy target ";
        if (_duration == 1)
            Description += "for 1 turn";
        else
            Description += $"for {_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            if (MyRandom.Float(0, 1) < chanceToDeflect)
            {
                info.finalDamage = 0;
                List<GridUnit> targets = GameManager.Instance.GetUnits().Where(
                    gu => gu.ManhattanDistance(info.hostOfEffect.CurrentTile) <= 2 && gu != info.hostOfEffect).ToList();
                if (targets.Count > 0)
                {
                    GridUnit target = MyRandom.Choice(targets);
                    target.TakeDamage(info.baseDamage);
                    this._discard = true;
                }
            }
        }
        //ticking down
        base.DoEffect(info);
    }
}

public class ReverseDamageTakenToHealing : LingeringEffect
{
    public ReverseDamageTakenToHealing(int duration, string name)
    {
        //TODO sprite
        this._duration = duration;
        Name = name;
        IsToBeDisplayed = true;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        Description = $"Damage taken is turned into healing instead ";
        if (_duration == 1)
            Description += "for 1 turn";
        else
            Description += $"for {_duration} turns";
    }

    public override void DoEffect(EventInfo info)
    {
        if (info.eventType == EventType.TakeDamage)
        {
            info.hostOfEffect.RestoreHealth(info.finalDamage);
            info.finalDamage = 0;
        }
        //ticking down
        base.DoEffect(info);
    }
}