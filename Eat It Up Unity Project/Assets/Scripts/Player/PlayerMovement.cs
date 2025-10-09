using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController charController;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float gravity = 10f;
    [SerializeField]
    private PlayerCollision myPlayerCollision;

    private MovementDirection currentDirection = MovementDirection.Right;
    private Vector3 movementDirection = Vector3.right;
    private bool applyGravity = false;
    
    public MovementDirection CurrentDirection {get { return currentDirection; } }

    
    public delegate void DirectionChange(MovementDirection newDirection);
    public event DirectionChange OnDirectionChange;

    public enum MovementDirection
    {
        Left,
        Right
    }

    void OnEnable()
    {
        myPlayerCollision.OnWallTouch += ChangeDirection;
        myPlayerCollision.OnFloorTouch += ToggleGravity;
    }

    void OnDisable()
    {
        myPlayerCollision.OnWallTouch -= ChangeDirection;
        myPlayerCollision.OnFloorTouch -= ToggleGravity;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        Gravity();
    }

    private void HorizontalMovement()
    {
        if (currentDirection == MovementDirection.Right)
            movementDirection.x = 1;
        if (currentDirection == MovementDirection.Left)
            movementDirection.x = -1;

        charController.Move(movementDirection * speed * Time.deltaTime);
    }

    private void Gravity()
    {
        movementDirection.y = -gravity;
    }

    private void ChangeDirection()
    {
        switch (currentDirection)
        {
            case MovementDirection.Left:
                currentDirection = MovementDirection.Right;
                OnDirectionChange(currentDirection);
                break;

            case MovementDirection.Right:
                currentDirection = MovementDirection.Left;
                OnDirectionChange(currentDirection);
                break;

            default:
                break;
        }
    }

    private void ToggleGravity()
    {
     //   applyGravity = !applyGravity;
    }
}
