using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

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
    [Tooltip("Asignar un AudioMixer para controlar volúmenes maestros, music y SFX")]
    public AudioMixer audioMixer;
    public string masterVolParam = "MasterVolume";
    public string musicVolParam = "MusicVolume";
    public string sfxVolParam = "SFXVolume";

    [Header("Lista de Clips de Audio")]
    [Tooltip("Arrastra aquí todos los clips que usarás en esta escena y ajusta volumen/loop")]
    public SoundClip[] soundClips;

    // Interno: paralelo a soundClips
    private AudioSource[] sources;

    void Awake()
    {
        // Crear un AudioSource por cada SoundClip
        sources = new AudioSource[soundClips.Length];
        for (int i = 0; i < soundClips.Length; i++)
        {
            var sc = soundClips[i];
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = sc.clip;
            src.volume = sc.volume;
            src.loop = sc.loop;

            // Opcional: enrutar a grupos de AudioMixer
            if (audioMixer != null)
            {
                var group = sc.loop
                    ? audioMixer.FindMatchingGroups("Music")
                    : audioMixer.FindMatchingGroups("SFX");
                if (group.Length > 0)
                    src.outputAudioMixerGroup = group[0];
            }

            sources[i] = src;
        }

        // Ajustar volúmenes iniciales en el mixer
        if (audioMixer != null)
        {
            SetMasterVolume(1f);
            SetMusicVolume(1f);
            SetSFXVolume(1f);
        }
    }

    void Start()
    {
        // Auto‐play de todos los clips marcados loop (música de fondo)
        for (int i = 0; i < sources.Length; i++)
            if (soundClips[i].loop)
                sources[i].Play();
    }

    /// <summary>Reproduce el clip en el índice dado (no‐loop).</summary>
    public void PlaySFX(int index, bool loop = false)
    {
        if (index < 0 || index >= sources.Length) return;
        var src = sources[index];
        if (!soundClips[index].loop && src.clip != null)
            src.PlayOneShot(src.clip, src.volume);
    }

    /// <summary>Detiene inmediatamente el clip en el índice dado (loop).</summary>
    public void StopMusic(int index)
    {
        if (index < 0 || index >= sources.Length) return;
        var src = sources[index];
        if (soundClips[index].loop)
            src.Stop();
    }

    /// <summary>Detiene todos los audios de esta escena.</summary>
    public void StopAll()
    {
        foreach (var src in sources)
            src.Stop();
    }

    /// <summary>Ajusta volumen maestro (0–1).</summary>
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
