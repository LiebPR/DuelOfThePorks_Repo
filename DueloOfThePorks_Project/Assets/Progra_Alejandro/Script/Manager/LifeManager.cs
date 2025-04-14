using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] int maxVidas = 3;
    int vidas;

    [SerializeField] Transform[] puntosDeRespawn;
    [SerializeField] HitDetector hitDetector; //Reinicia el daño
    [SerializeField] private Image[] heartImages; //Ui: Imagenes de corazones
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] PlayerOrbs playerOrbs;

    private void Start()
    {
        vidas = maxVidas;
        if(hitDetector == null)
        {
            hitDetector = GetComponent<HitDetector>();
        }

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if(playerOrbs == null)
        {
            playerOrbs = GetComponent<PlayerOrbs>();
        }

        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);
    }
    public void Die()
    {
        vidas--;

        //Apaga el corazon correspondiente
        if(vidas >= 0 && vidas < heartImages.Length)
        {
            heartImages[vidas].enabled = false;
        }

        if(vidas <= 0)
        {
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} se ha quedado sin vidas. GAME OVER. ");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} pierde una vida. Respawneando...");
            Respawn();
        }

        if(playerOrbs != null)
        {
            playerOrbs.RemoveOrb();
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} perdió una orbe al morir.");
        }

        //Detener el knockback cuando el jugador muere
        KnockbackManager knockbackManager = GetComponent<KnockbackManager>();
        if(knockbackManager != null && knockbackManager.IsInKnockback())
        {
            //Detener el knockback inmediatamente
            StopKnockback();
        }
    }

    void Respawn()
    {
        //Reseteamos el daño
        hitDetector.damagePercentage = 0f;

        //Respawn aleatorio
        int index = Random.Range(0, puntosDeRespawn.Length); //Obtenemos punto aleatorio
        Transform selectedRespawnPoint = puntosDeRespawn[index];

        //Evitamos que respawnee siempre en el mismo lado
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        transform.position = selectedRespawnPoint.position + randomOffset;

        //Actualiza visualmente el porcentaje
        if (hitDetector.damageHandler != null)
        {
            hitDetector.damageHandler?.UpdateHealthDisplay(hitDetector.damagePercentage);
        }

        //Activa la invecibilidad temporal
        StartCoroutine(InvulnerabilityCoroutine(3f));
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        hitDetector.isInvincible = true;

        if(spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.5f; // Hacer transparente
            spriteRenderer.color = color;
        }

        Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} es invencible por {duration} segundos.");

        yield return new WaitForSeconds(duration);

        hitDetector.isInvincible = false;

        if(spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f; //Restaurar opacidad
            spriteRenderer.color = color;
        }

        Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} ya no es invencible.");
    }

    public int GetLives()
    {
        return vidas; //Retorna las vidas actuales
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("DeathZone"))
        {
            Debug.Log("Zona de muerte tocada, perdiendo vida");
            Die();
        }
    }

    void StopKnockback()
    {
        KnockbackManager knockbackManager = GetComponent<KnockbackManager>();
        if(knockbackManager != null)
        {
            // Cancelamos cualquier movimiento residual
            knockbackManager.rb.velocity = Vector2.zero;  // Detenemos cualquier movimiento residual
            knockbackManager.rb.gravityScale = knockbackManager.originalGravity;  // Restauramos la gravedad
            knockbackManager.isKnockBack = false;  // Desactivamos el estado de knockback
        }
    }

    public Transform[] GetRespawnPoints()
    {
        return puntosDeRespawn;
    }
}
