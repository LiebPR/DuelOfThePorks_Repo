using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackPercentage : AttackManager
{
    [Header("Porcentage Damage Settings")]
    [SerializeField] float damagePercentageFactor = 50f;
    [SerializeField] float deathThersholdMin = 100f;
    [SerializeField] float deathThresholdMax = 200f;

    [Header("Death Effects")]
    [SerializeField] ParticleSystem deathEffect;
    [SerializeField] float deathEffectDuration = 2f;

    bool isDead = false;

   



}
