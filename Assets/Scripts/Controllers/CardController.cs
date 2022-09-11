using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JungleeCards.Models;
using JungleeCards.Views;
using JungleeCards.Common;
using UnityEngine.UI;

namespace JungleeCards.Controllers
{
    public class CardController : EntityController
    {
        [SerializeField] private List<CardsData> cardsData;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject cardGroupPrefab;
        [SerializeField] private Transform cardsContainerTransform;

        public Transform draggableAreaTransform;


        private List<CardGroupView> cardGroups;
        private List<CardView> cards;
        private List<string> selectedCardIds;
        private bool allowInput = false;

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

        private IEnumerator AlignDeck(float delay)
        {
            float cardWidth = GetData<CardModel>().cardWidth;
            float offsetDivisor = GetData<CardModel>().cardPlacementDivisorOffset;
            float sample = (cardWidth / 2) / offsetDivisor;

            yield return new WaitForSeconds(delay);
            for (int i = 0; i < cardGroups.Count; i++)
            {
                if (cardGroups[i].gameObject.activeInHierarchy)
                {
                    CardGroupView cardGroup = cardGroups[i];
                    float width = cardGroup.GetWidth();

                    List<string> cardsInGroup = cardGroup.GetContainingCards();
                    if (cardsInGroup.Count == 1)
                    {
                        CardView card = GetCardWithID(cardsInGroup[0]);
                        card.transform.position = cardGroup.transform.position;
                        continue;
                    }

                    float currentPosX = 0;
                    for (int c = 0; c < cardsInGroup.Count; c++)
                    {
                        CardView card = GetCardWithID(cardsInGroup[c]);
                        if (card != null)
                        {
                            if (c == 0)
                            {
                                currentPosX = (-cardGroup.GetWidth() / 2) + cardWidth / 2;
                            }
                            card.MoveToPosition(new Vector2(currentPosX, 0));
                            currentPosX += sample;
                        }
                        else
                        {
                            Debug.Log("CArd is null");
                        }
                    }
                }
            }
        }

        private void Groupify(CardGroupView cardGroupView, CardView cardView)
        {
            if (!cardGroupView.gameObject.activeInHierarchy)
            {
                cardGroupView.gameObject.SetActive(true);
            }
            cardView.SetGroupID(cardGroupView.ID);
            cardGroupView.AddCardWithID(cardView.ID);
            cardView.transform.SetParent(cardGroupView.transform);
            cardView.transform.SetAsLastSibling();
            cardView.transform.localPosition = Vector2.zero;
        }

        private void UnGroupify(CardGroupView cardGroupView, CardView cardView)
        {
            cardView.ResetCardStatus();
            cardView.transform.SetParent(null);
            cardGroupView.RemoveCardWithID(cardView.ID);
            cardView.SetGroupID(null);
        }

        private CardGroupView GetEmptyGroupFromPool()
        {
            for (int i = 0; i < cardGroups.Count; i++)
            {
                if (!cardGroups[i].gameObject.activeInHierarchy)
                {
                    cardGroups[i].gameObject.SetActive(true);
                    return cardGroups[i];
                }
            }
            return null;
        }

        private Sprite GetCardSprite(string cardName)
        {
            SuiteType suiteType = default(SuiteType);
            string cardNumber = cardName.Substring(1);
            int cardIndex = 0;

            switch (cardName[0])
            {
                case 'S':
                    suiteType = SuiteType.SPADES;
                    break;

                case 'C':
                    suiteType = SuiteType.CLUBS;
                    break;

                case 'D':
                    suiteType = SuiteType.DIAMONDS;
                    break;

                case 'H':
                    suiteType = SuiteType.HEARTS;
                    break;
            }


            switch (cardNumber)
            {
                //Minus 1 for index for J ,Q and K
                case "A":
                    cardIndex = 0;
                    break;

                case "J":
                    cardIndex = 10;
                    break;

                case "Q":
                    cardIndex = 11;
                    break;

                case "K":
                    cardIndex = 12;
                    break;
                default:
                    cardIndex = System.Convert.ToInt16(cardNumber);
                    break;
            }


            for (int i = 0; i < cardsData.Count; i++)
            {
                if (cardsData[i].suitType == suiteType)
                {
                    return cardsData[i].cards[cardIndex];
                }
            }
            return null;
        }

        private void AutoGroupify()
        {
            List<int> groupIndexes = new List<int>();
            List<int> cardsIndexes = new List<int>();
            for (int i = 0; i < this.cardGroups.Count; i++)
            {
                groupIndexes.Add(i);
                cardsIndexes.Add(i);
            }

            int gCount = this.GetData<CardModel>().autoGroupifySampleCount;
            int cCount = this.GetData<CardModel>().autoGroupifyCardRange;
            for (int i = 0; i < gCount; i++)
            {
                int gIndex = Random.Range(0, groupIndexes.Count);
                int groupIndex = groupIndexes[gIndex];
                groupIndexes.RemoveAt(gIndex);
                int cardRange = Random.Range(0, cCount);

                if (cardRange >= cardsIndexes.Count)
                {
                    break;
                }

                for (int c = 0; c < cardRange; c++)
                {
                    int cIndex = Random.Range(0, cardsIndexes.Count);
                    int cardsIndex = cardsIndexes[cIndex];
                    cardsIndexes.RemoveAt(cIndex);
                    CardView cardView = cards[cardsIndex];
                    UnGroupify(GetCardGroupWithID(cardView.CardGroupID), cardView);
                    CardGroupView groupView = cardGroups[groupIndex];
                    Groupify(groupView, cardView);
                }
            }

            groupIndexes.Clear();
            cardsIndexes.Clear();
        }

        public void PopulateDeck(DeckData deckData)
        {
            string[] cardsToShow = deckData.data.deck.ToArray();
            for (int i = 0; i < cardsToShow.Length; i++)
            {
                string cardName = cardsToShow[i];
                GameObject card = Instantiate(cardPrefab);
                CardView cView = card.GetComponent<CardView>();
                cView.SetController<CardController>(this);
                cView.InitView(cardName);
                cView.SetSprite(GetCardSprite(cardName));
                cards.Add(cView);


                GameObject cardGroup = Instantiate(cardGroupPrefab);
                cardGroup.transform.SetParent(cardsContainerTransform);
                cardGroup.transform.localPosition = Vector3.zero;
                CardGroupView cGView = cardGroup.GetComponent<CardGroupView>();
                cGView.SetController<CardController>(this);
                string groupId = GameStrings.cardGroupPrefix + (i + 1).ToString();
                cGView.InitView(groupId);
                cardGroups.Add(cGView);

                card.transform.SetParent(cGView.transform);
                card.transform.localPosition = Vector3.zero;
                Groupify(cGView, cView);
            }
            AutoGroupify();
            StartCoroutine(AlignDeck(0.5f));
        }

        public void OnCardStateChanged(string id, bool isSelected)
        {
            if (!isSelected)
            {
                if (selectedCardIds.Contains(id))
                {
                    selectedCardIds.RemoveAt(selectedCardIds.IndexOf(id));
                }
            }
            else
            {
                selectedCardIds.Add(id);
            }

            if (selectedCardIds.Count >= this.GetData<CardModel>().minCardsCountToGroupify)
            {
                gameController.ToggleGroupButton(true);
            }
            else
            {
                gameController.ToggleGroupButton(false);
            }
        }

        public void MakeNewGroup()
        {
            CardGroupView emptyGroup = GetEmptyGroupFromPool();
            if (cardGroupPrefab == null)
            {
                gameController.ToggleGroupButton(false);
                selectedCardIds.Clear();
                Debug.Log("Cannot make new group");
                return;
            }

            emptyGroup.transform.SetAsLastSibling();
            for (int i = 0; i < selectedCardIds.Count; i++)
            {
                CardView cardView = GetCardWithID(selectedCardIds[i]);
                CardGroupView groupView = GetCardGroupWithID(cardView.CardGroupID);
                UnGroupify(groupView, cardView);
                Groupify(emptyGroup, cardView);
            }
            gameController.ToggleGroupButton(false);
            selectedCardIds.Clear();
            StartCoroutine(AlignDeck(0.1f));
        }


        public override void OnGameStart()
        {
            if (IsValidDeck())
            {
                cardGroups = new List<CardGroupView>();
                cards = new List<CardView>();
                selectedCardIds = new List<string>();
            }
            else
            {
                Debug.LogError("Deck Mismatch");
            }
        }


        public void OnCardDragComplete(CardView card)
        {
            // Check for Overlap
            bool hasOverlapped = false;
            for (int i = 0; i < cardGroups.Count; i++)
            {
                if (cardGroups[i].gameObject.activeInHierarchy &&
                    card.CardGroupID != cardGroups[i].ID &&
                    cardGroups[i].IfOverlaps(card.GetComponent<RectTransform>()))
                {
                    UnGroupify(GetCardGroupWithID(card.CardGroupID), card);
                    Groupify(cardGroups[i], card);
                    hasOverlapped = true;
                    break;
                }
            }
            if (!hasOverlapped)
            {
                card.transform.SetParent(GetCardGroupWithID(card.CardGroupID).transform);
                card.transform.SetAsLastSibling();
            }
            StartCoroutine(AlignDeck(0.2f));
        }

        public bool IsInputAllowed()
        {
            return selectedCardIds.Count > 0 ? false : true;
        }

        public override void OnGameOver()
        {

        }


        //Cards Max Count = 52 
        //Insertion,Deletion and Search operation worst case will take o(K) -> o(52)
        //Having a constant time complexity we can check for the card by iterating over the list 

        public CardView GetCardWithID(string id)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (id == cards[i].ID)
                {
                    return cards[i];
                }
            }
            return null;
        }


        //Cards group Max Count = 52 as every single card can have 1 group 
        //Insertion,Deletion and Search operation worst case will take o(K) -> o(52)
        //Having a constant time complexity we can check for the card groups by iterating over the list 
        public CardGroupView GetCardGroupWithID(string id)
        {
            for (int i = 0; i < cardGroups.Count; i++)
            {
                if (id == cardGroups[i].ID)
                {
                    return cardGroups[i];
                }
            }
            return null;
        }


    }
}
