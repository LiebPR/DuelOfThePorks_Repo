using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public Attack[] attackSettingsArray; //Array de Atques
    public Transform attackPoint; //Punto de ataque

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        if(playerController != null && attackSettingsArray.Length > 0) // ¿El array de ataques esta asignado?
        {
            //Inicializamos todos los ataques del array
            foreach(var attack in attackSettingsArray)
            {
                if(attack != null)
                {
                    attack.Initialize(playerController);
                }
                Debug.Log("AttackSettings inicializado correctamente");
            }
        }
        else
        {
            Debug.LogError("PlayerController o AttackSettings no estan asignado correctamente.");
        }
    }

    private void Update()
    {
        //Detectamos la entrada del jugador para realizar el ataque
        if (Input.GetMouseButtonDown(1)) //Ataque asignado al clic derecho
        {
            PerformAttackIndex(0); //Ejecuta el ataque del Array que esta en el (0)
            Debug.Log("Se a realizado el ataque");
        }
        else if (Input.GetMouseButtonDown(0)) //Ataque asignado al click izquierdo
        {
            PerformAttackIndex(1); //Ejecuta el ataque del Array que esta en el (1)
            Debug.Log("Se a realizado el Ataque");
        }
    }

    //Método que maneja la ejecución del ataque según el índice del array
    private void PerformAttackIndex(int index)
    {
        if(index >= 0 && index < attackSettingsArray.Length && attackSettingsArray[index] != null)
        {
            //Ejecutamos el ataque según el tipo de ataque en el índice
            attackSettingsArray[index].PerformAttack(attackPoint);
            Debug.Log("Ataque realizado: " + attackSettingsArray[index].name);
        }
        else
        {
            Debug.LogError("Índice de ataque fuera de rando o ataque no asignado.");
        }
    }

    //Método para dibujar Gizmos del área de ataque
    private void OnDrawGizmos()
    {
        if (attackSettingsArray != null && attackPoint != null)
        {
            foreach(var attack in attackSettingsArray)
            {
                if(attack != null)
                {
                    attack.DrawGizmos(attackPoint); //Dibuja los Gizmos de cada ataque
                    
                }  
            }
        }
    }
}
