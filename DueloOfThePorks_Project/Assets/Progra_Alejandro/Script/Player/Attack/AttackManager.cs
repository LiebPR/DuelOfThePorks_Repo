using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] public Attack[] attackSettingsArray; // Array de ataques
    [SerializeField] Transform attackPoint; // Punto de ataque

    private InputManager inputManager;
    private PlayerOrbs playerOrbs;

    private float[] attackCooldownTimers; // Temporizador por cada ataque

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerOrbs = GetComponent<PlayerOrbs>();
    }

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        if (playerController != null && attackSettingsArray.Length > 0)
        {
            attackCooldownTimers = new float[attackSettingsArray.Length];

            for (int i = 0; i < attackSettingsArray.Length; i++)
            {
                if (attackSettingsArray[i] != null)
                {
                    attackSettingsArray[i].Initialize(playerController);
                    attackCooldownTimers[i] = 0f;
                }
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < attackCooldownTimers.Length; i++)
        {
            if (attackCooldownTimers[i] > 0f)
                attackCooldownTimers[i] -= Time.deltaTime;
        }
    }

    public bool TryPerformAttack(int index)
    {
        if (index < 0 || index >= attackSettingsArray.Length || attackSettingsArray[index] == null)
            return false;

        if (attackCooldownTimers[index] <= 0f)
        {
            attackSettingsArray[index].PerformAttack(attackPoint, inputManager.isPlayerOne);
            attackCooldownTimers[index] = attackSettingsArray[index].GetCooldownTime();
            Debug.Log($"Ataque realizado con daño: {attackSettingsArray[index].damage}, cooldown: {attackCooldownTimers[index]}s.");
            return true;
        }

        Debug.Log($"Cooldown activo para el ataque {attackSettingsArray[index].name}, queda: {attackCooldownTimers[index]}s.");
        return false;
    }

    private void OnDrawGizmos()
    {
        if (attackSettingsArray != null && attackPoint != null)
        {
            foreach (var attack in attackSettingsArray)
            {
                if (attack != null)
                    attack.DrawGizmos(attackPoint);
            }
        }
    }
}