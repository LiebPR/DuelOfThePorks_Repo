using UnityEngine;

[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
}


[RequireComponent(typeof(AudioSource))]
public class CharacterAudioController : MonoBehaviour
{
    private AudioSource src;

    [Header("Sonidos de Ataque")]
    public Sound baseAttack;
    public Sound upAttack;
    public Sound downAttack;
    public Sound strongAttack;
    public Sound specialAttack;    // ← agregado

    [Header("Otros Sonidos")]
    public Sound jump;
    public Sound dash;

    [Header("Sonidos de Orbes")]
    public Sound orbPickup; //SFX recoger orbe.
    public Sound orbLost; //SFX perder orbe.

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public void PlayBaseAttack() { Play(baseAttack); }
    public void PlayUpAttack() { Play(upAttack); }
    public void PlayDownAttack() { Play(downAttack); }
    public void PlayStrongAttack() { Play(strongAttack); }
    public void PlaySpecialAttack() { Play(specialAttack); }

    public void PlayJump() { Play(jump); }
    public void PlayDash() { Play(dash); }
    public void PlayOrbPickup() { Play(orbPickup); }
    public void PlayOrbLost() { Play(orbLost); }

    private void Play(Sound sound)
    {
        if(sound.clip != null)
        {
            src.PlayOneShot(sound.clip, sound.volume);
        }
    }
}
