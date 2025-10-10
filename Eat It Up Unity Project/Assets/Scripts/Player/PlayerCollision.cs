using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public delegate void WallTouch();
    public event WallTouch OnWallTouch;
    public delegate void FloorTouch();
    public event FloorTouch OnFloorTouch;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            OnFloorTouch?.Invoke();
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            OnWallTouch?.Invoke();
        }
    }

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
        
    }
}
