using UnityEngine;

[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
}

public enum AttackType
{
    Up = 0,
    Down = 1,
    Base = 2,
    Strong = 3,
    Special = 4
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
    public Sound specialAttack;

    [Header("Otros Sonidos")]
    public Sound jump;
    public Sound dash;

    [Header("Sonidos de Orbes")]
    public Sound orbPickup;
    public Sound orbLost;

    [Header("Sonidos Hit")]
    public Sound[] hitSounds;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    private void Play(Sound sound)
    {
        if (sound.clip != null)
        {
            src.PlayOneShot(sound.clip, sound.volume);
        }
    }

    public void PlayBaseAttack() => Play(baseAttack);
    public void PlayUpAttack() => Play(upAttack);
    public void PlayDownAttack() => Play(downAttack);
    public void PlayStrongAttack() => Play(strongAttack);
    public void PlaySpecialAttack() => Play(specialAttack);
    public void PlayJump() => Play(jump);
    public void PlayDash() => Play(dash);
    public void PlayOrbPickup() => Play(orbPickup);
    public void PlayOrbLost() => Play(orbLost);

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

    public void PlayAttackSound(AttackType type)
    {
        switch (type)
        {
            case AttackType.Up:
                Play(upAttack);
                break;
            case AttackType.Down:
                Play(downAttack);
                break;
            case AttackType.Base:
                Play(baseAttack);
                break;
            case AttackType.Strong:
                Play(strongAttack);
                break;
            case AttackType.Special:
                Play(specialAttack);
                break;
        }
    }
}