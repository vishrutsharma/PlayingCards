using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JungleeCards.Controllers;
using UnityEngine.EventSystems;

namespace JungleeCards.Views
{
    public class CardView : EntityView, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
    {

        #region ------------------------------------- Public Fields -----------------------------------------
        public string CardGroupID { get; private set; }
        public bool IsSelected { get; private set; }

        #endregion ------------------------------------------------------------------------------------------


        #region ------------------------------------- Private Fields -----------------------------------------
        private bool inMovingState = false;
        private float movementDuration = 0.2f;
        private bool inDraggingState = false;
        private GameObject selectedObject;
        private Image cardImage;
        private int lastSiblingIndex = 0;

        #endregion ------------------------------------------------------------------------------------------


        #region ------------------------------------- Private Methods -----------------------------------------
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

        #endregion ------------------------------------------------------------------------------------------


        #region ------------------------------------- Public Methods -----------------------------------------
        public void SetGroupID(string id)
        {
            this.CardGroupID = id;
        }

        public override void InitView(string id)
        {
            this.ID = id;
            IsSelected = false;
            selectedObject = transform.GetChild(0).gameObject;
            cardImage = GetComponent<Image>();
            selectedObject.SetActive(false);
        }

        public void SetSprite(Sprite cardSrite)
        {
            cardImage.sprite = cardSrite;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (this.GetController<CardController>().IsInputAllowed())
            {
                lastSiblingIndex = transform.GetSiblingIndex();
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
                    selectedObject.SetActive(IsSelected);
                    cardImage.color = IsSelected ? this.GetController<CardController>().selectedCardColor : Color.white;
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

        public void ResetToLastParent()
        {
            transform.SetSiblingIndex(lastSiblingIndex);
        }

        public void ResetCardStatus()
        {
            inMovingState = false;
            IsSelected = false;
            selectedObject.SetActive(false);
            cardImage.color = Color.white;
        }

        #endregion ------------------------------------------------------------------------------------------
    }
}
