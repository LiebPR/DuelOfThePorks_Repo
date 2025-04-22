using UnityEngine;
using UnityEngine.Video;
using System.Collections;

[RequireComponent(typeof(VideoPlayer))]
public sealed class VideoBackgroundSimple : MonoBehaviour
{
    [Header("Vídeo")]
    [Tooltip("Asigna aquí tu VideoClip (p.ej. MP4/WebM importado)")]
    [SerializeField] private VideoClip clip = default;

    [Header("Ajustes de reproducción")]
    [Tooltip("Si true, espera a Prepare() antes de Play para evitar parpadeos")]
    [SerializeField] private bool usePrepare = true;
    [Tooltip("Tiempo de espera máximo para Prepare(), en segundos")]
    [SerializeField, Min(0f)] private float prepareTimeout = 2f;

    private VideoPlayer _videoPlayer;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();

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
        _videoPlayer.skipOnDrop = true;                  // Minimiza stutters 
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
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
