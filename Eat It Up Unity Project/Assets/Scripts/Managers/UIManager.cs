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
    private ScoreManager scoreManager;
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
    private TextMeshProUGUI rareCollectableGameHUDText;
    [SerializeField]
    private TextMeshProUGUI goldCollectableGameHUDText;
    [SerializeField]
    private TextMeshProUGUI rareCollectableLevelWonText;
    [SerializeField]
    private TextMeshProUGUI goldCollectableLevelWonText;
    [SerializeField]
    private Slider progressSlider;

    private  int rareCollectables;
    private  int goldCollectables;
    private  int initialScore;
    private  int initialRareCollectables;
    private  int initialGoldCollectables;
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
        gameManager.OnPlayerWon += PlayerWonLevel;
        //gameManager.OnPlayerWon += PlayerFinishedGame;
        gameManager.OnPlayerPause += PauseGame;
        gameManager.OnCollectableCollected += UpdateCollectables;
    }

    void OnDisable()
    {
        gameManager.OnPlayerWon -= PlayerWonLevel;
        //gameManager.OnPlayerWon -= PlayerFinishedGame;
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

    public void PlayerWonLevel()
    {
        UpdateInitialCollectables(rareCollectables, goldCollectables);
        UpdateInitialScore(scoreManager.CurrentScore);
        SetLevelWonHUD();
        timerActive = false;
        SetFinalTimer();
        gameWonHud.SetActive(true);
        gameHud.SetActive(false);
    }

    public void PlayerFinishedGame()
    {

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
        UpdateScoreText();
        UpdateCollectableText();
    }

    public void UpdateCollectableText()
    {
        rareCollectableGameHUDText.SetText(rareCollectables.ToString("D2"));
        goldCollectableGameHUDText.SetText(goldCollectables.ToString("D2"));
        
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
        UpdateScoreText();
    }

    public void UpdateInitialScore(int newInitialScore)
    {
        initialScore = newInitialScore;
    }

    public void UpdateScoreText()
    {
        gameTimer.SetText(scoreManager.CurrentScore.ToString("D2"));
    }

    public void SetLevelWonHUD()
    {
        rareCollectableLevelWonText.SetText(":" + rareCollectables.ToString("D2") + "/" + scoreManager.MaxRareCollectablePerLevel.ToString("D2"));
        goldCollectableLevelWonText.SetText(":" + goldCollectables.ToString("D2") + "/" + scoreManager.MaxGoldCollectablePerLevel.ToString("D2"));
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
