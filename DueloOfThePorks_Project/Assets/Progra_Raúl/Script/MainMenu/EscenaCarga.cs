using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EscenaCarga : MonoBehaviour
{
    [Header("Interfaz de Carga")]
    public Slider barraProgreso;            // Slider para mostrar el avance
    public TextMeshProUGUI textoPorcentaje;   // Texto para mostrar el porcentaje

    [Header("Apariencia de la Barra")]
    public Color colorInicio = Color.red;     // Color para el 0-50%
    public Color colorMitad = Color.yellow;   // Color para el 50%
    public Color colorFinal = Color.green;    // Color para el 100%

    [Header("Parámetros")]
    [Range(1f, 10f)]
    public float suavizado = 5f;  // Velocidad para la interpolación del slider

    private string escenaDestino;  // Nombre de la escena a cargar
    private float progresoObjetivo = 0f;

    void Start()
    {
        // Se recupera la escena destino guardada en PlayerPrefs.
        // Asegúrate de asignarla antes de cambiar a la escena de carga.
        escenaDestino = PlayerPrefs.GetString("EscenaDestino", "");

        if (string.IsNullOrEmpty(escenaDestino))
        {
            Debug.LogError("No se ha definido ninguna escena destino en PlayerPrefs.");
        }
        else
        {
            StartCoroutine(CargarEscenaAsync(escenaDestino));
        }
    }

    void Update()
    {
        // Actualiza el valor del slider de forma suave hacia el progreso calculado
        if (barraProgreso != null)
        {
            barraProgreso.value = Mathf.Lerp(barraProgreso.value, progresoObjetivo, Time.deltaTime * suavizado);
        }

        // Actualiza el texto del porcentaje de carga
        if (textoPorcentaje != null && barraProgreso != null)
        {
            textoPorcentaje.text = $"{(barraProgreso.value * 100):0}%";
        }

        // Cambia el color del fill del slider según el avance
        if (barraProgreso != null && barraProgreso.fillRect != null)
        {
            Image fillImage = barraProgreso.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float progreso = barraProgreso.value;
                if (progreso <= 0.5f)
                    fillImage.color = Color.Lerp(colorInicio, colorMitad, progreso * 2f);
                else
                    fillImage.color = Color.Lerp(colorMitad, colorFinal, (progreso - 0.5f) * 2f);
            }
        }
    }

    IEnumerator CargarEscenaAsync(string nombreEscena)
    {
        // Inicia la carga asíncrona de la escena destino
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        operacion.allowSceneActivation = false;

        // La propiedad "progress" de la operacion llega hasta 0.9; se la normaliza a 1 (100%)
        while (!operacion.isDone)
        {
            progresoObjetivo = Mathf.Clamp01(operacion.progress / 0.9f);

            if (operacion.progress >= 0.9f)
            {
                // Muestra el 100% un momento y luego activa la escena
                yield return new WaitForSeconds(0.5f);
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
