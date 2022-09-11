using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JungleeCards.Controllers;

public class GameView : MonoBehaviour
{
    [SerializeField] private Button groupButton;

    private GameController gameController;


    public void InitController(GameController gameContoller)
    {
        this.gameController = gameContoller;
    }

    void Start()
    {
        groupButton.onClick.AddListener(OnGroupButtonClicked);
    }

    private void OnGroupButtonClicked()
    {
        gameController.OnGroupClick();
    }

    public void ToggleGroupButton(bool active)
    {
        groupButton.gameObject.SetActive(active);
    }
}
