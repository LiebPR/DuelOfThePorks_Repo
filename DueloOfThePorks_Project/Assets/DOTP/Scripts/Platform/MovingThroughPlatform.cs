using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
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
    [Tooltip("Player 2 bajará al presionar el botón del joystick derecho.")]
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
    // PADRE TEMPORAL DE JUGADORES
    // =====================================================
    // Guarda el parent original de cada jugador para restaurarlo
    private Dictionary<Transform, Transform> originalParents = new Dictionary<Transform, Transform>();

    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        initialPosition = transform.position;

        // Aseguramos un Rigidbody2D kinematic para que Unity gestione bien las colisiones
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        MovePlatform();
        HandleDropThroughInput();
    }

    private void HandleDropThroughInput()
    {
        int layer1 = GetLayerFromMask(playerLayer1);
        int layer2 = GetLayerFromMask(playerLayer2);
        int combinedMask = playerLayer1 | playerLayer2;

        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + checkBoxOffset, checkBoxSize, 0f, combinedMask);
        foreach (Collider2D hit in hits)
        {
            int plLayer = hit.gameObject.layer;
            if (plLayer == layer1 && Input.GetKeyDown(player1DownKey))
                StartCoroutine(DisableColliderTemporarily());
            else if (plLayer == layer2 && Input.GetKeyDown(player2DownKey))
                StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        // Antes de desactivar el collider, desanidamos a los jugadores para que puedan caer
        // Recorremos una copia de las llaves para evitar modificación durante iteración
        foreach (var kvp in originalParents.ToList())
        {
            var playerT = kvp.Key;
            var origParent = kvp.Value;
            if (playerT != null && playerT.parent == transform)
            {
                playerT.SetParent(origParent);
                originalParents.Remove(playerT);
            }
        }

        platformCollider.enabled = false;
        yield return new WaitForSeconds(fallTime);
        platformCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layer1 = GetLayerFromMask(playerLayer1);
        int layer2 = GetLayerFromMask(playerLayer2);
        int hitLayer = collision.gameObject.layer;

        if (hitLayer == layer1 || hitLayer == layer2)
        {
            var playerT = collision.transform;
            // Guardar el parent original si no existe
            if (!originalParents.ContainsKey(playerT))
                originalParents[playerT] = playerT.parent;
            // Convertir al player en hijo de la plataforma
            playerT.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        int layer1 = GetLayerFromMask(playerLayer1);
        int layer2 = GetLayerFromMask(playerLayer2);
        int hitLayer = collision.gameObject.layer;

        if (hitLayer == layer1 || hitLayer == layer2)
        {
            var playerT = collision.transform;
            // Restaurar parent original
            if (originalParents.TryGetValue(playerT, out var origParent))
            {
                playerT.SetParent(origParent);
                originalParents.Remove(playerT);
            }
            else
            {
                playerT.SetParent(null);
            }
        }
    }

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

    private int GetLayerFromMask(LayerMask mask)
    {
        int layerNumber = 0, maskValue = mask.value;
        while (maskValue > 0)
        {
            if ((maskValue & 1) == 1) return layerNumber;
            maskValue >>= 1;
            layerNumber++;
        }
        return -1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + checkBoxOffset;
        Gizmos.DrawWireCube(checkPosition, checkBoxSize);
    }
}
