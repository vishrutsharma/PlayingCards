using System;
using UnityEngine;
using System.Collections;
using JungleeCards.Views;
using JungleeCards.Common;
using JungleeCards.Models;
using System.Collections.Generic;

namespace JungleeCards.Controllers
{
    public class CardController : EntityController
    {
        #region ----------------------------- Serialize Fields ------------------------------
        [SerializeField] private List<CardsData> cardsData;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject cardGroupPrefab;
        [SerializeField] private Transform cardsContainerTransform;
        #endregion ---------------------------------------------------------------------------


        #region ----------------------------- Public Fields ----------------------------------
        public Color selectedCardColor;
        public Transform draggableAreaTransform;
        public Vector2 CardDimension { get; private set; } = Vector2.zero;
        #endregion ---------------------------------------------------------------------------


        #region ----------------------------- Private Fields ---------------------------------
        private List<CardGroupView> cardGroups;
        private List<CardView> cards;
        private List<string> selectedCardIds;
        private bool allowInput = false;
        #endregion ---------------------------------------------------------------------------

        #region ----------------------------- Private Methods ---------------------------------
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
            float cardWidth = CardDimension.x;
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
                            Debug.Log("Card is null");
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

        private Tuple<SuiteType, int> GetCardInfo(string cardName)
        {
            SuiteType suiteType = default(SuiteType);
            string cardNumber = cardName.Substring(1);
            int cardId = 0;

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
                    cardId = 1;
                    break;

                case "J":
                    cardId = 11;
                    break;

                case "Q":
                    cardId = 12;
                    break;

                case "K":
                    cardId = 13;
                    break;
                default:
                    cardId = System.Convert.ToInt16(cardNumber);
                    break;
            }
            return new System.Tuple<SuiteType, int>(suiteType, cardId);
        }

        private Sprite GetCardSprite(string cardName)
        {
            Tuple<SuiteType, int> data = GetCardInfo(cardName);

            for (int c = 0; c < cardsData.Count; c++)
            {
                if (cardsData[c].suitType == data.Item1)
                {
                    return cardsData[c].cards[data.Item2 - 1];
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
                int gIndex = UnityEngine.Random.Range(0, groupIndexes.Count);
                int groupIndex = groupIndexes[gIndex];
                groupIndexes.RemoveAt(gIndex);
                int cardRange = UnityEngine.Random.Range(5, cCount);

                if (cardRange < cardsIndexes.Count)
                {
                    for (int c = 0; c < cardRange; c++)
                    {
                        int cIndex = UnityEngine.Random.Range(0, cardsIndexes.Count);
                        int cardsIndex = cardsIndexes[cIndex];
                        cardsIndexes.RemoveAt(cIndex);
                        CardView cardView = cards[cardsIndex];
                        CardGroupView prevGroup = GetCardGroupWithID(cardView.CardGroupID);
                        CardGroupView newGroup = cardGroups[groupIndex];
                        if (prevGroup.ID != newGroup.ID)
                        {
                            UnGroupify(prevGroup, cardView);
                            Groupify(newGroup, cardView);
                        }
                    }
                }
            }

            for (int i = 0; i < cardGroups.Count; i++)
            {
                if (cardGroups[i].GetContainingCards().Count > 0)
                {
                    cardGroups[i].gameObject.SetActive(true);
                }
            }

            groupIndexes.Clear();
            cardsIndexes.Clear();
        }
        #endregion ---------------------------------------------------------------------------


        #region ----------------------------- Public Methods ---------------------------------
        public void PopulateDeck(DeckData deckData)
        {
            string[] cardsToShow = deckData.data.deck.ToArray();
            for (int i = 0; i < cardsToShow.Length; i++)
            {
                // Cards
                string cardName = cardsToShow[i];
                GameObject card = Instantiate(cardPrefab);
                CardView cView = card.GetComponent<CardView>();
                cView.SetController<CardController>(this);
                cView.InitView(cardName);
                cView.SetSprite(GetCardSprite(cardName));
                cards.Add(cView);

                if (CardDimension == Vector2.zero)
                {
                    CardDimension = new Vector2(card.GetComponent<RectTransform>().sizeDelta.x,
                                    card.GetComponent<RectTransform>().sizeDelta.y);
                }

                //Card Groups
                GameObject cardGroup = Instantiate(cardGroupPrefab);
                cardGroup.transform.SetParent(cardsContainerTransform);
                cardGroup.transform.localPosition = Vector3.zero;
                CardGroupView cGView = cardGroup.GetComponent<CardGroupView>();
                cGView.SetController<CardController>(this);
                string groupId = GameStrings.cardGroupPrefix + (i + 1).ToString();
                cGView.InitView(groupId);
                cardGroups.Add(cGView);

                card.transform.SetParent(cGView.transform);
                card.transform.localPosition = new Vector2(0, 0);
                Groupify(cGView, cView);
            }

            if (cards.Count <= this.GetData<CardModel>().autoGroupifySampleCount)
            {
                Debug.LogError("Cards in Deck are less than the Groups Sample Count");
                return;
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
                cardView.transform.localPosition = Vector2.zero;
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

        public string GetScore(List<string> cardsIds)
        {
            int score = 0;
            for (int i = 0; i < cardsIds.Count; i++)
            {
                Tuple<SuiteType, int> cardData = GetCardInfo(cardsIds[i]);
                score += cardData.Item2;
            }
            return score.ToString();
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
                CardGroupView group = GetCardGroupWithID(card.CardGroupID);
                card.transform.SetParent(group.transform);
                card.ResetToLastParent();
            }
            StartCoroutine(AlignDeck(0.2f));
        }

        public bool IsInputAllowed()
        {
            return selectedCardIds.Count > 0 ? false : true;
        }

        //Cards Max Count = 52 
        //Insertion,Deletion and Search operation worst case will take o(K) -> o(52)
        //Having a constant time complexity we can check for the card by iterating over the list instead of Hashing

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
        //Having a constant time complexity we can check for the card groups by iterating over the list instead of Hashing
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

        public override void OnGameOver()
        {

        }
        #endregion ---------------------------------------------------------------------------
    }
}
