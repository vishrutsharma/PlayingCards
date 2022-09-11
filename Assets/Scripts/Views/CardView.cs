using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JungleeCards.Controllers;
using JungleeCards.Models;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JungleeCards.Views
{
    public class CardView : EntityView, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
    {
        public string CardGroupID { get; private set; }
        public bool IsSelected { get; private set; }

        private bool inMovingState = false;
        private float movementDuration = 0.2f;
        private bool inDraggingState = false;

        private IEnumerator MoveTo(Vector2 desiredPos)
        {
            Vector2 startPos = transform.localPosition;
            float cTime = 0;
            while (cTime < 1)
            {
                transform.localPosition = Vector2.Lerp(startPos, desiredPos, cTime);
                yield return new WaitForEndOfFrame();
                cTime += Time.deltaTime / movementDuration;
            }
            transform.localPosition = desiredPos;
            inMovingState = false;
        }

        public void SetGroupID(string id)
        {
            this.CardGroupID = id;
        }

        public override void InitView(string id)
        {
            this.ID = id;
            IsSelected = false;
        }

        public void SetSprite(Sprite cardSrite)
        {
            this.GetComponent<Image>().sprite = cardSrite;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (this.GetController<CardController>().IsInputAllowed())
            {
                inDraggingState = true;
                this.transform.SetParent(this.GetController<CardController>().draggableAreaTransform);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (this.GetController<CardController>().IsInputAllowed())
            {
                if (inDraggingState)
                {
                    Vector2 newPos = new Vector2(eventData.position.x, transform.position.y);
                    transform.position = Vector2.Lerp(transform.position, newPos, 40 * Time.deltaTime);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (this.GetController<CardController>().IsInputAllowed())
            {
                this.GetController<CardController>().OnCardDragComplete(this);
                inDraggingState = false;
            }
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (!inDraggingState)
            {
                if (!inMovingState)
                {
                    inMovingState = true;
                    IsSelected = !IsSelected;
                    this.GetController<CardController>().OnCardStateChanged(ID, IsSelected);
                    Vector2 desiredPos = IsSelected ? new Vector2(transform.localPosition.x, 25) : new Vector2(transform.localPosition.x, 0);
                    StartCoroutine(MoveTo(desiredPos));
                }
            }
        }

        public void MoveToPosition(Vector2 newPos)
        {
            StartCoroutine(MoveTo(newPos));
        }

        public void ResetCardStatus()
        {
            inMovingState = false;
            IsSelected = false;
        }
    }
}
