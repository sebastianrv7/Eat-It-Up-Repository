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

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [ContextMenu("Start Question (Inspector)")]
    public void StartQuestion()
    {
        Debug.Log("Question started");
        Time.timeScale = 0f; // Pause the game
        OnQuestionStart?.Invoke();
    }

    [ContextMenu("End Question (Inspector)")]
    public void EndQuestion()
    {
        Debug.Log("Question ended");
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
}
