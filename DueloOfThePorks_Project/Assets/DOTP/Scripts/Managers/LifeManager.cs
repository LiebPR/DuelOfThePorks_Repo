using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    int maxVidas = 3;
    Transform[] puntosDeRespawn;
    Image[] heartImages;

    HitDetector hitDetectorlife;
    PlayerOrbs playerOrbslife;
    SpriteRenderer spriteRendererlife;
    ParticleEffectHandler effectHandlerlife;

    int vidas;
    bool diedByZone = false;
    Vector3 lastZoneDeathEuler = Vector3.zero;

    void Awake()
    {
        hitDetectorlife = GetComponent<HitDetector>();
        playerOrbslife = GetComponent<PlayerOrbs>();
        spriteRendererlife = GetComponent<SpriteRenderer>();
        effectHandlerlife = GetComponent<ParticleEffectHandler>();
    }

    void Start()
    {
        vidas = maxVidas;
        hitDetectorlife.damagePercentage = 0f;
        hitDetectorlife.damageHandler?.UpdateHealthDisplay(0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("DeathZone")) return;

        // Determina qué lado: arriba, abajo, derecha, izquierda
        Vector2 center = other.bounds.center;
        Vector2 dir = ((Vector2)transform.position - center).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // Lateral
            if (dir.x > 0f)
                lastZoneDeathEuler = new Vector3(0f, 90f,-90f);  // derecha → mira izquierda
            else
                lastZoneDeathEuler = new Vector3(0f, -90f, 90f);  // izquierda → mira derecha
        }
        else
        {
            // Vertical
            if (dir.y > 0f)
                lastZoneDeathEuler = new Vector3(-90f,0f,90f);   // arriba → mira abajo
            else
                lastZoneDeathEuler = new Vector3(-90f, 0f, 90f);  // abajo → mira arriba
        }

        diedByZone = true;
        Die();
    }

    public void Die()
    {
        Vector3 pos = transform.position;
        Quaternion rot;

        if (diedByZone)
        {
            rot = Quaternion.Euler(lastZoneDeathEuler);
            effectHandlerlife?.PlayZoneDeathEffect(pos, rot);
        }
        else
        {
            rot = transform.rotation;
            effectHandlerlife?.PlayKillDeathEffect(pos, rot);
        }

        diedByZone = false;

        vidas--;
        if (vidas >= 0 && vidas < heartImages.Length)
            heartImages[vidas].enabled = false;

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

        playerOrbslife?.RemoveOrb();
        StopKnockback();
    }

    IEnumerator RespawnRoutine()
    {
        hitDetectorlife.damagePercentage = 0f;
        hitDetectorlife.damageHandler?.UpdateHealthDisplay(0f);

        int idx = Random.Range(0, puntosDeRespawn.Length);
        Vector3 offset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            0f
        );
        transform.position = puntosDeRespawn[idx].position + offset;

        yield return StartCoroutine(InvulnerabilityCoroutine(3f));
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        hitDetectorlife.isInvincible = true;
        if (spriteRendererlife != null)
        {
            var c = spriteRendererlife.color;
            c.a = 0.5f;
            spriteRendererlife.color = c;
        }
        yield return new WaitForSeconds(duration);
        hitDetectorlife.isInvincible = false;
        if (spriteRendererlife != null)
        {
            var c = spriteRendererlife.color;
            c.a = 1f;
            spriteRendererlife.color = c;
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

    // Para inicializar desde otro componente:
    public void SetHeartImages(Image[] hearts) => heartImages = hearts;
    public void SetRespawnPoints(Transform[] resp) => puntosDeRespawn = resp;

    public int GetLives() => vidas;
    public Transform[] GetRespawnPoints() => puntosDeRespawn;
}
