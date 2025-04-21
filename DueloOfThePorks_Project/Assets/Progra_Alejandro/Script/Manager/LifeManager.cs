using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    [SerializeField] int maxVidas = 3;
    int vidas;

    [SerializeField] Transform[] puntosDeRespawn;
    [SerializeField] private Image[] heartImages;

    HitDetector hitDetector;
    PlayerOrbs playerOrbs;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Asociamos siempre los componentes de este mismo GameObject
        hitDetector = GetComponent<HitDetector>();
        playerOrbs = GetComponent<PlayerOrbs>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        vidas = maxVidas;
        // Reiniciar estado de daño en la UI
        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);
    }

    public void Die()
    {
        string quien = hitDetector.isPlayerOne ? "Player 1" : "Player 2";
        Debug.Log($"[{quien}] Die() en {gameObject.name} (vidas antes: {vidas + 1})");

        vidas--;
        if (vidas >= 0 && vidas < heartImages.Length)
            heartImages[vidas].enabled = false;

        if (vidas <= 0)
        {
            string ganador = hitDetector.isPlayerOne ? "Player 2" : "Player 1";
            Debug.Log($"{quien} sin vidas. GAME OVER.");
            PlayerPrefs.SetString("Winner", ganador);
            PlayerPrefs.SetString("Loser", quien);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Scene_Final");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"{quien} pierde una vida. Respawneando...");
            Respawn();
        }

        if (playerOrbs != null)
        {
            Debug.Log($"[{quien}] RemoveOrb() en {playerOrbs.gameObject.name}");
            playerOrbs.RemoveOrb();
        }

        StopKnockback();
    }

    void Respawn()
    {
        // Reset de daño y UI
        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);

        // Teletransportar a un punto de respawn aleatorio
        int idx = Random.Range(0, puntosDeRespawn.Length);
        Vector3 off = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            0f
        );
        transform.position = puntosDeRespawn[idx].position + off;

        StartCoroutine(InvulnerabilityCoroutine(3f));
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        string quien = hitDetector.isPlayerOne ? "Player 1" : "Player 2";
        Debug.Log($"[{quien}] Invulnerable en {gameObject.name}");

        hitDetector.isInvincible = true;
        if (spriteRenderer != null)
        {
            var c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;
        }

        yield return new WaitForSeconds(duration);

        hitDetector.isInvincible = false;
        if (spriteRenderer != null)
        {
            var c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        Debug.Log($"{quien} ya no es invulnerable.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("DeathZone tocada");
            Die();
        }
    }

    void StopKnockback()
    {
        var kb = GetComponent<KnockbackManager>();
        if (kb != null)
        {
            kb.rb.velocity = Vector2.zero;
            kb.rb.gravityScale = kb.originalGravity;
            kb.isKnockBack = false;
        }
    }

    // Métodos públicos para GameTimer
    public int GetLives() => vidas;
    public Transform[] GetRespawnPoints() => puntosDeRespawn;
}