using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialPanelController : MonoBehaviour
{

    [SerializeField] private GameObject tutorialPanel;

    public void HideTutorial()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        // Disparar inicio del juego
        if (QuestionManager.instance != null)
            QuestionManager.instance.GameStart();
    }


}
