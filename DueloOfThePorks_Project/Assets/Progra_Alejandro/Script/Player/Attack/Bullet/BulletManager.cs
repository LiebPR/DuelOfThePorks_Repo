using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] BulletSettings[] bulletSettingsArray;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float globalAttackCooldown = 0.4f;

    private InputManager inputManager;
    private AttackManager attackManager;
    private PlayerOrbs playerOrbs;

    private float[] bulletCooldowns;
    private float lastAttackTime = -999f; // ahora es campo de instancia
    public bool isLocked = false;         // modificado por GameTimer

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        attackManager = GetComponent<AttackManager>();
        playerOrbs = GetComponent<PlayerOrbs>();
        bulletCooldowns = new float[bulletSettingsArray.Length];
    }

    private void OnEnable()
    {
        for (int i = 0; i < bulletCooldowns.Length; i++)
            bulletCooldowns[i] = 0f;
    }

    private void Update()
    {
        if (isLocked) return;

        // Reducir cooldowns individuales
        for (int i = 0; i < bulletCooldowns.Length; i++)
            if (bulletCooldowns[i] > 0f)
                bulletCooldowns[i] -= Time.deltaTime;

        // Cooldown global
        if (Time.time - lastAttackTime < globalAttackCooldown) return;

        // Special Attack (índice 4)
        if (inputManager.specialAttackInput && playerOrbs.CanUseSpecialAttack())
        {
            bool didFire = HandleAttackLogic(4);
            if (didFire)
            {
                playerOrbs.ConsumeOrbs();
                lastAttackTime = Time.time;
            }
            ResetAllAttackInputs();
            return;
        }

        // Strong Attack (índice 3)
        if (inputManager.strongAttackInput)
        {
            bool didFire = HandleAttackLogic(3);
            if (didFire) lastAttackTime = Time.time;
            ResetAllAttackInputs();
            return;
        }

        // Base Attack (índices 0–2 según dirección)
        if (inputManager.baseAttackInput)
        {
            int idx = GetInputDirectionIndex();
            if (idx != -1)
            {
                bool didFire = HandleAttackLogic(idx);
                if (didFire) lastAttackTime = Time.time;
                ResetAllAttackInputs();
            }
        }
    }

    void ResetAllAttackInputs()
    {
        inputManager.ResetBaseAttackInput();
        inputManager.ResetStrongAttackInput();
        inputManager.ResetSpecialAttackInput();
    }

    int GetInputDirectionIndex()
    {
        if (inputManager.isWPressed && !inputManager.isSPressed) return 0;
        if (inputManager.isSPressed && !inputManager.isWPressed) return 1;
        if (!inputManager.isWPressed && !inputManager.isSPressed) return 2;
        return -1;
    }

    /// <summary>
    /// Ejecuta ataque o dispara bala según el índice.
    /// Devuelve true si realmente se disparó/atacó.
    /// </summary>
    bool HandleAttackLogic(int index)
    {
        bool hasBullet = index < bulletSettingsArray.Length && bulletSettingsArray[index] != null;
        bool hasAttack = attackManager != null &&
                         index < attackManager.attackSettingsArray.Length &&
                         attackManager.attackSettingsArray[index] != null;

        if (hasBullet && hasAttack)
        {
            Debug.LogError($"¡Conflicto! Ambos sistemas en índice {index}");
            return false;
        }

        var animator = GetComponent<AnimatorManager>();
        if (hasBullet)
        {
            TryShoot(index);
            animator?.PlayAttackAnimation(index);
            bulletCooldowns[index] = bulletSettingsArray[index].cooldownTime;
            return true;
        }
        if (hasAttack)
        {
            bool ok = attackManager.TryPerformAttack(index);
            if (ok) animator?.PlayAttackAnimation(index);
            return ok;
        }

        Debug.LogWarning($"No hay ataque ni bala asignado en índice {index}.");
        return false;
    }

    void TryShoot(int index)
    {
        if (bulletCooldowns[index] > 0f || isLocked) return;

        GameObject bulletObj = BulletPool.Instance.GetBullet();
        if (bulletObj == null)
        {
            Debug.LogWarning("No bullets available in pool!");
            return;
        }

        bulletObj.transform.position = bulletSpawnPoint.position;
        bulletObj.transform.rotation = Quaternion.identity;

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.settings = bulletSettingsArray[index];
        bullet.owner = gameObject;
        bullet.direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        bulletObj.SetActive(true);
    }
}
