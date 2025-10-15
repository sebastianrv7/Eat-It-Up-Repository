using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject gameHud;
    [SerializeField]
    private GameObject pauseGameHud;
    [SerializeField]
    private GameObject unpauseGameHud;
    [SerializeField]
    private GameObject gameWonHud;
    [SerializeField]
    private TextMeshProUGUI gameTimer;
    [SerializeField]
    private TextMeshProUGUI gameWonTimer;

    private int minutes;
    private int seconds;

    public delegate void ButtonPress();
    public event ButtonPress OnRestartButton, OnUnpauseButton;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        gameWonHud.SetActive(false);
        gameHud.SetActive(true);
    }

    void OnEnable()
    {
        gameManager.OnPlayerWon += PlayerWon;
        gameManager.OnPlayerPause += PauseGame;
    }

    void OnDisable()
    {
        gameManager.OnPlayerWon -= PlayerWon;
        gameManager.OnPlayerPause -= PauseGame;    
    }

    public void PlayerWon()
    {
        SetFinalTimer();
        gameWonHud.SetActive(true);
        gameHud.SetActive(false);
    }

    public void SetFinalTimer()
    {
        gameWonTimer.SetText("Your Time:\n" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
    }

    public void RestartButtonPressed()
    {
        OnRestartButton?.Invoke();    
    }

    public void PauseGame()
    {
        pauseGameHud.SetActive(true);
    }

    public void UnpauseGame()
    {
        pauseGameHud.SetActive(false);
        unpauseGameHud.SetActive(true);
        StartCoroutine(UnpausingGame());
    }

    private IEnumerator UnpausingGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        unpauseGameHud.SetActive(false);
        OnUnpauseButton?.Invoke();
    }
}
