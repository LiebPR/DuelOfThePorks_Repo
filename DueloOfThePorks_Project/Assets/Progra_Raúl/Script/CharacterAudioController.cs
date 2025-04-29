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
    private AudioSource sfxSource;    // Fuente para sonidos one-shot
    private AudioSource loopSource;   // Fuente para sonido de salto en bucle
    private PlayerController playerController;
    private bool wasDashing;

    [Header("Sonidos de Ataque")]
    public Sound baseAttack;
    public Sound upAttack;
    public Sound downAttack;
    public Sound strongAttack;
    public Sound specialAttack;

    [Header("Otros Sonidos")]
    public Sound jump;
    public Sound dash;

    [Header("Sonidos de Movimiento")]
    public Sound crouch;
    public Sound crouchWalk;
    public Sound standUp; 

    [Header("Sonidos de Orbes")]
    public Sound orbPickup;
    public Sound orbLost;

    [Header("Sonidos Hit")]
    public Sound[] hitSounds;

    private bool wasCrouching = false;
    private bool wasCrouchWalking = false;

    void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.playOnAwake = false;
        loopSource.loop = true;

        playerController = GetComponent<PlayerController>();
        wasDashing = false;

        // Configurar clip de loop con sonido de salto
        if (jump.clip != null)
        {
            loopSource.clip = jump.clip;
            loopSource.volume = jump.volume;
        }
    }

    void Update()
    {
        if (playerController == null) return;

        // Manejo de sonido de salto en bucle
        bool grounded = playerController.IsGrounded();
        if (!grounded && !loopSource.isPlaying)
        {
            loopSource.Play();
        }
        else if (grounded && loopSource.isPlaying)
        {
            loopSource.Stop();
        }

        // Dash
        bool isDashing = playerController.IsDashing();
        if (isDashing && !wasDashing)
        {
            PlayOneShot(dash);
        }
        wasDashing = isDashing;

        // Crouch 
        bool isCrouching = playerController.IsCrouching();
        if (isCrouching && !wasCrouching)
            PlayOneShot(crouch);
        wasCrouching = isCrouching;

        // Crouch Walk 
        bool isCrouchWalking = isCrouching && Mathf.Abs(playerController.GetHorizontalInput()) > 0.1f && grounded;
        if (isCrouchWalking && !wasCrouchWalking)
            PlayOneShot(crouchWalk);
        wasCrouchWalking = isCrouchWalking;

        //Levantarse 
        if(!isCrouching && wasCrouching)
        {
            PlayOneShot(standUp);
        }

    }

    private void PlayOneShot(Sound sound)
    {
        if (sound.clip != null)
            sfxSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlayBaseAttack() => PlayOneShot(baseAttack);
    public void PlayUpAttack() => PlayOneShot(upAttack);
    public void PlayDownAttack() => PlayOneShot(downAttack);
    public void PlayStrongAttack() => PlayOneShot(strongAttack);
    public void PlaySpecialAttack() => PlayOneShot(specialAttack);
    public void PlayOrbPickup() => PlayOneShot(orbPickup);
    public void PlayOrbLost() => PlayOneShot(orbLost);

    public void PlayRandomHitSound()
    {
        if (hitSounds.Length == 0) return;
        int index = Random.Range(0, hitSounds.Length);
        Sound selected = hitSounds[index];
        if (selected.clip != null)
        {
            sfxSource.PlayOneShot(selected.clip, selected.volume);
            Debug.Log($"Reproduciendo golpe {index}: {selected.clip.name}");
        }
    }

    public void PlayAttackSound(AttackType type)
    {
        switch (type)
        {
            case AttackType.Up: PlayOneShot(upAttack); break;
            case AttackType.Down: PlayOneShot(downAttack); break;
            case AttackType.Base: PlayOneShot(baseAttack); break;
            case AttackType.Strong: PlayOneShot(strongAttack); break;
            case AttackType.Special: PlayOneShot(specialAttack); break;
        }
    }
}