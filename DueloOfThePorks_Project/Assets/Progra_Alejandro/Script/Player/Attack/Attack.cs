using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Attack/ AttackSettings", order = 1)]
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

    Rigidbody2D playerRb;
    PlayerController playerController;
    static Transform debugAttackPoint;

    public void Initialize(PlayerController controller)
    {
        playerController = controller;
        playerRb = playerController.GetComponent<Rigidbody2D>();
    }

    public float GetCooldownTime()
    {
        return cooldownTime;
    }

    public void PerformAttack(Transform attackPoint, bool isPlayerOneAttacker)
    {
        OverlapAttack(attackPoint, isPlayerOneAttacker);
    }

    //Cast:
    public void OverlapAttack(Transform attackPoint, bool isPlayerOneAttacker)
    {
        // Usamos el signo del scale para reflejar la dirección del ataque
        float facingDirection = Mathf.Sign(attackPoint.lossyScale.x); // Usamos lossyScale para heredar bien el flip

        Vector2 flippedOffset = new Vector2(boxOffset.x * facingDirection, boxOffset.y);

        Vector2 origin = (Vector2)attackPoint.position + flippedOffset;

        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f, targetLayer);

        foreach (Collider2D hit in hits)
        {
            TryDamageTarget(hit, attackPoint, isPlayerOneAttacker);
        }
    }

    void TryDamageTarget(Collider2D targetCollider, Transform attackPoint, bool isPlayerOneAttacker)
    {
        InputManager targetInput = targetCollider.GetComponent<InputManager>();
        if(targetInput != null && targetInput.isPlayerOne == isPlayerOneAttacker)
        {
            //No ataca al mismo bando
            return;
        }

        HitDetector hitDetector = targetCollider.GetComponent<HitDetector>();
        KnockbackManager knockbackManager = targetCollider.GetComponent<KnockbackManager>();

        if(knockbackManager != null)
        {
            //Calculamos la dirección del knockback, que es la dirección desde el punto de ataque hacia el objetivo.
            Vector2 knockbacDirection = (targetCollider.transform.position - attackPoint.position).normalized;

            //Obtenemos el knockback, pasándole el porcentaje de daño.
            float targetDamagePercentage = hitDetector.damagePercentage;

            //Iniciamos el knockback, pasandole el porcentaje de daño.
            knockbackManager.StartKnockback(knockbacDirection, knockbackForce, 0.5f, targetDamagePercentage);
        }

        IDamageable damageable = targetCollider.GetComponent<IDamageable>();

        //Si es una orbe le pasamos quién la golpeó
        OrbsSpecialAtt orb = targetCollider.GetComponent<OrbsSpecialAtt>();
        if(orb != null)
        {
            orb.SetLastHitter(playerController.gameObject);
        }

        if(damageable != null) 
        {
            damageable.ReciveDamage(damage);
        }
    }

    public void DrawGizmos(Transform attackPoint)
    {
        if (attackPoint == null) return;

        float facingDirection = Mathf.Sign(attackPoint.lossyScale.x);
        Vector2 flippedOffset = new Vector2(boxOffset.x * facingDirection, boxOffset.y);
        Vector2 origin = (Vector2)attackPoint.position + flippedOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}