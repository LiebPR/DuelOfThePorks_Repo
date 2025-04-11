using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject howToPlayCanvas; // Asigna el Canvas de "How to Play" en el Inspector

    // Método para abrir el Canvas con las instrucciones
    public void AbrirHowToPlay()
    {
        // Verifica si el Canvas está asignado y abre
        if (howToPlayCanvas != null)
        {
            Debug.Log("¡Botón How to Play presionado!"); // Esto aparecerá en la consola
            howToPlayCanvas.SetActive(true); // Activa el Canvas de "How to Play"
        }
        else
        {
            Debug.LogError("El Canvas de 'How to Play' no está asignado en el Inspector.");
        }
    }

    // Método para cerrar el Canvas y volver al estado normal
    public void CerrarHowToPlay()
    {
        // Verifica si el Canvas está asignado y cierra
        if (howToPlayCanvas != null)
        {
            howToPlayCanvas.SetActive(false); // Desactiva el Canvas de "How to Play"
        }
        else
        {
            Debug.LogError("El Canvas de 'How to Play' no está asignado en el Inspector.");
        }
    }
}
