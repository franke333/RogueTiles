using System.Collections;
using System.Collections.Generic;
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

    class TemporaryHPEffect : LingeringEffect
    {
        int tempHP;
        public TemporaryHPEffect(int tempHP, int duration)
        {
            this.tempHP = tempHP;
            this._duration = duration;
            _tag = "shield";
        }

        public override void DoEffect(EventInfo info)
        {
            if(info.eventType == EventType.TakeDamage)
            {
                int tempHPDamage = Mathf.Min(tempHP, info.finalDamage);
                tempHP -= tempHPDamage;
                info.finalDamage -= tempHPDamage;
                if (tempHP == 0)
                    this._discard = true;
            }
            base.DoEffect(info);
        }
    }

    public void DealDamage(int damage)
    {
        var data = CardData.ReadData();
        data.currentTarget.TakeDamage(damage);
    }

    public void GainTemporaryHealthFor10Turns(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp,10));
    }

    public void GainTemporaryHealthFor5Turns(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp,5));
    }

    public void GainTemporaryHealthFor2Turns(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp,2));
    }
}
