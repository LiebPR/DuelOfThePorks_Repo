using UnityEngine;
using Game.VFX;

public class FighterVFX : MonoBehaviour
{
    [Header("IDs de VFX (configurables en el Inspector)")]
    [Tooltip("Efecto al impactar")]
    [SerializeField] private string hitEffectID = "hit";
    [Tooltip("Efecto al saltar")]
    [SerializeField] private string jumpEffectID = "jump";
    [Tooltip("Efecto al aterrizar")]
    [SerializeField] private string landEffectID = "dust";
    [Tooltip("Efecto de explosión")]
    [SerializeField] private string explosionEffectID = "explosion";
    [Tooltip("Efecto de muerte")]
    [SerializeField] private string deathEffectID = "death";

    // Métodos para Animation Events (sin parámetros)
    public void SpawnHitVFX() => PlayVFX(hitEffectID);
    public void SpawnJumpVFX() => PlayVFX(jumpEffectID);
    public void SpawnLandVFX() => PlayVFX(landEffectID);
    public void SpawnExplosionVFX() => PlayVFX(explosionEffectID);
    public void SpawnDeathVFX() => PlayVFX(deathEffectID);

    // Método genérico si quieres pasar un ID desde el Animation Event
    public void SpawnVFX(string effectID) => PlayVFX(effectID);

    // Lógica interna
    private void PlayVFX(string id)
    {
        if (ParticleManager.Instance != null)
            ParticleManager.Instance.Play(id, transform.position);
    }
}
