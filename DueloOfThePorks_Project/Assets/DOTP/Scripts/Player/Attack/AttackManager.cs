using System.Collections;
using System.Collections.Generic;
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

    [Header("Láser Especial")]
    [SerializeField] private bool specialLaserEnabled = true;
    [SerializeField] private GameObject specialEffectPrefab;
    [SerializeField] private Transform specialSpawnPoint;  // Punto de salida del láser
    [SerializeField] private float effectDuration = 1.5f;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerOrbs = GetComponent<PlayerOrbs>();
    }

    private void Start()
    {
        cooldownTimers = new float[attackSettingsArray.Length];
        for (int i = 0; i < cooldownTimers.Length; i++)
            cooldownTimers[i] = 0f;
    }

    private void Update()
    {
        // Disminuir cooldowns
        for (int i = 0; i < cooldownTimers.Length; i++)
            if (cooldownTimers[i] > 0f)
                cooldownTimers[i] -= Time.deltaTime;

        // Ataque especial
        if (inputManager.specialAttackInput)
        {
            if (TryPerformAttack(4))
                SpawnSpecialLaser();
            inputManager.ResetSpecialAttackInput();
        }
    }

    /// <summary>
    /// Intenta ejecutar el ataque 'index'. Devuelve true si se dispara.
    /// </summary>
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
        return true;
    }

    /// <summary>
    /// Instancia un láser como hijo para que siga al jugador y lo destruye tras effectDuration.
    /// </summary>
    private void SpawnSpecialLaser()
    {
        if (!specialLaserEnabled || specialEffectPrefab == null || specialSpawnPoint == null)
            return;

        var laser = Instantiate(
            specialEffectPrefab,
            transform   // parent
        );

        laser.transform.localPosition = specialSpawnPoint.localPosition;
        laser.transform.localRotation = specialSpawnPoint.localRotation;

        Destroy(laser, effectDuration);
    }

    /// <summary>
    /// Llamado desde Animation Event para activar hitbox.
    /// </summary>
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
