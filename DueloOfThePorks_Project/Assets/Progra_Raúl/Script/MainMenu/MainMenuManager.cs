using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public CanvasGroup panelMainMenu;
    public CanvasGroup panelHowToPlay;

    [Header("Transición Visual (Opcional)")]
    public CanvasGroup canvasTransicion;
    public float duracionTransicion = 1f;

    [Header("Escena a Cargar")]
    public string escenaAJugar; // Ej: "Scene_Pract"
    public string escenaDeCarga = "EscenaCarga"; // Asegúrate que esta escena esté en Build Settings

    [Header("Fondo de Vídeo (Opcional)")]
    public GameObject videoBackground; // Arrastra aquí el GameObject con VideoBackground

    private void Start()
    {
        SoundManager.instance.PlayMusic("FarmMusic");

        SoundManager.instance.PlaySoundEffect("MolinoViento");
        SoundManager.instance.PlaySoundEffect("Brisa");
        if (panelMainMenu != null)
            panelMainMenu.gameObject.SetActive(true);
        if (panelHowToPlay != null)
            panelHowToPlay.gameObject.SetActive(false);

        if (canvasTransicion != null)
        {
            canvasTransicion.alpha = 0f;
            canvasTransicion.interactable = false;
            canvasTransicion.blocksRaycasts = false;
        }

        // Activar fondo de vídeo si existe
        if (videoBackground != null)
            videoBackground.SetActive(true);
    }

    public void ReproducirSonidoBotton()
    {
        SoundManager.instance.PlaySoundEffect("ButtonClick");
    }

    #region Gestión de Paneles
    public void AbrirHowToPlay()
    {
        ReproducirSonidoBotton();

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

    public void CerrarHowToPlay()
    {
        ReproducirSonidoBotton();
        if (panelHowToPlay != null)
            StartCoroutine(FadeOutAndDisable(panelHowToPlay, 0.5f));

        if (panelMainMenu != null)
            panelMainMenu.gameObject.SetActive(true);
    }
    #endregion

    #region Cambio de Escena (Jugar)
    public void Jugar()
    {
        ReproducirSonidoBotton();

        SoundManager.instance.StopMusic("FramMusic");
        SoundManager.instance.StopSoundEffect("MolinoViento");
        SoundManager.instance.StopSoundEffect("Brisa");


        PlayerPrefs.SetString("EscenaDestino", escenaAJugar);
        PlayerPrefs.Save();

        if (canvasTransicion != null)
        {
            canvasTransicion.gameObject.SetActive(true);
            canvasTransicion.interactable = true;
            canvasTransicion.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(canvasTransicion, 0f, 1f, 0.5f, () =>
            {
                SceneManager.LoadScene(escenaDeCarga);
            }));
        }
        else
        {
            SoundManager.instance.StopMusic("FramMusic");
            SoundManager.instance.StopSoundEffect("MolinoViento");
            SoundManager.instance.StopSoundEffect("Brisa");

            SceneManager.LoadScene(escenaDeCarga);
        }
    }
    #endregion

    #region Corrutinas de Fade
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
    public void SalirDelJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}
