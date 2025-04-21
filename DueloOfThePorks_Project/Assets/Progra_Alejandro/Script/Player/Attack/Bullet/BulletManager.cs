using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] BulletSettings[] bulletSettingsArray;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float globalAttackCooldown = 0.4f;
    float lastAttackTime = -999f;

    private InputManager inputManager;
    private AttackManager attackManager;

    private float[] bulletCooldowns;

    //Variable publica para el GameTimer
    public bool isLocked = false; //Se podrá modificar desde GameTimer

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        attackManager = GetComponent<AttackManager>();
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

        //Reducimos cooldowns individuales
        for (int i = 0; i < bulletCooldowns.Length; i++)
        {
            if (bulletCooldowns[i] > 0f)
                bulletCooldowns[i] -= Time.deltaTime;
        }

        if (Time.time - lastAttackTime < globalAttackCooldown) return;

        //Orden de prioridad de ataques

        if (!isLocked && inputManager.specialAttackInput && GetComponent<PlayerOrbs>().CanUseSpecialAttack())
        {
            GetComponent<PlayerOrbs>().ConsumeOrbs();
            HandleAttackLogic(4);
            ResetAllAttackInoputs();
        }

        if (!isLocked && inputManager.strongAttackInput)
        {
            HandleAttackLogic(3);
            ResetAllAttackInoputs();
        }

        if (!isLocked && inputManager.baseAttackInput)
        {
            int index = GetInputDirectionIndex();
            if (index != -1)
            {
                HandleAttackLogic(index);
                ResetAllAttackInoputs();
            }
        }  
    }

    // Método de utilidad para limpiar todos los inputs de ataque
    void ResetAllAttackInoputs()
    {
        inputManager.ResetBaseAttackInput();
        inputManager.ResetStrongAttackInput();
        inputManager.ResetStrongAttackInput();
    }

    int GetInputDirectionIndex()
    {
        if (inputManager.isWPressed && !inputManager.isSPressed) return 0;
        if (inputManager.isSPressed && !inputManager.isWPressed) return 1;
        if (!inputManager.isWPressed && !inputManager.isSPressed) return 2;
        return -1;
    }

    void HandleAttackLogic(int index)
    {
        bool bulletExists = index < bulletSettingsArray.Length && bulletSettingsArray[index] != null;
        bool attackExists = attackManager != null && index < attackManager.attackSettingsArray.Length && attackManager.attackSettingsArray[index] != null;

        if (bulletExists && attackExists)
        {
            Debug.LogError($"¡Conflicto! Ambos sistemas tienen ataques en la misma posición del array ({index})");
            return;
        }

        // Obtenemos el AnimatorManager
        var animatorManager = GetComponent<AnimatorManager>();

        if (bulletExists)
        {
            TryShoot(index); // Disparo
            animatorManager?.PlayAttackAnimation(index); // Animación correspondiente
            return;
        }
        if (attackExists)
        {
            if (attackManager.TryPerformAttack(index)) // Si el ataque se ejecuta correctamente
            {
                animatorManager?.PlayAttackAnimation(index); // Animación correspondiente
            }
            return;
        }

        Debug.LogWarning($"No hay ataque ni bala asignado en el índice {index}.");
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
        bulletCooldowns[index] = bulletSettingsArray[index].cooldownTime;

        Debug.Log("Bullet fired from index: " + index);
    }
}
