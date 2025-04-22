using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    [SerializeField] int maxVidas = 3;
    [SerializeField] Transform[] puntosDeRespawn;
    [SerializeField] private Image[] heartImages;

    private HitDetector hitDetector;
    private PlayerOrbs playerOrbs;
    private SpriteRenderer spriteRenderer;
    private ParticleEffectHandler effectHandler;

    int vidas;
    bool diedByZone = false;

    private void Awake()
    {
        hitDetector = GetComponent<HitDetector>();
        playerOrbs = GetComponent<PlayerOrbs>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        effectHandler = GetComponent<ParticleEffectHandler>();
    }

    private void Start()
    {
        vidas = maxVidas;
        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);
    }

    public void Die()
    {
        // 1) Efecto según tipo de muerte
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        if (diedByZone)
            effectHandler?.PlayZoneDeathEffect(pos, rot);
        else
            effectHandler?.PlayKillDeathEffect(pos, rot);

        diedByZone = false;

        // 2) Reducir vidas y actualizar UI
        vidas--;
        if (vidas >= 0 && vidas < heartImages.Length)
            heartImages[vidas].enabled = false;

        // 3) Game Over o respawn
        if (vidas <= 0)
        {
            string perdedor = hitDetector.isPlayerOne ? "Player 1" : "Player 2";
            string ganador = hitDetector.isPlayerOne ? "Player 2" : "Player 1";

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

        // 4) Pierde orbe
        playerOrbs?.RemoveOrb();
        StopKnockback();
    }

    private IEnumerator RespawnRoutine()
    {
        // Reset de daño y UI
        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);

        // Teletransporte
        int idx = Random.Range(0, puntosDeRespawn.Length);
        Vector3 off = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            0f
        );
        transform.position = puntosDeRespawn[idx].position + off;

        // Invulnerabilidad
        StartCoroutine(InvulnerabilityCoroutine(3f));
        yield return null;
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        hitDetector.isInvincible = true;
        if (spriteRenderer != null)
        {
            var c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;
        }

        yield return new WaitForSeconds(duration);

        hitDetector.isInvincible = false;
        effectHandler?.ResetSpriteColor();
    }

    public int GetLives() => vidas;
    public Transform[] GetRespawnPoints() => puntosDeRespawn;

    private void OnTriggerEnter2D(Collider2D other)
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
}
