using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour, IDamageable
{
    public float damagePercentage = 0f;
    public bool isPlayerOne = false;

    DamageHandler damageHandler;

    private void Awake()
    {
        damageHandler = GetComponent<DamageHandler>();
    }

    public void ReciveDamage(float damage)
    {
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
        if (damagePercentage >= 100f && damagePercentage < 200f)
        {
            float deathChance = (damagePercentage - 100f) / 100f;
            float randomChance = Random.Range(0f, 1f);

            if (randomChance <= deathChance)
            {
                Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} ha muerto por probabilidad de daño: {damagePercentage}%");
                Die();
            }
            else if (damagePercentage >= 200f)
            {
                Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} ha muerto por exceso de daño: {damagePercentage}%");
                Die();
            }

        }
    }

    void Die()
    {
        //Lógica para morir, perder una vida y respawnear
        //Por ejemplo, desactivamos el jugador y le restamos una vida
        // Aquí podrías implementar la lógica de reseteo de vida y respawn
        Debug.Log($"{(isPlayerOne ? "Player1" : "Player2")} HA MUERTO!!!");
    }
}
