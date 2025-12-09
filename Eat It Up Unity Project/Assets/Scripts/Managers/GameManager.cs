using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private GameObject playerStartPosition;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private CameraManager cameraManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private LevelManager levelManager;
    [SerializeField]
    private ScoreManager scoreManager;

    private GameObject currentPlayer;
    private GameObject playerFinalPosition;
    private bool playerIsDead;
    private static bool levelFinished;

    private static bool gamePaused;
    private static int currentLevel = -1;

    public float PlayerStartPosition { get { return playerStartPosition.transform.position.y; } }
    public float PlayerEndPosition { get { return playerFinalPosition.transform.position.y; } }
    public float CurrentPlayerPosition { get { return currentPlayer.transform.position.y; } }
    public int CurrentLevel { get { return currentLevel; } }
    public static bool GamePaused { get { return gamePaused; } }
    public static bool LevelFinished { get { return levelFinished; } }

    public delegate void Gametatus();
    public event Gametatus OnPlayerWon, OnPlayerPause, OnPlayerUnpause;
    public delegate void CollectableBehaviour(Collectable collectableCollected);
    public event CollectableBehaviour OnCollectableCollected;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void OnEnable()
    {
        uiManager.OnRestartButton += RestartGame;
        uiManager.OnUnpauseButton += UnpauseGame;
        uiManager.OnQuitButton += QuitGame;
        levelManager.OnGameFinished += GameFinished;
        cameraManager.OnCinematicFinished += CinematicFinished;
    }

    void OnDisable()
    {
        uiManager.OnRestartButton -= RestartGame;
        uiManager.OnUnpauseButton -= UnpauseGame;
        uiManager.OnQuitButton -= QuitGame;
        levelManager.OnGameFinished -= GameFinished;
        cameraManager.OnCinematicFinished -= CinematicFinished;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    void Start()
    {
        StartGame();
    }

    [ContextMenu("GameOver")]
    public void GameOver()
    {
        currentLevel--;
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch -= LevelWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch -= CollectableCollected;
        currentPlayer.GetComponent<PlayerCollision>().OnCameraStopTouch -= StopCameraFollow;
        currentPlayer.GetComponent<PlayerController>().OnPause -= PauseGame;
        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.Death);
        playerIsDead = true;
        StartCoroutine(RestartingGame());
    }

    public void StartGame()
    {
        currentLevel++;
        levelFinished = false;
        playerFinalPosition = GameObject.FindGameObjectWithTag("Finish");
        uiManager.SetProgressBar();
        SpawnPlayer();
        scoreManager.TrackCollectablesInLevel();
        scoreManager.InitializeScorePerLevel();
    }

    [ContextMenu("Restart")]
    public void RestartGame()
    {
        levelManager.reloadScene();
    }

    public void SpawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, playerStartPosition.transform);
        cameraManager.SetTrackingTarget(currentPlayer.transform);

        
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch += LevelWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch += CollectableCollected;
        currentPlayer.GetComponent<PlayerCollision>().OnCameraStopTouch += StopCameraFollow;
        currentPlayer.GetComponent<PlayerController>().OnPause += PauseGame;

        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.Spawn);
    }

    public void StopCameraFollow()
    {
        cameraManager.SetTrackingTarget(null);
    }

    public void LevelWon()
    {
        levelFinished = true;
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch -= LevelWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch -= CollectableCollected;
        currentPlayer.GetComponent<PlayerCollision>().OnCameraStopTouch -= StopCameraFollow;
        currentPlayer.GetComponent<PlayerController>().OnPause -= PauseGame;
        scoreManager.UpdateScorePerLevel();
        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.Door);
        if (levelManager.CheckIfFinalLevel())
        {
            GameFinished();
            return;
        }
        OnPlayerWon?.Invoke();
        //levelManager.tryLoadNextScene();
    }

    public void PauseGame()
    {
        if (gamePaused)
        {
            uiManager.UnpauseGame();
            return;
        }
        gamePaused = true;
        currentPlayer.GetComponent<PlayerController>().DisableController();
        OnPlayerPause?.Invoke();
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        gamePaused = false;
        Time.timeScale = 1;
        currentPlayer.GetComponent<PlayerController>().EnableController();
    }


    public void CollectableCollected(Collectable collectableCollected)
    {
        scoreManager.CollectableCollected(collectableCollected);
        OnCollectableCollected?.Invoke(collectableCollected);
    }

    public void GameFinished()
    {
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerMovement>().StopAllMovement();
        uiManager.ToggleGameHud(false);
        SoundManager.instance.StopAllSounds();
        cameraManager.GameFinished();
    }

    public void CinematicFinished()
    {
        SoundManager.instance.PlayFinalCinematicSound();
        uiManager.PlayerFinishedGame();
    }

    private IEnumerator RestartingGame()
    {
        yield return new WaitForSeconds(.75f);
        RestartGame();
    }

}
