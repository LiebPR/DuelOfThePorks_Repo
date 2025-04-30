using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class FloorSpikeTrap : MonoBehaviour
{
    [Header("Inicio automático")]
    [Tooltip("Segundos que tarda la trampa en activarse por primera vez")]
    [SerializeField] private float initialActivationDelay = 2f;

    [Header("Collider Ajustable")]
    [Tooltip("Tamaño del área de la trampa")]
    [SerializeField] private Vector2 colliderSize = new Vector2(1f, 1f);
    [Tooltip("Offset del área respecto al pivote")]
    [SerializeField] private Vector2 colliderOffset = Vector2.zero;

    [Header("Visual")]
    [Tooltip("Arrastra aquí el SpriteRenderer de las púas")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Sprite con púas retraídas (idle)")]
    [SerializeField] private Sprite idleSprite;
    [Tooltip("Sprite con púas en warning (intermedio)")]
    [SerializeField] private Sprite warningSprite;
    [Tooltip("Sprite con púas extendidas (active)")]
    [SerializeField] private Sprite activeSprite;
    [Tooltip("Sprite con púas retrocediendo (retract)")]
    [SerializeField] private Sprite retractSprite;

    [Header("Temporización")]
    [Tooltip("Segundos que dura el warning")]
    [SerializeField] private float warningDuration = 0.5f;
    [Tooltip("Segundos que la trampa está activa (daño)")]
    [SerializeField] private float activeDuration = 1f;
    [Tooltip("Segundos que dura la retracción")]
    [SerializeField] private float retractDuration = 0.5f;
    [Tooltip("Segundos antes de volver a activar")]
    [SerializeField] private float cooldownTime = 4f;

    [Header("Daño")]
    [Tooltip("Porcentaje de daño que aplica la trampa")]
    [SerializeField] private float damageAmount = 10f;
    [Tooltip("Capas a las que hace daño (marca Player1 y Player2)")]
    [SerializeField] private LayerMask damageLayers;

    [Header("Knockback")]
    [Tooltip("Fuerza de knockback aplicada al jugador")]
    [SerializeField] private float knockbackForce = 5f;
    [Tooltip("Duración del knockback")]
    [SerializeField] private float knockbackDuration = 0.5f;

    [Header("Audio")]
    [Tooltip("Sonido que se reproduce cuando la trampa hace daño")]
    [SerializeField] private AudioClip hitSound;
    [Range(0f, 1f)]
    [Tooltip("Volumen al reproducir el sonido de golpe")]
    [SerializeField] private float hitSoundVolume = 1f;

    private BoxCollider2D col;
    private AudioSource audioSource;

    private void OnValidate()
    {
        col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.isTrigger = true;
            col.size = colliderSize;
            col.offset = colliderOffset;
        }
    }

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = colliderSize;
        col.offset = colliderOffset;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (spriteRenderer != null && idleSprite != null)
            spriteRenderer.sprite = idleSprite;

        StartCoroutine(AutomaticTrap());
    }

    private IEnumerator AutomaticTrap()
    {
        // Espera inicial antes de la primera activación
        yield return new WaitForSeconds(initialActivationDelay);

        while (true)
        {
            // 1) Warning
            if (spriteRenderer != null && warningSprite != null)
                spriteRenderer.sprite = warningSprite;
            yield return new WaitForSeconds(warningDuration);

            // 2) Active + Daño + Knockback + Sonido
            if (spriteRenderer != null && activeSprite != null)
                spriteRenderer.sprite = activeSprite;

            // Reproducir sonido de golpe con el volumen ajustado
            if (hitSound != null)
                audioSource.PlayOneShot(hitSound, hitSoundVolume);

            // Detectar a quién golpea
            var hits = Physics2D.OverlapBoxAll(
                (Vector2)transform.position + colliderOffset,
                colliderSize,
                0f,
                damageLayers
            );
            foreach (var hit in hits)
            {
                // Aplicar daño
                var d = hit.GetComponent<IDamageable>();
                if (d != null)
                    d.ReciveDamage(damageAmount);

                // Aplicar knockback si existe el componente
                var hd = hit.GetComponent<HitDetector>();
                var kb = hit.GetComponent<KnockbackManager>();
                if (kb != null && hd != null)
                {
                    Vector2 dir = (hit.transform.position - transform.position).normalized;
                    float extra = Mathf.Floor(hd.damagePercentage / 10f) * 2f;
                    kb.StartKnockback(dir, knockbackForce + extra, knockbackDuration, hd.damagePercentage);
                }
            }

            yield return new WaitForSeconds(activeDuration);

            // 3) Retract (retroceso)
            if (spriteRenderer != null && retractSprite != null)
                spriteRenderer.sprite = retractSprite;
            yield return new WaitForSeconds(retractDuration);

            // 4) Idle
            if (spriteRenderer != null && idleSprite != null)
                spriteRenderer.sprite = idleSprite;

            // 5) Cooldown
            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 pos = (Vector2)transform.position + colliderOffset;
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(pos, colliderSize);
    }
}
