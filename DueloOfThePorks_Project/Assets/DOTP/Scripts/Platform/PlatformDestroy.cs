using UnityEngine;

public class PlatformDestroy : MonoBehaviour
{
    [SerializeField] public float destructionTime = 5f; // Tiempo en segundos antes de la destrucción

    private void Start()
    {
        // Invoca la destrucción del objeto después del tiempo especificado
        Destroy(gameObject, destructionTime);
    }
}