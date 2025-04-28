using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    Rigidbody2D rb; 
    BoxCollider2D boxCollider;
    PlatformEffector2D platformEff2D; 
    float originalGravity;
    InputManager inputManager;
    KnockbackManager _knockbackManager;
    //Stats Player
    [Header("Variables Globales")]
    float horizontalInput;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f; //Velocidad de movimiento

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f; //Fuerza de primer salto
    [SerializeField] float secondJumpForce = 10f; //Fuerza de segundo salto
    int jumpCount = 0; //Contador de saltos
    [SerializeField] float coyoteTime = 0.2f; //Tiempo extra para poder realizar un salto.
    float coyoteTimeCounter; //Contador coyoteTime

    [Header("Falling")]
    [SerializeField] float fallMultipler = 2.5f; //Que tan rapido cae el jugador comparado con la gravedad normal.
    [SerializeField] float lowJumpMultiplier = 2f; //Para hacer el salto más corto si se suelta el boton antes.

    [Header("Dash")]
    [SerializeField] float dashForce = 12f; //Fuerza aplicada en el dash
    [SerializeField] float dashDuration = 0.2f; //Duración del dash
    [SerializeField] float dashCooldown = 1f; //Tiempo de reutilización del dash
    bool isDashing = false; //Esta haciendo un dash?
    bool canDash = true; //No esta haciendo un dash?
    float lastDashTime = -Mathf.Infinity; //Infinito negativo.

    [Header("Crounch")]
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Collider2D crouchingCollider;
    bool isCrouching = false; //Esta agachado?

    //Detectores:
    [Header("Ground Check")]
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.2f); //Tamaño del detector
    [SerializeField] Vector2 groundCheckOffset = new Vector2(0f, -0.5f); //Posición del detector
    [SerializeField] LayerMask groundLayer; //Layer del Ground
    [SerializeField] bool isGrounded; //Esta tocando el suelo?

    [Header("Wall Check")]
    [SerializeField] Vector2 wallCheckSize = new Vector2(0.3f, 0.5f); //Tamaño del detector
    [SerializeField] Vector2 wallCheckOffset = new Vector2(0.5f, 0f); //Posición del detector
    bool isTouchingWall; //Esta tocando la pared?
    
    
    private void Start()
    {
        platformEff2D = GetComponent<PlatformEffector2D>(); //Para asignar el componente
        inputManager = GetComponent<InputManager>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
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
        //Variables globales:
        horizontalInput = inputManager.moveInput.x;
        GroundCheck();
        Jump();
        HandleCrouch();
        HandleDash();
    }

    //Voids encargados de los statas del player.
    void Move()
    {
        if (_knockbackManager.IsInKnockback()) return; //Si tiene aplicado el Knockback el player no puede caminar.

        //Estas diciendo que quieres que la velocidad del rb sea igual a la velocidad en el eje x multiplicada por el moveSpeed y que mantenga la velocidad en y para que no se vea alterada.
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y); 

        if (horizontalInput != 0 && !isTouchingWall) //Cuando la posición es lo contrario a 0 el player Flip. "!isTouchingWall" te dice que si no estas tocando la pared tambien se flipea.
        {
            Flip();
        }
    }

    void Flip() 
    {
        /*La dirección a la que me quiero mover es diferente a la que estoy mirando? (Mathf.Sign(horizontaInput es la brujula de la direccion hor y
         * Mathf.Sign(transform.localScale.x) es la escala definida en los 3 valores de Sign (-1, 1, 0) por lo que si el move es 1 y la escala es -1 ejecuta el "if"*/ 
        if (Mathf.Sign(horizontalInput) != Mathf.Sign(transform.localScale.x)) 
        {
            //Te flipea el personaje al usar Mathf.Sign ya sabe cual es la izquierda y cual la derecha por lo que te coje la direccion del movimiento y se lo aplica a la escala para que mire al lado correcto.
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
           
            /*Creamos una nueva variable Vector2 llamada newOffset. Referenciamos el capsulleCollider mas exactamente su posición respecto al pivote del player.
             *Si el pivote del player a rotado el resto de componentes rotaran junto al player en la dirección en la que este orientado y mantenemos igual la direccion 
             *en y para que los componentes no flipeen.*/
            Vector2 newOffset = new Vector2(boxCollider.offset.x * Mathf.Sign(horizontalInput), boxCollider.offset.y); 
            boxCollider.offset = newOffset;
        }
    }

    void ApplyBetterJumpPhysics()
    {
        if(rb.velocity.y < 0)
        {
            //Si esta callendo
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultipler - 0.5f) * Time.deltaTime;
        }
        else if(rb.velocity.y > 0 && !inputManager.jumpInput)
        {
            //Si esta subiendo pero ya soltó el botón de salto
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Jump()
    {
        if (inputManager.jumpInput) //Condición; Si presiono el boton asignado en el jumpInput hace: 
        {
            if (isCrouching) //Condición si esta agachado:
            {
                
                inputManager.ResetJumpInput(); //Reset del jumpInput. (Para evitar saltos fantasma)
                return; //Vuelve a leerlo, por lo que si no esta agachado pasa al siguiente if.
            }
           
            if (_knockbackManager.IsInKnockback()) //Condición si esta con knockback aplicado hace:
                return; //vuelve a leer el void desde el principio. 

            if (jumpCount == 0 && (isGrounded || coyoteTimeCounter > 0f)) //Condiciones para hacer:
            {
                inputManager.jumpInput = false; //Restableze el input para poder realizar un segundo salto.
                rb.velocity = new Vector2(rb.velocity.x, 0); //Restableze la velocidad del RB en 0 y mantiene el eje en x intacto.
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //Se aplica una fuerza 2D al RB en el eje vertical y se multiplica por JumpForce. Tipo de fuerza aplicada Impulse.
                
                jumpCount = 1; // Indica a la consola si has saltado y si has saltado 1 vez se suma 1.
            }
            // Segundo salto:
            else 
            {
                if (jumpCount == 1) //Condición si el jumpCount es igual a 1 hace:
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0); //Mantiene la velocidad en el eje x y la restablece en el eje y a 0 (Para evitar problemas).

                    /*Se genera una nueva variable que solo afecta al SecondJump, esta aplica una fuerza horizontal que es la propia del moveSpeed,
                     * esta se multiplica * 0.5 por lo tanto se reduce a la mitad.Y por ultimo se le aplica una fuerza en y con el SecondJumpforce para que tenga una fuerza inferior.*/
                    Vector2 jumpDirection = new Vector2(horizontalInput * moveSpeed * 0.5f, secondJumpForce); 
                    rb.AddForce(jumpDirection, ForceMode2D.Impulse); //Aplica el tipo de fuerza que se aplico en el salto 1 y se lo aplica con la variable creada en este if.
                    
                    jumpCount = 2; // Se le suma a 2 el contador porque a realizado el segundo salto. Por lo tanto se restableze a 0 en el GroundCheck.
                }
                    
            }
            inputManager.ResetJumpInput(); //Se restablece el jumpInput.
        }
    }

    public int GetJumpCount()
    {
        return jumpCount;
    }

    void HandleCrouch()
    {
        if (inputManager.crouchInput && isGrounded) //Condición; Sí aprietas el boton de agacharse y estas isgrounded hace:
        {
            isCrouching = true; //Esta agachado.
            rb.velocity = new Vector2(rb.velocity.x * crouchSpeedMultiplier, rb.velocity.y); //Te multiplica la velocidad por el valor asignado en el crouchSpeedMultipler en x y te mantiene el eje y.

            if (standingCollider != null && crouchingCollider != null) //Condición: ¿Estan asignados los collider? No. Pues seguimos con el resto del codigo || Si, hacemos:
            {
                standingCollider.enabled = false; //Desactiva el collider de estar de pie.
                crouchingCollider.enabled = true; //Activa el collider de estar agachado.
            }
        }
        else
        {
            Vector2 checkPosition = (Vector2)transform.position + Vector2.up * boxCollider.bounds.extents.y; //Cojemos la posición del transform y la sumamos en el eje y, para multiplicarlo por 
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

    public bool IsDashing()
    {
        return isDashing;
    }

    //Detectores:
    void GroundCheck()
    {
        // Ground
        Vector2 groundOrigin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D groundHit = Physics2D.BoxCast(groundOrigin, groundCheckSize, 0f, Vector2.down, 0f, groundLayer);
        bool groundBelow = groundHit.collider != null;

        // Wall (usa dirección del personaje)
        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * direction, wallCheckOffset.y);
        RaycastHit2D wallHit = Physics2D.BoxCast(wallOrigin, wallCheckSize, 0f, Vector2.right * direction, 0f, groundLayer);
        bool wallTouch = wallHit.collider != null;

        isGrounded = groundBelow || wallTouch; //Si cualquiera de las 2 variables toca el suelo isGrounded es true.
        isTouchingWall = wallTouch;

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
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                return;
            }
        }

        Gizmos.color = Color.green;

        // Ground
        Vector2 groundOrigin = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireCube(groundOrigin, groundCheckSize);

        // Wall
        float direction = Application.isPlaying ? Mathf.Sign(transform.localScale.x) : 1f;
        Vector2 wallOrigin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * direction, wallCheckOffset.y);
        Gizmos.DrawWireCube(wallOrigin, wallCheckSize);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}



