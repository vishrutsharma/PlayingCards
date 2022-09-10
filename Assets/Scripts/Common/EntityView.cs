using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JungleeCards.Controllers;

namespace JungleeCards.Views
{
    public abstract class EntityView : MonoBehaviour
    {
        protected EntityController controller = null;
        public string ID { get; protected set; }

        public void SetController<T>(T controller) where T : EntityController
        {
            this.controller = (T)controller;
        }

        protected T GetController<T>() where T : EntityController
        {
            return (T)Convert.ChangeType(this.controller, typeof(T));
        }

        public abstract void InitView(string id = null);
    }
}

