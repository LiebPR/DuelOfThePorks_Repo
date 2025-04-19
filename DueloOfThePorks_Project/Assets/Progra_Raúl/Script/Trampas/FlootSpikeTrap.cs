using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FloorSpikeTrap : MonoBehaviour
{
    [Header("Animación de las púas")]
    [Tooltip("Animator con los clips Warning, Activate y Retract")]
    [SerializeField] private Animator animator;
    [Tooltip("Permite activar o desactivar la animación de las púas")]
    [SerializeField] private bool enableVFX = true;

    [Header("Temporización")]
    [Tooltip("Tiempo de aviso antes de que salgan las púas")]
    [SerializeField] private float warningTime = 1f;
    [Tooltip("Duración en que las púas hacen daño")]
    [SerializeField] private float activeTime = 1f;
    [Tooltip("Tiempo antes de que la trampa pueda activarse de nuevo")]
    [SerializeField] private float cooldownTime = 4f;

    [Header("Daño (%)")]
    [Tooltip("Porcentaje de daño que aplica la trampa")]
    [SerializeField] private float damagePercent = 10f;

    [Header("Audio")]
    [Tooltip("Permite activar o desactivar el sonido")]
    [SerializeField] private bool enableSFX = true;
    [Tooltip("Sonido que suena al activarse las púas (opcional)")]
    [SerializeField] private AudioClip spikeSound;

    private enum State { Idle, Warning, Active, Cooldown }
    private State currentState = State.Idle;
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != State.Idle) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TrapSequence());
    }

    private IEnumerator TrapSequence()
    {
        // 1) Aviso (telegráfico)
        currentState = State.Warning;
        if (enableVFX && animator != null)
            animator.SetTrigger("Warning");
        yield return new WaitForSeconds(warningTime);

        // 2) Activación: animación, sonido y daño
        currentState = State.Active;
        if (enableVFX && animator != null)
            animator.SetTrigger("Activate");
        if (enableSFX && spikeSound != null)
            AudioSource.PlayClipAtPoint(spikeSound, transform.position);

        // Desactivamos el trigger para controlar manualmente el daño
        col.enabled = false;

        float timer = 0f;
        while (timer < activeTime)
        {
            var hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Player")) continue;
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.ReciveDamage(damagePercent);
            }
            timer += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        // 3) Retracción de las púas
        if (enableVFX && animator != null)
            animator.SetTrigger("Retract");

        // Restauramos el collider y pasamos a cooldown
        col.enabled = true;
        currentState = State.Cooldown;
        yield return new WaitForSeconds(cooldownTime);
        currentState = State.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }

    /// <summary>
    /// Habilita o deshabilita la animación de las púas en tiempo de ejecución.
    /// </summary>
    public void SetVFXEnabled(bool enabled) => enableVFX = enabled;
    /// <summary>
    /// Habilita o deshabilita el sonido de la trampa en tiempo de ejecución.
    /// </summary>
    public void SetSFXEnabled(bool enabled) => enableSFX = enabled;
}
