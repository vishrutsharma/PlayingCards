using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JungleeCards.Models;
using JungleeCards.Common;
using JungleeCards.Controllers;
using System.Collections.Generic;

namespace JungleeCards.Views
{
    public class CardGroupView : EntityView
    {
        #region ------------------------------ Serialize Fields ---------------------------------------
        [SerializeField] private Text pointsText;

        #endregion--------------------------------------------------------------------------------------


        #region ------------------------------ Private Fields ------------------------------------------
        private List<string> containingCardsID;
        private RectTransform rectTransform;
        private float desiredWidth = 0;
        private float scaleAnimDuration = 0.1f;

        #endregion--------------------------------------------------------------------------------------


        #region ------------------------------ Private Methods ------------------------------------------
        private IEnumerator ScaleRect(float width)
        {
            float cTime = 0;
            Vector2 currentSize = rectTransform.sizeDelta;
            while (cTime < 1)
            {
                rectTransform.sizeDelta = Vector2.Lerp(currentSize, new Vector2(width, rectTransform.sizeDelta.y), cTime);
                yield return new WaitForEndOfFrame();
                cTime += Time.deltaTime / scaleAnimDuration;
            }
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

            if (rectTransform.sizeDelta.x == 0)
            {
                transform.localPosition = Vector2.zero;
                this.gameObject.SetActive(false);
            }
        }

        private void UpdateScore()
        {
            string score = GetController<CardController>().GetScore(containingCardsID);
            pointsText.text = string.Format(GameStrings.groupScorePrefix, score);
        }

        #endregion--------------------------------------------------------------------------------------


        #region ------------------------------ Public Methods ------------------------------------------

        public void UpdateRectSize()
        {
            if (containingCardsID.Count == 0)
            {
                return;
            }
            float cardWidth = this.GetController<CardController>().CardDimension.x;
            float cardOffsetDivisor = this.GetController<CardController>().GetData<CardModel>().cardPlacementDivisorOffset;
            float res1 = containingCardsID.Count * cardWidth;
            float res2 = (((containingCardsID.Count - 1) * cardWidth) / cardOffsetDivisor);
            desiredWidth = res1 - res2;

            if (containingCardsID.Count == 1)
            {
                desiredWidth = cardWidth;
            }
            float w = desiredWidth + this.GetController<CardController>().GetData<CardModel>().cardGroupWidthPadding;
            StartCoroutine(ScaleRect(w));
        }

        public override void InitView(string id)
        {
            this.ID = id;
            containingCardsID = new List<string>();
            rectTransform = GetComponent<RectTransform>();
            var ratio = GetController<CardController>().GetData<CardModel>().cardGroupHeightPadding;
            var height = GetController<CardController>().CardDimension.y + ratio;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }

        public void AddCardWithID(string cardId)
        {
            containingCardsID.Add(cardId);
            UpdateScore();
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
                this.gameObject.SetActive(false);
            }
            UpdateScore();
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

        #endregion--------------------------------------------------------------------------------------
    }
}
