using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JungleeCards.Models;
using JungleeCards.Views;
using JungleeCards.Common;

namespace JungleeCards.Controllers
{
    public class CardController : EntityController
    {
        [SerializeField] private CardView cardView;
        [SerializeField] private List<CardsData> cardsData;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject cardGroupPrefab;
        [SerializeField] private Transform cardsContainerTransform;
        private Dictionary<string, CardGroupView> cardGroups;
        private Dictionary<string, CardView> cards;

        private bool IsValidDeck()
        {
            if (cardsData.Count != 4 ||
            cardsData[0].cards.Count != 13 ||
            cardsData[1].cards.Count != 13 ||
            cardsData[2].cards.Count != 13 ||
            cardsData[3].cards.Count != 13)
            {
                return false;
            }

            for (int i = 0; i < cardsData.Count; i++)
            {
                for (int j = 0; j < cardsData[i].cards.Count; j++)
                {
                    if (cardsData[i].cards[j] == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void AlignCards()
        {

        }

        private void Groupify(CardGroupView cardGroupView, CardView cardView)
        {
            cardView.SetGroupID(cardGroupView.ID);
            cardGroupView.AddCardWithID(cardView.ID);
            AlignCards();
        }

        private void UnGroupify(CardGroupView cardGroupView, CardView cardView)
        {

        }

        public void PopulateDeck(DeckData deckData)
        {
            string[] cardsToShow = deckData.data.deck.ToArray();
            for (int i = 0; i < cardsToShow.Length; i++)
            {
                string cardName = cardsToShow[i];
                GameObject card = Instantiate(cardPrefab);
                card.transform.SetParent(cardsContainerTransform);
                CardView cView = card.GetComponent<CardView>();
                cView.InitView(cardName);
                cards.Add(cardName, cView);


                GameObject cardGroup = Instantiate(cardGroupPrefab);
                CardGroupView cGView = cardGroup.GetComponent<CardGroupView>();
                string groupId = GameStrings.cardGroupPrefix + (i + 1).ToString();
                cGView.InitView(groupId);
                cardGroups.Add(groupId, cGView);
                Groupify(cGView, cView);
            }
        }


        public override void OnGameStart()
        {
            if (IsValidDeck())
            {
                cardGroups = new Dictionary<string, CardGroupView>();
                cards = new Dictionary<string, CardView>();
            }
            else
            {
                Debug.LogError("Deck Mismatch");
            }
        }

        public override void OnGameOver()
        {

        }
    }
}
