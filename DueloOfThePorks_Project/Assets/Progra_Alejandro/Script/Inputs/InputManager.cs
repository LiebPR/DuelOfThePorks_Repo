using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputActions playerInput;
    public bool isPlayerOne;

    // Propiedad estática para acceder desde cualquier lugar
    public static InputManager Instance { get; private set; }

    // Movement Inputs
    public Vector2 moveInput;
    public bool dashInput;
    public bool crouchInput;
    public bool jumpInput;

    // Attack Inputs
    public bool baseAttackInput;
    public bool strongAttackInput;
    public bool specialAttackInput;

    // Variables para detectar ataques Up y Down
    public bool isWPressed;
    public bool isSPressed;

    // Variables para identificar si se debe realizar un UpAttack o DownAttack
    public bool isUpAttackReady;
    public bool isDownAttackReady;

    private void OnEnable()
    {
        // Todo lo que está en OnEnable se ejecuta una vez cuando el objeto se enciende
        if (playerInput == null)
        {
            playerInput = new PlayerInputActions(); // Crea una copia del mapa de inputs para este objeto

            if (isPlayerOne)
            {
                playerInput.Player1.Move.performed += i => moveInput = i.ReadValue<Vector2>();
                playerInput.Player1.Move.canceled += i => moveInput = Vector2.zero;
                playerInput.Player1.Dash.performed += i => dashInput = true;
                playerInput.Player1.Dash.canceled += i => dashInput = false;
                playerInput.Player1.Crouch.performed += i => crouchInput = true;
                playerInput.Player1.Crouch.canceled += i => crouchInput = false;
                playerInput.Player1.Jump.performed += i => jumpInput = true;

                // Detectamos las teclas W o S
                playerInput.Player1.Up.performed += i => isWPressed = true;
                playerInput.Player1.Up.canceled += i => isWPressed = false;

                playerInput.Player1.Down.performed += i => isSPressed = true;
                playerInput.Player1.Down.canceled += i => isSPressed = false;

                // Detectamos Click Left
                playerInput.Player1.BaseAttack.performed += i => TryExecuteAttack();
                playerInput.Player1.StrongAttack.performed += i => strongAttackInput = true;

                //Ataque espcial
                playerInput.Player1.SpecialAttack.performed += i => specialAttackInput = true;
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

                // Detectamos las teclas W o S
                playerInput.Player2.Up.performed += i => isWPressed = true;
                playerInput.Player2.Up.canceled += i => isWPressed = false;

                playerInput.Player2.Down.performed += i => isSPressed = true;
                playerInput.Player2.Down.canceled += i => isSPressed = false;

                // Detectamos Click Left
                playerInput.Player2.BaseAttack.performed += i => TryExecuteAttack();
                playerInput.Player2.StrongAttack.performed += i => strongAttackInput = true;

                //Ataque especial
                playerInput.Player2.SpecialAttack.performed += i => specialAttackInput = true;
            }

            // Activamos el mapa de Inputs
            playerInput.Enable();
        }
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Método que ejecuta el ataque en función de la tecla presionada (W, S, o nada)
    void TryExecuteAttack()
    {
        if (isWPressed && !isSPressed)  // Si la tecla W está presionada y no S
        {
            // Activamos el ataque hacia arriba
            baseAttackInput = true;
            Debug.Log("Ready to perform UpAttack");
        }
        else if (isSPressed && !isWPressed)  // Si la tecla S está presionada y no W
        {
            // Activamos el ataque hacia abajo
            baseAttackInput = true;
            Debug.Log("Ready to perform DownAttack");
        }
        else  // Si no se presionan W ni S
        {
            // Realizamos el ataque base normal
            baseAttackInput = true;
            Debug.Log("Ready to perform Normal BaseAttack");
        }
    }

    public void ResetJumpInput() => jumpInput = false;
    public void ResetBaseAttackInput() => baseAttackInput = false;
    public void ResetStrongAttackInput() => strongAttackInput = false;
    public void ResetSpecialAttackInput() => specialAttackInput = false;
}