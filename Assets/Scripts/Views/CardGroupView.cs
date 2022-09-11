using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JungleeCards.Controllers;
using JungleeCards.Models;

namespace JungleeCards.Views
{
    public class CardGroupView : EntityView
    {
        private List<string> containingCardsID;
        private bool isActiveInPool = false;
        private RectTransform rectTransform;
        private float desiredWidth = 0;

        private IEnumerator ScaleRect()
        {
            float cTime = 0;
            Vector2 currentSize = rectTransform.sizeDelta;
            while (cTime < 1)
            {
                rectTransform.sizeDelta = Vector2.Lerp(currentSize, new Vector2(desiredWidth, rectTransform.sizeDelta.y), cTime);
                yield return new WaitForEndOfFrame();
                cTime += Time.deltaTime / 0.1f;
            }
            rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.sizeDelta.y);

            if (rectTransform.sizeDelta.x == 0)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void UpdateRectSize()
        {
            if (containingCardsID.Count == 0)
            {
                return;
            }
            float cardWidth = this.GetController<CardController>().GetData<CardModel>().cardWidth;

            float cardOffsetDivisor = this.GetController<CardController>().GetData<CardModel>().cardPlacementDivisorOffset;

            float res1 = containingCardsID.Count * cardWidth;
            float res2 = (((containingCardsID.Count - 1) * cardWidth) / cardOffsetDivisor);
            desiredWidth = res1 - res2;

            if (containingCardsID.Count == 1)
            {
                desiredWidth = cardWidth;
            }
            StartCoroutine(ScaleRect());
        }


        public override void InitView(string id)
        {
            this.ID = id;
            containingCardsID = new List<string>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void AddCardWithID(string cardId)
        {
            containingCardsID.Add(cardId);
            UpdateRectSize();
        }

        public void RemoveCardWithID(string cardId)
        {
            for (int i = 0; i < containingCardsID.Count; i++)
            {
                if (containingCardsID[i] == cardId)
                {
                    containingCardsID.RemoveAt(i);
                    break;
                }
            }

            if (containingCardsID.Count == 0)
            {
                desiredWidth = 0;
                StartCoroutine(ScaleRect());
            }
            UpdateRectSize();
        }


        public float GetWidth()
        {
            return desiredWidth;
        }

        public List<string> GetContainingCards()
        {
            return containingCardsID;
        }

        public bool IfOverlaps(RectTransform cardRectTransform)
        {
            Rect rect1 = new Rect(cardRectTransform.position.x, cardRectTransform.position.y, cardRectTransform.rect.width, cardRectTransform.rect.height);
            Rect rect2 = new Rect(rectTransform.position.x, rectTransform.position.y, rectTransform.rect.width, rectTransform.rect.height);
            return rect1.Overlaps(rect2);
        }

    }
}
