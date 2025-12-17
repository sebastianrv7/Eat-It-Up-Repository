using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager instance;

    // Events for UI
    public event Action OnQuestionStart;
    public event Action OnQuestionEnd;

    // Events for game start and end
    public event Action OnGameStart;
    public event Action OnGameEnd;

    //  NUEVO: Flag para saber si hay una pregunta activa
    private bool isQuestionActive = false;

    //  NUEVO: Propiedad pública de solo lectura para que otros scripts consulten
    public bool IsQuestionActive => isQuestionActive;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [ContextMenu("Start Question (Inspector)")]
    public void StartQuestion()
    {

        if (isQuestionActive)
            return; // Evita empezar otra mientras hay una activa

        Debug.Log("Question started");
        isQuestionActive = true;
        Time.timeScale = 0f; // Pause the game
        OnQuestionStart?.Invoke();
    }

    [ContextMenu("End Question (Inspector)")]
    public void EndQuestion()
    {

        if (!isQuestionActive)
            return;

        Debug.Log("Question ended");
        isQuestionActive = false;
        Time.timeScale = 1f; // Resume the game
        OnQuestionEnd?.Invoke();
    }

    [ContextMenu("Correct Answer (Inspector)")]
    public void CorrectAnswer()
    {
        Debug.Log("Correct answer");

        // Increase the multiplier
        if (ScoreMultiplierManager.Instance != null)
            ScoreMultiplierManager.Instance.ActivateNextMultiplier();

        // Update UI text for multiplier
        if (UIManager.instance != null)
            UIManager.instance.UpdateCollectableText();

        EndQuestion(); // End question and resume
    }

    [ContextMenu("Incorrect Answer (Inspector)")]
    public void IncorrectAnswer()
    {
        Debug.Log("Incorrect answer");

        // Reset the multiplier
        if (ScoreMultiplierManager.Instance != null)
            ScoreMultiplierManager.Instance.ResetMultiplier();

        // Update UI text for multiplier
        if (UIManager.instance != null)
            UIManager.instance.UpdateCollectableText();

        EndQuestion(); // End question and resume
    }

    [ContextMenu("Resume Game From Inspector")]
    public void ResumeGame()
    {
        EndQuestion();
    }

    public void GameStart()
    {
        Debug.Log("GAME START");
        OnGameStart?.Invoke();
    }

    public  void EndGame()
    {
        Debug.Log("GAME END");
        OnGameEnd?.Invoke();
    }

    //  NUEVO: Método seguro para forzar pausa externa (útil si necesitas)
    public void ForcePauseIfQuestionActive()
    {
        if (isQuestionActive)
            Time.timeScale = 0f;
    }
}
