using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private SceneAsset mainScene;
    [SerializeField]
    private GameObject playerStartPosition;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private CameraManager cameraManager;
    [SerializeField]
    private UIManager uiManager;

    private GameObject currentPlayer;
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
    }

    void OnDisable()
    {
        uiManager.OnRestartButton -= RestartGame;
        uiManager.OnUnpauseButton -= UnpauseGame;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame();
    }

    [ContextMenu("GameOver")]
    public void GameOver()
    {
        currentPlayer.GetComponent<PlayerHealth>().OnDeath -= GameOver;
        playerIsDead = true;
        StartCoroutine(RestartingGame());
    }

    public void StartGame()
    {
        SpawnPlayer();
    }

    [ContextMenu("Restart")]
    public void RestartGame()
    {
        SceneManager.LoadScene(mainScene.name);
    }

    public void SpawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, playerStartPosition.transform);
        cameraManager.SetTrackingTarget(currentPlayer.transform);
        currentPlayer.GetComponent<PlayerHealth>().OnDeath += GameOver;
        currentPlayer.GetComponent<PlayerCollision>().OnFinishTouch += GameWon;
        currentPlayer.GetComponent<PlayerController>().OnPause += PauseGame;
    }

    public void GameWon()
    {
        playerPrefab.GetComponent<PlayerController>().DisableController();
        OnPlayerWon?.Invoke();
    }

    public void PauseGame()
    {
        if (gamePaused)
        {
            uiManager.UnpauseGame();
            return;
        }
        playerPrefab.GetComponent<PlayerController>().DisableController();
        OnPlayerPause?.Invoke();
        gamePaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        gamePaused = false;
        Time.timeScale = 1;
        playerPrefab.GetComponent<PlayerController>().EnableController();
    }

    private IEnumerator RestartingGame()
    {
        yield return new WaitForSeconds(.5f);
        RestartGame();
    }
}
