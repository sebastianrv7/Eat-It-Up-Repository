using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController charController;
    [SerializeField]
    private PlayerCollision myPlayerCollision;
    [SerializeField]
    private PlayerController myPlayerController;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float gravity = 10f;
    [SerializeField]
    private float gravityForce = 1f;
    [SerializeField]
    private float jumpHeight = 10f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float doubleJumpHeight = 10f;
    [SerializeField]
    private float doubleJumpForce = 10f;

    private MovementDirection currentDirection = MovementDirection.Right;
    private Coroutine currentJumpCoroutine;
    private Vector3 movementDirection = Vector3.right;
    private bool isJumping = false;
    private bool isDoubleJumping = false;

    public MovementDirection CurrentDirection { get { return currentDirection; } }


    public delegate void DirectionChange(MovementDirection newDirection);
    public event DirectionChange OnDirectionChange;
    public delegate void JumpBehaviour();
    public event JumpBehaviour OnStartJump, OnStartDoubleJump, OnMaxJumpHeight, OnFinishJump;
//    public delegate void StartDoubleJump();
//    public event StartDoubleJump OnStartDoubleJump;

    public enum MovementDirection
    {
        Left,
        Right
    }

    void OnEnable()
    {
        myPlayerCollision.OnWallTouch += ChangeDirection;
        myPlayerCollision.OnFloorTouch += TouchingFloor;
        myPlayerController.OnJump += JumpPressed;
    }

    void OnDisable()
    {
        myPlayerCollision.OnWallTouch -= ChangeDirection;
        myPlayerCollision.OnFloorTouch -= TouchingFloor;
        myPlayerController.OnJump -= JumpPressed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementDirection.y = -gravity;
        isJumping = false;
        isDoubleJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (currentDirection == MovementDirection.Right)
            movementDirection.x = 1;
        if (currentDirection == MovementDirection.Left)
            movementDirection.x = -1;

        charController.Move(movementDirection * speed * Time.deltaTime);
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

    private void TouchingFloor()
    {
        isJumping = false;
        isDoubleJumping = false;
        OnFinishJump();
    }

    private void JumpPressed()
    {
        //Debug.Log("Jump is: " +isJumping + " double Jump is: " + isDoubleJumping);
        if (!isJumping)
        {
            Jump();
            return;
        }
        if (!isDoubleJumping)
        {
            DoubleJump();
        }
        return;
    }

    [ContextMenu("TestJump")]
    private void Jump()
    {
        isJumping = true;
        currentJumpCoroutine = StartCoroutine(JumpCoroutine());
        OnStartJump();
    }

    private void DoubleJump()
    {
        isDoubleJumping = true;
        StopCoroutine(currentJumpCoroutine);
        movementDirection.y = 0;
        currentJumpCoroutine = StartCoroutine(DoubleJumpCoroutine());
        OnStartDoubleJump();
    }

    private IEnumerator JumpCoroutine()
    {
        float newYMovementDirection;
        float step = gravityForce;
        yield return new WaitForSeconds(0.25f);
        while (movementDirection.y < jumpHeight)
        {
            step += Time.deltaTime * jumpForce;
            newYMovementDirection = Mathf.Lerp(0f, jumpHeight, step);
            movementDirection.y = newYMovementDirection;
            yield return new WaitForEndOfFrame();
        }
        OnMaxJumpHeight();
        while (movementDirection.y > -gravity)
        {
            step += Time.deltaTime * gravityForce;
            newYMovementDirection = Mathf.Lerp(jumpForce, -gravity, step);
            movementDirection.y = newYMovementDirection;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

        private IEnumerator DoubleJumpCoroutine()
    {
        float newYMovementDirection;
        float step = gravityForce;


        while (movementDirection.y < doubleJumpHeight)
        {
            step += Time.deltaTime * doubleJumpForce;
            newYMovementDirection = Mathf.Lerp(0f, doubleJumpHeight, step);
            movementDirection.y = newYMovementDirection;
            yield return new WaitForEndOfFrame();
        }

        OnMaxJumpHeight();
        while (movementDirection.y > -gravity)
        {
            step += Time.deltaTime * gravityForce;
            newYMovementDirection = Mathf.Lerp(jumpForce, -gravity, step);
            movementDirection.y = newYMovementDirection;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
