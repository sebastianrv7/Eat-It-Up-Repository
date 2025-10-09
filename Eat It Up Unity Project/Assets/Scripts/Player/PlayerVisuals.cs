using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private GameObject visuals;
    [SerializeField]
    private PlayerMovement myPlayerMovement;   

    void OnEnable()
    {
        myPlayerMovement.OnDirectionChange += ChangeVisualDirection;
    }

    void OnDisable()
    {
        myPlayerMovement.OnDirectionChange -= ChangeVisualDirection;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeVisualDirection(myPlayerMovement.CurrentDirection);
    }
    
    private void ChangeVisualDirection(PlayerMovement.MovementDirection newDirection)
    {
        switch (newDirection)
        {
            case PlayerMovement.MovementDirection.Left:
                gameObject.transform.localScale = new Vector3(-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                break;

            case PlayerMovement.MovementDirection.Right:
                gameObject.transform.localScale = new Vector3(1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                break;

            default:
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
    

}
