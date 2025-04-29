using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Ataques")]
    [SerializeField] public Attack[] attackSettingsArray;  // 0: Up, 1: Down, 2: Base, 3: Strong, 4: Special
    [SerializeField] private Transform attackPoint;

    [Header("Cooldowns")]
    private float[] cooldownTimers;

    private int currentAttackIndex = -1;
    private InputManager inputManager;
    private PlayerOrbs playerOrbs;
    private CharacterAudioController audioController;

    [Header("Láser Especial")]
    [SerializeField] private bool specialLaserEnabled = true;
    [SerializeField] private GameObject specialEffectPrefab;
    [SerializeField] private Transform specialSpawnPoint;
    [SerializeField] private float effectDuration = 1.5f;

    private GameObject currentSpecialLaser;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerOrbs = GetComponent<PlayerOrbs>();
        audioController = GetComponent<CharacterAudioController>();
    }

    private void Start()
    {
        cooldownTimers = new float[attackSettingsArray.Length];
        for (int i = 0; i < cooldownTimers.Length; i++)
            cooldownTimers[i] = 0f;
    }

    private void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
            if (cooldownTimers[i] > 0f)
                cooldownTimers[i] -= Time.deltaTime;

        if (inputManager.specialAttackInput)
        {
            if (TryPerformAttack(4))
            {
                // Se lanza el ataque especial
            }
            inputManager.ResetSpecialAttackInput();
        }
    }

    public bool TryPerformAttack(int index)
    {
        if (index < 0 || index >= attackSettingsArray.Length) return false;
        if (attackSettingsArray[index] == null) return false;
        if (cooldownTimers[index] > 0f) return false;
        if (index == 4 && !playerOrbs.CanUseSpecialAttack()) return false;

        currentAttackIndex = index;
        GetComponent<AnimatorManager>().PlayAttackAnimation(index);

        if (index == 4)
            playerOrbs.ConsumeOrbs();

        cooldownTimers[index] = attackSettingsArray[index].GetCooldownTime();

        // 🔊 Reproducir sonido de ataque
        audioController?.PlayAttackSound((AttackType)index);

        return true;
    }

    public void SpawnSpecialLaser()
    {
        if (!specialLaserEnabled || specialEffectPrefab == null || specialSpawnPoint == null)
            return;

        if (currentSpecialLaser != null)
            Destroy(currentSpecialLaser);

        currentSpecialLaser = Instantiate(specialEffectPrefab, transform);
        currentSpecialLaser.transform.localPosition = specialSpawnPoint.localPosition;
        currentSpecialLaser.transform.localRotation = specialSpawnPoint.localRotation;
        Destroy(currentSpecialLaser, effectDuration);
    }

    public void StopSpecialLaser()
    {
        if (currentSpecialLaser != null)
        {
            Destroy(currentSpecialLaser);
            currentSpecialLaser = null;
        }
    }

    public void TriggerAttackHitbox()
    {
        if (currentAttackIndex < 0 || currentAttackIndex >= attackSettingsArray.Length)
            return;

        attackSettingsArray[currentAttackIndex]
            .PerformAttack(attackPoint, gameObject, inputManager.isPlayerOne);

        currentAttackIndex = -1;
    }

    private void OnDrawGizmos()
    {
        if (attackSettingsArray == null || attackPoint == null) return;
        foreach (var atk in attackSettingsArray)
            atk?.DrawGizmos(attackPoint);
    }
}
