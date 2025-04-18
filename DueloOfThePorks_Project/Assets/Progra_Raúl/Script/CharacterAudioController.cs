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
    public AudioClip specialAttack;
    public AudioClip jumpAttack;

    [Header("Otros Sonidos")]
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip hurt;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    // Métodos públicos para reproducir cada sonido
    public void PlayBaseAttack() => Play(baseAttack);
    public void PlayUpAttack() => Play(upAttack);
    public void PlayDownAttack() => Play(downAttack);
    public void PlayStrongAttack() => Play(strongAttack);
    public void PlaySpecialAttack() => Play(specialAttack);
    public void PlayJumpAttack() => Play(jumpAttack);

    public void PlayJump() => Play(jump);
    public void PlayDash() => Play(dash);
    public void PlayHurt() => Play(hurt);

    private void Play(AudioClip clip)
    {
        if (clip != null) src.PlayOneShot(clip);
    }
}
