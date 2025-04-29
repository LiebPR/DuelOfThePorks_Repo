using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
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

    private BoxCollider2D col;

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

            // 2) Active + Daño
            if (spriteRenderer != null && activeSprite != null)
                spriteRenderer.sprite = activeSprite;
            var hits = Physics2D.OverlapBoxAll(
                (Vector2)transform.position + colliderOffset,
                colliderSize,
                0f,
                damageLayers
            );
            foreach (var hit in hits)
            {
                var d = hit.GetComponent<IDamageable>();
                if (d != null)
                    d.ReciveDamage(damageAmount);
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
