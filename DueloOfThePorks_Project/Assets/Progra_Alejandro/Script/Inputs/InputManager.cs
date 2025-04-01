using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputActions playerInput;
    public bool isPlayerOne;

    //Variables para almacenar la información de input
    //Una variable por cada accion, del mismo tipo que la acción. Si són Button = bool
    public Vector2 moveInput;
    public bool dashInput;
    public bool crouchInput;
    public bool jumpInput;

    private void OnEnable()
    {
        //Todo lo que está en OnEnable se ejecuta una vez cuando el objeto se enciende: Awake()
        if(playerInput == null)
        {
            playerInput = new PlayerInputActions(); //Crea una copia del mapa de inputs para este objeto en concreto
            if (isPlayerOne)
            {
                playerInput.Player1.Move.performed += i => moveInput = i.ReadValue<Vector2>();
                playerInput.Player1.Move.canceled += i => moveInput = Vector2.zero;
                playerInput.Player1.Dash.performed += i => dashInput = true;
                playerInput.Player1.Dash.canceled += i => dashInput = false;
                playerInput.Player1.Crouch.performed += i => crouchInput = true;
                playerInput.Player1.Crouch.canceled += i => crouchInput = false;
                playerInput.Player1.Jump.performed += i => jumpInput = true;
            }
            else
            {
                playerInput.Player2.Move.performed += i => moveInput = i.ReadValue<Vector2>();
                playerInput.Player2.Move.canceled += i => moveInput = Vector2.zero;
                playerInput.Player2.Dash.performed += i => dashInput = true;
                playerInput.Player2.Dash.canceled += i => dashInput = false;
                playerInput.Player2.Crouch.performed += i => crouchInput = true;
                playerInput.Player2.Crouch.canceled += i => crouchInput = false;
                playerInput.Player2.Jump.performed += i => jumpInput = true;
            }
            //Activar el mapa de Inputs de este objeto en concreto
            playerInput.Enable();
        }
    }

    private void OnDisable()
    {
        //Todo lo que está en OnDisable se ejecuta una vez cuando el objeto se apaga
        playerInput.Disable();
    }

    public void ResetJumpInput()
    {
        jumpInput = false;
    }
}
