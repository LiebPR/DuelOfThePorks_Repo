using UnityEngine;

public class FlyingPig : MonoBehaviour
{
    public float speed = 10f;
    public float trapDamage = 15f;
    public AudioClip pigScream;
    public LayerMask affectedLayers;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.right * speed;

        if (pigScream) AudioSource.PlayClipAtPoint(pigScream, transform.position);
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & affectedLayers) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.ReciveDamage(trapDamage);
            }
        }
    }
}