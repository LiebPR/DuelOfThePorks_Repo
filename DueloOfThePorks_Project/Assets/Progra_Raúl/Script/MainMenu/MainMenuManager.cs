using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public CanvasGroup panelMainMenu;    // Panel principal del menú (visible al inicio)
    public CanvasGroup panelHowToPlay;   // Panel de "How To Play" (oculto al inicio)

    [Header("Transición Visual (Opcional)")]
    public CanvasGroup canvasTransicion; // Canvas para la transición (fade in/out)
    public float duracionTransicion = 1f;

    [Header("Escena a Cargar")]
    public string escenaAJugar;          // Nombre de la escena a cargar al pulsar "Jugar"

    private void Start()
    {
        // Se muestra solo el menú principal y se oculta el panel "How To Play"
        if (panelMainMenu != null)
            panelMainMenu.gameObject.SetActive(true);
        if (panelHowToPlay != null)
            panelHowToPlay.gameObject.SetActive(false);

        // Configura el canvas de transición para que inicie oculto (opcional)
        if (canvasTransicion != null)
        {
            canvasTransicion.alpha = 0f;
            canvasTransicion.interactable = false;
            canvasTransicion.blocksRaycasts = false;
        }
    }

    #region Gestión de Paneles
    // Abre el panel "How To Play": oculta el menú principal y muestra el panel con un fade in
    public void AbrirHowToPlay()
    {
        if (panelMainMenu != null)
            panelMainMenu.gameObject.SetActive(false);

        if (panelHowToPlay != null)
        {
            panelHowToPlay.gameObject.SetActive(true);
            panelHowToPlay.alpha = 0f;
            panelHowToPlay.interactable = true;
            panelHowToPlay.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(panelHowToPlay, 0f, 1f, 0.5f));
        }
    }

    // Cierra el panel "How To Play" y vuelve a mostrar el menú principal
    public void CerrarHowToPlay()
    {
        if (panelHowToPlay != null)
            StartCoroutine(FadeOutAndDisable(panelHowToPlay, 0.5f));

        if (panelMainMenu != null)
            panelMainMenu.gameObject.SetActive(true);
    }
    #endregion

    #region Cambio de Escena (Jugar)
    // Llama a este método desde el botón "Jugar"
    public void Jugar()
    {
        // Opcional: Realiza un efecto de fade de salida antes de cambiar de escena
        if (canvasTransicion != null)
        {
            canvasTransicion.gameObject.SetActive(true);
            canvasTransicion.interactable = true;
            canvasTransicion.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(canvasTransicion, 0f, 1f, 0.5f, () =>
            {
                // Una vez completado el fade, carga la escena
                SceneManager.LoadScene(escenaAJugar);
            }));
        }
        else
        {
            // Si no se usa transición, carga la escena de inmediato
            SceneManager.LoadScene(escenaAJugar);
        }
    }
    #endregion

    #region Corrutinas de Fade
    // Corrutina para realizar un fade in/out de un CanvasGroup
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float targetAlpha, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        cg.alpha = startAlpha;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = targetAlpha;
        onComplete?.Invoke();
    }

    // Corrutina que realiza el fade out de un CanvasGroup y luego lo desactiva
    private IEnumerator FadeOutAndDisable(CanvasGroup cg, float duration)
    {
        float startAlpha = cg.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.gameObject.SetActive(false);
    }
    #endregion

    #region Salir del Juego
    // Método para salir del juego (incluye salida en el Editor de Unity)
    public void SalirDelJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}
