using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[System.Serializable]
public class LoadSound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;
}

public class EscenaCarga : MonoBehaviour
{
    [Header("UI de carga")]
    public TextMeshProUGUI textoCarga;

    [Header("Animación de puntos")]
    public float dotDelay = 0.5f;

    [Header("Duración mínima en pantalla")]
    public float minDisplayTime = 2f;

    [Header("Sonidos de Carga")]
    public LoadSound[] loadSounds;

    AudioSource[] loadSources;
    string escenaDestino;
    float dotTimer, startTime;
    int dotCount;

    void Awake()
    {
        // Crear AudioSource por cada LoadSound
        loadSources = new AudioSource[loadSounds.Length];
        for (int i = 0; i < loadSounds.Length; i++)
        {
            var ls = loadSounds[i];
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = ls.clip;
            src.volume = ls.volume;
            src.loop = ls.loop;
            loadSources[i] = src;
        }
    }

    void Start()
    {
        // Reproducir todos los sonidos de carga
        foreach (var src in loadSources)
            if (src.clip != null)
                src.Play();

        if (textoCarga == null)
        {
            Debug.LogError("[EscenaCarga] Falta asignar 'textoCarga'.", this);
            enabled = false;
            return;
        }

        escenaDestino = PlayerPrefs.GetString("EscenaDestino", "");
        if (string.IsNullOrEmpty(escenaDestino))
        {
            Debug.LogError("[EscenaCarga] No se encontró 'EscenaDestino'.", this);
            enabled = false;
            return;
        }

        startTime = Time.time;
        StartCoroutine(CargarEscenaAsync());
    }

    void Update()
    {
        dotTimer += Time.deltaTime;
        if (dotTimer >= dotDelay)
        {
            dotTimer = 0f;
            dotCount = (dotCount + 1) % 4;
            textoCarga.text = "Loading" + new string('.', dotCount);
        }
    }

    IEnumerator CargarEscenaAsync()
    {
        var op = SceneManager.LoadSceneAsync(escenaDestino);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        float elapsed = Time.time - startTime;
        if (elapsed < minDisplayTime)
            yield return new WaitForSeconds(minDisplayTime - elapsed);

        // Detener todos los sonidos de carga
        foreach (var src in loadSources)
            src.Stop();

        op.allowSceneActivation = true;
    }

    void OnDisable()
    {
        // Asegura que no quede audio al desactivar
        foreach (var src in loadSources)
            src.Stop();
    }
}
