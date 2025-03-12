using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    BaseAttack,
    SpecialAttack
}

public enum DetectionType
{
    Raycast,
    Overlap,
    Boxcast
}
public class AttackSystem : MonoBehaviour
{
    [Header("Attack Setings")]
    [SerializeField] AttackType attackType;
    [SerializeField] DetectionType detectionType;
    [SerializeField] float baseAttackDamage = 10f;
    [SerializeField] float specialAttackDamage = 20f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackCooldown = 0.5f;
    float lastAttackTime;

    [Header("AttackDetectión")]
    [SerializeField] LayerMask enemyLayer;

    private void Update()
    {
        
    }

    void HandleAttackInput()
    {
        if(Input.GetMouseButtonDown(1) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            
        }

        else if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.W) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
        }

        else if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.S) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
        }

        else if(Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
        }
    }

    void BaseAttack()
    {
        Vector2 attackDirection = transform.right;
        switch (detectionType)
        {
            case DetectionType.Raycast:
                RaycastAttack(transform.right);
                break;

            case DetectionType.Boxcast:
                BoxcastAttack(transform.right);
                break;

            case DetectionType.Overlap:
                OverlapAttack(transform.position);
                break;
        }
    }

    void baseAttackUpward()
    {
        Vector2 attackDirection = transform.up;
        switch (detectionType)
        {
            case DetectionType.Raycast:
                RaycastAttack(attackDirection);
                break;

            case DetectionType.Boxcast:
                BoxcastAttack(attackDirection);
                break;

            case DetectionType.Overlap:
                OverlapAttack(transform.position + Vector3.up * attackRange);
                break;
        }
    }
    void BaseAttackFalling()
    {
        Vector2 attackDirection = -transform.up;
        switch (detectionType)
        {
            case DetectionType.Raycast:
                RaycastAttack(-transform.up);
                break;

            case DetectionType.Boxcast:
                BoxcastAttack(-transform.up);
                break;
            case DetectionType.Overlap:
                OverlapAttack(transform.position - Vector3.up * attackRange);
                break;
        }
    }

    void RaycastAttack(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, enemyLayer);
        if(hit.collider != null && hit.collider.CompareTag("Player2"))
        {
            Debug.Log("!Ataque base con Raycast a Player2!");
            //hit.collider.GetComponent<PlayerController>().TakeDamage(baseAttackDamage); //Referencia al playerController para hacer daño al player2.
        }
    }

    void BoxcastAttack(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(attackRange, 1f), 0f, direction, attackRange, enemyLayer);
        if(hit.colliders.collider != null && hit.colliders.collider.CompareTag("Player2"))
        {

        }
    }

    void OverlapAttack()
    {

    }
}
