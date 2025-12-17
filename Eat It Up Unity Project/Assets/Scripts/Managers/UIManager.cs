using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private ScoreManager scoreManager;
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private GameObject gameHud;
    [SerializeField]
    private GameObject pauseGameHud;
    
    [SerializeField]
    private GameObject levelWonHud;
    [SerializeField]
    private GameObject gameWonHud;
    [SerializeField]
    private TextMeshProUGUI gameHUDScore;
    
    [SerializeField]
    private UILevelGroupManager levelGroupInfoPrefab;
    [SerializeField]
    private TextMeshProUGUI levelWonScore;
    [SerializeField]
    private TextMeshProUGUI gameWonTimer;
    [SerializeField]
    private TextMeshProUGUI rareMultiplierText;

    

    [SerializeField]
    private TextMeshProUGUI rareCollectableLevelWonText;
    [SerializeField]
    private TextMeshProUGUI goldCollectableLevelWonText;
    [SerializeField]
    private Slider progressSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Image toggleMusicIcon;
    [SerializeField]
    private Image toggleSFXIcon;
    [SerializeField]
    private UIStarsManager levelWonStarsManager;
    [SerializeField]
    private List<Sprite> numbersInSprites;

    private List<UILevelGroupManager> levelGroup = new List<UILevelGroupManager>(); 

    private int rareCollectables;
    private int goldCollectables;
    private int initialScore;
    private int initialRareCollectables;
    private int initialGoldCollectables;
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

        levelWonHud.SetActive(false);
        pauseGameHud.SetActive(false);
        
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
        if (progressBarSet)
            UpdateProgressBar();
    }

    public void PlayerWonLevel()
    {
        
        
        timerActive = false;
        //SetFinalTimer();
        levelWonHud.SetActive(true);
        
        gameHud.SetActive(false);

        
    }

    public void PlayerFinishedGame()
    {
        gameHud.SetActive(false);
        int finalScore = Score.Instance.GetTotalScore();
        SetLevelWonHUD(finalScore);
        gameWonHud.SetActive(true);

        // Avisar que el juego terminó
        if (QuestionManager.instance != null)
            QuestionManager.instance.EndGame();

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
        pauseButton.interactable = false;
    }

    public void UnpauseGame()
    {
        StartCoroutine(UnpausingGame());
    }

    public void ToggleMusic()
    {
        SoundManager.instance.ToggleMusicVolume(!toggleMusicIcon.IsActive());
        toggleMusicIcon.gameObject.SetActive(!toggleMusicIcon.IsActive());
    }

    public void ToggleSFX()
    {
        SoundManager.instance.ToggleSFXVolume(!toggleSFXIcon.IsActive());
        toggleSFXIcon.gameObject.SetActive(!toggleSFXIcon.IsActive());
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


        int multiplier = ScoreMultiplierManager.Instance.GetCurrentMultiplier();
        rareMultiplierText.SetText($"X{multiplier}");

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

    public void ToggleGameHud(bool enable)
    {
        gameHud.SetActive(enable);
    }

    public void UpdateScoreText()
    {
        int totalScore = Score.Instance.GetTotalScore();
        gameHUDScore.SetText(totalScore.ToString("D2"));
    }

    public void SetLevelWonHUD(int score)
    {
        
        
        levelWonScore.SetText(score.ToString("D2"));
    }

    

    private IEnumerator UnpausingGame()
    {

        yield return new WaitForSecondsRealtime(0.1f);
        pauseGameHud.SetActive(false);


        //  AQUÍ LA LÓGICA CLAVE
        if (QuestionManager.instance != null && QuestionManager.instance.IsQuestionActive)
        {
            // Hay una pregunta activa  NO reanudamos el juego
            // Time.timeScale sigue en 0 (gracias a StartQuestion())
            // NO invocamos OnUnpauseButton para no reanudar lógica del juego
            Debug.Log("Pausa desactivada, pero juego sigue congelado por pregunta activa");
            yield break;  // Salimos aquí, no hacemos nada más
        }

        Time.timeScale = 1f;
        OnUnpauseButton?.Invoke();
        pauseButton.interactable = true;
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
            gameHUDScore.SetText(minutes.ToString("D2") + ":" + seconds.ToString("D2"));
        }

    }


    
}