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
        public List<Sprite> cards;
    }

    [Serializable]
    public class CardModel
    {

        public int autoGroupifySampleCount;
        public int autoGroupifyCardRange;
        public float groupOffset;
        public float cardPlacementDivisorOffset;
        public float cardWidth;
        public float cardHeight;
        public int minCardsCountToGroupify;
    }
}