// SceneAudioManager.cs
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClip
{
    [Tooltip("El archivo de audio (.wav/.mp3)")]
    public AudioClip clip;
    [Range(0f, 1f)]
    [Tooltip("Volumen inicial (0–1)")]
    public float volume = 1f;
    [Tooltip("¿Debe repetirse en bucle? (ideal para música)")]
    public bool loop = false;
}

public class SceneAudioManager : MonoBehaviour
{
    [Header("Audio Mixer (opcional)")]
    public AudioMixer audioMixer;
    public string masterVolParam = "MasterVolume";
    public string musicVolParam = "MusicVolume";
    public string sfxVolParam = "SFXVolume";

    [Header("Lista de Clips de Audio")]
    public SoundClip[] soundClips;

    // Paralelo a soundClips: un AudioSource por Clip
    public AudioSource[] sources;

    void Awake()
    {
        sources = new AudioSource[soundClips.Length];
        for (int i = 0; i < soundClips.Length; i++)
        {
            var sc = soundClips[i];
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = sc.clip;
            src.volume = sc.volume;
            src.loop = sc.loop;

            if (audioMixer != null)
            {
                var group = sc.loop
                    ? audioMixer.FindMatchingGroups("Music")
                    : audioMixer.FindMatchingGroups("SFX");
                if (group.Length > 0)
                    src.outputAudioMixerGroup = group[0];
            }

            // Nos aseguramos de no reproducir nada en Start de SFX
            if (!sc.loop)
                src.playOnAwake = false;

            sources[i] = src;
        }

        if (audioMixer != null)
        {
            SetMasterVolume(1f);
            SetMusicVolume(1f);
            SetSFXVolume(1f);
        }
    }

    void Start()
    {
        // Reproducir solo la música en bucle (los que tienen loop = true)
        for (int i = 0; i < sources.Length; i++)
        {
            if (soundClips[i].loop)
                sources[i].Play();
        }
    }

    /// <summary>
    /// Reproduce el clip en el índice dado (no‐loop). 
    /// Si ya está sonando, no lo vuelve a disparar.
    /// </summary>
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sources.Length) return;
        var sc = soundClips[index];
        var src = sources[index];
        if (sc.loop) return;                // no usamos PlaySFX para los loop
        if (src.isPlaying) return;          // evita superposiciones dobles
        src.PlayOneShot(sc.clip, sc.volume);
    }

    /// <summary>
    /// Detiene inmediatamente el clip en el índice dado (loop).
    /// </summary>
    public void StopMusic(int index)
    {
        if (index < 0 || index >= sources.Length) return;
        var sc = soundClips[index];
        var src = sources[index];
        if (sc.loop)
            src.Stop();
    }

    /// <summary>
    /// Detiene todos los audios de esta escena.
    /// </summary>
    public void StopAll()
    {
        foreach (var src in sources)
            src.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer?.SetFloat(masterVolParam, Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer?.SetFloat(musicVolParam, Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer?.SetFloat(sfxVolParam, Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
    }
}
