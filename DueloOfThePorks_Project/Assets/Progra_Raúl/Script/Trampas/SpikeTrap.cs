using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public float activeTime = 2f;
    public float inactiveTime = 2f;
    public float trapDamage = 10f;
    public LayerMask affectedLayers;

    private bool isActive = false;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
        StartCoroutine(TrapCycle());
    }

    private IEnumerator TrapCycle()
    {
        while (true)
        {
            isActive = true;
            col.enabled = true;
            yield return new WaitForSeconds(activeTime);
            isActive = false;
            col.enabled = false;
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (((1 << other.gameObject.layer) & affectedLayers) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.ReciveDamage(trapDamage);
            }
        }
    }
}