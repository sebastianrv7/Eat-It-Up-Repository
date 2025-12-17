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
    private GameObject floorCollisionStartCenter;
    [SerializeField]
    private GameObject floorCollisionStartLeft;
    [SerializeField]
    private GameObject floorCollisionStartRight;
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
    private float slideSpeed = 2f;
    [SerializeField]
    private float slideForce = 2f;
    [SerializeField]
    private float wallJumpHeight = 10f;
    [SerializeField]
    private float wallJumpForce = 10f;
    [SerializeField]
    private CapsuleCollider2D bodyCollision;

    [SerializeField] private SpriteRenderer spriteRenderer;  // Asigna esto en el Inspector
    private Color originalColor;

    private float normalSpeed;
    private bool isSlowed = false;

    private System.Action deathStunCallback;
    private System.Action hitStunCallback;

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

    void Awake()
    {
        // ... tu código existente
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void OnEnable()
    {
        myPlayerCollision.OnWallTouch += HandleWallTouch;
        myPlayerCollision.OnWallStopTouch += StopWallTouch;
        //myPlayerCollision.OnFloorTouch += TouchingFloor;
        myPlayerController.OnJump += JumpPressed;


        deathStunCallback = () => StunForSeconds(1f);
        hitStunCallback = () => StunForSeconds(1f);

        myPlayerHealth.OnDeath += deathStunCallback;
        myPlayerHealth.OnHit += hitStunCallback;
    }


    void OnDisable()
    {
        myPlayerCollision.OnWallTouch -= HandleWallTouch;
        myPlayerCollision.OnWallStopTouch -= StopWallTouch;
        //myPlayerCollision.OnFloorTouch -= TouchingFloor;
        myPlayerController.OnJump -= JumpPressed;
        myPlayerHealth.OnDeath -= deathStunCallback;
        myPlayerHealth.OnHit -= hitStunCallback;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementDirection.y = -gravity;
        isJumping = false;
        isDoubleJumping = false;
        isGrounded = true;
        isSliding = false;
        movementDirection.y = 0f;
        normalSpeed = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (GameManager.LevelFinished)
            return;

        if (CheckIfGrounded())
                TouchingFloor();
        //        if (isSliding || !isGrounded)
        if (isStunned) return;

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
        {
            Vector2 newVelocity = myRigidbody.velocity;
            newVelocity.x = movementDirection.x * speed * Time.fixedDeltaTime;
            myRigidbody.velocity = newVelocity;

        }
    }


    private void HandleWallTouch()
    {
        if (!isGrounded)
        {
            //  Si venías de un salto desde muro, fuerza reset del slide
            if (isSliding)
            {
                StopWallTouch();    // <-- ESTA ES LA LÍNEA IMPORTANTE
            }

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

    private void StopWallTouch()
    {
        if (isGrounded)
            return;
        StopSlide();
        OnMaxJumpHeight?.Invoke();
    }

    private void ChangeDirection()
    {
        switch (currentDirection)
        {
            case MovementDirection.Left:
                currentDirection = MovementDirection.Right;
                OnDirectionChange?.Invoke(currentDirection);
                break;

            case MovementDirection.Right:
                currentDirection = MovementDirection.Left;
                OnDirectionChange?.Invoke(currentDirection);
                break;

            default:
                break;
        }
    }

    private void TouchingFloor()
    {
        if (!isGrounded)
        {
            isGrounded = true;
            isJumping = false;
            isDoubleJumping = false;
            if (isSliding)
                StopSlide();
            OnFinishJump?.Invoke();
            movementDirection.y = 0f;
        }
    }

    private bool CheckIfGrounded()
    {
        layerMask = LayerMask.GetMask("Floor");
        if ((Physics2D.Raycast(floorCollisionStartCenter.transform.position, transform.up * -1, 0.35f, layerMask)
            || Physics2D.Raycast(floorCollisionStartLeft.transform.position, transform.up * -1, 0.35f, layerMask)
            || Physics2D.Raycast(floorCollisionStartRight.transform.position, transform.up * -1, 0.35f, layerMask))
            //&& !isGrounded
            && myRigidbody.velocity.y <= 0)
        {
            return true;
        }
        isGrounded = false;
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
        //currentJumpCoroutine = StartCoroutine(JumpCoroutine());

        // CORRECCIÓN: reasignar el vector completo en lugar de modificar .y directamente
        Vector2 newVelocity = myRigidbody.velocity;
        newVelocity.y = 0f;
        myRigidbody.velocity = newVelocity;

        // aplicar impulso para el salto
        myRigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

        OnStartJump?.Invoke();
        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.Jump);
    }

    private void DoubleJump()
    {
        isGrounded = false;
        isDoubleJumping = true;

        if (currentJumpCoroutine != null)
            StopCoroutine(currentJumpCoroutine);

        movementDirection.y = 0;
        //currentJumpCoroutine = StartCoroutine(DoubleJumpCoroutine());

        // CORRECCIÓN: reasignar el vector completo en lugar de modificar .y directamente
        Vector2 newVelocity = myRigidbody.velocity;
        newVelocity.y = 0f;
        myRigidbody.velocity = newVelocity;

        // aplicar impulso para el doble salto
        myRigidbody.AddForce(new Vector2(0f, doubleJumpForce), ForceMode2D.Impulse);

        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.DoubleJump);
        OnStartDoubleJump?.Invoke();
    }

    private void WallJump()
    {
        StopSlide();
        ChangeDirection();

        // CORRECCIÓN: reasignar el vector completo en lugar de modificar .y directamente
        Vector2 newVelocity = myRigidbody.velocity;
        newVelocity.y = 0f;
        myRigidbody.velocity = newVelocity;

        // aplicar impulso para el wall jump
        myRigidbody.AddForce(new Vector2(0f, wallJumpForce), ForceMode2D.Impulse);

        isJumping = true;
        isDoubleJumping = false;

        SoundManager.instance.PlaySFX(SoundManager.SoundFXType.Jump);
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

    public void StopAllMovement()
    {
        StopAllCoroutines();
        movementDirection = new Vector3(0f, 0f, 0f);
    }

    private IEnumerator JumpCoroutine()
    {
        float newYMovementDirection;
        float step = gravityForce;
        //yield return new WaitForSeconds(0.25f);
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
        
        while (isSliding)
        {

            step += Time.deltaTime * slideForce;
            newYMovementDirection = Mathf.SmoothStep(0f, -slideSpeed, step);
            movementDirection.y = newYMovementDirection;

            // corregido: reasignar el vector completo
            Vector2 newVelocity = myRigidbody.velocity;
            newVelocity.y = movementDirection.y * speed * Time.fixedDeltaTime;
            myRigidbody.velocity = newVelocity;

            yield return new WaitForEndOfFrame();
        }
        StopSlide();
        yield return null;
    }
    
        private IEnumerator WallJumpCoroutine()
    {
        float newYMovementDirection;
        float step = gravityForce;

        while (movementDirection.y < wallJumpHeight)
        {
            step += Time.deltaTime * wallJumpForce;
            newYMovementDirection = Mathf.Lerp(0f, wallJumpHeight, step);
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

    private bool isStunned = false;

    public void SetStunnedState(bool value)
    {
        if (value)
        {
            if (!isSlowed)
            {
                StartCoroutine(SlowAndRecover());
            }
        }
    }

    private IEnumerator StunSequence()
    {
        isStunned = true;

        // 1) Frenar movimiento horizontal pero NO el vertical (para evitar flotar)
        movementDirection = Vector3.zero;
        if (myRigidbody != null)
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

        // 2) Detener coroutines activas de movimiento
        if (currentJumpCoroutine != null)
        {
            StopCoroutine(currentJumpCoroutine);
            currentJumpCoroutine = null;
        }

        if (wallSlideCoroutine != null)
        {
            StopCoroutine(wallSlideCoroutine);
            wallSlideCoroutine = null;
        }

        // STUN ACTIVO POR 1 SEGUNDO
        yield return new WaitForSeconds(1f);

        // 3) Cambiar dirección después del stun
        

        // 4) Limpiar flags para evitar quedarse en slide/jump
        isSliding = false;
        isJumping = false;
        isDoubleJumping = false;

        isStunned = false;
    }

    private Coroutine stunCoroutine;

    public void StunForSeconds(float duration)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        
        SetStunnedState(true);        // Se queda quieto
        yield return new WaitForSeconds(duration);
        SetStunnedState(false);
        
        stunCoroutine = null;
        
    }

    private IEnumerator SlowAndRecover()
    {
        isSlowed = true;

        // Guardar color original (por si acaso)
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // 1) Reducir velocidad
        speed = normalSpeed * 0.5f;

        // 2) Cambiar dirección inmediatamente
        ChangeDirection();

        // === NUEVO: Restar 100 puntos por recibir daño ===
        if (Score.Instance != null)
        {
            Score.Instance.AddScore(-100);
        }

        // === Actualizar UI del puntaje (similar a IncorrectAnswer) ===
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScoreText();  // <-- Cambio correcto aquí
                                                   
        }

        // === EFECTO VISUAL: Parpadeo rojo ===
        if (spriteRenderer != null)
        {
            float flashDuration = 1f;     // Duración total del efecto
            float flashSpeed = 0.25f;        // Cuánto tiempo dura cada "parpadeo"
            Color damageColor = new Color(1f, 0.6f, 0.65f, 1f);  // Color rojo intenso

            float elapsed = 0f;

            while (elapsed < flashDuration)
            {
                // Alternar entre rojo y color original
                spriteRenderer.color = (Mathf.FloorToInt(elapsed / flashSpeed) % 2 == 0) ? damageColor : originalColor;

                elapsed += Time.deltaTime;
                yield return null;  // Espera al siguiente frame
            }

            // Asegurarse de volver al color original al final
            spriteRenderer.color = originalColor;
        }

        // Esperar 1 segundo mientras sigue moviéndose lento
        yield return new WaitForSeconds(1f);

        // 3) Restaurar velocidad
        speed = normalSpeed;

        isSlowed = false;
    }
}
