using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class MenuSound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = true;           // por defecto true para música
}

[System.Serializable]
public class SfxSound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;          // por defecto false para SFX
}

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public CanvasGroup panelMainMenu;
    public CanvasGroup panelHowToPlay;

    [Header("Transición Visual")]
    public CanvasGroup canvasTransicion;
    public float duracionTransicion = 1f;

    [Header("Escena a Cargar")]
    public string escenaAJugar;
    public string escenaDeCarga = "EscenaCarga";

    [Header("Música de Fondo")]
    public MenuSound[] musicSounds;

    [Header("Efectos de Sonido")]
    public SfxSound[] sfxSounds;

    AudioSource[] musicSources;
    AudioSource[] sfxSources;

    void Awake()
    {
        // Crear AudioSources para cada pista de música
        musicSources = new AudioSource[musicSounds.Length];
        for (int i = 0; i < musicSounds.Length; i++)
        {
            var m = musicSounds[i];
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = m.clip;
            src.volume = m.volume;
            src.loop = m.loop;
            musicSources[i] = src;
        }
        // Crear AudioSources para cada SFX
        sfxSources = new AudioSource[sfxSounds.Length];
        for (int i = 0; i < sfxSounds.Length; i++)
        {
            var fx = sfxSounds[i];
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = fx.clip;
            src.volume = fx.volume;
            src.loop = fx.loop;
            sfxSources[i] = src;
        }
    }

    void Start()
    {
        // Mostrar UI inicial
        panelMainMenu?.gameObject.SetActive(true);
        panelHowToPlay?.gameObject.SetActive(false);
        if (canvasTransicion != null)
        {
            canvasTransicion.alpha = 0f;
            canvasTransicion.interactable = false;
            canvasTransicion.blocksRaycasts = false;
        }

        // Reproducir todas las pistas de música
        foreach (var src in musicSources)
            if (src.clip != null)
                src.Play();
    }

    public void AbrirHowToPlay()
    {
        ReproducirSFX(0); // botón
        panelMainMenu?.gameObject.SetActive(false);
        if (panelHowToPlay != null)
        {
            panelHowToPlay.gameObject.SetActive(true);
            panelHowToPlay.alpha = 0f;
            panelHowToPlay.interactable = true;
            panelHowToPlay.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(panelHowToPlay, 0f, 1f, duracionTransicion));
        }
    }

    public void CerrarHowToPlay()
    {
        ReproducirSFX(0); // botón
        if (panelHowToPlay != null)
            StartCoroutine(FadeOutAndDisable(panelHowToPlay, duracionTransicion));
        panelMainMenu?.gameObject.SetActive(true);
    }

    public void ReproducirSFX(int index = 0)
    {
        if (index >= 0 && index < sfxSources.Length)
        {
            var src = sfxSources[index];
            if (src.clip != null)
                src.Play();
        }
    }

    public void Jugar()
    {
        // Detener todo el audio antes de cambiar de escena
        foreach (var src in musicSources) src.Stop();
        foreach (var src in sfxSources) src.Stop();

        ReproducirSFX(0); // sonido de click

        PlayerPrefs.SetString("EscenaDestino", escenaAJugar);
        PlayerPrefs.Save();

        if (canvasTransicion != null)
        {
            canvasTransicion.gameObject.SetActive(true);
            canvasTransicion.interactable = true;
            canvasTransicion.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(canvasTransicion, 0f, 1f, duracionTransicion, () =>
            {
                SceneManager.LoadScene(escenaDeCarga);
            }));
        }
        else
        {
            SceneManager.LoadScene(escenaDeCarga);
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float dur, System.Action onComplete = null)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
        onComplete?.Invoke();
    }

    IEnumerator FadeOutAndDisable(CanvasGroup cg, float dur)
    {
        float start = cg.alpha, t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, 0f, t / dur);
            yield return null;
        }
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        foreach (var src in musicSources) src.Stop();
        foreach (var src in sfxSources) src.Stop();
    }
}
