using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStarsManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> scoreStars;

    void Awake()
    {
        foreach (GameObject star in scoreStars)
        {
            star.SetActive(false);
        }
    }

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
