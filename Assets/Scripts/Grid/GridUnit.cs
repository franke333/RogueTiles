using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GridUnit : GridObject
{
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;


    [SerializeField]
    protected bool _isEnemy;

    List<Effect> _activeEffects;

    public virtual int visibleRange { get => 0; }
    public bool IsEnemy
    {
        get => _isEnemy;
    }

    public override bool TakeDamage(int dmg)
    {
        var ei = new EventInfo(EventType.TakeDamage, dmg);
        RaiseEvent(ei);
        dmg = Math.Max(ei.finalDamage, 0);
        _hp -= dmg;
        Log.Info($"{alias} took {dmg} dmg", gameObject);
        if (_hp < 0)
            Die();
        return dmg != 0;
    }

    public void ApplyEffect(Effect effect)
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
        RaiseEvent(new EventInfo(EventType.StartTurn));
        bool res = PlayTurn();
        RaiseEvent(new EventInfo(EventType.EndTurn));
        return res;
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
        _activeEffects = new List<Effect>();
        GameManager.Instance.RegisterUnit(this);
        
    }

    protected void RaiseEvent(EventInfo ei)
    {

        for(int i = 0; i < _activeEffects.Count; i++)
        {
            _activeEffects[i].DoEffect(ei);
            if (_activeEffects[i].discard)
                _activeEffects.RemoveAt(i--);
        }
    }

    private void Die()
    {
        Log.Debug("Dying not yet implemented", gameObject);
        GameManager.Instance.UnregisterUnit(this);
        _currentTile.Deoccupy();
        gameObject.SetActive(false);
    }



   
}
