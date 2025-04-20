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
    private CharacterAudioController audioCtrl;
    private FieldInfo groundedField;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerController = GetComponent<PlayerController>();
        audioCtrl = GetComponent<CharacterAudioController>();

        groundedField = typeof(PlayerController)
            .GetField("isGrounded", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleDash();
        HandleAttacks();
    }

    private bool IsGrounded()
    {
        if (groundedField != null)
            return (bool)groundedField.GetValue(playerController);
        return false;
    }

    private void HandleMovement()
    {
        float h = inputManager.moveInput.x;
        animator.SetBool("Walk", Mathf.Abs(h) > 0.1f);
        if (h != 0f)
            transform.localScale = new Vector3(Mathf.Sign(h), 1f, 1f);
    }

    private void HandleJump()
    {
        bool grounded = IsGrounded();
        animator.SetBool("Jump", !grounded);
        if (inputManager.jumpInput && !grounded)
        {
            audioCtrl.PlayJump();
            inputManager.jumpInput = false;
        }
    }

    private bool isCrouched;
    private void HandleCrouch()
    {
        bool grounded = IsGrounded();

        // entrar/agacharse
        if (inputManager.crouchInput && grounded && !isCrouched)
        {
            animator.SetTrigger("Crouch");
            isCrouched = true;
        }
        // levantarse
        else if (!inputManager.crouchInput && isCrouched)
        {
            animator.SetTrigger("StandUp");
            isCrouched = false;
        }

        // caminar agachado
        if (isCrouched)
        {
            float h = inputManager.moveInput.x;
            animator.SetBool("CrouchWalk", Mathf.Abs(h) > 0.1f);
        }
        else animator.SetBool("CrouchWalk", false);
    }

    private void HandleDash()
    {
        if (inputManager.dashInput)
        {
            animator.SetTrigger("Dash");
            audioCtrl.PlayDash();
            inputManager.dashInput = false;
        }
    }

    private void HandleAttacks()
    {
        // básico / up / down / special
        if (inputManager.baseAttackInput)
        {
            if (inputManager.isWPressed && !inputManager.isSPressed)
            {
                animator.SetTrigger("UpAttack");
                audioCtrl.PlayUpAttack();
            }
            else if (inputManager.isSPressed && !inputManager.isWPressed)
            {
                animator.SetTrigger("DownAttack");
                audioCtrl.PlayDownAttack();
            }
            else
            {
                animator.SetTrigger("AttackBasic");
                audioCtrl.PlayBaseAttack();
            }

            inputManager.ResetBaseAttackInput();
        }

        // ataque fuerte
        if (inputManager.strongAttackInput)
        {
            animator.SetTrigger("AttackStrong");
            audioCtrl.PlayStrongAttack();
            inputManager.ResetStrongAttackInput();
        }

        // **ataque especial** (si tienes un input para ello)
        if (inputManager.specialAttackInput)  // ← asume que existe en tu InputManager
        {
            animator.SetTrigger("AttackSpecial");
            audioCtrl.PlaySpecialAttack();
            inputManager.ResetSpecialAttackInput();
        }
    }
}
