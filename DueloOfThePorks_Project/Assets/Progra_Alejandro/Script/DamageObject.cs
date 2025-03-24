using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [SerializeField] private float minDamage = 5f; // M�nimo da�o
    [SerializeField] private float maxDamage = 15f; // M�ximo da�o
    [SerializeField] private float pushForce = 5f; // Fuerza de empuje
    [SerializeField] private float pushMultiplier = 1.5f; // Multiplicador de empuje si el da�o es mayor o igual a 100%

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que toc� es el jugador
        DamagePercentage playerDamage = collision.GetComponent<DamagePercentage>();
        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

        if (playerDamage != null && playerRb != null)
        {
            // Aumenta el porcentaje de da�o del jugador
            float damage = Random.Range(minDamage, maxDamage);
            playerDamage.damagePercentage += damage;
            playerDamage.UpdateDamageText();

            // Genera un umbral aleatorio de da�o entre 100 y 200
            float deathThreshold = Random.Range(100f, 200f);

            // Verifica si el da�o es igual o mayor al umbral aleatorio y mata al jugador si es el caso
            if (playerDamage.damagePercentage >= deathThreshold)
            {
                Die(playerRb);
            }
            else
            {
                // Si el da�o es mayor o igual a 100%, el empuje ser� mayor
                bool enhancedPush = playerDamage.damagePercentage >= 100f;
                ApplyPush(playerRb, enhancedPush);
            }

            Debug.Log($"El jugador recibi� {damage}% de da�o!");
        }
    }

    // Aplica empuje al jugador
    private void ApplyPush(Rigidbody2D playerRb, bool enhancedPush)
    {
        Vector2 pushDirection = (playerRb.transform.position - transform.position).normalized; // Direcci�n hacia el jugador
        float appliedPushForce = enhancedPush ? pushForce * pushMultiplier : pushForce; // Aplica empuje aumentado si el da�o es mayor o igual a 100%

        playerRb.AddForce(pushDirection * appliedPushForce, ForceMode2D.Impulse); // Aplica la fuerza al Rigidbody del jugador
    }

    // Matar al jugador cuando el da�o sea suficiente
    private void Die(Rigidbody2D playerRb)
    {
        // Desactiva el jugador o aplica cualquier l�gica para la muerte
        Debug.Log("El jugador ha muerto debido a un da�o excesivo.");
        playerRb.gameObject.SetActive(false); // Desactiva el objeto del jugador (simula la muerte)

        // Aqu� puedes agregar m�s l�gica, como mostrar una pantalla de "Game Over" o reproducir una animaci�n de muerte.
    }
}
