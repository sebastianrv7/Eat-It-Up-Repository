using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D myRigidbody;
    [SerializeField]
    private PlayerCollision myPlayerCollision;
    [SerializeField]
    private PlayerController myPlayerController;
    [SerializeField]
    private PlayerHealth myPlayerHealth;
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
    [SerializeField]
    private float slideForce = 2f;
    [SerializeField]
    private float wallJumpForce = 10f;

    private MovementDirection currentDirection = MovementDirection.Right;
    private LayerMask layerMask;
    private Coroutine currentJumpCoroutine;
    private Coroutine wallSlideCoroutine;
    private Vector3 movementDirection = Vector3.right;
    private bool isJumping = false;
    private bool isDoubleJumping = false;
    private bool isGrounded = true;
    private bool isSliding = false;

    public MovementDirection CurrentDirection { get { return currentDirection; } }


    public delegate void DirectionChange(MovementDirection newDirection);
    public event DirectionChange OnDirectionChange;
    public delegate void JumpBehaviour();
    public event JumpBehaviour OnStartJump, OnStartDoubleJump, OnMaxJumpHeight, OnFinishJump;
    public delegate void Action();
    public event Action OnStartSlide, OnStopSlide;

    public enum MovementDirection
    {
        Left,
        Right
    }

    void OnEnable()
    {
        myPlayerCollision.OnWallTouch += HandleWallTouch;
        myPlayerCollision.OnFloorTouch += TouchingFloor;
        myPlayerController.OnJump += JumpPressed;
        myPlayerHealth.OnDeath += StopAllMovement;
    }


    void OnDisable()
    {
        myPlayerCollision.OnWallTouch -= HandleWallTouch;
        myPlayerCollision.OnFloorTouch -= TouchingFloor;
        myPlayerController.OnJump -= JumpPressed;
        myPlayerHealth.OnDeath -= StopAllMovement;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementDirection.y = -gravity;
        isJumping = false;
        isDoubleJumping = false;
        isGrounded = true;
        isSliding = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!myPlayerHealth.IsAlive)
            return;
        Movement();
        //CheckIfGrounded();
    }

    private void Movement()
    {
        if (currentDirection == MovementDirection.Right)
            movementDirection.x = 1;
        if (currentDirection == MovementDirection.Left)
            movementDirection.x = -1;

        if (myRigidbody != null)
            myRigidbody.linearVelocity = movementDirection * speed * Time.deltaTime;
    }


    private void HandleWallTouch()
    {
        if (!isGrounded)
        {
            if (!isSliding)
            {
                isSliding = true;
                OnStartSlide?.Invoke();
                if (currentJumpCoroutine != null)
                    StopCoroutine(currentJumpCoroutine);

                wallSlideCoroutine = StartCoroutine(WallSlideCoroutine());
                return;
            }
        }
        ChangeDirection();
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
        if (CheckIfGrounded())
        {
            isGrounded = true;
            isJumping = false;
            StopSlide();
            isDoubleJumping = false;
            OnFinishJump?.Invoke();
        }
    }

    private bool CheckIfGrounded()
    {
        layerMask = LayerMask.GetMask("Floor");
        if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), 1.25f, layerMask) && !isGrounded)
        {
            return true;
        }
        return false;
    }

    private void JumpPressed()
    {
        if (isSliding)
        {
            WallJump();
            return;
        }
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
        isGrounded = false;
        isJumping = true;
        currentJumpCoroutine = StartCoroutine(JumpCoroutine());
        OnStartJump?.Invoke();
    }

    private void DoubleJump()
    {
        isGrounded = false;
        isDoubleJumping = true;
        StopCoroutine(currentJumpCoroutine);
        movementDirection.y = 0;
        currentJumpCoroutine = StartCoroutine(DoubleJumpCoroutine());
        OnStartDoubleJump?.Invoke();
    }

    private void WallJump()
    {
        OnDirectionChange?.Invoke(currentDirection);
        StopSlide();
        ChangeDirection();

        // Impulso de salto en pared
        movementDirection.y = wallJumpForce;

        // Opcional: ligero impulso horizontal para separarse de la pared
        movementDirection.x = currentDirection == MovementDirection.Right ? 1 : -1;

        isJumping = true;
        OnStartJump?.Invoke();
    }

    private void StopSlide()
    {
        isSliding = false;
        OnStopSlide?.Invoke();
        if (wallSlideCoroutine != null)
            StopCoroutine(wallSlideCoroutine);
        if (isGrounded)
            ChangeDirection();
    }

    private void StopAllMovement()
    {
        StopAllCoroutines();
        movementDirection = new Vector3(0f, 0f, 0f);
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


    private IEnumerator WallSlideCoroutine()
    {
        float newYMovementDirection;
        float step = slideForce;
        // El jugador cae lentamente mientras está en contacto con la pared
        while (isSliding)
        {
            
            step += Time.deltaTime * slideForce;
            newYMovementDirection = Mathf.Lerp(0f, -gravity, step);
            movementDirection.y = newYMovementDirection;
            yield return new WaitForEndOfFrame();
        }
        StopSlide();
        yield return null;
    }
}
