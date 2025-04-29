using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections;

[Serializable]
public class MenuSound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = true;
}

[Serializable]
public class SfxSound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;
}

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public CanvasGroup panelMainMenu;

    [Header("'Cómo Jugar' Panels")]
    public CanvasGroup[] howToPlayPanels;
    private int currentHowToPlayIndex = 0;

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
        // Crear AudioSources para música
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
        // Crear AudioSources para SFX
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
        // Mostrar menú principal y ocultar páginas de 'Cómo jugar'
        panelMainMenu?.gameObject.SetActive(true);

        if (howToPlayPanels != null)
        {
            foreach (var p in howToPlayPanels)
            {
                if (p == null) continue;
                p.gameObject.SetActive(false);
                p.alpha = 0f;
                p.interactable = false;
                p.blocksRaycasts = false;
            }
        }

        // Preparar transición
        if (canvasTransicion != null)
        {
            canvasTransicion.alpha = 0f;
            canvasTransicion.interactable = false;
            canvasTransicion.blocksRaycasts = false;
        }

        // Reproducir música
        foreach (var src in musicSources)
            if (src.clip != null)
                src.Play();
    }

    void Update()
    {
        // Navegación con A/D y clic fuera para cerrar
        if (howToPlayPanels != null && howToPlayPanels.Length > 0)
        {
            var currentPanel = howToPlayPanels[currentHowToPlayIndex];
            if (currentPanel != null && currentPanel.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.D)) NextHowPanel();
                if (Input.GetKeyDown(KeyCode.A)) PrevHowPanel();

                if (Input.GetMouseButtonDown(0))
                {
                    // Detectar clic fuera del panel
                    RectTransform rt = currentPanel.GetComponent<RectTransform>();
                    if (rt != null && !RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, null))
                    {
                        CerrarHowToPlay();
                    }
                }
            }
        }
    }

    public void AbrirHowToPlay()
    {
        ReproducirSFX(0);
        panelMainMenu?.gameObject.SetActive(false);

        if (howToPlayPanels != null && howToPlayPanels.Length > 0)
            ShowHowToPlayPanel(0);
    }

    public void CerrarHowToPlay()
    {
        ReproducirSFX(0);

        // Ocultar página actual
        if (howToPlayPanels != null && howToPlayPanels.Length > 0)
            StartCoroutine(FadeOutAndDisable(howToPlayPanels[currentHowToPlayIndex], duracionTransicion));

        panelMainMenu?.gameObject.SetActive(true);
    }

    public void NextHowPanel()
    {
        if (currentHowToPlayIndex < howToPlayPanels.Length - 1)
            StartCoroutine(SwitchPanel(currentHowToPlayIndex + 1));
    }

    public void PrevHowPanel()
    {
        if (currentHowToPlayIndex > 0)
            StartCoroutine(SwitchPanel(currentHowToPlayIndex - 1));
    }

    private void ShowHowToPlayPanel(int index)
    {
        currentHowToPlayIndex = index;
        var p = howToPlayPanels[index];
        p.gameObject.SetActive(true);
        p.alpha = 0f;
        p.interactable = true;
        p.blocksRaycasts = true;
        StartCoroutine(FadeCanvasGroup(p, 0f, 1f, duracionTransicion));
    }

    private IEnumerator SwitchPanel(int newIndex)
    {
        var old = howToPlayPanels[currentHowToPlayIndex];
        yield return StartCoroutine(FadeCanvasGroup(old, old.alpha, 0f, duracionTransicion));
        old.interactable = false;
        old.blocksRaycasts = false;
        old.gameObject.SetActive(false);

        ShowHowToPlayPanel(newIndex);
    }

    public void ReproducirSFX(int index = 0)
    {
        if (index < 0 || index >= sfxSources.Length) return;
        var src = sfxSources[index];
        if (src.clip != null) src.Play();
    }

    public void Jugar()
    {
        foreach (var src in musicSources) src.Stop();
        foreach (var src in sfxSources) src.Stop();

        ReproducirSFX(0);
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

    public void SalirDelJuego()
    {
        foreach (var src in musicSources) src.Stop();
        foreach (var src in sfxSources) src.Stop();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float dur, Action onComplete = null)
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
