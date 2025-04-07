using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageHandler : MonoBehaviour
{
    public TextMeshProUGUI damageText; // Texto de daño en la UI
    public bool hasBeenAttacked = false; // Booleano para saber si ha sido atacado
    public float percentage = 0f; // Porcentaje de daño acumulado
    public bool deathThresholdReached = false; // Umbral de muerte alcanzado

    private float knockbackIncrease = 0.5f; // Incremento del knockback basado en porcentaje
    private bool hasDied = false; // Para controlar si el jugador ha muerto

    private void Update()
    {
        
        if (damageText != null)
        {
            damageText.text = "Damage: " + Mathf.RoundToInt(percentage) + "%"; 
        }

        
        if (hasBeenAttacked)
        {
            if (percentage >= 10f && !deathThresholdReached)
            {
                knockbackIncrease += 0.5f; 
            }

            CheckDeathThreshold();
            hasBeenAttacked = false;
        }
    }

    
    public void ReceiveDamage(float damage)
    {
        
        hasBeenAttacked = true;

        
        percentage += damage;

        
        CheckDeathThreshold();
    }

    
    private void CheckDeathThreshold()
    {
        if (percentage >= 100f && !deathThresholdReached)
        {
            
            float deathChance = (percentage - 100f) / 100f; 

            float randomChance = Random.Range(0f, 1f);

            
            if(randomChance <= deathChance)
            {
                deathThresholdReached = true;  
                Debug.Log("Probabilidad de muerte alcanzado Porcentaje de daño: " + percentage + "%");
            }
        }

        if (percentage >= 170f && !hasDied)
        {
            hasDied = true; 
            Debug.Log("Jugador muerto por daño acumulado Porcentaje de daño: " + percentage + "%");
        }
    }
}
