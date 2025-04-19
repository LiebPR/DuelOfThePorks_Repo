using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    [SerializeField] int maxVidas = 3;
    int vidas;

    [SerializeField] Transform[] puntosDeRespawn;
    [SerializeField] HitDetector hitDetector;
    [SerializeField] private Image[] heartImages;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] PlayerOrbs playerOrbs;

    private void Start()
    {
        vidas = maxVidas;
        if (hitDetector == null) hitDetector = GetComponent<HitDetector>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerOrbs == null) playerOrbs = GetComponent<PlayerOrbs>();

        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);
    }

    public void Die()
    {
        vidas--;

        if (vidas >= 0 && vidas < heartImages.Length)
        {
            heartImages[vidas].enabled = false;
        }

        if (vidas <= 0)
        {
            string perdedor = hitDetector.isPlayerOne ? "Player 1" : "Player 2";
            string ganador = hitDetector.isPlayerOne ? "Player 2" : "Player 1";

            Debug.Log($"{perdedor} se ha quedado sin vidas. GAME OVER.");

            PlayerPrefs.SetString("Winner", ganador);
            PlayerPrefs.SetString("Loser", perdedor);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Scene_Final");

            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player 1" : "Player 2")} pierde una vida. Respawneando...");
            Respawn();
        }

        if (playerOrbs != null)
        {
            playerOrbs.RemoveOrb();
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player 1" : "Player 2")} perdió una orbe al morir.");
        }

        StopKnockback();
    }

    void Respawn()
    {
        hitDetector.damagePercentage = 0f;

        int index = Random.Range(0, puntosDeRespawn.Length);
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        transform.position = puntosDeRespawn[index].position + randomOffset;

        hitDetector.damageHandler?.UpdateHealthDisplay(hitDetector.damagePercentage);

        StartCoroutine(InvulnerabilityCoroutine(3f));
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        hitDetector.isInvincible = true;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
        }

        Debug.Log($"{(hitDetector.isPlayerOne ? "Player 1" : "Player 2")} es invencible por {duration} segundos.");

        yield return new WaitForSeconds(duration);

        hitDetector.isInvincible = false;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        Debug.Log($"{(hitDetector.isPlayerOne ? "Player 1" : "Player 2")} ya no es invencible.");
    }

    public int GetLives()
    {
        return vidas;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("Zona de muerte tocada, perdiendo vida");
            Die();
        }
    }

    void StopKnockback()
    {
        KnockbackManager knockbackManager = GetComponent<KnockbackManager>();
        if (knockbackManager != null)
        {
            knockbackManager.rb.velocity = Vector2.zero;
            knockbackManager.rb.gravityScale = knockbackManager.originalGravity;
            knockbackManager.isKnockBack = false;
        }
    }

    public Transform[] GetRespawnPoints()
    {
        return puntosDeRespawn;
    }
}
