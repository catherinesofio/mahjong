using UnityEngine;
using UnityEngine.UI;

public class CanvasGame : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _scoreWonText;
    [SerializeField]
    private Text _highscoreWonText;

    [SerializeField]
    private GameObject _wonPopup;
    [SerializeField]
    private GameObject _lostPopup;
    [SerializeField]
    private GameObject _quitPopup;

    private void Awake()
    {
        UpdateScore(0);
    }

    private void Start()
    {
        EventManager.AddEventListener(EventId.SCORE_UPDATE, UpdateScore);
        EventManager.AddEventListener(EventId.GAME_LOST, ShowLostPopup);
        EventManager.AddEventListener(EventId.GAME_QUIT, ShowQuitPopup);
        EventManager.AddEventListener(EventId.GAME_RESUME, HideQuitPopup);
        EventManager.AddEventListener(EventId.GAME_WON, ShowWonPopup);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.SCORE_UPDATE, UpdateScore);
        EventManager.RemoveEventListener(EventId.GAME_LOST, ShowLostPopup);
        EventManager.RemoveEventListener(EventId.GAME_QUIT, ShowQuitPopup);
        EventManager.RemoveEventListener(EventId.GAME_RESUME, HideQuitPopup);
        EventManager.RemoveEventListener(EventId.GAME_WON, ShowWonPopup);
    }

    private void UpdateScore(object obj)
    {
        var score = (int)obj;
        _scoreText.text = score.ToString();
    }

    #region PopUps
    private void ShowLostPopup(object obj = null)
    {
        _quitPopup.SetActive(false);
        _lostPopup.SetActive(true);
    }

    private void ShowQuitPopup(object obj = null)
    {
        _quitPopup.SetActive(true);
    }

    private void HideQuitPopup(object obj = null)
    {
        _quitPopup.SetActive(false);
    }

    private void ShowWonPopup(object obj)
    {
        var score = (int)obj;

        _scoreWonText.text = score.ToString();
        _highscoreWonText.text = DataManager.LevelHighscore.ToString();
        _wonPopup.SetActive(true);
    }
    #endregion
}
