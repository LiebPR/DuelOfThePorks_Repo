using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(KnockbackManager))]
[RequireComponent(typeof(CharacterAudioController))]
[RequireComponent(typeof(AttackManager))]
public class PlayerAnimationBridge : MonoBehaviour
{
    private Animator animator;
    private InputManager inputManager;
    private PlayerController playerController;
    private CharacterAudioController audioCtrl;
    private AttackManager attackManager;

    // Cached Animator parameter hashes
    static readonly int HashWalk = Animator.StringToHash("Walk");
    static readonly int HashJump = Animator.StringToHash("Jump");
    static readonly int HashCrouch = Animator.StringToHash("Crouch");
    static readonly int HashStandUp = Animator.StringToHash("StandUp");
    static readonly int HashCrouchWalk = Animator.StringToHash("CrouchWalk");
    static readonly int HashDash = Animator.StringToHash("Dash");
    static readonly int HashAttackBasic = Animator.StringToHash("AttackBasic");
    static readonly int HashUpAttack = Animator.StringToHash("UpAttack");
    static readonly int HashDownAttack = Animator.StringToHash("DownAttack");
    static readonly int HashStrong = Animator.StringToHash("AttackStrong");
    static readonly int HashSpecial = Animator.StringToHash("AttackSpecial");

    private bool isCrouched;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerController = GetComponent<PlayerController>();
        audioCtrl = GetComponent<CharacterAudioController>();
        attackManager = GetComponent<AttackManager>();

        // Suscribir al evento de ataque
        attackManager.onAttackPerformed += OnAttackPerformed;

        // Opcional: permite que el Animator siga corriendo si Time.timeScale == 0
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    void OnDestroy()
    {
        attackManager.onAttackPerformed -= OnAttackPerformed;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleDash();
        // Los ataques se manejan por evento, no por polling
    }

    private void HandleMovement()
    {
        float h = inputManager.moveInput.x;
        animator.SetBool(HashWalk, Mathf.Abs(h) > 0.1f);
        if (h != 0f)
            transform.localScale = new Vector3(Mathf.Sign(h), 1f, 1f);
    }

    private void HandleJump()
    {
        bool grounded = playerController.IsGrounded();
        animator.SetBool(HashJump, !grounded);
        if (inputManager.jumpInput && grounded)
        {
            audioCtrl.PlayJump();
            inputManager.jumpInput = false;
        }
    }

    private void HandleCrouch()
    {
        bool grounded = playerController.IsGrounded();
        float h = inputManager.moveInput.x;

        if (inputManager.crouchInput && grounded && !isCrouched)
        {
            animator.SetTrigger(HashCrouch);
            isCrouched = true;
        }
        if (isCrouched)
        {
            if (inputManager.crouchInput)
                animator.SetBool(HashCrouchWalk, Mathf.Abs(h) > 0.1f && grounded);
            else
            {
                animator.SetTrigger(HashStandUp);
                animator.SetBool(HashCrouchWalk, false);
                isCrouched = false;
            }
        }
    }

    private void HandleDash()
    {
        if (inputManager.dashInput)
        {
            animator.SetTrigger(HashDash);
            audioCtrl.PlayDash();
            inputManager.dashInput = false;
        }
    }

    // Se ejecuta únicamente cuando AttackManager dispara el evento
    private void OnAttackPerformed(int index)
    {
        switch (index)
        {
            case 0:
                if (inputManager.isWPressed && !inputManager.isSPressed)
                    animator.SetTrigger(HashUpAttack);
                else if (inputManager.isSPressed && !inputManager.isWPressed)
                    animator.SetTrigger(HashDownAttack);
                else
                    animator.SetTrigger(HashAttackBasic);
                break;
            case 1:
                animator.SetTrigger(HashStrong);
                break;
            case 2:
                animator.SetTrigger(HashSpecial);
                break;
        }
    }
}
