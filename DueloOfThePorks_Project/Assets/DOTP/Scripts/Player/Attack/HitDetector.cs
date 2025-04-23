using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour, IDamageable
{
    public float damagePercentage = 0f;
    public bool isPlayerOne = false;
    public bool isInvincible = false;

    [Header("Referencias")]
    public DamageHandler damageHandler;
    private LifeManager lifeManager;
    private ParticleEffectHandler effectHandler;

    private void Awake()
    {
        damageHandler = GetComponent<DamageHandler>();
        lifeManager = GetComponent<LifeManager>();
        effectHandler = GetComponent<ParticleEffectHandler>();
        if (effectHandler == null)
            Debug.LogWarning($"{name}: falta ParticleEffectHandler para el flash visual.");
    }

    public void ReciveDamage(float damage)
    {
        // 1) Solo daño positivo
        if (isInvincible || damage <= 0f)
            return;

        // 2) Actualizar % de daño
        damagePercentage += damage;
        damageHandler?.UpdateHealthDisplay(damagePercentage);

        // 3) Disparar flash
        effectHandler?.PlayHitEffect(
            transform.position,
            transform.rotation,
            Mathf.Clamp(damagePercentage, 0f, 100f)
        );

        // 4) Comprobar muerte
        CheckDeathProbability();
    }

    private void CheckDeathProbability()
    {
        if (damagePercentage >= 200f)
        {
            lifeManager.Die();
        }
        else if (damagePercentage >= 100f)
        {
            float deathChance = (damagePercentage - 100f) / 100f;
            if (Random.value <= deathChance)
                lifeManager.Die();
        }
    }
}
