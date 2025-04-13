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
    }

    void Respawn()
    {
        //Reseteamos el daño
        hitDetector.damagePercentage = 0f;

        //Respawn aleatorio
        int index = Random.Range(0, puntosDeRespawn.Length);
        transform.position = puntosDeRespawn[index].position;

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
}
