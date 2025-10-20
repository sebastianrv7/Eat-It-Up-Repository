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

}
