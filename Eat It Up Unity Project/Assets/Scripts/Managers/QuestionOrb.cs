using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionOrb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            QuestionManager.instance.StartQuestion();
            gameObject.SetActive(false); // Desaparece el orbe
        }
    }
}
