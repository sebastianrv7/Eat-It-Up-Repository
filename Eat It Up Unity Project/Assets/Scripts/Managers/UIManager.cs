using System;
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
    [SerializeField]
    private TextMeshProUGUI rareCollectableText;
    [SerializeField]
    private TextMeshProUGUI goldCollectableText;
    [SerializeField]
    private Slider progressSlider;

    private static int currentScore;
    private static int rareCollectables;
    private static int goldCollectables;
    private static int initialScore;
    private static int initialRareCollectables;
    private static int initialGoldCollectables;
    private int minutes;
    private int seconds;
    private float progressValue;
    private float totalDistance;
    private bool timerActive;
    private bool progressBarSet;

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
        progressBarSet = false;
    }

    void OnEnable()
    {
        gameManager.OnPlayerWon += PlayerWon;
        gameManager.OnPlayerWon += PlayerFinishedGame;
        gameManager.OnPlayerPause += PauseGame;
        gameManager.OnCollectableCollected += UpdateCollectables;
    }

    void OnDisable()
    {
        gameManager.OnPlayerWon -= PlayerWon;
        gameManager.OnPlayerWon -= PlayerFinishedGame;
        gameManager.OnPlayerPause -= PauseGame;
        gameManager.OnCollectableCollected -= UpdateCollectables;
    }

    void Start()
    {
        timerActive = true;
        SetInitialScore();
        SetInitialCollectableText();
        //StartCoroutine(CurrentTimer());
    }

    void Update()
    {
        if(progressBarSet)
            UpdateProgressBar();
    }

    public void PlayerWon()
    {
        UpdateInitialCollectables(rareCollectables, goldCollectables);
        UpdateInitialScore(currentScore);
    }

    public void PlayerFinishedGame()
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

    public void SetProgressBar()
    {
        totalDistance = gameManager.PlayerEndPosition - gameManager.PlayerStartPosition;
        progressBarSet = true;
    }

    public void UpdateProgressBar()
    {
        float step = gameManager.CurrentPlayerPosition / totalDistance;
        progressValue = Mathf.Lerp(0, 1, step);
        progressSlider.value = progressValue;

    }

    public void UpdateCollectables(Collectable collectableCollected)
    {
        currentScore += collectableCollected.MyScore;
        UpdateScoreText();
        switch (collectableCollected.MyCollectableType)
        {
            case Collectable.CollectableType.Normal:
                break;
            case Collectable.CollectableType.Rare:
                rareCollectables++;
                break;
            case Collectable.CollectableType.Gold:
                goldCollectables++;
                break;
            default:
                break;
        }
        UpdateCollectableText();
    }

    public void UpdateCollectableText()
    {
        rareCollectableText.SetText(rareCollectables.ToString("D2"));
        goldCollectableText.SetText(goldCollectables.ToString("D2"));
    }

    public void SetInitialCollectableText()
    {
        rareCollectables = initialRareCollectables;
        goldCollectables = initialGoldCollectables;
        UpdateCollectableText();
    }

    public void UpdateInitialCollectables(int newInitialRareCollectables, int newInitialGoldCollectables)
    {
        initialRareCollectables = newInitialRareCollectables;
        initialGoldCollectables = newInitialGoldCollectables;
    }

    public void SetInitialScore()
    {
        currentScore = initialScore;
        UpdateScoreText();
    }

    public void UpdateInitialScore(int newInitialScore)
    {
        initialScore = newInitialScore;
    }

    public void UpdateScoreText()
    {
        gameTimer.SetText(currentScore.ToString("D2"));
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
