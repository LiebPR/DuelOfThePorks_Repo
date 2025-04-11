using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    [Header("Configuración de la Escena")]
    [Tooltip("Nombre de la escena a cargar. Si está vacío, se usa el índice.")]
    public string nombreEscena;

    [Tooltip("Índice de la escena a cargar. Se usa si no se proporciona nombre.")]
    public int indiceEscena = -1;

    [Header("Opciones de Carga")]
    [Tooltip("Determina si la carga de escena será asíncrona.")]
    public bool cargaAsincrona = true;

    [Tooltip("Usar una escena de carga intermedia.")]
    public bool usarEscenaCarga = true;

    [Tooltip("Nombre de la escena de carga, si se usa una escena intermedia.")]
    public string nombreEscenaCarga = "EscenaCarga";

    [Header("Transición Visual")]
    [Tooltip("Si se activa, realiza una transición visual al cambiar de escena.")]
    public bool usarTransicion = false;

    [Tooltip("CanvasGroup utilizado para realizar la transición de fade.")]
    public CanvasGroup canvasTransicion;

    [Tooltip("Duración de la transición de fade.")]
    public float duracionTransicion = 1f;

    /// <summary>
    /// Cambia de escena según la configuración.
    /// </summary>
    public void CambiarEscena()
    {
        if (usarEscenaCarga)
        {
            // Almacena la escena destino en PlayerPrefs y carga la escena de carga
            string escenaDestino = !string.IsNullOrEmpty(nombreEscena) ? nombreEscena : SceneManager.GetSceneByBuildIndex(indiceEscena).name;
            PlayerPrefs.SetString("EscenaDestino", escenaDestino);
            PlayerPrefs.Save();
            SceneManager.LoadScene(nombreEscenaCarga);
        }
        else
        {
            // Carga la escena de manera sincrónica o asincrónica
            if (cargaAsincrona)
                StartCoroutine(CargarEscenaAsincrona());
            else
                StartCoroutine(CargarEscena());
        }
    }

    /// <summary>
    /// Carga la escena de forma sincrónica con un pequeño delay.
    /// </summary>
    private IEnumerator CargarEscena()
    {
        if (usarTransicion) yield return StartCoroutine(Fade(1f));
        yield return new WaitForSeconds(1f);

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
        else if (indiceEscena >= 0)
            SceneManager.LoadScene(indiceEscena);
    }

    /// <summary>
    /// Carga la escena de manera asincrónica con un control de progreso.
    /// </summary>
    private IEnumerator CargarEscenaAsincrona()
    {
        if (usarTransicion) yield return StartCoroutine(Fade(1f));
        yield return new WaitForSeconds(1f);

        AsyncOperation operacion;
        if (!string.IsNullOrEmpty(nombreEscena))
            operacion = SceneManager.LoadSceneAsync(nombreEscena);
        else if (indiceEscena >= 0)
            operacion = SceneManager.LoadSceneAsync(indiceEscena);
        else
            yield break;

        operacion.allowSceneActivation = false;

        // Monitorea el progreso de la carga asincrónica
        while (!operacion.isDone)
        {
            if (operacion.progress >= 0.9f)
                operacion.allowSceneActivation = true;

            yield return null;
        }
    }

    /// <summary>
    /// Realiza una transición de fade in/out en el CanvasGroup.
    /// </summary>
    private IEnumerator Fade(float alphaObjetivo)
    {
        if (canvasTransicion == null) yield break;

        float alphaInicial = canvasTransicion.alpha;
        float tiempo = 0f;

        // Interpolación de alpha para lograr el efecto de fade
        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            canvasTransicion.alpha = Mathf.Lerp(alphaInicial, alphaObjetivo, tiempo / duracionTransicion);
            yield return null;
        }
    }

    /// <summary>
    /// Cierra la aplicación de manera profesional.
    /// </summary>
    public void SalirDelJuego()
    {
        // Cierra la aplicación en plataformas de ejecución
        Application.Quit();

        // Si está ejecutándose en el editor de Unity, detiene la ejecución
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
