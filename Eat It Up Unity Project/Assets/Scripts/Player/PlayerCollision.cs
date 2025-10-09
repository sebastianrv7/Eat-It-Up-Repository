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
            OnFloorTouch();
            Debug.Log("Floor Touched");
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            OnWallTouch();
        }
    }
}
