using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackManager : MonoBehaviour
{
    public bool isKnockBack; //¿Esta haciendo un Knockback?
    [SerializeField] float knockbackForce; //Fuerza del knockback
    [SerializeField] float knockbackDuration; //Duracion del Knockback

    public Rigidbody2D rb;
    Vector2 knockBackDirection;
    public float originalGravity;
    

    InputManager _inputManager;
    PlayerController _playerController;
    HitDetector _hitDetector;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;

        _inputManager = GetComponent<InputManager>();
        _playerController = GetComponent<PlayerController>();
        _hitDetector = GetComponent<HitDetector>();
    }

    //Funcion que activa el KnockBack
    public void StartKnockback(Vector2 direction, float force, float duration, float damagePercentage)
    {
        if(_hitDetector != null && _hitDetector.isInvincible)
        {
            return;
        }
        if (_inputManager.crouchInput && _playerController.IsGrounded()) return; //No le afecta el knockback si esta agachado. 
        if (isKnockBack) return; //Cuando esta en Knockback vuelve a la normalidad.

        isKnockBack = true;
        knockBackDirection = direction.normalized;

        //Calculamos el knockback extra según el porcentaje del jugador
        float knockbackExtra = Mathf.Floor(damagePercentage / 10f) * 2; //PARTE IMPORTANTE (Si quieres que el knockback base se sume más cada 10% aumentar en este apartado)
        knockbackForce = force + knockbackExtra; // Sumamos el knockback extra a la fuerza del ataque
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
