using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // M�todo p�blico para cambiar a la escena 1
    public void LoadScene1()
    {
        SceneManager.LoadScene("Level1"); // Puedes usar el �ndice o el nombre de la escena
    }

    // Alternativa usando nombre de escena (m�s legible)
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
