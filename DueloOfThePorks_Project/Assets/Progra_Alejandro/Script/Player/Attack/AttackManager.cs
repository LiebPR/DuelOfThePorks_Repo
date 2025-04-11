using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] Attack[] attackSettingsArray; // Array de ataques
    [SerializeField] Transform attackPoint; // Punto de ataque

    private InputManager inputManager;

    float[] attackCooldownTimers; //Temporizador por cada ataque

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        if (playerController != null && attackSettingsArray.Length > 0) // ¿El array de ataques está asignado?
        {
            //Inizializamos todos los ataques del array
            attackCooldownTimers = new float[attackSettingsArray.Length]; //Inizializamos el array de coldowns

            for (int i = 0; i < attackSettingsArray.Length; i++)
            {
                if (attackSettingsArray[i] != null)
                {
                    attackSettingsArray[i].Initialize(playerController);
                    attackCooldownTimers[i] = 0f; //Al principio, los ataques no tienen cooldown
                }
            }
            Debug.Log("AttackSettings inicializado correctamente");
        }
        else
        {
            Debug.LogError("PlayerController o AttackSettings no están asignados correctamente.");
        }
    }

    private void Update()
    {
        //Actualizamos cooldown de cada ataque
        for (int i = 0; i < attackCooldownTimers.Length; i++)
        {
            if (attackCooldownTimers[i] > 0f)
            {
                attackCooldownTimers[i] -= Time.deltaTime; //Reducimos el coldown de ese ataque
            }
        }

        //Detectamos la entrada del jugador para ralizar el ataque
        if (inputManager.baseAttackInput)//Ataque asignado al clic derecho
        {
            if (inputManager.isWPressed && !inputManager.isSPressed)
            {
                PerformAttackIndex(0);
                inputManager.ResetBaseAttackInput();
                Debug.Log("UpAttack ejecutado");
            }
            else if(inputManager.isSPressed && !inputManager.isWPressed)
            {
                PerformAttackIndex(1);
                inputManager.ResetBaseAttackInput();
                Debug.Log("UpAttack ejecutado");
            }
            else
            {
                PerformAttackIndex(2);
                inputManager.ResetBaseAttackInput();
                Debug.Log("BaseAttack ejecutado");
            }
        }
        else if (inputManager.strongAttackInput)
        {
            PerformAttackIndex(3); //Ejecutamos el ataque del array que está en el índice
            inputManager.ResetStrongAttackInput();
            Debug.Log("Se ha realizado el StrongAttack");
        }
    }

    // Método que maneja la ejecución del ataque según el índice del array
    private void PerformAttackIndex(int index)
    {
        if (index >= 0 && index < attackSettingsArray.Length && attackSettingsArray[index] != null)
        {
            if (attackCooldownTimers[index] <= 0f)
            {
                attackSettingsArray[index].PerformAttack(attackPoint, inputManager.isPlayerOne);
                attackCooldownTimers[index] = attackSettingsArray[index].GetCooldownTime();
                Debug.Log($"Ataque realizado con daño: {attackSettingsArray[index].damage}, cooldown: {attackCooldownTimers[index]} segundos.");
            }
            else
            {
                Debug.Log($"Cooldown activo para el ataque {attackSettingsArray[index].name}, queda: {attackCooldownTimers[index]} segundos.");
            }
            
        }
        else
        {
            Debug.LogError("Índice de ataque fuera de rango o ataque no asignado.");
        }
    }

    // Método para dibujar Gizmos del área de ataque
    private void OnDrawGizmos()
    {
        if (attackSettingsArray != null && attackPoint != null)
        {
            foreach (var attack in attackSettingsArray)
            {
                if (attack != null)
                {
                    attack.DrawGizmos(attackPoint); // Dibuja los Gizmos de cada ataque
                }
            }
        }
    }
}
