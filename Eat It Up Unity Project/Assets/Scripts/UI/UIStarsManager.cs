using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStarsManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> scoreStars;

    public void ShowStars(int starsEarned)
    {
        StartCoroutine(AnimatingStars(starsEarned));
    }

    private IEnumerator AnimatingStars(int starsEarned)
    {
        for (int i = 0; i < starsEarned; i++)
        {
            scoreStars[i].SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}
