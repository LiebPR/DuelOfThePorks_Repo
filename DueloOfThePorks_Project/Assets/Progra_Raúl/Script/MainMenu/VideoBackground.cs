using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RectTransform))]
public class VideoBackground : MonoBehaviour
{
    [Header("Video Clip")]
    [Tooltip("Vídeo a reproducir en bucle como fondo")]
    public VideoClip clip;

    [Header("Opciones de reproducción")]
    public bool loop = true;
    [Range(0f, 1f)] public float volume = 0f;

    private VideoPlayer _videoPlayer;
    private RawImage _rawImage;
    private RectTransform _rt;

    void Awake()
    {
        // Ajustar RectTransform para cubrir todo el padre
        _rt = GetComponent<RectTransform>();
        _rt.anchorMin = Vector2.zero;
        _rt.anchorMax = Vector2.one;
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;

        // Crear o coger el RawImage y ocultarlo hasta que esté listo
        _rawImage = GetComponent<RawImage>() ?? gameObject.AddComponent<RawImage>();
        _rawImage.enabled = false;

        // Configurar VideoPlayer sin playOnAwake
        _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.playOnAwake = false;
        _videoPlayer.isLooping = loop;
        _videoPlayer.clip = clip;
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        // Audio del vídeo
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        _videoPlayer.SetTargetAudioSource(0, audioSource);

        // Cuando termine de preparar, llamamos a OnPrepared
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        // Prepara inmediatamente (sin mostrar nada aún)
        _videoPlayer.Prepare();
    }

    private void OnDestroy()
    {
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Asignar textura y mostrar RawImage, luego reproducir
        _rawImage.texture = vp.texture;
        _rawImage.enabled = true;
        vp.Play();
    }
}
