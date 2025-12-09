using System;
using UnityEngine;

public class ScoreMultiplierManager : MonoBehaviour
{

    public static ScoreMultiplierManager Instance;

    // Multiplicador actual
    [SerializeField]
    private int currentMultiplier = 1;

    // Secuencia de multiplicadores
    private int[] multipliers = { 1, 2, 3, 5 };
    private int currentIndex = 0;

    // Eventos vacíos (los puedes conectar luego a triggers del juego)
    public event Action OnMultiplierActivated;
    public event Action OnMultiplierLost;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Presiona U para aumentar el multiplicador
        if (Input.GetKeyDown(KeyCode.U))
        {
            ActivateNextMultiplier();

            // Actualizar la UI
            if (UIManager.instance != null)
                UIManager.instance.UpdateCollectableText();
        }

        // Presiona I para resetear el multiplicador
        if (Input.GetKeyDown(KeyCode.I))
        {
            ResetMultiplier();

            // Actualizar la UI
            if (UIManager.instance != null)
                UIManager.instance.UpdateCollectableText();
        }
    }

    /// <summary>
    /// Activa el siguiente potenciador en la secuencia (x1  x2  x3  x5).
    /// </summary>
    public void ActivateNextMultiplier()
    {
        if (currentIndex < multipliers.Length - 1)
            currentIndex++;

        currentMultiplier = multipliers[currentIndex];

        // Dispara el evento vacío
        OnMultiplierActivated?.Invoke();

        Debug.Log("Nuevo multiplicador de score: x" + currentMultiplier);
    }

    /// <summary>
    /// Resetea el multiplicador a x1.
    /// </summary>
    public void ResetMultiplier()
    {
        currentIndex = 0;
        currentMultiplier = multipliers[currentIndex];

        // Dispara el evento vacío
        OnMultiplierLost?.Invoke();

        Debug.Log("Multiplicador reseteado a x" + currentMultiplier);
    }

    /// <summary>
    /// Devuelve el multiplicador actual.
    /// </summary>
    public int GetCurrentMultiplier()
    {
        return currentMultiplier;
    }
}
