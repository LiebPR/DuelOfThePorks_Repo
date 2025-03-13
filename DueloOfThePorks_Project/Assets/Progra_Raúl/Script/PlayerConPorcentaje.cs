using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerConPorcentaje : MonoBehaviour
{
    [Header("Componentes")]
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    PlatformEffector2D platformEff2D;
    float originalGravity;

    //Stats Player
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float secondJumpForce = 6f;
    [SerializeField] int maxJumpCount = 2;
    int jumpCount = 0;
    [SerializeField] float coyoteTime = 0.2f;
    float coyoteTimeCounter;

    [Header("Dash")]
    [SerializeField] float dashForce = 12f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    bool isDashing = false;
    bool canDash = true;
    float lastDashTime = -Mathf.Infinity;

    //Detectores:
    [Header("Raycast")]
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;

    [Header("Wall Check")]
    bool isTouchingWall;
    [SerializeField] float lateralCheckDistance = 0.5f;

    public DamagePercentage damageScript;
    private void Start()
    {
        platformEff2D = GetComponent<PlatformEffector2D>();
        damageScript = GetComponent<DamagePercentage>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalGravity = rb.gravityScale;
        if (Input.GetKeyDown(KeyCode.J)) damageScript.Attack1();
        if (Input.GetKeyDown(KeyCode.K)) damageScript.Attack2();
        if (Input.GetKeyDown(KeyCode.L)) damageScript.Attack3();
        if (Input.GetKeyDown(KeyCode.Semicolon)) damageScript.Attack4();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
    }

    private void Update()
    {
        GroundCheck();
        Jump();
        HandleDash();
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
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

    void Jump()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            isGrounded = false;
            coyoteTimeCounter = 0f;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < maxJumpCount)
            {
                if (jumpCount == 0 && (isGrounded || coyoteTimeCounter > 0f))
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    AudioManager.instance.Play("Jump");
                    jumpCount = 1;
                }
                else if (jumpCount == 1)
                {
                    rb.velocity = new Vector2(rb.velocity.x, secondJumpForce);
                    AudioManager.instance.Play("Jump");
                    jumpCount = 2;
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

        if (Input.GetKeyDown(KeyCode.Q) && canDash && !isDashing)
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
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), true);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        rb.velocity = new Vector2(rb.velocity.x, currentYVelocity);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), false);
    }

    void GroundCheck()
    {
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);
        bool groundBelow = Physics2D.Raycast(bottomOfCapsule, Vector2.down, groundCheckDistance, groundLayer);
        float direction = transform.localScale.x;
        Vector2 boxSize = new Vector2(0.3f, capsuleCollider.bounds.size.y * 0.5f);
        Vector2 boxOrigin = (Vector2)transform.position + new Vector2(direction * lateralCheckDistance, -capsuleCollider.bounds.extents.y * 0.5f);
        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, groundCheckDistance, groundLayer);
        bool groundSide = hit.collider != null;
        isGrounded = groundBelow || groundSide;
        if (isGrounded)
        {
            if (jumpCount == 2)
            {
                jumpCount = 0;
            }
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
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
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomOfCapsule, bottomOfCapsule + Vector2.down * groundCheckDistance);
        float direction = transform.localScale.x;
        Vector2 frontOfCapsule = (Vector2)transform.position + new Vector2(direction * lateralCheckDistance, 0);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(frontOfCapsule + Vector2.down * groundCheckDistance / 2, new Vector2(0.2f, 0.1f));
    }
}
