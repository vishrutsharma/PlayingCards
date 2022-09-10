using System;
using UnityEngine;
using System.Collections.Generic;

namespace JungleeCards.Models
{
    [Serializable]
    public enum SuiteType
    {
        SPADES,
        CLUBS,
        HEARTS,
        DIAMONDS
    }

    [Serializable]
    public class CardsData
    {
        public SuiteType suitType;
        public List<Texture2D> cards;
    }

    [Serializable]
    public class CardModel
    {
        public string cardId;

        public CardModel()
        {
            cardId = "Vish";
        }
    }
}