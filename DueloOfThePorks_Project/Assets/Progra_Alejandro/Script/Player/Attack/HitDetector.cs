using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour, IDamageable
{
    public float damagePercentage = 0f;
    public bool isPlayerOne = false;
    public bool isInvincible = false; //Necesaria para el LifeManager

    public DamageHandler damageHandler;
    LifeManager lifeManager;

    private void Awake()
    {
        damageHandler = GetComponent<DamageHandler>();
        lifeManager = GetComponent<LifeManager>(); 
    }

    public void ReciveDamage(float damage)
    {
        if (isInvincible)
        {
            Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} es invencible no recibe daño.");
            return; //Se le devuelve para que no le aplique el daño
        }

        damagePercentage += damage;
        Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} ha recibido {damage} de daño. Nuevo porcentaje de daño: {damagePercentage}%");
        if (damageHandler != null)
        {
            damageHandler.UpdateHealthDisplay(damagePercentage);
        }

        CheckDeathProbability();
    }

    void CheckDeathProbability()
    {
        if(damagePercentage >= 200f)
        {
            Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} ha muerto por exceso de daño: {damagePercentage}%");
            lifeManager.Die();
        }
        else if (damagePercentage >= 100f)
        {
            float deathChance = (damagePercentage - 100f) / 100f;
            float randomChance = Random.Range(0, 1f);

            if(randomChance <= deathChance)
            {
                Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} ha muerto por probabilidad de daño: {damagePercentage}%");
                lifeManager.Die();
            }
        }
    }
}
