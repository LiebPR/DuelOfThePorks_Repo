using System.Collections;
using System.Collections.Generic;
using UnityEngine;   
public class MovingThroughPlatform : MonoBehaviour
{
    // =====================================================
    // PARTE DE DROP-THROUGH
    // =====================================================
    private Collider2D platformCollider;

    [Header("Configuración de la Plataforma")]
    [Tooltip("Tiempo en segundos que el collider estará desactivado para permitir que el jugador caiga.")]
    [SerializeField] private float fallTime = 0.3f;

    [Header("Detección de Jugadores (Layers)")]
    [Tooltip("Layer asignado a Player 1.")]
    [SerializeField] private LayerMask playerLayer1;
    [Tooltip("Layer asignado a Player 2.")]
    [SerializeField] private LayerMask playerLayer2;

    [Header("Configuración del OverlapBox")]
    [Tooltip("Tamaño del área de detección para saber si el jugador está sobre la plataforma.")]
    [SerializeField] private Vector2 checkBoxSize = new Vector2(2f, 0.5f);
    [Tooltip("Desplazamiento del área de detección respecto al centro de la plataforma.")]
    [SerializeField] private Vector2 checkBoxOffset = new Vector2(0f, 0.5f);

    [Header("Input para Drop-Through")]
    [Tooltip("Player 1 bajará al presionar la tecla S.")]
    [SerializeField] private KeyCode player1DownKey = KeyCode.S;
    [Tooltip("Player 2 bajará al presionar el botón del joystick derecho. Ajusta este KeyCode según tu configuración.")]
    [SerializeField] private KeyCode player2DownKey = KeyCode.Joystick2Button9;

    // =====================================================
    // PARTE DE MOVIMIENTO DE LA PLATAFORMA
    // =====================================================
    public enum MovementType { Horizontal, Vertical }
    [Header("Configuración de Movimiento")]
    [Tooltip("Tipo de movimiento: horizontal (derecha a izquierda) o vertical (arriba-abajo).")]
    [SerializeField] private MovementType movementType = MovementType.Horizontal;
    [Tooltip("Distancia máxima de desplazamiento desde la posición inicial.")]
    [SerializeField] private float moveDistance = 2f;
    [Tooltip("Velocidad de desplazamiento de la plataforma.")]
    [SerializeField] private float moveSpeed = 2f;
    private Vector2 initialPosition;

    // =====================================================
    // MÉTODOS DE INICIALIZACIÓN
    // =====================================================
    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        initialPosition = transform.position;
    }

    // =====================================================
    // ACTUALIZACIÓN
    // =====================================================
    private void Update()
    {
        // Actualiza el movimiento de la plataforma
        MovePlatform();

        // Determina la detección de drop-through
        // Se obtienen los números de layer asumiendo que cada LayerMask tiene asignado UN solo layer.
        int layer1 = GetLayerFromMask(playerLayer1);
        int layer2 = GetLayerFromMask(playerLayer2);

        // Se combinan ambos layers para la detección del área.
        int combinedMask = playerLayer1 | playerLayer2;
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + checkBoxOffset, checkBoxSize, 0f, combinedMask);

        // Se revisa cada collider detectado en el área.
        foreach (Collider2D hit in hits)
        {
            Transform player = hit.transform;
            int playerLayer = player.gameObject.layer;
            if (playerLayer == layer1)
            {
                // Drop-through para Player 1: presiona la tecla S.
                if (Input.GetKeyDown(player1DownKey))
                {
                    StartCoroutine(DisableColliderTemporarily());
                }
            }
            else if (playerLayer == layer2)
            {
                // Drop-through para Player 2: presiona el botón del joystick derecho.
                if (Input.GetKeyDown(player2DownKey))
                {
                    StartCoroutine(DisableColliderTemporarily());
                }
            }
        }
    }

    // =====================================================
    // MÉTODOS ADICIONALES
    // =====================================================

    // Método para desactivar temporalmente el collider de la plataforma.
    private IEnumerator DisableColliderTemporarily()
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(fallTime);
        platformCollider.enabled = true;
    }

    // Método auxiliar para extraer el número de layer de un LayerMask (se asume un único layer asignado).
    private int GetLayerFromMask(LayerMask mask)
    {
        int layerNumber = 0;
        int maskValue = mask.value;
        while (maskValue > 0)
        {
            if ((maskValue & 1) == 1)
                return layerNumber;
            maskValue >>= 1;
            layerNumber++;
        }
        return -1;
    }

    // Método para mover la plataforma de forma oscilante.
    private void MovePlatform()
    {
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        switch (movementType)
        {
            case MovementType.Horizontal:
                transform.position = initialPosition + new Vector2(offset, 0f);
                break;
            case MovementType.Vertical:
                transform.position = initialPosition + new Vector2(0f, offset);
                break;
        }
    }

    // Dibujado de Gizmos para visualizar el área de detección en la escena.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + checkBoxOffset;
        Gizmos.DrawWireCube(checkPosition, checkBoxSize);
    }
}