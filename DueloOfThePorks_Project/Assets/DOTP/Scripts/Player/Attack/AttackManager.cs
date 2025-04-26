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

    // Instancia actual del láser especial
    private GameObject currentSpecialLaser;

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

        // Ataque especial: dispara animación; spawn y stop via Animation Events
        if (inputManager.specialAttackInput)
        {
            if (TryPerformAttack(4))
            {
                // SpawnSpecialLaser() llamado desde Animation Event
            }
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
    /// Instancia el láser especial. Llamado desde Animation Event.
    /// </summary>
    public void SpawnSpecialLaser()
    {
        if (!specialLaserEnabled || specialEffectPrefab == null || specialSpawnPoint == null)
            return;

        // Destruye instancia previa
        if (currentSpecialLaser != null)
            Destroy(currentSpecialLaser);

        currentSpecialLaser = Instantiate(specialEffectPrefab, transform);
        currentSpecialLaser.transform.localPosition = specialSpawnPoint.localPosition;
        currentSpecialLaser.transform.localRotation = specialSpawnPoint.localRotation;
        Destroy(currentSpecialLaser, effectDuration);
    }

    /// <summary>
    /// Detiene el láser especial. Llamado desde Animation Event.
    /// </summary>
    public void StopSpecialLaser()
    {
        if (currentSpecialLaser != null)
        {
            Destroy(currentSpecialLaser);
            currentSpecialLaser = null;
        }
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
