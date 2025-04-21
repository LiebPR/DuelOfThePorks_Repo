using System;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class AttackManager : MonoBehaviour
{
    [SerializeField] public Attack[] attackSettingsArray; // Ahora público para acceso externo
    [SerializeField] public Transform attackPoint;        // Ahora público para acceso externo

    private InputManager inputManager;
    private float[] attackCooldownTimers;

    // Evento que dispara el índice de ataque ejecutado
    public event Action<int> onAttackPerformed;

    void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    void Start()
    {
        if (attackSettingsArray.Length > 0)
        {
            attackCooldownTimers = new float[attackSettingsArray.Length];
            for (int i = 0; i < attackSettingsArray.Length; i++)
            {
                attackSettingsArray[i]?.Initialize(GetComponent<PlayerController>());
                attackCooldownTimers[i] = 0f;
            }
        }
    }

    void Update()
    {
        if (attackCooldownTimers == null) return;
        for (int i = 0; i < attackCooldownTimers.Length; i++)
            attackCooldownTimers[i] = Mathf.Max(0f, attackCooldownTimers[i] - Time.deltaTime);
    }

    public bool TryPerformAttack(int index)
    {
        if (index < 0 || index >= attackSettingsArray.Length) return false;
        var attack = attackSettingsArray[index];
        if (attack == null) return false;

        if (attackCooldownTimers[index] <= 0f)
        {
            attack.PerformAttack(attackPoint, inputManager.isPlayerOne);
            attackCooldownTimers[index] = attack.GetCooldownTime();
            Debug.Log($"[AttackManager] Attack {index} performed, cooldown: {attackCooldownTimers[index]:0.00}s");
            onAttackPerformed?.Invoke(index);
            return true;
        }

        Debug.Log($"[AttackManager] Attack {index} on cooldown: {attackCooldownTimers[index]:0.00}s remaining");
        return false;
    }

    void OnDrawGizmos()
    {
        if (attackSettingsArray == null || attackPoint == null) return;
        foreach (var atk in attackSettingsArray)
            atk?.DrawGizmos(attackPoint);
    }
}
