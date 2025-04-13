using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackManager : MonoBehaviour
{
    [SerializeField] bool isKnockBack; //¿Esta haciendo un Knockback?
    [SerializeField] float knockbackForce; //Fuerza del knockback
    [SerializeField] float knockbackDuration; //Duracion del Knockback

    Rigidbody2D rb;
    Vector2 knockBackDirection;
    float originalGravity;
    

    InputManager _inputManager;
    PlayerController _playerController;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;

        _inputManager = GetComponent<InputManager>();
        _playerController = GetComponent<PlayerController>();
    }

    //Funcion que activa el KnockBack
    public void StartKnockback(Vector2 direction, float force, float duration)
    {
        if (_inputManager.crouchInput && _playerController.IsGrounded()) return;
        if (isKnockBack) return;

        isKnockBack = true;
        knockBackDirection = direction.normalized;
        knockbackForce = force;
        knockbackDuration = duration;

        rb.gravityScale = 0; // Desactivamos gravedad momentáneamente
        StartCoroutine(KnockbackRoutine());
    }

    //Coroutine que maneja la duración del Knockback
    IEnumerator KnockbackRoutine()
    {
        float timer = 0f;
        float t = 0f;

        Vector2 initialVelocity = knockBackDirection * knockbackForce;

        while (timer < knockbackDuration)
        {
            // Usamos interpolación para simular la desaceleración
            t = timer / knockbackDuration;
            Vector2 currentVelocity = Vector2.Lerp(initialVelocity, Vector2.zero, t);
            rb.velocity = currentVelocity;

            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isKnockBack = false;
    }

    //Utilizado para verificar si el jugador está en knockback
    public bool IsInKnockback()
    {
        return isKnockBack;
    }
}
