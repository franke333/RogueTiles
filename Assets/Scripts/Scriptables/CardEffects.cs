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

    public void DealDamage()
    {
        var data = CardData.ReadData();
        int dmg = data.playedCard.effectValue;
        data.currentTarget.TakeDamage(dmg);
    }
}
