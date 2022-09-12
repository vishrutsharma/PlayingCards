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
        [SerializeField] private GameView gameView;
        #endregion --------------------------------------------------------------------------------------


        #region --------------------------------- Private Fields ----------------------------------------
        private GameModel gameData;
        #endregion --------------------------------------------------------------------------------------


        #region --------------------------------- Private Methods ----------------------------------------
        private void Start()
        {
            LoadGameData();
            gameView.InitController(this);
        }

        private void LoadGameData()
        {
            jsonLoader.LoadJSON<GameModel>(GameStrings.gameConfigPath,
            delegate (GameModel gameModel)
            {
                gameData = gameModel;
                cardController.SetGameController(this);
                cardController.LoadData<CardModel>(gameData.cardModel);
                cardController.OnGameStart();

                jsonLoader.LoadJSON<DeckData>(GameStrings.cardDataPath,
                            delegate (DeckData deckData)
                            {
                                cardController.PopulateDeck(deckData);
                            });
            });
        }
        #endregion --------------------------------------------------------------------------------------


        #region --------------------------------- Public Methods ----------------------------------------
        public void ToggleGroupButton(bool active)
        {
            gameView.ToggleGroupButton(active);
        }

        public void OnGroupClick()
        {
            cardController.MakeNewGroup();
        }
        #endregion --------------------------------------------------------------------------------------
    }
}
