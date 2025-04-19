using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletSettings settings; //Configuración de la bala
    public Vector2 direction; //Direccion de la bala
    public GameObject owner; //Propietario (quien disparó la bala)

    private Vector3 startPosition; //Posición inicial de la bala
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        startPosition = transform.position; //Al activarse, guardamos la posición de inicio

        //Aplicamos movimiento al Rigidbody2D
        if(_rb != null)
        {
            _rb.velocity = direction.normalized * settings.speed;
        }
    }

    private void Update()
    {
        //Comporbamos la distancia recorrida por la bala
        float distanceTravelled = Vector3.Distance(startPosition, transform.position);
        if (distanceTravelled > settings.maxTravelDistance)
        {
            DeactivateBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //No hacer nada si la bala colisiona con el propietario
        if (collision.gameObject == owner) return;

        //Si no esta en el targetLayer, simplemente la ignoramos
        if(((1 << collision.gameObject.layer) & settings.targetLayer) == 0)
        {
            return;
        }

        //Si la ba golpea a otro jugador del mismo equipo, no hacer nada
        InputManager targetInput = collision.GetComponent<InputManager>();
        if (targetInput != null && targetInput.isPlayerOne == owner.GetComponent<InputManager>().isPlayerOne)
        {
            return;
        }

        //Verificar si el objeto que colisiona tiene un KnockbackManager (gestión del retroceso)
        KnockbackManager knockbackManager = collision.GetComponent<KnockbackManager>();
        if (knockbackManager != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized; //Calcular la direccion del retroceso
            float damagePercent = collision.GetComponent<HitDetector>()?.damagePercentage ?? 0f; //Calcular el porcentaje de daño
            knockbackManager.StartKnockback(knockbackDir, settings.knockbackForce, 0.5f, damagePercent); //Aplicar retroceso
        }

        //Comprobar si el objeto tiene un componente IDamageable
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            //Aplicar el daño al objetivo
            damageable.ReciveDamage(settings.damage);
        }

        //Desactivar la bala despues del impacto
        DeactivateBullet();
    }

    void DeactivateBullet()
    {
        if(_rb != null)
        {
            _rb.velocity = Vector2.zero;
        }

        //Devolver la bala al pool para su reutilización
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}