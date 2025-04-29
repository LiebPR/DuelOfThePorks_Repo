using System.Collections;
using UnityEngine;

public class ParticleEffectHandler : MonoBehaviour
{
    [Header("Prefabs de Partículas")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject killDeathEffectPrefab;
    [SerializeField] private GameObject zoneDeathEffectPrefab;
    [SerializeField] private float effectLifetime = 2f;

    [Header("Offsets de Posición")]
    [SerializeField] private Vector3 hitOffset = Vector3.zero;
    [SerializeField] private Vector3 killDeathOffset = Vector3.zero;
    [SerializeField] private Vector3 zoneDeathOffset = new Vector3(0f, -1f, 0f);

    [Header("Destello Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float minFlashIntensity = 0.5f;
    [SerializeField] private float maxFlashIntensity = 2f;

    [Header("Seguir Rotación")]
    [Tooltip("Si es true, usa la rotación que venga; si false, ignora rotación externa y usa identidad.")]
    [SerializeField] private bool followRotation = true;

    [Header("Onda Expansiva")]
    [SerializeField] private GameObject shockwaveEffectPrefab;
    [SerializeField] private float shockwaveScale = 1f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void PlayHitEffect(Vector3 pos, Quaternion rot, float damagePercent)
    {
        Quaternion finalRot = followRotation ? rot : Quaternion.identity;
        SpawnEffect(hitEffectPrefab, pos + hitOffset, finalRot);
        FlashSprite(damagePercent);
    }

    public void PlayKillDeathEffect(Vector3 pos, Quaternion rot)
    {
        Quaternion finalRot = followRotation ? rot : Quaternion.identity;
        SpawnEffect(killDeathEffectPrefab, pos + killDeathOffset, finalRot);
        FlashSprite(100f);
    }

    public void PlayZoneDeathEffect(Vector3 pos, Quaternion rot)
    {
        Quaternion finalRot = followRotation ? rot : Quaternion.identity;
        SpawnEffect(zoneDeathEffectPrefab, pos + zoneDeathOffset, finalRot);
        FlashSprite(100f);
    }

    private void SpawnEffect(GameObject prefab, Vector3 spawnPos, Quaternion rot)
    {
        if (prefab == null) return;

        var inst = Instantiate(prefab, spawnPos, rot);

        foreach (var ps in inst.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        Destroy(inst, effectLifetime);
    }

    private void FlashSprite(float damagePercent)
    {
        if (spriteRenderer == null) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        float t = Mathf.Clamp01(damagePercent / 100f);
        float intensity = Mathf.Lerp(minFlashIntensity, maxFlashIntensity, t);
        flashCoroutine = StartCoroutine(FlashRoutine(intensity));
    }

    private IEnumerator FlashRoutine(float intensity)
    {
        var c = flashColor * intensity;
        c.a = originalColor.a;
        spriteRenderer.color = c;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    public void ResetSpriteColor()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    // --- NUEVO: Función para animación de Onda Expansiva ---

    /// <summary>
    /// Llama desde un evento de Animator para instanciar una onda expansiva en la posición del personaje.
    /// </summary>
    public void PlayShockwaveEffect()
    {
        PlayShockwaveEffect(transform.position);
    }

    private void PlayShockwaveEffect(Vector3 position)
    {
        if (shockwaveEffectPrefab == null) return;

        var shockwave = Instantiate(shockwaveEffectPrefab, position, Quaternion.identity);

        shockwave.transform.localScale = Vector3.one * shockwaveScale;

        foreach (var ps in shockwave.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        Destroy(shockwave, effectLifetime);
    }
}
