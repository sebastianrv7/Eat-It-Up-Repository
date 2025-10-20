using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField]
    private List<string> levels;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void loadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

	public void loadScene(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

	public void loadNextScene()
    {
        int sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
		if(sceneToLoad>=SceneManager.sceneCountInBuildSettings)
		{
			sceneToLoad=0;
		}
		SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

	public void reloadScene()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
	}

	public void tryLoadScene(int sceneToLoad)
    {

		if(sceneToLoad>=SceneManager.sceneCountInBuildSettings)
		{
			sceneToLoad = SceneManager.sceneCountInBuildSettings-1;
		}
		else if(sceneToLoad<0)
		{
			sceneToLoad = 0;
		}
		SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
		
    }

	public void tryLoadScene(string sceneToLoad)
    {
		SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

	public void tryLoadPreviousScene()
    {
		int sceneIndex = SceneManager.GetActiveScene().buildIndex;
		sceneIndex--;
		if(sceneIndex<0)
		{
			sceneIndex = 0;
		}
		SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }

	public void tryLoadNextScene()
    {
		int sceneIndex = SceneManager.GetActiveScene().buildIndex;
		sceneIndex++;
		if(sceneIndex>=SceneManager.sceneCountInBuildSettings)
		{
			sceneIndex = SceneManager.sceneCountInBuildSettings-1;
		}
		SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }

	public bool isSceneLoaded(int sceneToLoad)
	{
		Scene currentScene = SceneManager.GetActiveScene();
		if(!currentScene.buildIndex.Equals(sceneToLoad))
		{
			return false;
		}
		return true;
	}

	public void loadSceneZero()
    {
        Scene sceneZero = SceneManager.GetSceneByBuildIndex(0);
		loadScene(0);
    }

    public void loadSceneOne()
    {
        Scene sceneOne = SceneManager.GetSceneByBuildIndex(1);
		loadScene(1);
    }

	public int getSceneIndex()
    {	
		return SceneManager.GetActiveScene().buildIndex;
    }

	public int getSceneCount()
	{
		return SceneManager.sceneCountInBuildSettings;
	}

	public string getSceneName()
	{
		return SceneManager.GetActiveScene().name;
	}
}
