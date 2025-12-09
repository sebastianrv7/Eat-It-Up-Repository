using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionUI : MonoBehaviour
{
    [SerializeField] private GameObject questionPanel;

    private void OnEnable()
    {
        if (QuestionManager.instance != null)
        {
            QuestionManager.instance.OnQuestionStart += ShowQuestionPanel;
            QuestionManager.instance.OnQuestionEnd += HideQuestionPanel;
        }
    }

    private void OnDisable()
    {
        if (QuestionManager.instance != null)
        {
            QuestionManager.instance.OnQuestionStart -= ShowQuestionPanel;
            QuestionManager.instance.OnQuestionEnd -= HideQuestionPanel;
        }
    }

    private void Start()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false); // Make sure panel is hidden at start
    }

    private void ShowQuestionPanel()
    {
        if (questionPanel != null)
            questionPanel.SetActive(true);

        Debug.Log("Question panel shown");
    }

    private void HideQuestionPanel()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);

        Debug.Log("Question panel hidden");
    }

    // Optional: method to resume game from Inspector or button
    public void ResumeQuestion()
    {
        if (QuestionManager.instance != null)
            QuestionManager.instance.EndQuestion();
    }
}
