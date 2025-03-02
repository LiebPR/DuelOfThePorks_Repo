using UnityEngine;

public class PlatformDestroy : MonoBehaviour
{
    [SerializeField] public float destructionTime = 5f; // Tiempo en segundos antes de la destrucci�n

    private void Start()
    {
        // Invoca la destrucci�n del objeto despu�s del tiempo especificado
        Destroy(gameObject, destructionTime);
    }
}