using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a unit on the grid
/// </summary>
public abstract class GridUnit : GridObject
{
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;

    private bool _takingTurn = false;

    /// <summary>
    /// Last change in hp
    /// </summary>
    public int HpChange { get; private set; }

    /// <summary>
    /// Event invoked when hp changes
    /// </summary>
    public UnityEvent ChangeHealthEvent = new UnityEvent();

    /// <summary>
    /// Current hp of the unit
    /// </summary>
    public int HP { get => _hp; 
        set {
            if(value != _hp)
            {
                HpChange = value - _hp;
                _hp = value;
                ChangeHealthEvent.Invoke();
            }
        }
    }

    public int MaxHp { get => _maxHp; }

    [SerializeField]
    protected bool _isEnemy;

    List<LingeringEffect> _activeEffects;

    [SerializeField]
    GameObject _childrenRenderer;

    public virtual int VisibleRange { get => 0; }

    public bool IsEnemy
    {
        get => _isEnemy;
    }

    public override bool TakeDamage(int dmg)
    {
        // apply lingering effects through RaiseEvent method
        var ei = new EventInfo(EventType.TakeDamage,this, dmg);
        RaiseEvent(ei);

        // cancel negative damage
        dmg = Math.Max(ei.finalDamage, 0);
        HP -= dmg;
        Log.Info($"{Alias} took {dmg} dmg", gameObject);
        if (HP <= 0 && HP+dmg > 0)
            Die();

        // if any damage was dealt, play the damage vfx and sfx
        if (dmg != 0)
        {
            AudioManager.Instance.PlaySFX(AudioManager.SFXType.TakeDamage);
            this.TakeDamageVFX();
            return true;
        }
        return false;
    }

    public bool RestoreHealth(int amount)
    {
        if (HP == MaxHp || amount == 0)
            return false;

        // Hp cannot go above maxhp
        HP = Math.Min(HP + amount, MaxHp);
        this.HealVFX();
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Heal);
        return true;
    }

    /// <summary>
    /// Apply a lingering effect to this unit
    /// </summary>
    /// <param name="effect">Lingering effect to be applied</param>
    public void ApplyEffect(LingeringEffect effect)
    {
        // lingering effects with no tag (empty string) are stackable and are applied regardless
        if (effect.tag != "")
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                // if effect with same tag is already applied, replace it
                if (_activeEffects[i].tag == effect.tag)
                {
                    _activeEffects[i] = effect;
                    return;
                }
            }
        _activeEffects.Add(effect);
    }

    /// <summary>
    /// Method to process a turn for this unit
    /// </summary>
    /// <returns> True if the turn was finished by this unit</returns>
    public bool TakeTurn()
    {
        
        if (!_takingTurn)
        {
            //turn started event
            RaiseEvent(new EventInfo(EventType.StartTurn,this));
            _takingTurn = true;
        }

        // call specific unit's turn logic
        bool turnEnded = PlayTurn();

        if (turnEnded)
        {
            //turn ending event
            RaiseEvent(new EventInfo(EventType.EndTurn,this));
            _takingTurn = false;
        }
        return turnEnded;
    }

    /// <summary>
    /// Initialize the unit
    /// </summary>
    /// <param name="maxHp">MaxHP</param>
    /// <param name="enemy">Whether the unit is an enemy</param>
    public virtual void Init(int maxHp,bool enemy)
    {
        _maxHp = maxHp;
        _isEnemy = enemy;
        HpChange = 0;
        HP = maxHp;
        _childrenRenderer = transform.GetChild(0).gameObject;
        _targetable = true;
    }

    protected bool IsSameFaction(GridUnit otherUnit)
        => otherUnit.IsEnemy == IsEnemy;

    protected bool IsOppositeFaction(GridUnit otherUnit)
        => !IsSameFaction(otherUnit);

    /// <summary>
    /// Specific unit's turn logic
    /// </summary>
    /// <returns>True if the turn was finished by this unit</returns>
    protected virtual bool PlayTurn()
    {
        Log.Error("Trying to TakeTurn in abstract GridUnit", gameObject);

        return true;
    }



    private void OnEnable()
    {
        //autoamtically register unit on enable
        _walkable = false;
        _activeEffects = new List<LingeringEffect>();
        GameManager.Instance.RegisterUnit(this);
        
    }

    /// <summary>
    /// Event system to activate lingering effects
    /// </summary>
    /// <param name="ei">Info about the event</param>
    protected virtual void RaiseEvent(EventInfo ei)
    {

        for(int i = 0; i < _activeEffects.Count; i++)
        {
            _activeEffects[i].DoEffect(ei);
            // check if lingering effect should be discarded
            if (_activeEffects[i].discard)
                _activeEffects.RemoveAt(i--);
        }
    }

    protected virtual void Die()
    {
        GameManager.Instance.UnregisterUnit(this);
        _currentTile.Occupy(null);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Toggle visibility of the unit
    /// </summary>
    /// <param name="value">Whether the unit should be visible</param>
    public override void SetVisible(bool value)
    {
        _childrenRenderer.SetActive(value);
    }

    /// <summary>
    /// Display a vfx of the unit taking damage
    /// </summary>
    public void TakeDamageVFX()
    {
        var vfx = _childrenRenderer.AddComponent<FadeColorVFX>();
        vfx.SetColor(Color.red);
    }

    /// <summary>
    /// Display a vfx of the unit healing
    /// </summary>
    public void HealVFX()
    {
        var vfx = _childrenRenderer.AddComponent<FadeColorVFX>();
        vfx.SetColor(Color.green);
    }

}
