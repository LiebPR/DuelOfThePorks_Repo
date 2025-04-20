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
    public bool playOnAwake = true;
    public float volume = 0f;

    private VideoPlayer _videoPlayer;
    private RawImage _rawImage;
    private RectTransform _rt;

    void Awake()
    {
        // Aseguramos que el RectTransform llene el área padre
        _rt = GetComponent<RectTransform>();
        _rt.anchorMin = Vector2.zero;
        _rt.anchorMax = Vector2.one;
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;

        // Creamos o buscamos el RawImage
        _rawImage = GetComponent<RawImage>();
        if (_rawImage == null)
            _rawImage = gameObject.AddComponent<RawImage>();

        // Creamos el VideoPlayer en este GameObject
        _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.playOnAwake = playOnAwake;
        _videoPlayer.isLooping = loop;
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        // Creamos un AudioSource para el vídeo (invisible)
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        _videoPlayer.SetTargetAudioSource(0, audioSource);

        // Asignamos el vídeo
        _videoPlayer.clip = clip;
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        // Preparamos (evita frame inicial en negro)
        _videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // Cuando está listo, asignamos la textura al RawImage
        _rawImage.texture = source.texture;
        if (playOnAwake)
            source.Play();
    }
}
