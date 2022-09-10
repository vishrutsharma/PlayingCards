using UnityEngine;
using JungleeCards.Models;
using JungleeCards.Common;

namespace JungleeCards.Controllers
{
    public class GameController : MonoBehaviour
    {
        #region --------------------------------- Serialize Fields -------------------------------------

        [SerializeField] private CardController cardController = null;
        [SerializeField] private JSONLoader jsonLoader;

        #endregion --------------------------------------------------------------------------------------


        #region --------------------------------- Private Fields ----------------------------------------

        private void Start()
        {
            GameModel gameModel = new GameModel();
            cardController.SetGameController(this);
            cardController.LoadData<CardModel>(gameModel.cardModel);
            cardController.OnGameStart();
            LoadCurrentDeck();
        }

        private void LoadCurrentDeck()
        {
            jsonLoader.LoadJSON<DeckData>(GameStrings.cardDataPath,
            delegate (DeckData deckData)
            {
                cardController.PopulateDeck(deckData);
            });
        }

        #endregion --------------------------------------------------------------------------------------
    }
}
