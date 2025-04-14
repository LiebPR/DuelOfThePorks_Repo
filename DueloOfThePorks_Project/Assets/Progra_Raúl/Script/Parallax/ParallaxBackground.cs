using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración de la Cámara")]
    // Transform de la cámara principal. Si no se asigna, se buscará la cámara principal.
    public Transform cameraTransform;

    [Header("Parámetros del Parallax")]
    // Multiplicador que determina qué tan rápido se mueve el fondo en relación a la cámara.
    public float parallaxMultiplier = 0.5f;

    [Header("Simulación de Movimiento (para cámara fija)")]
    // Si se activa, se simula un movimiento en el fondo aun cuando la cámara no se mueve.
    public bool simulateMovement = true;
    // Velocidad de la simulación.
    public float simulatedSpeed = 0.5f;
    // Amplitud del movimiento simulado.
    public float simulatedAmplitude = 1f;

    // Guarda la posición de la cámara en el cuadro anterior (modo cámara en movimiento).
    private Vector3 lastCameraPosition;
    // Posición inicial del fondo.
    private Vector3 initialPosition;

    void Start()
    {
        // Si no se asigna la cámara, se usa la principal.
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        lastCameraPosition = cameraTransform.position;
        initialPosition = transform.position;
    }

    // LateUpdate para asegurarse de que se procese después de los movimientos de la cámara.
    void LateUpdate()
    {
        if (simulateMovement)
        {
            // Se genera un desplazamiento simulado usando una función seno, que oscila con el tiempo.
            float simulatedOffset = Mathf.Sin(Time.time * simulatedSpeed) * simulatedAmplitude;
            // Se actualiza la posición del fondo; en este ejemplo se simula el movimiento horizontal.
            transform.position = new Vector3(initialPosition.x + simulatedOffset * parallaxMultiplier,
                                             transform.position.y,
                                             transform.position.z);
        }
        else
        {
            // Calcula el movimiento de la cámara desde el último frame.
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            // Aplica el efecto parallax al fondo.
            transform.position += new Vector3(deltaMovement.x * parallaxMultiplier,
                                              deltaMovement.y * parallaxMultiplier, 0);
            // Actualiza la posición anterior de la cámara.
            lastCameraPosition = cameraTransform.position;
        }
    }
}
