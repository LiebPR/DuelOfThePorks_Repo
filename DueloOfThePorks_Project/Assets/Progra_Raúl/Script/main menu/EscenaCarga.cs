using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EscenaCarga : MonoBehaviour
{
    [Header("UI de carga")]
    public Slider barraProgreso;
    public TextMeshProUGUI textoPorcentaje;

    [Header("Colores de la barra")]
    public Color colorInicio = Color.red;
    public Color colorMitad = Color.yellow;
    public Color colorFinal = Color.green;

    [Header("Velocidad de animación")]
    [Range(1f, 10f)]
    public float suavizado = 5f;

    private string escenaDestino;
    private float progresoObjetivo = 0f;

    private void Start()
    {
        escenaDestino = PlayerPrefs.GetString("EscenaDestino", "");

        if (!string.IsNullOrEmpty(escenaDestino))
        {
            StartCoroutine(CargarEscenaAsync(escenaDestino));
        }
        else
        {
            Debug.LogWarning("No se ha definido ninguna escena destino en PlayerPrefs.");
        }
    }

    private void Update()
    {
        // Animación suave del slider
        barraProgreso.value = Mathf.Lerp(barraProgreso.value, progresoObjetivo, Time.deltaTime * suavizado);

        // Actualizar texto
        if (textoPorcentaje != null)
        {
            textoPorcentaje.text = $"{(barraProgreso.value * 100f):0}%";
        }

        // Cambiar color dinámico según el progreso
        if (barraProgreso != null)
        {
            float progreso = barraProgreso.value;

            if (progreso <= 0.5f)
            {
                barraProgreso.fillRect.GetComponent<Image>().color = Color.Lerp(colorInicio, colorMitad, progreso * 2f);
            }
            else
            {
                barraProgreso.fillRect.GetComponent<Image>().color = Color.Lerp(colorMitad, colorFinal, (progreso - 0.5f) * 2f);
            }
        }
    }

    private IEnumerator CargarEscenaAsync(string nombreEscena)
    {
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        operacion.allowSceneActivation = false;

        while (!operacion.isDone)
        {
            progresoObjetivo = Mathf.Clamp01(operacion.progress / 0.9f);

            if (operacion.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
