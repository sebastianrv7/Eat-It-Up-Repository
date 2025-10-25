using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // Método público para cambiar a la escena 1
    public void LoadScene1()
    {
        SceneManager.LoadScene("Level1"); // Puedes usar el índice o el nombre de la escena
    }

    // Alternativa usando nombre de escena (más legible)
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
