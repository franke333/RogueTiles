using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GridUnit : GridObject
{
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;

    private bool takingTurn = false;

    public int HpChange { get; private set; }

    public UnityEvent ChangeHealthEvent = new UnityEvent();

    public int hp { get => _hp; 
        set {
            if(value != _hp)
            {
                HpChange = value - _hp;
                _hp = value;
                ChangeHealthEvent.Invoke();
            }
        }
    }
    public int maxHp { get => _maxHp; }

    [SerializeField]
    protected bool _isEnemy;

    List<LingeringEffect> _activeEffects;

    [SerializeField]
    GameObject childrenRenderer;

    public virtual int visibleRange { get => 0; }
    public bool IsEnemy
    {
        get => _isEnemy;
    }

    // returns true if damage was taken
    public override bool TakeDamage(int dmg)
    {
        var ei = new EventInfo(EventType.TakeDamage,this, dmg);
        RaiseEvent(ei);
        dmg = Math.Max(ei.finalDamage, 0);
        hp -= dmg;
        Log.Info($"{alias} took {dmg} dmg", gameObject);
        if (hp <= 0)
            Die();
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
        if (hp == maxHp || amount == 0)
            return false;
        hp = Math.Min(hp + amount, maxHp);
        this.HealVFX();
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Heal);
        return true;
    }

    public void ApplyEffect(LingeringEffect effect)
    {
        if (effect.tag != "")
            for (int i = 0; i < _activeEffects.Count; i++)
            {
                if (_activeEffects[i].tag == effect.tag)
                {
                    _activeEffects[i] = effect;
                    return;
                }
            }
        _activeEffects.Add(effect);
    }

    public bool TakeTurn()
    {
        if (!takingTurn)
        {
            //turn started
            RaiseEvent(new EventInfo(EventType.StartTurn,this));
            takingTurn = true;
        }
        bool turnEnded = PlayTurn();
        if (turnEnded)
        {
            //turn ending
            RaiseEvent(new EventInfo(EventType.EndTurn,this));
            takingTurn = false;
        }
        return turnEnded;
    }

    public virtual void Init(int maxHp,bool enemy)
    {
        _maxHp = maxHp;
        _isEnemy = enemy;
        HpChange = 0;
        hp = maxHp;
        childrenRenderer = transform.GetChild(0).gameObject;
        _targetable = true;
    }

    protected bool IsSameFaction(GridUnit otherUnit)
        => otherUnit.IsEnemy == IsEnemy;

    protected bool IsOppositeFaction(GridUnit otherUnit)
        => !IsSameFaction(otherUnit);

    protected virtual bool PlayTurn()
    {
        Log.Error("Trying to TakeTurn in abstract GridUnit", gameObject);

        return true;
    }



    private void OnEnable()
    {
        _walkable = false;
        _activeEffects = new List<LingeringEffect>();
        GameManager.Instance.RegisterUnit(this);
        
    }

    protected virtual void RaiseEvent(EventInfo ei)
    {

        for(int i = 0; i < _activeEffects.Count; i++)
        {
            _activeEffects[i].DoEffect(ei);
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

    public override void SetVisible(bool value)
    {
        childrenRenderer.SetActive(value);
    }

    public void TakeDamageVFX()
    {
        var vfx = childrenRenderer.AddComponent<FadeColorVFX>();
        vfx.SetColor(Color.red);
    }

    public void HealVFX()
    {
        var vfx = childrenRenderer.AddComponent<FadeColorVFX>();
        vfx.SetColor(Color.green);
    }

}
