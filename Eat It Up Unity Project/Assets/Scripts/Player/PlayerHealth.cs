using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerCollision myPlayerCollision;
    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    private bool isAlive;

    public bool IsAlive { get{ return isAlive; }}

    public delegate void HealthActions();
    public event HealthActions OnHit, OnDeath;


    void Awake()
    {
        currentHealth = maxHealth;
        isAlive = true;
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
        isAlive = false;
        OnDeath?.Invoke();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
