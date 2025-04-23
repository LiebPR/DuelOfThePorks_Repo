using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    int maxVidas = 3;
    Color originalColor;

    Transform[] puntosDeRespawn;
    Image[] heartImages;

    HitDetector hitDetectorlife;
    PlayerOrbs playerOrbslife;
    SpriteRenderer spriteRendererlife;
    ParticleEffectHandler effectHandlerlife;

    int vidas;
    bool diedByZone = false;

    void Awake()
    {
        hitDetectorlife = GetComponent<HitDetector>();
        playerOrbslife = GetComponent<PlayerOrbs>();
        spriteRendererlife = GetComponent<SpriteRenderer>();
        effectHandlerlife = GetComponent<ParticleEffectHandler>();

        if(spriteRendererlife != null)
        {
            originalColor = spriteRendererlife.color;
        }
    }

    void Start()
    {
        vidas = maxVidas;
        hitDetectorlife.damagePercentage = 0f;
        hitDetectorlife.damageHandler?.UpdateHealthDisplay(0f);
    }

    public void Die()
    {
        // Efecto visual
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (diedByZone)
            effectHandlerlife?.PlayZoneDeathEffect(pos, rot);
        else
            effectHandlerlife?.PlayKillDeathEffect(pos, rot);

        diedByZone = false;

        // Reducir vida y actualizar UI
        vidas--;
        if (vidas >= 0 && vidas < heartImages.Length)
            heartImages[vidas].enabled = false;

        // Game Over o Respawn
        if (vidas <= 0)
        {
            string perdedor = hitDetectorlife.isPlayerOne ? "Player 1" : "Player 2";
            string ganador = hitDetectorlife.isPlayerOne ? "Player 2" : "Player 1";

            PlayerPrefs.SetString("Winner", ganador);
            PlayerPrefs.SetString("Loser", perdedor);
            PlayerPrefs.Save();

            SceneManager.LoadScene("FinalScene");
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(RespawnRoutine());
        }

        // Quitar orbe
        playerOrbslife?.RemoveOrb();
        StopKnockback();
    }

    IEnumerator RespawnRoutine()
    {
        // Reset daño y UI
        hitDetectorlife.damagePercentage = 0f;
        hitDetectorlife.damageHandler?.UpdateHealthDisplay(0f);

        // Elegir respawn aleatorio
        int idx = Random.Range(0, puntosDeRespawn.Length);
        Vector3 offset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            0f
        );
        transform.position = puntosDeRespawn[idx].position + offset;

        // Invulnerabilidad temporal
        StartCoroutine(InvulnerabilityCoroutine(3f));
        yield return null;
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        hitDetectorlife.isInvincible = true;

        if (spriteRendererlife != null)
        {
            Color newColor = spriteRendererlife.material.color;
            newColor.a = 0.5f;
            spriteRendererlife.material.color = newColor;
        }

        yield return new WaitForSeconds(duration);

        hitDetectorlife.isInvincible = false;

        if(spriteRendererlife != null)
        {
            Color newColor = spriteRendererlife.material.color;
            newColor.a = 1f;
            spriteRendererlife.material.color = newColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            diedByZone = true;
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

    // Métodos usados por PlayerInitializer
    public void SetHeartImages(Image[] hearts)
    {
        heartImages = hearts;
    }

    public void SetRespawnPoints(Transform[] respawns)
    {
        puntosDeRespawn = respawns;
    }

    // Información para otros componentes si se requiere
    public int GetLives() => vidas;
    public Transform[] GetRespawnPoints() => puntosDeRespawn;
}