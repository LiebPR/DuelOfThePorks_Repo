using UnityEngine;
using System.Collections;

public class FireTrap : MonoBehaviour
{
    public float duration = 3f;
    public float trapDamage = 5f;
    public LayerMask affectedLayers;
    private bool isActive = false;

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);
            StartCoroutine(DeactivateAfterTime());
        }
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        isActive = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isActive && ((1 << other.gameObject.layer) & affectedLayers) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.ReciveDamage(trapDamage * Time.deltaTime); // daño acumulativo por segundo
            }
        }
    }
}