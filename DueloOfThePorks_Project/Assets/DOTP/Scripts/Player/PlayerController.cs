using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{

    [Header("Componentes")]
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    PlatformEffector2D platformEffector;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float coyoteTime = 0.2f;
    float coyoteTimeCounter;

    [Header("Raycast")]
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;

    [Header("Wall Check")]
    bool isTouchingWall;
    [SerializeField] float lateralCheckDistance = 0.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformEffector = GetComponent<PlatformEffector2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        Move();

    }
    private void Update()
    {
        GroundCheck();
        WallCheck();
        Jump();
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if(horizontalInput != 0 && !isTouchingWall)
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

    void GroundCheck()
    {
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);
        isGrounded = Physics2D.Raycast(bottomOfCapsule, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(bottomOfCapsule, Vector2.down * groundCheckDistance, Color.red);

        //CoyoteTime:
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void WallCheck()
    {
        Vector2 frontOfCapsule = (Vector2)transform.position + new Vector2(Mathf.Sign(rb.velocity.x) * lateralCheckDistance, 0);
        RaycastHit2D hit = Physics2D.Raycast(frontOfCapsule, Vector2.zero, 0f, groundLayer);
        isTouchingWall = hit.collider != null;
        Debug.DrawRay(frontOfCapsule, Vector2.down * groundCheckDistance, Color.red);
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            isGrounded = false;
            coyoteTimeCounter = 0f;
        }
        if (Input.GetButtonDown("Jump") && (isGrounded || coyoteTimeCounter > 0f))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AudioManager.instance.Play("Jump");
        }
    }
}