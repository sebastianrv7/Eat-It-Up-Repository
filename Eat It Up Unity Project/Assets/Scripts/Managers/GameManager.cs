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

    private GameObject currentPlayer;
    private GameObject playerFinalPosition;
    private static List<List<Collectable>> rareCollectablesPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> rareCollectablesCollectedPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> goldCollectablesPerLevel = new List<List<Collectable>>();
    private static List<List<Collectable>> goldCollectablesCollectedPerLevel = new List<List<Collectable>>();
    //private List<Collectable> rareCollectablesPerLevel = new List<Collectable>();
    //private List<Collectable> goldCollectablesPerLevel = new List<Collectable>();
    private List<Collectable> collectablesCollected = new List<Collectable>();
    private bool playerIsDead;
    private bool gamePaused;
    private static int currentLevel = -1;

    public float PlayerStartPosition { get { return playerStartPosition.transform.position.y; } }
    public float PlayerEndPosition { get { return playerFinalPosition.transform.position.y; } }
    public float CurrentPlayerPosition { get { return currentPlayer.transform.position.y; } }
    public int MaxRareCollectablePerLevel { get { return rareCollectablesPerLevel[currentLevel].Count; } }
    public int MaxGoldCollectablePerLevel { get { return goldCollectablesPerLevel[currentLevel].Count; } }

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
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch -= LevelWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch -= CollectibleCollected;
        currentPlayer.GetComponent<PlayerController>().OnPause -= PauseGame;
        playerIsDead = true;
        StartCoroutine(RestartingGame());
    }

    public void StartGame()
    {
        currentLevel++;
        playerFinalPosition = GameObject.FindGameObjectWithTag("Finish");
        uiManager.SetProgressBar();
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
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch += LevelWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch += CollectibleCollected;
        currentPlayer.GetComponent<PlayerController>().OnPause += PauseGame;
    }

    public void LevelWon()
    {
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch -= LevelWon;
        currentPlayer.GetComponent<PlayerCollision>().OnCollectableTouch -= CollectibleCollected;
        currentPlayer.GetComponent<PlayerController>().OnPause -= PauseGame;
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
        rareCollectablesPerLevel.Add(new List<Collectable>());
        rareCollectablesCollectedPerLevel.Add(new List<Collectable>());
        goldCollectablesPerLevel.Add(new List<Collectable>());
        goldCollectablesCollectedPerLevel.Add(new List<Collectable>());
        UnityEngine.Object[] collectablesFound = FindObjectsByType(typeof(Collectable), FindObjectsSortMode.None);
        foreach (UnityEngine.Object collectable in collectablesFound)
        {
            Collectable collectableScript = (Collectable)collectable.GetComponent(typeof(Collectable));
            switch (collectableScript.MyCollectableType)
            {
                case Collectable.CollectableType.Normal:
                    break;
                case Collectable.CollectableType.Rare:
                    rareCollectablesPerLevel[currentLevel].Add(collectableScript);
                    break;
                case Collectable.CollectableType.Gold:
                    goldCollectablesPerLevel[currentLevel].Add(collectableScript);
                    break;
                default:
                    break;
            }
        }
    }

    public void CollectibleCollected(Collectable collectableCollected)
    {
        switch (collectableCollected.MyCollectableType)
        {
            case Collectable.CollectableType.Normal:
                break;
            case Collectable.CollectableType.Rare:
                rareCollectablesCollectedPerLevel[currentLevel].Add(collectableCollected);
                break;
            case Collectable.CollectableType.Gold:
                goldCollectablesCollectedPerLevel[currentLevel].Add(collectableCollected);
                break;
            default:
                break;
        }
        OnCollectableCollected?.Invoke(collectableCollected);
    }

    public void GameFinished()
    {
        currentPlayer.GetComponent<PlayerController>().DisableController();
        currentPlayer.GetComponent<PlayerMovement>().StopAllMovement();
        cameraManager.GameFinished();
    }

    public void CinematicFinished()
    {
        uiManager.PlayerWonLevel();
    }

    private IEnumerator RestartingGame()
    {
        yield return new WaitForSeconds(.5f);
        RestartGame();
    }

    [ContextMenu("PrintCollectables")]
    public void PrintCollectablesScore()
    {
        for (int i = 0; i < rareCollectablesCollectedPerLevel.Count; i++)
        {
            print("Rare per Level " + i + ": "+ rareCollectablesCollectedPerLevel[i].Count);
            
        }
        
        for (int i = 0; i < goldCollectablesCollectedPerLevel.Count; i++)
        {
            print("Gold Level " + i + ": "+ goldCollectablesCollectedPerLevel[i].Count);
            
        }
        for (int i = 0; i < rareCollectablesPerLevel.Count; i++)
        {
            print("Rare total per Level " + i + ": "+ rareCollectablesPerLevel[i].Count);
            
        }
        
        for (int i = 0; i < goldCollectablesPerLevel.Count; i++)
        {
            print("Gold total per Level " + i + ": "+ goldCollectablesPerLevel[i].Count);
            
        }
    }
}
