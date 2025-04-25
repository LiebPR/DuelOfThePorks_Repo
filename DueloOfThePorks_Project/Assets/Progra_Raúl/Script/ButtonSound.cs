using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [Tooltip("Sonido al hacer clic en el botón")]
    public AudioClip clickSound;
    [Tooltip("Sonido al pasar el cursor sobre el botón")]
    public AudioClip hoverSound;
    [Range(0f, 1f), Tooltip("Volumen de los sonidos (0–1)")]
    public float volume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Llamado al hacer clic
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound, volume);
    }

    // Llamado al pasar el cursor encima
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound, volume);
    }
}
