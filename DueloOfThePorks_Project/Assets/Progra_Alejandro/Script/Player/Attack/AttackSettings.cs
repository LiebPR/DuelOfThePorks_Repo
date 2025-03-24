using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


enum typeAttack
{
    boxCast,
    rayCast,
    Overlap
}
[CreateAssetMenu(fileName = "NuevoAttack", menuName = "Attack", order = 1)]
public class AttackSettings : ScriptableObject
{

    Attack attack;

    [Header("Settings")]
    [SerializeField] float damage = 5f;
    [SerializeField] float cooldown = 0.5f;
    [SerializeField] float KnockBack = 4f;
    [SerializeField] GameObject attackPoint;
    [SerializeField] typeAttack tA;

    void AtaqueType()
    {
        
    }

    void OverlapCircle()
    {

    }

    void BoxCast()
    {

    }

    void RayCast() 
    { 

    }

}
