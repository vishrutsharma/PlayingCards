using System;
using UnityEngine;
using JungleeCards.Controllers;

namespace JungleeCards.Views
{
    public abstract class EntityView : MonoBehaviour
    {
        #region --------------------------------- Protected Fields -------------------------------------
        protected EntityController controller = null;
        #endregion -------------------------------------------------------------------------------------


        #region --------------------------------- Public Fields ----------------------------------------
        public string ID { get; protected set; }
        #endregion -------------------------------------------------------------------------------------


        #region --------------------------------- Public Methods ----------------------------------------
        public void SetController<T>(T controller) where T : EntityController
        {
            this.controller = (T)controller;
        }

        protected T GetController<T>() where T : EntityController
        {
            return (T)Convert.ChangeType(this.controller, typeof(T));
        }

        public abstract void InitView(string id = null);
        #endregion -------------------------------------------------------------------------------------
    }
}

