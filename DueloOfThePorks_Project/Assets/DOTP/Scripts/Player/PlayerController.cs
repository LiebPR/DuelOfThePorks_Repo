using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    PlatformEffector2D platformEffector;

    //Stats Player
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float secondJumpForce = 6f;
    [SerializeField] int maxJumpCount = 2;
    [SerializeField] int jumpCount = 0;
    [SerializeField] float coyoteTime = 0.2f; //Tiempo extra para poder realizar un salto.
    float coyoteTimeCounter; //Contador coyoteTime

    //Detectores:
    [Header("Raycast")]
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;

    [Header("Wall Check")]
    bool isTouchingWall;
    [SerializeField] float lateralCheckDistance = 0.5f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        GroundCheck();
        Jump();
        
    }

    //Voids encargados de los statas del player.
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

                if (jumpCount == 0)
                {
                    if (isGrounded || coyoteTimeCounter > 0f)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        AudioManager.instance.Play("Jump");
                        jumpCount = 1;
                    }
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

    //Detectores:
    void GroundCheck()
    {
        //Detector del suelo:
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);
        bool groundBelow = Physics2D.Raycast(bottomOfCapsule, Vector2.down, groundCheckDistance, groundLayer);

        //Detector de la pared:
        float direction = transform.localScale.x;
        Vector2 boxSize = new Vector2(0.3f, capsuleCollider.bounds.size.y * 0.5f);
        Vector2 boxOrigin = (Vector2)transform.position + new Vector2(direction * lateralCheckDistance, -capsuleCollider.bounds.extents.y * 0.5f);

        RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0, Vector2.down, groundCheckDistance, groundLayer);
        bool groundSide = hit.collider != null;

        isGrounded = groundBelow || groundSide; //Si cualquiera de las 2 variables toca el suelo isGrounded es true.


        if (isGrounded) //Cuando player toca el suelo...
        {
           
            
            //Doble jump:
            if (jumpCount == 2)
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

        // Raycast Suelo
        Vector2 bottomOfCapsule = (Vector2)transform.position - new Vector2(0, capsuleCollider.bounds.extents.y);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomOfCapsule, bottomOfCapsule + Vector2.down * groundCheckDistance);

        // Boxcast lateral
        float direction = transform.localScale.x;
        Vector2 frontOfCapsule = (Vector2)transform.position + new Vector2(direction * lateralCheckDistance, 0);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(frontOfCapsule + Vector2.down * groundCheckDistance / 2, new Vector2(0.2f, 0.1f));
    }
}
