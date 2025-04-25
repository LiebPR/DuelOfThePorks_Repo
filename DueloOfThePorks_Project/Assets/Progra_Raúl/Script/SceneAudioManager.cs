using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SoundClip
{
    [Tooltip("Identificador único para este clip")]
    public string name;
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
    [Tooltip("Define aquí todos los clips que usarás en esta escena")]
    public SoundClip[] soundClips;

    // Interno: map nombre→fuente
    private Dictionary<string, AudioSource> sources;

    void Awake()
    {
        // Crear diccionario y AudioSource por cada SoundClip
        sources = new Dictionary<string, AudioSource>();
        foreach (var sc in soundClips)
        {
            if (sc.clip == null || string.IsNullOrEmpty(sc.name))
                continue;

            var src = gameObject.AddComponent<AudioSource>();
            src.clip = sc.clip;
            src.volume = sc.volume;
            src.loop = sc.loop;

            // Si tienes un mixer, enrútalo al grupo adecuado
            if (audioMixer != null)
            {
                var group = sc.loop
                    ? audioMixer.FindMatchingGroups("Music")
                    : audioMixer.FindMatchingGroups("SFX");
                if (group.Length > 0)
                    src.outputAudioMixerGroup = group[0];
            }

            sources[sc.name] = src;
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
        // Auto‐play de todos los clips marcados loop (p.ej. música de fondo)
        foreach (var kv in sources)
        {
            if (kv.Value.loop)
                kv.Value.Play();
        }
    }

    /// <summary>Reproduce un clip no‐loop nombrado.</summary>
    public void PlaySFX(string name)
    {
        if (sources.TryGetValue(name, out var src) && !src.loop)
            src.PlayOneShot(src.clip, src.volume);
        else
            Debug.LogWarning($"[SceneAudioManager] SFX '{name}' no encontrado o está en loop.");
    }

    /// <summary>Detiene inmediatamente un clip loop (música).</summary>
    public void StopMusic(string name)
    {
        if (sources.TryGetValue(name, out var src) && src.loop)
            src.Stop();
        else
            Debug.LogWarning($"[SceneAudioManager] Música '{name}' no encontrada o no está en loop.");
    }

    /// <summary>Detiene todos los audios de esta escena.</summary>
    public void StopAll()
    {
        foreach (var src in sources.Values)
            src.Stop();
    }

    /// <summary>Ajusta volumen maestro (0–1).</summary>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(masterVolParam, Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(musicVolParam, Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(sfxVolParam, Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
    }
}
