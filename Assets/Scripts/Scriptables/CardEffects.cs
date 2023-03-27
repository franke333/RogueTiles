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
        public TemporaryHPEffect(int tempHP)
        {
            this.tempHP = tempHP;
            _tag = "tempHP";
        }

        public override void DoEffect(EventInfo info)
        {
            if(info.eventType == EventType.TakeDamage)
            {
                int tempHPDamage = Mathf.Max(tempHP, info.finalDamage);
                tempHP -= tempHPDamage;
                info.finalDamage -= tempHPDamage;
                if (tempHP == 0)
                    this._discard = true;
            }
        }
    }

    public void DealDamage(int damage)
    {
        var data = CardData.ReadData();
        data.currentTarget.TakeDamage(damage);
    }

    public void GainTemporaryHealth(int tmphp)
    {
        var data = CardData.ReadData();
        data.currentUnit.ApplyEffect(new TemporaryHPEffect(tmphp));
    }
}
