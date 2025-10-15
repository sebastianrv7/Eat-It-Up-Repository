using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public delegate void PlayerTouch();
    public event PlayerTouch OnWallTouch, OnFloorTouch, OnFinishTouch;
    public delegate void EnemyTouch(int damage);
    public event EnemyTouch OnDamageReceived;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            OnFloorTouch?.Invoke();
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            OnWallTouch?.Invoke();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            int damage = collision.GetComponentInParent<DamagePlayer>().MyDamage;
            OnDamageReceived?.Invoke(damage);
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            OnFinishTouch?.Invoke();
        }

    }
}
