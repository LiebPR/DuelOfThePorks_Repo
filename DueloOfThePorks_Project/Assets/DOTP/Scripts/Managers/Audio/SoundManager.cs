using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop;

    [HideInInspector] public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Sonidos de Música")]
    public Sound[] musicTracks; // Música de fondo (por ejemplo, música del menú, música de la escena de juego)

    [Header("Sonidos de Efectos")]
    public Sound[] soundEffects; // Efectos de sonido (clics, notificaciones, etc.)

    private void Awake()
    {
        // Singleton para asegurarse de que solo haya una instancia
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // Asegura que no se destruya al cambiar de escena

        // Inicialización de los sonidos
        InitializeSounds();
    }

    // Inicializa todos los sonidos, asignándoles un AudioSource
    private void InitializeSounds()
    {
        foreach (Sound s in musicTracks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = true; // La música de fondo suele ser en loop
        }

        foreach (Sound s in soundEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = false; // Los efectos de sonido no deben hacer loop
        }
    }

    #region Stop Sound
    public void StopAllMusic()
    {
        foreach (Sound s in musicTracks)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }
    public void StopAllSoundEffects()
    {
        foreach (Sound effect in soundEffects)
        {
            if (effect.source.isPlaying)
            {
                effect.source.Stop();
            }
        }
    }
    #endregion

    #region Gestión de Música

    // Reproduce la música especificada
    public void PlayMusic(string musicName)
    {
        Sound music = System.Array.Find(musicTracks, track => track.name == musicName);
        if (music != null)
        {
            music.source.Play();
        }
        else
        {
            Debug.LogWarning("Música no encontrada: " + musicName);
        }
    }

    // Detiene la música especificada
    public void StopMusic(string musicName)
    {
        Sound music = System.Array.Find(musicTracks, track => track.name == musicName);
        if (music != null)
        {
            music.source.Stop();
        }
        else
        {
            Debug.LogWarning("Música no encontrada: " + musicName);
        }
    }

    // Realiza una transición suave entre dos canciones
    public void CrossfadeMusic(string fromMusicName, string toMusicName, float duration)
    {
        Sound fromMusic = System.Array.Find(musicTracks, track => track.name == fromMusicName);
        Sound toMusic = System.Array.Find(musicTracks, track => track.name == toMusicName);

        if (fromMusic != null && toMusic != null)
        {
            toMusic.source.volume = 0f;
            toMusic.source.Play();
            StartCoroutine(FadeVolume(fromMusic, 0f, duration, stopAfter: true));
            StartCoroutine(FadeVolume(toMusic, toMusic.volume, duration));
        }
        else
        {
            Debug.LogWarning("Música no encontrada en el crossfade.");
        }
    }

    // Transición de volumen para hacer fade in/out
    private IEnumerator FadeVolume(Sound sound, float targetVolume, float duration, bool stopAfter = false)
    {
        float startVolume = sound.source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            sound.source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        sound.source.volume = targetVolume;

        if (stopAfter)
            sound.source.Stop();
    }

    #endregion

    #region Gestión de Efectos de Sonido

    // Reproduce un efecto de sonido
    public void PlaySoundEffect(string soundName)
    {
        Sound effect = System.Array.Find(soundEffects, sound => sound.name == soundName);
        if (effect != null)
        {
            effect.source.Play();
        }
        else
        {
            Debug.LogWarning("Efecto de sonido no encontrado: " + soundName);
        }
    }

    // Detiene un efecto de sonido
    public void StopSoundEffect(string soundName)
    {
        Sound effect = System.Array.Find(soundEffects, sound => sound.name == soundName);
        if (effect != null)
        {
            effect.source.Stop();
        }
        else
        {
            Debug.LogWarning("Efecto de sonido no encontrado: " + soundName);
        }
    }

    #endregion
}
