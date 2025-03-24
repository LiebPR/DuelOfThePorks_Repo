using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [SerializeField] private float minDamage = 5f; // Mínimo daño
    [SerializeField] private float maxDamage = 15f; // Máximo daño
    [SerializeField] private float pushForce = 5f; // Fuerza de empuje
    [SerializeField] private float pushMultiplier = 1.5f; // Multiplicador de empuje si el daño es mayor o igual a 100%

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que tocó es el jugador
        DamagePercentage playerDamage = collision.GetComponent<DamagePercentage>();
        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

        if (playerDamage != null && playerRb != null)
        {
            // Aumenta el porcentaje de daño del jugador
            float damage = Random.Range(minDamage, maxDamage);
            playerDamage.damagePercentage += damage;
            playerDamage.UpdateDamageText();

            // Genera un umbral aleatorio de daño entre 100 y 200
            float deathThreshold = Random.Range(100f, 200f);

            // Verifica si el daño es igual o mayor al umbral aleatorio y mata al jugador si es el caso
            if (playerDamage.damagePercentage >= deathThreshold)
            {
                Die(playerRb);
            }
            else
            {
                // Si el daño es mayor o igual a 100%, el empuje será mayor
                bool enhancedPush = playerDamage.damagePercentage >= 100f;
                ApplyPush(playerRb, enhancedPush);
            }

            Debug.Log($"El jugador recibió {damage}% de daño!");
        }
    }

    // Aplica empuje al jugador
    private void ApplyPush(Rigidbody2D playerRb, bool enhancedPush)
    {
        Vector2 pushDirection = (playerRb.transform.position - transform.position).normalized; // Dirección hacia el jugador
        float appliedPushForce = enhancedPush ? pushForce * pushMultiplier : pushForce; // Aplica empuje aumentado si el daño es mayor o igual a 100%

        playerRb.AddForce(pushDirection * appliedPushForce, ForceMode2D.Impulse); // Aplica la fuerza al Rigidbody del jugador
    }

    // Matar al jugador cuando el daño sea suficiente
    private void Die(Rigidbody2D playerRb)
    {
        // Desactiva el jugador o aplica cualquier lógica para la muerte
        Debug.Log("El jugador ha muerto debido a un daño excesivo.");
        playerRb.gameObject.SetActive(false); // Desactiva el objeto del jugador (simula la muerte)

        // Aquí puedes agregar más lógica, como mostrar una pantalla de "Game Over" o reproducir una animación de muerte.
    }
}
