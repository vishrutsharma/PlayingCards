using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JungleeCards.Controllers;

namespace JungleeCards.Views
{
    public class CardGroupView : EntityView
    {
        private HashSet<string> containingCardsID;

        public override void InitView(string id)
        {
            this.ID = id;
            containingCardsID = new HashSet<string>();
        }

        public void AddCardWithID(string cardId)
        {
            containingCardsID.Add(cardId);
        }
    }
}
