using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaDamgeType
{
    Raycast,
    Overlap,
    Boxcast
}

[CreateAssetMenu(fileName = "NewAttack", menuName = "Attack/ AttackSettings", order = 1)]
public class Attack : ScriptableObject
{
    [Header("Attack Settings")]
    public float damage;
    public float knockbackForce;
    public AreaDamgeType areaDamgeType;

    [Header("Cast Settings")]
    [SerializeField] Vector2 boxSize = new Vector2(1, 1);
    [SerializeField] Vector2 boxOffset = Vector2.zero;
    [SerializeField] float rayDistance = 1.0f;
    public LayerMask targetLayer; // Objetos que pueden ser golpeados por el ataque.

    Rigidbody2D playerRb;
    PlayerController playerController;
    static Transform debugAttackPoint;

    public void Initialize(PlayerController controller)
    {
        playerController = controller;
        playerRb = playerController.GetComponent<Rigidbody2D>();
    }

    public void PerformAttack(Transform attackPoint, bool isPlayerOneAttacker)
    {
        switch (areaDamgeType)
        {
            case AreaDamgeType.Raycast:
                RaycastAttack(attackPoint, isPlayerOneAttacker);
                break;
            case AreaDamgeType.Overlap:
                OverlapAttack(attackPoint, isPlayerOneAttacker);
                break;
            case AreaDamgeType.Boxcast:
                BoxcastAttack(attackPoint, isPlayerOneAttacker);
                break;
        }
    }

    //Cast:
    private void RaycastAttack(Transform attackPoint, bool isPlayerOneAttacker)
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, Vector2.right * rayDistance, targetLayer);
        if (hit.collider != null)
        {
            TryDamageTarget(hit.collider, attackPoint, isPlayerOneAttacker);
        }
    }

    public void OverlapAttack(Transform attackPoint, bool isPlayerOneAttacker)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, rayDistance, targetLayer);
        foreach(Collider2D hit in hits)
        {
            TryDamageTarget(hit, attackPoint, isPlayerOneAttacker);
        }
    }

    private void BoxcastAttack(Transform attackPoint, bool isPlayerOneAttacker)
    {
        Vector2 origin = (Vector2)attackPoint.position + boxOffset;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0, Vector2.right * 0, targetLayer);
        if(hit.collider != null)
        {
            TryDamageTarget(hit.collider, attackPoint, isPlayerOneAttacker);
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

        ApplyKnockback(targetCollider, attackPoint);

        IDamageable damageable = targetCollider.GetComponent<IDamageable>();
        if(damageable != null) 
        {
            Debug.Log($"Aplicando daño: {damage} a {targetCollider.name}");
            damageable.ReciveDamage(damage);
        }
    }

    void ApplyKnockback(Collider2D target, Transform attackPoint) // Añade una fuerza extra al empuje
    {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null)
        {
            Vector2 direction = (target.transform.position - attackPoint.position).normalized;
            targetRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }

    public void DrawGizmos(Transform attackPoint) 
    {
        debugAttackPoint = attackPoint; // Se guarda temporalmente el attackPoint
        if (debugAttackPoint == null) return;

        Gizmos.color = Color.red;

        switch (areaDamgeType)
        {
            case AreaDamgeType.Raycast:
                Gizmos.DrawLine(debugAttackPoint.position, debugAttackPoint.position + Vector3.right * rayDistance);
                break;
            case AreaDamgeType.Overlap:
                Gizmos.DrawWireSphere(debugAttackPoint.position, rayDistance);
                break;
            case AreaDamgeType.Boxcast:
                Vector2 origin = (Vector2)debugAttackPoint.position + boxOffset;
                Gizmos.DrawWireCube(origin, boxSize);
                break;
        }
    }
}