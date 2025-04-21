using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] public Attack[] attackSettingsArray; // Array de ataques
    [SerializeField] Transform attackPoint;               // Punto de ataque

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
        // Inicializar cooldowns
        cooldownTimers = new float[attackSettingsArray.Length];
        for (int i = 0; i < cooldownTimers.Length; i++)
            cooldownTimers[i] = 0f;
    }

    private void Update()
    {
        // Reducir cooldowns
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
        // Validaciones básicas
        if (index < 0 || index >= attackSettingsArray.Length) return false;
        if (attackSettingsArray[index] == null) return false;
        if (cooldownTimers[index] > 0f) return false;

        // Sólo permitir Special (4) si hay orbes
        if (index == 4 && !playerOrbs.CanUseSpecialAttack())
            return false;

        // 1) Ejecutar el ataque real
        attackSettingsArray[index]
            .PerformAttack(attackPoint, gameObject, inputManager.isPlayerOne);

        // 2) Disparar la animación **antes** de consumir orbes
        GetComponent<AnimatorManager>().PlayAttackAnimation(index);

        // 3) Si era Special, ahora sí consumimos las orbes
        if (index == 4)
            playerOrbs.ConsumeOrbs();

        // 4) Poner el cooldown
        cooldownTimers[index] = attackSettingsArray[index].GetCooldownTime();

        Debug.Log($"Ataque {index} realizado. Cooldown: {cooldownTimers[index]}s.");
        return true;
    }

    private void OnDrawGizmos()
    {
        if (attackSettingsArray == null || attackPoint == null) return;
        foreach (var atk in attackSettingsArray)
            atk?.DrawGizmos(attackPoint);
    }
}
