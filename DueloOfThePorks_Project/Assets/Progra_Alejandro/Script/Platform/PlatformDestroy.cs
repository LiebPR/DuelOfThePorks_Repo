using UnityEngine;
using System.Collections;

public class PlatformDestroy : MonoBehaviour
{
    [Header("Destrucción Automática")]
    [Tooltip("Tiempo en segundos antes de destruir la plataforma automáticamente.")]
    [SerializeField] private float autoDestructionTime = 5f;

    [Header("Destrucción por Toques")]
    [Tooltip("Cantidad de toques acumulados (entre ambos jugadores) requeridos para iniciar la destrucción.")]
    [SerializeField] private int requiredTouches = 3;

    [Tooltip("Tiempo en segundos para destruir la plataforma después de alcanzar los toques requeridos.")]
    [SerializeField] private float hitDestructionDelay = 1f;

    [Header("Efecto Visual de Destrucción")]
    [Tooltip("Si se activa, se aplicará un efecto de fade out antes de la destrucción (requiere SpriteRenderer).")]
    [SerializeField] private bool fadeOutOnDestroy = true;

    [Tooltip("Duración del efecto de fade out en segundos.")]
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("Animación de Destrucción")]
    [Tooltip("Si se activa, reproducirá una animación de destrucción antes de destruir la plataforma.")]
    [SerializeField] private bool useDestructionAnimation = false;

    [Tooltip("Referencia al Animator que reproduce la animación de destrucción. Si no se asigna, se buscará en el GameObject.")]
    [SerializeField] private Animator destructionAnimator;

    [Tooltip("Duración de la animación de destrucción en segundos.")]
    [SerializeField] private float destructionAnimationDuration = 1f;

    [Header("Layers de Jugadores")]
    [Tooltip("Layer asignado al Player 1.")]
    [SerializeField] private LayerMask playerLayer1;

    [Tooltip("Layer asignado al Player 2.")]
    [SerializeField] private LayerMask playerLayer2;

    // Contador de toques acumulados
    private int currentTouches = 0;
    // Bandera para evitar activar la destrucción más de una vez
    private bool destructionTriggered = false;
    // Referencia al SpriteRenderer para efectos visuales de fade out
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        // Obtiene el SpriteRenderer (si está presente) para el fade out
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Si se opta por la animación de destrucción y no se asignó un Animator, se intenta obtenerlo del GameObject.
        if (useDestructionAnimation && destructionAnimator == null)
        {
            destructionAnimator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // Inicia la rutina de destrucción automática
        StartCoroutine(AutoDestructionRoutine());
    }

    // Destruye la plataforma después de un tiempo predefinido, si aún no se ha activado otra destrucción.
    private IEnumerator AutoDestructionRoutine()
    {
        yield return new WaitForSeconds(autoDestructionTime);
        if (!destructionTriggered)
        {
            TriggerDestruction();
        }
    }

    // Detecta toques de colisión de jugadores.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el objeto colisionante pertenece a alguno de los layers definidos para jugadores.
        if (IsInLayerMask(collision.gameObject, playerLayer1) || IsInLayerMask(collision.gameObject, playerLayer2))
        {
            currentTouches++;

            // Si se supera el umbral de toques y aún no se ha activado la destrucción, inicia la cuenta atrás.
            if (currentTouches >= requiredTouches && !destructionTriggered)
            {
                destructionTriggered = true;
                StartCoroutine(DelayedDestruction(hitDestructionDelay));
            }
        }
    }

    // Método auxiliar que comprueba si el GameObject está en el LayerMask dado.
    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((mask.value & (1 << obj.layer)) != 0);
    }

    // Inicia la cuenta atrás para destruir la plataforma.
    private IEnumerator DelayedDestruction(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerDestruction();
    }

    // Marca la destrucción y lanza la rutina que realizará el proceso visual.
    private void TriggerDestruction()
    {
        if (!destructionTriggered)
        {
            destructionTriggered = true;
        }
        StartCoroutine(DoDestruction());
    }

    // Realiza la transición visual (animación o fade out) y finalmente destruye el objeto.
    private IEnumerator DoDestruction()
    {
        if (useDestructionAnimation && destructionAnimator != null)
        {
            // Se dispara el trigger "Destroy" en el Animator.
            destructionAnimator.SetTrigger("Destroy");
            // Se espera la duración definida de la animación.
            yield return new WaitForSeconds(destructionAnimationDuration);
        }
        else if (fadeOutOnDestroy && spriteRenderer != null)
        {
            float timer = 0f;
            while (timer < fadeOutDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(originalColor.a, 0, timer / fadeOutDuration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
