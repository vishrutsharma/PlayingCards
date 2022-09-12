using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JungleeCards.Controllers;

public class GameView : MonoBehaviour
{
    #region --------------------------------- Serialize Fields ---------------------------------
    [SerializeField] private Button groupButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private CanvasGroup canvasGroup;

    #endregion ----------------------------------------------------------------------------------


    #region ---------------------------------- Private Fields ------------------------------------
    private GameController gameController;
    private float fadeDuration = 1;
    private float introWaitDelay = 1;

    #endregion ----------------------------------------------------------------------------------


    #region ---------------------------------- Private Methods ------------------------------------
    void Start()
    {
        canvasGroup.alpha = 0;
        groupButton.onClick.AddListener(OnGroupButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        groupButton.gameObject.SetActive(false);
        StartCoroutine(Fade(false));
    }

    private IEnumerator Fade(bool isFadeOut)
    {
        yield return new WaitForSeconds(introWaitDelay);

        float a = isFadeOut ? 1 : 0;
        float b = isFadeOut ? 0 : 1;
        float cTime = 0;
        while (cTime < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(a, b, cTime);
            yield return new WaitForEndOfFrame();
            cTime += Time.deltaTime / fadeDuration;
        }
        canvasGroup.alpha = b;
    }

    private void OnGroupButtonClicked()
    {
        gameController.OnGroupClick();
    }

    private void OnHomeButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    #endregion ----------------------------------------------------------------------------------


    #region ---------------------------------- Public Methods ------------------------------------
    public void InitController(GameController gameContoller)
    {
        this.gameController = gameContoller;
    }

    public void ToggleGroupButton(bool active)
    {
        groupButton.gameObject.SetActive(active);
    }
    #endregion ----------------------------------------------------------------------------------
}
