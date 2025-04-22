using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Attack/AttackSettings", order = 1)]
public class Attack : ScriptableObject
{
    [Header("Attack Settings")]
    public float damage;
    public float knockbackForce;
    public float cooldownTime;

    [Header("Cast Settings")]
    [SerializeField] Vector2 boxSize = new Vector2(1f, 1f);
    [SerializeField] Vector2 boxOffset = Vector2.zero;
    [SerializeField] LayerMask targetLayer;


    public float GetCooldownTime() => cooldownTime;

    /// <summary>
    /// Ejecuta el ataque. 
    /// - attackPoint: Transform del punto de origen.
    /// - hitter: GameObject que realiza el ataque.
    /// - isPlayerOneAttacker: si es el jugador 1 o no.
    /// </summary>
    public void PerformAttack(Transform attackPoint, GameObject hitter, bool isPlayerOneAttacker)
    {
        OverlapAttack(attackPoint, hitter, isPlayerOneAttacker);

       
    }

    private void OverlapAttack(Transform attackPoint, GameObject hitter, bool isPlayerOneAttacker)
    {
        float facing = Mathf.Sign(attackPoint.lossyScale.x);
        Vector2 flippedOffset = new Vector2(boxOffset.x * facing, boxOffset.y);
        Vector2 origin = (Vector2)attackPoint.position + flippedOffset;

        // Detecta las colisiones
        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f, targetLayer);
        Debug.Log($"Hits detectados: {hits.Length}");  // Log de cuantos hits se han detectado

        // Itera sobre los hits detectados
        foreach (var hit in hits)
        {
            Debug.Log($"Golpeando: {hit.name}");  // Log de quien ha sido golpeado
            TryDamageTarget(hit, attackPoint, hitter, isPlayerOneAttacker);  // Aplica el daño
        }
    }

    private void TryDamageTarget(Collider2D target, Transform attackPoint, GameObject hitter, bool isPlayerOneAttacker)
    {
        // No golpear aliados
        var inp = target.GetComponent<InputManager>();
        if (inp != null && inp.isPlayerOne == isPlayerOneAttacker)
            return;

        // Knockback
        var hd = target.GetComponent<HitDetector>();
        var kb = target.GetComponent<KnockbackManager>();
        if (kb != null && hd != null)
        {
            Vector2 dir = (target.transform.position - attackPoint.position).normalized;
            float extra = Mathf.Floor(hd.damagePercentage / 10f) * 2f;
            kb.StartKnockback(dir, knockbackForce + extra, 0.5f, hd.damagePercentage);
        }

        // Orbes especiales: asignar hitter
        var orb = target.GetComponent<OrbsSpecialAtt>();
        if (orb != null)
            orb.SetLastHitter(hitter);

        // Daño
        var dmg = target.GetComponent<IDamageable>();
        if (dmg != null)
            dmg.ReciveDamage(damage);
    }

    // Para dibujar el área de ataque en el Scene view
    public void DrawGizmos(Transform attackPoint)
    {
        if (attackPoint == null) return;
        float facing = Mathf.Sign(attackPoint.lossyScale.x);
        Vector2 flippedOffset = new Vector2(boxOffset.x * facing, boxOffset.y);
        Vector2 origin = (Vector2)attackPoint.position + flippedOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}