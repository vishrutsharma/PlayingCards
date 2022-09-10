using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JungleeCards.Models
{
    [Serializable]
    public class DeckData
    {
        public CurrentDeck data;
    }

    [Serializable]
    public class CurrentDeck
    {
        public List<string> deck;
    }

    public class GameModel
    {
        public CardModel cardModel;
    }
}
