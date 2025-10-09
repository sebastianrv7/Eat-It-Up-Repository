using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public delegate void WallTouch();
    public event WallTouch OnWallTouch;
    public delegate void FloorTouch();
    public event FloorTouch OnFloorTouch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            OnFloorTouch();
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            OnWallTouch();
        }
    }
}
