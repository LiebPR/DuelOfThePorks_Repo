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
    public Vector2 boxSize = new Vector2(1, 1);
    public Vector2 boxOffset = Vector2.zero;
    public float rayDistance = 1.0f;
    public LayerMask targetLayer; //Objetos que pueden ser golpeados por el ataque.

    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private static Transform debugAttackPoint;

    public void Initialize(PlayerController controller)
    {
        playerController = controller;
        playerRb = playerController.GetComponent<Rigidbody2D>();
    }

    public void PerformAttack(Transform attackPoint)
    {
        switch (areaDamgeType)
        {
            case AreaDamgeType.Raycast:
                RaycastAttack(attackPoint);
                break;
            case AreaDamgeType.Overlap:
                OverlapAttack(attackPoint);
                break;
            case AreaDamgeType.Boxcast:
                BoxcastAttack(attackPoint);
                break;
        }
    }

    //Cast:
    private void RaycastAttack(Transform attackPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, Vector2.right * rayDistance, targetLayer);
        if (hit.collider != null)
        {
            ApplyKnockback(hit.collider, attackPoint);
            Debug.Log("Raycast hit: " + hit.collider.name);
        }
    }

    public void OverlapAttack(Transform attackPoint)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, rayDistance, targetLayer);
        foreach(Collider2D hit in hits)
        {
            ApplyKnockback(hit, attackPoint);
            Debug.Log("Overlap hit: " + hit.name);
        }
    }

    private void BoxcastAttack(Transform attackPoint)
    {
        Vector2 origin = (Vector2)attackPoint.position + boxOffset;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0, Vector2.right * 0, targetLayer);
        if(hit.collider != null)
        {
            ApplyKnockback(hit.collider, attackPoint);
            Debug.Log("Boxcast hit: " + hit.collider.name);
        }
    }

    public void DrawGizmos(Transform attackPoint) 
    {
        debugAttackPoint = attackPoint; //Se guarda temporalmente el attackPoint
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

    void ApplyKnockback(Collider2D target, Transform attackPoint) //Añade una fuerza extra al empuje
    {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if(targetRb != null)
        {
            Vector2 direction = (target.transform.position - attackPoint.position).normalized;
            targetRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
