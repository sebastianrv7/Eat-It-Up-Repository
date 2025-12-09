using UnityEngine;
using System.Collections;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerCollision myPlayerCollision;
    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    

    


    public event System.Action OnHit, OnDeath;


    void Awake()
    {
        currentHealth = maxHealth;
        
        if (myPlayerCollision == null)
            myPlayerCollision = GetComponent<PlayerCollision>();
    }

    void OnEnable()
    {
        myPlayerCollision.OnDamageReceived += ReceiveDamage;
    }

    void OnDisable()
    {
        myPlayerCollision.OnDamageReceived -= ReceiveDamage;        
    }

    private void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        CheckIfAlive();
    }

    private void CheckIfAlive()
    {
        if (currentHealth > 0)
        {
            OnHit?.Invoke();
            return;
        }
        OnDeath?.Invoke();
        currentHealth = maxHealth; // Reinicia la vida

        

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
