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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    //Funcion que activa el KnockBack
    public void StartKnockback(Vector2 direction, float force, float duration)
    {
        if (isKnockBack) return; //Si ya esta en knockback ya no hacemos nada
        isKnockBack = true;
        knockBackDirection = direction;
        knockbackForce = force;
        knockbackDuration = duration;

        rb.velocity = Vector2.zero; //Detectamos al jugador antes de aplicar el knockback
        rb.gravityScale = 0; //Desactivamos la gravedad para que el knockback no sea afectado por ella

        //Aplicamos el efecto de knockback
        rb.AddForce(knockBackDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(KnockbackRoutine());
    }

    //Coroutine que maneja la duración del Knockback
    IEnumerator KnockbackRoutine()
    {
        yield return new WaitForSeconds(knockbackDuration);

        isKnockBack = false;
        rb.gravityScale = originalGravity; //Restaurar la gravedad
    }

    //Utilizado para verificar si el jugador está en knockback
    public bool IsInKnockback()
    {
        return isKnockBack;
    }
}
