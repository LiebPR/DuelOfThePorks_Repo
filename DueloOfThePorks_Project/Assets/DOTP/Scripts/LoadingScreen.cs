using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // Referencia a la barra de carga

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Obtener el índice de la siguiente escena desde PlayerPrefs
        int sceneToLoad = PlayerPrefs.GetInt("NextScene", 1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; // Evitar que se active la escena de inmediato

        // Mientras la escena se esté cargando
        while (!operation.isDone)
        {
            // Actualizar la barra de progreso
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // Mostrar un mensaje de depuración en la consola para ver el progreso
            Debug.Log("Cargando: " + (progress * 100) + "%");

            // Cuando se haya alcanzado el 90% de progreso, podemos permitir la activación de la escena
            if (operation.progress >= 0.9f)
            {
                // Esperar un poco para simular una carga
                yield return new WaitForSeconds(2f); // Asegura que la escena de carga dure al menos 2 segundos

                // Activar la escena
                operation.allowSceneActivation = true;
            }

            // Esperar el siguiente frame
            yield return null;
        }
    }
}
