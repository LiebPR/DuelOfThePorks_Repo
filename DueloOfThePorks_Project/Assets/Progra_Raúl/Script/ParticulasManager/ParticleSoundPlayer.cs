using UnityEngine;

public class ParticleSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);
    }
}