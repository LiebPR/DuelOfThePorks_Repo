using UnityEngine;
using System.Reflection;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(KnockbackManager))]
[RequireComponent(typeof(CharacterAudioController))]
public class PlayerAnimationBridge : MonoBehaviour
{
    private Animator animator;
    private InputManager inputManager;
    private PlayerController playerController;
    private KnockbackManager knockbackManager;
    private CharacterAudioController audioCtrl;
    private FieldInfo groundedField;

    [Header("Configuración de sufijo")]
    [Tooltip("Ej: _Jamonnator, _Tocinete… Si queda vacío, se detecta del primer clip.")]
    public string characterSuffix;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerController = GetComponent<PlayerController>();
        knockbackManager = GetComponent<KnockbackManager>();
        audioCtrl = GetComponent<CharacterAudioController>();

        // Para leer el campo privado isGrounded de PlayerController
        groundedField = typeof(PlayerController)
            .GetField("isGrounded", BindingFlags.Instance | BindingFlags.NonPublic);

        // Auto‑sufijo si no se pone manual
        if (string.IsNullOrEmpty(characterSuffix))
        {
            var clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                string n = clips[0].clip.name;
                int i = n.IndexOf('_');
                if (i != -1) characterSuffix = n.Substring(i);
            }
        }
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleDash();
        HandleAttackInputs();
    }

    private bool IsGrounded()
    {
        if (groundedField != null)
            return (bool)groundedField.GetValue(playerController);
        return false;
    }

    void HandleMovement()
    {
        float x = inputManager.moveInput.x;
        animator.SetBool("Walk", Mathf.Abs(x) > 0.1f);
        if (x != 0f)
            transform.localScale = new Vector3(Mathf.Sign(x), 1f, 1f);
    }

    void HandleJump()
    {
        bool grounded = IsGrounded();
        animator.SetBool("Jump", !grounded);

        if (inputManager.jumpInput && !grounded)
            audioCtrl.PlayJump();
    }

    void HandleCrouch()
    {
        bool grounded = IsGrounded();
        bool crouch = inputManager.crouchInput && grounded;
        animator.SetBool("Crouch", crouch);
        animator.SetBool("CrouchWalk", crouch && Mathf.Abs(inputManager.moveInput.x) > 0.1f);
    }

    void HandleDash()
    {
        if (inputManager.dashInput)
        {
            animator.SetTrigger("Dash" + characterSuffix);
            audioCtrl.PlayDash();
        }
    }

    void HandleAttackInputs()
    {
        if (inputManager.baseAttackInput)
        {
            if (inputManager.isWPressed && !inputManager.isSPressed) PlayUpAttack();
            else if (inputManager.isSPressed && !inputManager.isWPressed) PlayDownAttack();
            else PlayBaseAttack();

            inputManager.ResetBaseAttackInput();
        }
        if (inputManager.strongAttackInput)
        {
            PlayStrongAttack();
            inputManager.ResetStrongAttackInput();
        }
    }

    // Cada método de ataque dispara animación + audio correspondiente:
    public void PlayBaseAttack()
    {
        animator.SetTrigger("AttackBasic" + characterSuffix);
        audioCtrl.PlayBaseAttack();
    }

    public void PlayUpAttack()
    {
        animator.SetTrigger("UpAttack" + characterSuffix);
        audioCtrl.PlayUpAttack();
    }

    public void PlayDownAttack()
    {
        animator.SetTrigger("DownAttack" + characterSuffix);
        audioCtrl.PlayDownAttack();
    }

    public void PlayStrongAttack()
    {
        animator.SetTrigger("AttackStrong" + characterSuffix);
        audioCtrl.PlayStrongAttack();
    }

    public void PlaySpecialAttack()
    {
        animator.SetTrigger("AttackSpecial" + characterSuffix);
        audioCtrl.PlaySpecialAttack();
    }

    public void PlayJumpAttack()
    {
        animator.SetTrigger("JumpAttack" + characterSuffix);
        audioCtrl.PlayJumpAttack();
    }

    public void PlayHurt()
    {
        animator.SetTrigger("Hurt" + characterSuffix);
        audioCtrl.PlayHurt();
    }
}
