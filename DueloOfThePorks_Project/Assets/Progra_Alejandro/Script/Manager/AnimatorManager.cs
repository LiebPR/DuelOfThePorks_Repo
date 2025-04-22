using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator animManager;
    PlayerController playerControlanim;
    InputManager animInput;

    bool wasDashing = false;
    public bool isCrouching;

    private void Awake()
    {
        animManager = GetComponent<Animator>();
        playerControlanim = GetComponent<PlayerController>();
        animInput = GetComponent<InputManager>();
    }

    private void Update()
    {
        HandleJump();
        HandleDash();
        wasDashing = playerControlanim.IsDashing();
        HandleWalk();
        HandleCrouch();
    }

    void HandleJump()
    {
        bool grounded = playerControlanim.IsGrounded();
        if ((animInput.jumpInput && grounded) || !grounded)
            animManager.SetBool("Jump", true);
        else
            animManager.SetBool("Jump", false);
    }

    void HandleDash()
    {
        bool isDashingNow = playerControlanim.IsDashing();
        if (!wasDashing && isDashingNow)
        {
            animManager.ResetTrigger("Dash");
            animManager.SetTrigger("Dash");
        }
    }

    void HandleWalk()
    {
        bool isMoving = Mathf.Abs(playerControlanim.GetComponent<Rigidbody2D>().velocity.x) > 0.1f;
        animManager.SetBool("Walk", playerControlanim.IsGrounded() && isMoving);
    }

    void HandleCrouch()
    {
        bool grounded = playerControlanim.IsGrounded();
        float h = animInput.moveInput.x;

        if (animInput.crouchInput && grounded && !isCrouching)
        {
            animManager.SetTrigger("Crouch");
            isCrouching = true;
        }

        if (isCrouching)
        {
            if (animInput.crouchInput)
                animManager.SetBool("CrouchWalk", Mathf.Abs(h) > 0.1f && grounded);
            else
            {
                animManager.SetTrigger("StandUp");
                animManager.SetBool("CrouchWalk", false);
                isCrouching = false;
            }
        }
    }

    
    public void PlayAttackAnimation(int index)
    {
        switch (index)
        {
            case 0:
                animManager.SetTrigger("UpAttack");
                break;
            case 1:
                animManager.SetTrigger("DownAttack");
                break;
            case 2:
                animManager.SetTrigger("AttackBasic");
                break;
            case 3:
                animManager.SetTrigger("AttackStrong");
                break;
            case 4:
                if (GetComponent<PlayerOrbs>().CanUseSpecialAttack())
                    animManager.SetTrigger("AttackSpecial");
                break;
        }
    }
}