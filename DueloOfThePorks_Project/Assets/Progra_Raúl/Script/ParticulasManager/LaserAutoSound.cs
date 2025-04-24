
using UnityEngine;

public class LaserAutoSound : MonoBehaviour
{
    void Start()
    {
        var soundPlayer = GetComponent<ParticleSoundPlayer>();
        if (soundPlayer != null)
            soundPlayer.PlayHitSound();
    }
}
