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
        Debug.Log("Player activo: " + gameObject.activeSelf + " | Sprite activo: " + GetComponent<SpriteRenderer>().enabled);
        HandleJump();
        HandleDash();
        wasDashing = playerControlanim.IsDashing();
        HandleWalk();
        HandleCrouch();
    }

    void HandleJump()
    {
        if(animInput.jumpInput && playerControlanim.IsGrounded())
        {
            animManager.SetBool("Jump", true);
        }
        else if (!playerControlanim.IsGrounded())
        {
            animManager.SetBool("Jump", true);
        }
        else
        {
            animManager.SetBool("Jump", false);
        }
    }

    void HandleDash()
    {
        bool isDashingNow = playerControlanim.IsDashing();

        if(!wasDashing && isDashingNow)
        {
            animManager.ResetTrigger("Dash"); //Por si estaba bloqueado
            animManager.SetTrigger("Dash");
        }
    }

    void HandleWalk()
    {
        bool isMoving = Mathf.Abs(playerControlanim.GetComponent<Rigidbody2D>().velocity.x) > 0.1f; //El player se esta moviendo

        if (playerControlanim.IsGrounded() && isMoving)
        {
            animManager.SetBool("Walk", true);
        }
        else
        {
            animManager.SetBool("Walk", false);
        }
    }

    
    void HandleCrouch()
    {
        bool ground = playerControlanim.IsGrounded();
        float h = animInput.moveInput.x;

        if(animInput.crouchInput && ground && !isCrouching)
        {
            animManager.SetTrigger("Crouch");
            isCrouching = true;
        }

        if (isCrouching)
        {
            if (animInput.crouchInput)
            {
                animManager.SetBool("CrouchWalk", Mathf.Abs(h) > 0.1f && ground);
            }
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
        if (index == 0)
            animManager.SetTrigger("UpAttack");
        else if (index == 1)
            animManager.SetTrigger("DownAttack");
        else if (index == 2)
            animManager.SetTrigger("AttackBasic");
        else if (index == 3)
            animManager.SetTrigger("AttackStrong");
        else if (index == 4 && GetComponent<PlayerOrbs>().CanUseSpecialAttack())
            animManager.SetTrigger("AttackSpecial");
    }
}
