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
    [SerializeField] LayerMask groundLayer; //Layer del Ground
    [SerializeField] bool isGrounded; //Esta tocando el suelo?

    [Header("Wall Check")]
    [SerializeField] Vector2 wallCheckSize = new Vector2(0.3f, 0.5f); //Tamaño del detector
    [SerializeField] float lateralCheckDistance = 0.5f; //Posición del detector
    bool isTouchingWall; //Esta tocando la pared?
    
    
    private void Start()
    {
        platformEff2D = GetComponent<PlatformEffector2D>(); //Para asignar el componente
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
            Vector2 newOffset = new Vector2(capsuleCollider.offset.x * Mathf.Sign(horizontalInput), capsuleCollider.offset.y); 
            capsuleCollider.offset = newOffset;
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
        if (inputManager.jumpInput) //El input de Jump: Si presiono el boton asignado el el InputManager salta.
        {
            if (isCrouching) //Si esta agachado:
            {
                //Resetea el input de Jump: Si no hiciera esto si soltara el boton de crouch el player almacenaría la presión del boton y te haría un salto fantasma, de esta manera te aseguras de que no pase.
                inputManager.ResetJumpInput(); 
                //Vuelve a al anterior if (¿Has pulsado el boton? Si ¿Estas agachado? No. Pasa al siguiente.
                return; 
            }
            /*Hacemos referencia al _knockBackManager para usar el void publico IsKnockback() que es una comprobación de si esta en knockback o no, si lo esta vuelve al principio
             * y vuelve a leer, así hasta que no este en knockback y viceversa*/
            if (_knockbackManager.IsInKnockback()) return; 

            // Primer salto: Cuando el contador de saltos es igual a 0 (y esta tocando el suelo o coyoteTimeCounter es mayor a 0, hace:
            if (jumpCount == 0 && (isGrounded || coyoteTimeCounter > 0f))
            {
                inputManager.jumpInput = false; //Pasa el input de salto a falso para poder realizar el segundo salto. 
                rb.velocity = new Vector2(rb.velocity.x, 0); //Se coje la velocidad del rigidbody y se matiene en x, pero se reestableze a 0 en y 
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Se le aplica una fuerza igual al valor de jumpForce y se le aplica esta fuerza con el tipo de Fuerza2D Impulse. 
                AudioManager.instance.Play("Jump"); //Para el audio de salto.
                jumpCount = 1; // El contador detecta que has hecho un salto. No lo detecta se lo dices tu pero así podra pasar al segundo salto.
            }
            // Segundo salto
            else 
            {
                if (jumpCount == 1) //Si el jumpCount es 1 puede ralizar el segundo salto.
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    Vector2 jumpDirection = new Vector2(horizontalInput * moveSpeed * 0.5f, secondJumpForce); //Diferencia con el primer salto, el salto 2 se aplica el valor de jumpForce2 a la fuerza en Impulse.
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

/*Mathf: 
 *Es una clase estetica que contiene un montón de funciones y constantes matemáticas útiles, pensadas para trabajar con números tipo Float.
 *(Ahorra tiempo de codeo y te facilita el no tener que saber de algebra o trigonometria)*/

/*Infinity:
 *Es el valor más extremo de algo si. (Mathf.Infinity = Es el infinito positiovo osea el numero más grande / -Mathf.Infinity = Es el infinito negativo que es el numero más pequeño)
 *Solo se usa en comparaciones "Absurdas" de un numero entero o Float con un numero infinito positivo o negativo.*/

/*Sign:
 *Si dices Mathf.Sign(x) pillaría 1, -1 o 0. Lo mismo con todos los ejes.(Tiene definido de base en unity cual es la izquierda o derecha y cual es arriba o abajo)
 *Muy util para ahorrar codigo. Ejemplo.
 *Mathf.Sign(y) == 1 Significa que esta Subiendo
 *Mathf.Sign(y) == -1 Significa que esta Bajando
 *Mathf.Sign(y) == 0 Significa que esta Flotando*/

/* void Flip(float horizontalInput): 
 * Esto es una clase con un parametro tipo Float  horitzontalInput es el nombre de la variable que va a recibir este valor.*/

/*offset: 
 *Este se encarga de ajustar o orinter correctamente los componentes del GameObject que tiene adjuntado el script.*/

/* AddForce:
 * Función de unity que se usa para aplicar una fuerza a un Rigidbody ya sea 2D o 3D
 * Formas de aplicar dicha fuerza:
 *      -Impulse = Le das una patada -> sale volando.
 *      -Force = Aplica una fuerza constante pero teniendo en cuenta la masa del player.
 *      -VelocityChange = La teletransportas con velocidad
 *      -Acceleratión = Aplica una fuerza constante sin tener en cuenta la masa del Rigidbody*/


