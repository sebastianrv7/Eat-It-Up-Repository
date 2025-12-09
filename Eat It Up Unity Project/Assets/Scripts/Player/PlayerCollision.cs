using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public delegate void PlayerTouch();
    public event PlayerTouch OnWallTouch, OnWallStopTouch, OnFloorTouch, OnFinishTouch, OnCameraStopTouch;
    public delegate void EnemyTouch(int damage);
    public event EnemyTouch OnDamageReceived;
    public delegate void CollectableTouch(Collectable colectable);
    public event CollectableTouch OnCollectableTouch;

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
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Collectable collectableCollected = collision.GetComponentInParent<Collectable>();
            if (collectableCollected == null)
                return;
            collectableCollected.ObjectCollected();
            OnCollectableTouch?.Invoke(collectableCollected);
        }
        if (collision.gameObject.CompareTag("CameraStop"))
        {
            OnCameraStopTouch?.Invoke();
        }
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            OnWallStopTouch?.Invoke();
        }
    }

    
}
