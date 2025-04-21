using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] public Attack[] attackSettingsArray;
    [SerializeField] Transform attackPoint;

    private InputManager inputManager;
    private PlayerOrbs playerOrbs;
    private float[] cooldownTimers;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerOrbs = GetComponent<PlayerOrbs>();
    }

    private void Start()
    {
        cooldownTimers = new float[attackSettingsArray.Length];
        for (int i = 0; i < cooldownTimers.Length; i++)
            cooldownTimers[i] = 0f;
    }

    private void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
            if (cooldownTimers[i] > 0f)
                cooldownTimers[i] -= Time.deltaTime;
    }

    /// <summary>
    /// Intenta realizar el ataque de índice ‘index’.
    /// Devuelve true si se ejecutó correctamente.
    /// </summary>
    public bool TryPerformAttack(int index)
    {
        if (index < 0 || index >= attackSettingsArray.Length) return false;
        if (attackSettingsArray[index] == null) return false;
        if (cooldownTimers[index] > 0f) return false;

        // Sólo permitir Special (4) si hay orbes suficientes
        if (index == 4 && !playerOrbs.CanUseSpecialAttack())
            return false;

        // Ejecutar ataque
        attackSettingsArray[index]
            .PerformAttack(attackPoint, gameObject, inputManager.isPlayerOne);
        cooldownTimers[index] = attackSettingsArray[index].GetCooldownTime();

        // Si era Special, consumimos las orbes
        if (index == 4)
            playerOrbs.ConsumeOrbs();

        return true;
    }

    private void OnDrawGizmos()
    {
        if (attackSettingsArray == null || attackPoint == null) return;
        foreach (var atk in attackSettingsArray)
            atk?.DrawGizmos(attackPoint);
    }
}
