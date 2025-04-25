using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscenaCarga : MonoBehaviour
{
    [Header("UI de carga")]
    [Tooltip("TextMeshProUGUI que mostrará el texto animado 'Cargando...'")]
    public TextMeshProUGUI textoCarga;

    [Header("Parámetros de animación de puntos")]
    [Tooltip("Tiempo (en segundos) entre añadir un punto extra")]
    public float dotDelay = 0.5f;

    [Header("Duración mínima en pantalla")]
    [Tooltip("Tiempo mínimo (en segundos) que debe mostrarse esta pantalla incluso si la escena ya está lista")]
    public float minDisplayTime = 2f;

    private string escenaDestino;
    private float dotTimer;
    private int dotCount;
    private float startTime;

    void Start()
    {
        

        SoundManager.instance.PlaySoundEffect("Thunder");
        SoundManager.instance.PlayMusic("Storming");

        // Validación de referencias
        if (textoCarga == null)
        {
            Debug.LogError("[EscenaCarga] Falta asignar 'textoCarga' en el Inspector.", this);
            enabled = false;
            return;
        }

        // Recupera la escena destino
        escenaDestino = PlayerPrefs.GetString("EscenaDestino", "");
        if (string.IsNullOrEmpty(escenaDestino))
        {
            Debug.LogError("[EscenaCarga] No se encontró 'EscenaDestino' en PlayerPrefs.", this);
            enabled = false;
            return;
        }

        // Guarda el momento de inicio
        startTime = Time.time;
        // Arranca la carga asíncrona
        StartCoroutine(CargarEscenaAsync());
    }

    void Update()
    {
        // Anima los puntos de "Cargando"
        dotTimer += Time.deltaTime;
        if (dotTimer >= dotDelay)
        {
            dotTimer = 0f;
            dotCount = (dotCount + 1) % 4; // 0,1,2 o 3 puntos
            textoCarga.text = "Cargando" + new string('.', dotCount);
        }
    }

    private IEnumerator CargarEscenaAsync()
    {
        // Inicia la carga sin activar automáticamente
        var operacion = SceneManager.LoadSceneAsync(escenaDestino);
        operacion.allowSceneActivation = false;

        // Espera hasta que Unity alcance el 90% de progreso
        while (operacion.progress < 0.9f)
            yield return null;

        // Calcula tiempo restante para cumplir minDisplayTime
        float elapsed = Time.time - startTime;
        if (elapsed < minDisplayTime)
            yield return new WaitForSeconds(minDisplayTime - elapsed);

        // Finalmente activa la escena cargada
        operacion.allowSceneActivation = true;
        SoundManager.instance.StopMusic("Storming");
    }
}