using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    PlatformEffector2D platformEff2D;
    float originalGravity;
    InputManager inputManager;
    KnockbackManager _knockbackManager;
    //Stats Player
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float secondJumpForce = 10f;
    [SerializeField] int maxJumpCount = 2;
    int jumpCount = 0;
    [SerializeField] float coyoteTime = 0.2f; //Tiempo extra para poder realizar un salto.
    float coyoteTimeCounter; //Contador coyoteTime

    [Header("Falling")]
    [SerializeField] float fallMultipler = 2.5f; //Que tan rapido cae rl jugsdor comparado con la gravedad normal.
    [SerializeField] float lowJumpMultiplier = 2f; //Para hacer el salto más corto si se suelta el boton antes.

    [Header("Dash")]
    [SerializeField] float dashForce = 12f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    bool isDashing = false;
    bool canDash = true;
    float lastDashTime = -Mathf.Infinity;

    [Header("Crounch")]
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Collider2D crouchingCollider;
    bool isCrouching = false;

    //Detectores:
    [Header("Boxcast")]
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.2f);
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;

    [Header("Wall Check")]
    bool isTouchingWall;
    [SerializeField] float lateralCheckDistance = 0.5f;
    [SerializeField] Vector2 wallCheckSize = new Vector2(0.3f, 0.5f);

    private void Start()
    {
        platformEff2D = GetComponent<PlatformEffector2D>();
        inputManager = GetComponent<InputManager>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalGravity = rb.gravityScale;
        _knockbackManager = GetComponent<KnockbackManager>();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
            ApplyBetterJumpPhysics();
        }
    }

    private void Update()
    {
        GroundCheck();
        Jump();
        HandleCrouch();
        HandleDash();
    }

    //Voids encargados de los statas del player.
    void Move()
    {
        if (_knockbackManager.IsInKnockback()) return;
        float horizontalInput = inputManager.moveInput.x;

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (horizontalInput != 0 && !isTouchingWall)
        {
            Flip(horizontalInput);
        }
    }

    void Flip(float horizontalInput)
    {
        if (Mathf.Sign(horizontalInput) != Mathf.Sign(transform.localScale.x))
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
            Vector2 newOffset = new Vector2(capsuleCollider.offset.x * Mathf.Sign(horizontalInput), capsuleCollider.offset.y);
            capsuleCollider.offset = newOffset;
        }
    }

    void ApplyBetterJumpPhysics()
    {
        if(rb.velocity.y < 0)
        {
            //Si esta callendo
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultipler - 1) * Time.deltaTime;
        }
        else if(rb.velocity.y > 0 && !inputManager.jumpInput)
        {
            //Si esta subiendo pero ya soltó el botón de salto
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Jump()
    {
        if (inputManager.jumpInput)
        {
            if (isCrouching)
            {
                inputManager.ResetJumpInput();
                return;
            }
            if (_knockbackManager.IsInKnockback()) return;

            float horizontalInput = inputManager.moveInput.x;

            // Primer salto
            if (jumpCount == 0 && (isGrounded || coyoteTimeCounter > 0f))
            {
                inputManager.jumpInput = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                AudioManager.instance.Play("Jump");
                jumpCount = 1; // Primer salto
            }
            // Segundo salto
            else 
            {
                if (jumpCount == 1)
                {
                    inputManager.jumpInput = false;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    Vector2 jumpDirection = new Vector2(horizontalInput * moveSpeed * 0.5f, secondJumpForce);
                    rb.AddForce(jumpDirection, ForceMode2D.Impulse);
                    AudioManager.instance.Play("Jump");
                    jumpCount = 2; // Segundo salto
                }
                    
            }
            inputManager.ResetJumpInput();
        }
    }

    void HandleCrouch()
    {
        if (inputManager.crouchInput && isGrounded)
        {
            isCrouching = true;
            rb.velocity = new Vector2(rb.velocity.x * crouchSpeedMultiplier, rb.velocity.y);

            if (standingCollider != null && crouchingCollider != null)
            {
                standingCollider.enabled = false;
                crouchingCollider.enabled = true;
            }
        }
        else
        {
            Vector2 checkPosition = (Vector2)transform.position + Vector2.up * capsuleCollider.bounds.extents.y;
            bool headBlocked = Physics2D.Raycast(checkPosition, Vector2.up, 0.1f, groundLayer);

            if (!headBlocked)
            {
                isCrouching = false;

                if (standingCollider != null && crouchingCollider != null)
                {
                    standingCollider.enabled = true;
                    crouchingCollider.enabled = false;
                }
            }
        }
    }

    void HandleDash()
    {
        if (Time.time - lastDashTime >= dashCooldown && !isDashing)
        {
            canDash = true;
        }

        if (inputManager.dashInput && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        rb.velocity = Vector2.zero;

        lastDashTime = Time.time;

        float direction = transform.localScale.x;

        float currentYVelocity = rb.velocity.y;

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        rb.AddForce(new Vector2(direction * dashForce, 0), ForceMode2D.Impulse);

        if(gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player2"), true);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player1"), true);
        }

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        rb.velocity = new Vector2(rb.velocity.x, currentYVelocity);

        if(gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player2"), false);
        }
        else if(gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player1"), false);
        }

        
    }

    //Detectores:
    void GroundCheck()
    {
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);

        //Detector del suelo:
        Vector2 boxOrigin = bottomOfCapsule;
        Vector2 boxSize = new Vector2(capsuleCollider.bounds.size.x, groundCheckSize.y);

        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, 0f, groundLayer);
        bool groundBelow = hit.collider != null;

        //Detector de la pared:
        float direction = transform.localScale.x;
        Vector2 lateralBoxOrigin = (Vector2)transform.position + new Vector2(direction * lateralCheckDistance, -capsuleCollider.bounds.extents.y * 0.5f);
        Vector2 lateralBoxSize = wallCheckSize;

        RaycastHit2D sidehit = Physics2D.BoxCast(lateralBoxOrigin, lateralBoxSize, 0f, Vector2.right * direction, 0f, groundLayer);
        bool groundSide = sidehit.collider != null;

        isGrounded = groundBelow || groundSide; //Si cualquiera de las 2 variables toca el suelo isGrounded es true.


        if (isGrounded)
        {
            //Doble jump:
            if(jumpCount == 2)
            {
                jumpCount = 0;
            }
            //CoyoteTime:
            coyoteTimeCounter = coyoteTime; //...el contador del coyoteTime recibe de vuelta el valor de coyoteTime...
        }
        else //Cuando el player esta en el aire...
        {
            coyoteTimeCounter -= Time.deltaTime; //... el tiempo aplicado en coyoteTimeCounter se reduce poco a poco.
        }
    }

    private void OnDrawGizmos()
    {
        if (capsuleCollider == null)
        {
            capsuleCollider = GetComponent<CapsuleCollider2D>();
            if (capsuleCollider == null)
            {
                return;
            }
        }

        // Boxcast Suelo
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bottomOfCapsule, new Vector2(capsuleCollider.bounds.size.x, groundCheckSize.y));

        // Boxcast lateral
        float direction = transform.localScale.x;
        Vector2 lateralBoxOrigin = (Vector2)transform.position + new Vector2(direction * lateralCheckDistance, -capsuleCollider.bounds.extents.y * 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(lateralBoxOrigin, wallCheckSize);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
