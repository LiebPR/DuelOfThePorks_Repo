using UnityEngine;
using UnityEngine.Video;
using System.Collections;

[RequireComponent(typeof(VideoPlayer), typeof(Renderer))]
public sealed class VideoBackgroundSimple : MonoBehaviour
{
    [Header("Vídeo")]
    [SerializeField] private VideoClip clip = default;

    [Header("Ajustes de reproducción")]
    [SerializeField] private bool usePrepare = true;
    [SerializeField, Min(0f)] private float prepareTimeout = 2f;

    private VideoPlayer _videoPlayer;
    private Renderer _renderer;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _renderer = GetComponent<Renderer>();

        // —> Fuerza Order in Layer = -10
        _renderer.sortingOrder = -10;
        // opcional: cambiar también la capa de sorting si la tienes personalizada
        // _renderer.sortingLayerName = "Background";

        if (clip == null)
        {
            Debug.LogError($"{nameof(VideoBackgroundSimple)}: debes asignar un VideoClip en el inspector.", this);
            enabled = false;
            return;
        }

        // Configuración básica
        _videoPlayer.clip = clip;
        _videoPlayer.isLooping = true;
        _videoPlayer.playOnAwake = false;
        _videoPlayer.waitForFirstFrame = true;
        _videoPlayer.skipOnDrop = true;
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        // Asegúrate de en el Inspector tener:
        // Render Mode = Material Override
        // Target Material Renderer → este mismo Renderer
    }

    private void OnEnable()
    {
        if (usePrepare)
            StartCoroutine(PrepareAndPlay());
        else
            _videoPlayer.Play();
    }

    private IEnumerator PrepareAndPlay()
    {
        _videoPlayer.Prepare();
        float timer = 0f;
        while (!_videoPlayer.isPrepared && timer < prepareTimeout)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (_videoPlayer.isPrepared)
            _videoPlayer.Play();
        else
        {
            Debug.LogWarning($"{nameof(VideoBackgroundSimple)}: Prepare() tardó >{prepareTimeout}s; iniciando Play() igualmente.", this);
            _videoPlayer.Play();
        }
    }

    private void OnDisable()
    {
        if (_videoPlayer.isPlaying)
            _videoPlayer.Stop();
    }
}
