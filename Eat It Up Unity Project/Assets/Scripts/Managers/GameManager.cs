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
    private string mainScene;
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

    private GameObject currentPlayer;
    private List<Collectable> collectablesPerLevel = new List<Collectable>();
    private List<Collectable> collectablesCollected = new List<Collectable>();
    private bool playerIsDead;
    private bool gamePaused;

    public delegate void Gametatus();
    public event Gametatus OnPlayerWon, OnPlayerPause, OnPlayerUnpause;

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
    }

    void OnDisable()
    {
        uiManager.OnRestartButton -= RestartGame;
        uiManager.OnUnpauseButton -= UnpauseGame;
        uiManager.OnQuitButton -= QuitGame;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame();
    }

    [ContextMenu("GameOver")]
    public void GameOver()
    {
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch -= GameWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch -= CollectibleCollected;
        currentPlayer.GetComponent<PlayerController>().OnPause -= PauseGame;
        playerIsDead = true;
        StartCoroutine(RestartingGame());
    }

    public void StartGame()
    {
        SpawnPlayer();
        GetCollectablesInLevel();
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
        currentPlayer.GetComponent<PlayerHealth>().OnDeath += GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch += GameWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch += CollectibleCollected;
        currentPlayer.GetComponent<PlayerController>().OnPause += PauseGame;
    }

    public void GameWon()
    {
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch -= GameWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch -= CollectibleCollected;
        currentPlayer.GetComponent<PlayerController>().OnPause -= PauseGame;
        levelManager.tryLoadNextScene();
        OnPlayerWon?.Invoke();
    }

    public void PauseGame()
    {
        if (gamePaused)
        {
            uiManager.UnpauseGame();
            return;
        }
        currentPlayer.GetComponent<PlayerController>().DisableController();
        OnPlayerPause?.Invoke();
        gamePaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        gamePaused = false;
        Time.timeScale = 1;
        currentPlayer.GetComponent<PlayerController>().EnableController();
    }

    public void GetCollectablesInLevel()
    {
        UnityEngine.Object[] collectablesFound = FindObjectsByType(typeof(Collectable), FindObjectsSortMode.None);
        foreach (UnityEngine.Object collectable in collectablesFound)
        {
            Collectable collectableScript = (Collectable)collectable.GetComponent(typeof(Collectable));
            collectablesPerLevel.Add(collectableScript);
        }
    }

    public void CollectibleCollected(Collectable collectableCollected)
    {
        collectablesCollected.Add(collectableCollected);
    }

    private IEnumerator RestartingGame()
    {
        yield return new WaitForSeconds(.5f);
        RestartGame();
    }
}
