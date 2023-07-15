using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    private struct CardData
    {
        public readonly Card playedCard;
        public readonly GridUnit currentUnit;
        public readonly GridObject currentTarget;

        private CardData(Card playedCard, GridUnit currentUnit, GridObject currentTarget)
        {
            this.playedCard = playedCard;
            this.currentUnit = currentUnit;
            this.currentTarget = currentTarget;
        }

        public static CardData ReadData()
            => new CardData(Card.playedCard, Card.currentUnit, Card.currentTarget);
    }

    // ---- MELEE ----

    public void DealDamage(int damage)
    {
        var data = CardData.ReadData();
        data.currentTarget.TakeDamage(damage);
    }

    public void Rend()
    {
        var data = CardData.ReadData();
        //apply if target is GridUnit
        if (data.currentTarget is GridUnit)
        {
            GridUnit unit = data.currentTarget as GridUnit;
            unit.ApplyEffect(new DOTEffect(1,5,data.playedCard.name,""));
        }
    }

    public void Cleave()
    {
        var data = CardData.ReadData();
        //apply if target is GridUnit
        if (data.currentTarget is GridUnit)
        {
            GridUnit unit = data.currentTarget as GridUnit;
            unit.ApplyEffect(new DOTEffect(1, 3, data.playedCard.name, ""));
        }
        data.currentTarget.TakeDamage(2);
    }

    public void DeepWound()
    {
        var data = CardData.ReadData();
        //apply if target is GridUnit
        if (data.currentTarget is GridUnit)
        {
            GridUnit unit = data.currentTarget as GridUnit;
            unit.ApplyEffect(new DOTEffect(2, 3, data.playedCard.name, ""));
        }
        data.currentTarget.TakeDamage(1);
    }

    // ---- RANGED ----

    readonly static Tuple<int, int>[] _RANGE_OF_DAMAGES =
    {
        new Tuple<int, int>(1, 3), //arrow shot
        new Tuple<int, int>(2, 4), //powered arrow shot
        new Tuple<int, int>(0, 3), //snipe

    };

    public void DealDamageInRange(int rangeOfDamageIndex)
    {
        if(rangeOfDamageIndex < 0 || rangeOfDamageIndex >= _RANGE_OF_DAMAGES.Length)
            throw new ArgumentOutOfRangeException(nameof(rangeOfDamageIndex));

        var data = CardData.ReadData();
        var (min, max) = _RANGE_OF_DAMAGES[rangeOfDamageIndex];
        data.currentTarget.TakeDamage(MyRandom.Int(min,max+1));
    }

    // ---- MAGIC ----

    public void RingOfFire()
    {
        var data = CardData.ReadData();
        foreach (var enemy in GameManager.Instance.GetUnits().Where(u => u.IsEnemy)
            .Where(u => 2 >= u.ManhattanDistance(data.currentUnit.CurrentTile)).ToList())
        {
            enemy.TakeDamage(1);
        }
    }

    public void SyphonLife()
    {
        var data = CardData.ReadData();
        data.currentTarget.TakeDamage(2);
        data.currentUnit.RestoreHealth(1);
    }

    public void CurseOfWeakness()
    {
        var data = CardData.ReadData();
        if (data.currentTarget is GridUnit)
        {
            GridUnit unit = data.currentTarget as GridUnit;
            unit.ApplyEffect(new WeaknessEffect(1,5, data.playedCard.name));
            AudioManager.Instance.PlaySFX(AudioManager.SFXType.Debuff);
        }
    }

    public void ConsumingFlame()
    {
        var data = CardData.ReadData();
        data.currentTarget.TakeDamage(5);
        data.currentUnit.TakeDamage(MyRandom.Int(0, 3));
    }

    // ---- ARMOR ----

    public void ArmorUp()
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new ResistanceEffect(1,5, data.playedCard.name));
    }

    public void BraceYourselves()
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new ResistanceEffect(2, 3, data.playedCard.name));
    }

    public void GainTemporaryHealthFor10Turns(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp,10,data.playedCard.name));
    }

    public void GainTemporaryHealthFor5Turns(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp,5, data.playedCard.name));
    }

    public void GainTemporaryHealthFor2Turns(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp,2, data.playedCard.name));
    }

    // ---- TRINKET ----

    public void FireArmor()
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new FireArmorEffect(1,1,3, data.playedCard.name));
    }

    public void SyphonSoul()
    {
        var data = CardData.ReadData();
        data.currentTarget.TakeDamage(1);
        if(data.currentTarget is GridUnit)
        {
            GridUnit unit = data.currentTarget as GridUnit;
            if(unit.hp <= 0)
                data.currentUnit.RestoreHealth(3);
        }
    }

    public void Dodge()
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new DodgeEffect(0.50f,3,3, data.playedCard.name));
    }

    public void Deflect()
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new DeflectEffect(0.35f,3, data.playedCard.name));
    }

    public void MindGames()
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new ReverseDamageTakenToHealing(1, data.playedCard.name));
    }
}
