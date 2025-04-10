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
    [SerializeField] float radius = 1.0f;
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
        Vector2 origin = (Vector2)attackPoint.position + boxOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f, targetLayer);
        foreach(Collider2D hit in hits)
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

        KnockbackManager knockbackManager = targetCollider.GetComponent<KnockbackManager>();
        if(knockbackManager != null)
        {
            Vector2 knockbacDirection = (targetCollider.transform.position - attackPoint.position).normalized;
            knockbackManager.StartKnockback(knockbacDirection, knockbackForce, 0.5f); //Configura la fuerza y duración
        }

        IDamageable damageable = targetCollider.GetComponent<IDamageable>();
        if(damageable != null) 
        {
            damageable.ReciveDamage(damage);
        }
    }

    public void DrawGizmos(Transform attackPoint) 
    {
        debugAttackPoint = attackPoint; // Se guarda temporalmente el attackPoint
        if (debugAttackPoint == null) return;

        Gizmos.color = Color.red;
        Vector2 origin = (Vector2)debugAttackPoint.position + boxOffset;
        Gizmos.DrawWireCube(origin, boxSize);
        
        
    }
}