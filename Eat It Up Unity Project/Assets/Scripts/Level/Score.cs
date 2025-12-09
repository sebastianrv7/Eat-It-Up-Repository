using System;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    private int totalScore = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int scoreToAdd)
    {
        totalScore += scoreToAdd;
        Debug.Log($"Puntaje añadido: {scoreToAdd} | Puntaje total acumulado: {totalScore}");
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}
