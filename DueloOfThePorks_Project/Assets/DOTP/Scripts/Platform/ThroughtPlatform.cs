using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughtPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    [Header("Configuración de la Plataforma")]
    [SerializeField] float fallTime = 0.3f;

    [Header("Detección de Jugadores (Layers)")]
    [SerializeField] LayerMask playerLayer1; // Asigna aquí el layer para Player 1.
    [SerializeField] LayerMask playerLayer2; // Asigna aquí el layer para Player 2.

    [Header("Configuración del OverlapBox")]
    [SerializeField] Vector2 checkBoxSize = new Vector2(2f, 0.5f);
    [SerializeField] Vector2 checkBoxOffset = new Vector2(0f, 0.5f);

    [Header("Input para Player 1")]
    // Player 1 bajará al presionar únicamente la tecla S.
    [SerializeField] KeyCode player1DownKey = KeyCode.S;

    [Header("Input para Player 2")]
    // Player 2 bajará presionando el botón del joystick derecho.
    // Nota: Dependiendo del controlador y su configuración, ajusta este KeyCode.
    [SerializeField] KeyCode player2DownKey = KeyCode.Joystick2Button9;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Para identificar el layer numérico de cada jugador (se asume que cada LayerMask tiene asignado UN solo layer)
        int layer1 = GetLayerFromMask(playerLayer1);
        int layer2 = GetLayerFromMask(playerLayer2);

        // Combina ambos layers para la detección en el área.
        int combinedMask = playerLayer1 | playerLayer2;

        // Detecta todos los colliders de jugadores en el área definida.
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + checkBoxOffset, checkBoxSize, 0f, combinedMask);

        foreach (Collider2D hit in hits)
        {
            Transform player = hit.transform;
            int playerLayer = player.gameObject.layer;

            if (playerLayer == layer1)
            {
                // Player 1 baja al presionar la tecla S.
                if (Input.GetKeyDown(player1DownKey))
                {
                    StartCoroutine(DisableColliderTemporarily());
                }
            }
            else if (playerLayer == layer2)
            {
                // Player 2 baja al presionar el botón del joystick derecho.
                if (Input.GetKeyDown(player2DownKey))
                {
                    StartCoroutine(DisableColliderTemporarily());
                }
            }
        }
    }

    IEnumerator DisableColliderTemporarily()
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(fallTime);
        platformCollider.enabled = true;
    }

    // Función auxiliar para extraer el número de layer a partir de un LayerMask (se asume que solo tiene un layer activo)
    int GetLayerFromMask(LayerMask mask)
    {
        int layerNumber = 0;
        int maskValue = mask.value;
        while (maskValue > 0)
        {
            if ((maskValue & 1) == 1)
                return layerNumber;
            maskValue = maskValue >> 1;
            layerNumber++;
        }
        return -1;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + checkBoxOffset;
        Gizmos.DrawWireCube(checkPosition, checkBoxSize);
    }
}
