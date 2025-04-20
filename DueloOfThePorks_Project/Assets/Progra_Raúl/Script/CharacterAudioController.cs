using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterAudioController : MonoBehaviour
{
    private AudioSource src;

    [Header("Sonidos de Ataque")]
    public AudioClip baseAttack;
    public AudioClip upAttack;
    public AudioClip downAttack;
    public AudioClip strongAttack;
    public AudioClip specialAttack;    // ← agregado

    [Header("Otros Sonidos")]
    public AudioClip jump;
    public AudioClip dash;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public void PlayBaseAttack() { if (baseAttack != null) src.PlayOneShot(baseAttack); }
    public void PlayUpAttack() { if (upAttack != null) src.PlayOneShot(upAttack); }
    public void PlayDownAttack() { if (downAttack != null) src.PlayOneShot(downAttack); }
    public void PlayStrongAttack() { if (strongAttack != null) src.PlayOneShot(strongAttack); }
    public void PlaySpecialAttack() { if (specialAttack != null) src.PlayOneShot(specialAttack); }  // ← agregado

    public void PlayJump() { if (jump != null) src.PlayOneShot(jump); }
    public void PlayDash() { if (dash != null) src.PlayOneShot(dash); }
}
