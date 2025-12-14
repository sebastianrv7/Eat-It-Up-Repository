using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlowController : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private ParticleSystem glow;
    private ParticleSystem.MainModule main;
    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        glow = player.GlowParticles;
        main = glow.main;
        emission = glow.emission;

        // Suscribirse a los eventos del multiplicador
        ScoreMultiplierManager.Instance.OnMultiplierActivated += UpdateGlow;
        ScoreMultiplierManager.Instance.OnMultiplierLost += UpdateGlow;

        UpdateGlow(); // Actualizar en inicio
    }

    private void UpdateGlow()
    {
        int m = ScoreMultiplierManager.Instance.GetCurrentMultiplier();

        switch (m)
        {
            case 1:
                glow.Stop();
                break;

            case 2:
                glow.Play();
                main.startSize = 0.25f;
                emission.rateOverTime = 20;
                break;

            case 3:
                glow.Play();
                main.startSize = 0.45f;
                emission.rateOverTime = 40;
                break;

            case 5:
                glow.Play();
                main.startSize = 0.50f;
                emission.rateOverTime = 80;
                break;
        }
    }
}
