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

    [Header("Sonidos Hit")]
    public Sound[] hitSounds;

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

    public void PlayRandomHitSound()
    {
        if (hitSounds.Length == 0) return;

        int index = Random.Range(0, hitSounds.Length);
        Sound selected = hitSounds[index];

        if (selected.clip != null)
        {
            src.PlayOneShot(selected.clip, selected.volume);
            Debug.Log($"Reproduciendo golpe {index}: {selected.clip.name}");
        }
    }
}
