using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletSettings settings;
    public Vector2 direction;
    public GameObject owner;
    public string bulletTypeName; // Nuevo: para saber a qué pool pertenece

    private Vector3 startPosition;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        startPosition = transform.position;
        if (_rb != null)
            _rb.velocity = direction.normalized * settings.speed;
    }

    private void Update()
    {
        float distanceTravelled = Vector3.Distance(startPosition, transform.position);
        if (distanceTravelled > settings.maxTravelDistance)
        {
            DeactivateBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner) return;
        if (((1 << collision.gameObject.layer) & settings.targetLayer) == 0) return;

        InputManager targetInput = collision.GetComponent<InputManager>();
        if (targetInput != null && targetInput.isPlayerOne == owner.GetComponent<InputManager>().isPlayerOne)
            return;

        KnockbackManager knockbackManager = collision.GetComponent<KnockbackManager>();
        if (knockbackManager != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            float damagePercent = collision.GetComponent<HitDetector>()?.damagePercentage ?? 0f;
            knockbackManager.StartKnockback(knockbackDir, settings.knockbackForce, 0.5f, damagePercent);
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.ReciveDamage(settings.damage);
        }

        OrbsSpecialAtt orb = collision.GetComponent<OrbsSpecialAtt>();
        if (orb != null && settings.attackIndex == 3)
        {
            orb.SetLastHitter(owner);
            orb.ReciveDamage(settings.damage);
        }

        DeactivateBullet();
    }

    void DeactivateBullet()
    {
        if (_rb != null)
            _rb.velocity = Vector2.zero;

        BulletPool.Instance.ReturnBullet(bulletTypeName, gameObject);
    }
}
