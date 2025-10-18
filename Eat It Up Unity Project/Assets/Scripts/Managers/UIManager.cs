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
    private bool timerActive;

    public delegate void ButtonPress();
    public event ButtonPress OnRestartButton, OnUnpauseButton, OnQuitButton;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        gameWonHud.SetActive(false);
        pauseGameHud.SetActive(false);
        unpauseGameHud.SetActive(false);
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

    void Start()
    {
        timerActive = true;
        StartCoroutine(CurrentTimer());
    }

    public void PlayerWon()
    {
        timerActive = false;
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

    public void QuitGame()
    {
        OnQuitButton?.Invoke();
    }

    private IEnumerator UnpausingGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        unpauseGameHud.SetActive(false);
        OnUnpauseButton?.Invoke();
    }

    private IEnumerator CurrentTimer()
    {
        while (timerActive)
        {
            yield return new WaitForSeconds(1f);
            seconds++;
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }
            gameTimer.SetText(minutes.ToString("D2") + ":" + seconds.ToString("D2"));
        }
        
    }
}
