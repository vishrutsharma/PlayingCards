using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JungleeCards.Controllers;
using JungleeCards.Models;
using UnityEngine.UI;

namespace JungleeCards.Views
{
    public class CardView : EntityView
    {
        public string CardGroupID { get; private set; }

        public void SetGroupID(string id)
        {
            this.CardGroupID = id;
        }

        public override void InitView(string id)
        {
            this.ID = id;
        }

        public void SetSprite(Sprite cardSrite)
        {
            this.GetComponent<Image>().sprite = cardSrite;
        }

    }
}
