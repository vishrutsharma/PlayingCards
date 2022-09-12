using System;
using UnityEngine;

namespace JungleeCards.Controllers
{
    public abstract class EntityController : MonoBehaviour
    {
        #region --------------------------------- Protected Fields -------------------------------------
        protected GameController gameController;
        protected object data;
        #endregion --------------------------------------------------------------------------------------


        #region --------------------------------- Public Methods ----------------------------------------
        public abstract void OnGameStart();
        public abstract void OnGameOver();

        public void SetGameController(GameController gameController)
        {
            this.gameController = gameController;
        }

        public void LoadData<T>(T data)
        {
            this.data = (T)Convert.ChangeType(data, typeof(T));
        }

        public T GetData<T>()
        {
            return (T)Convert.ChangeType(this.data, typeof(T));
        }
        #endregion --------------------------------------------------------------------------------------

    }
}
