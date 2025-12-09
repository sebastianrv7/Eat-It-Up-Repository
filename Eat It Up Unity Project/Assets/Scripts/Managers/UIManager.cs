using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    private Button pauseButton;
    [SerializeField]
    private GameObject gameHud;
    [SerializeField]
    private GameObject pauseGameHud;
    [SerializeField]
    private GameObject unpauseGameHud;
    [SerializeField]
    private GameObject levelWonHud;
    [SerializeField]
    private GameObject gameWonHud;
    [SerializeField]
    private TextMeshProUGUI gameHUDScore;
    [SerializeField]
    private GameObject levelGroupParent;
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
        if (progressBarSet)
            UpdateProgressBar();
    }

    public void PlayerWonLevel()
    {
        UpdateInitialCollectables(rareCollectables, goldCollectables);
        UpdateInitialScore(scoreManager.CurrentScore);
        SetLevelWonHUD();
        timerActive = false;
        //SetFinalTimer();
        levelWonHud.SetActive(true);
        ShowStars();
        gameHud.SetActive(false);
    }

    public void PlayerFinishedGame()
    {
        gameHud.SetActive(false);
        gameWonHud.SetActive(true);
        for (int i = 0; i < scoreManager.Levels; i++)
        {
            levelGroup.Add(Instantiate(levelGroupInfoPrefab, levelGroupParent.transform));
            levelGroup[i].SetScore(
                i + 1,
                scoreManager.GetCollectablesCollectedPerLevel(Collectable.CollectableType.Rare, i),
                scoreManager.GetMaxCollectablesPerLevel(Collectable.CollectableType.Rare, i),
                scoreManager.GetCollectablesCollectedPerLevel(Collectable.CollectableType.Gold, i),
                scoreManager.GetMaxCollectablesPerLevel(Collectable.CollectableType.Gold, i),
                scoreManager.GetScorePerLevel(i),
                scoreManager.GetStarsPerLevel(i));
        }
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
        toggleMusicIcon.gameObject.SetActive(SoundManager.instance.MusicEnabled);
        toggleSFXIcon.gameObject.SetActive(SoundManager.instance.SoundFXEnabled);
        musicSlider.value = SoundManager.instance.GetMusicHUDValue();
        sfxSlider.value = SoundManager.instance.GetSFXHUDValue();
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

    public void SetLevelWonHUD()
    {
        // string rareText = rareCollectables.ToString("D2");
        // string maxRareText = scoreManager.MaxRareCollectablePerLevel.ToString("D2");
        // string goldText = goldCollectables.ToString("D2");
        // string maxGoldText = scoreManager.MaxGoldCollectablePerLevel.ToString("D2");
        // string scoreText = scoreManager.CurrentScore.ToString("D2");

        // rareCollectableLevelWonText.SetText("");
        // for (int i = 0; i < rareText.Length; i++)
        // {
        //     rareCollectableLevelWonText.text += "<sprite index=" + rareText[i].ToString() + ">";
        // }
        // rareCollectableLevelWonText.text += "/";
        // for (int i = 0; i < maxRareText.Length; i++)
        // {
        //     rareCollectableLevelWonText.text += "<sprite index=" + maxRareText[i].ToString() + ">";
        // }

        // goldCollectableLevelWonText.SetText("");
        // for (int i = 0; i < goldText.Length; i++)
        // {
        //     goldCollectableLevelWonText.text += "<sprite index=" + goldText[i].ToString() + ">";
        // }
        // goldCollectableLevelWonText.text += "/";
        // for (int i = 0; i < maxGoldText.Length; i++)
        // {
        //     goldCollectableLevelWonText.text += "<sprite index=" + maxGoldText[i].ToString() + ">";
        // }

        // levelWonScore.SetText("");
        // for (int i = 0; i < scoreText.Length; i++)
        // {
        //     levelWonScore.text += "<sprite index=" + scoreText[i].ToString() + ">";
        // }

        rareCollectableLevelWonText.SetText(rareCollectables.ToString("D2") + ";" + scoreManager.MaxRareCollectablePerLevel.ToString("D2"));
        goldCollectableLevelWonText.SetText(goldCollectables.ToString("D2") + ";" + scoreManager.MaxGoldCollectablePerLevel.ToString("D2"));
        levelWonScore.SetText(scoreManager.CurrentScore.ToString("D2"));
    }

    public void ShowStars()
    {
        levelWonStarsManager.ShowStars(scoreManager.StarsEarned);
    }

    private IEnumerator UnpausingGame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        pauseGameHud.SetActive(false);
        unpauseGameHud.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        unpauseGameHud.SetActive(false);
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