using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] BulletSettings[] bulletSettingsArray;
    [SerializeField] Transform bulletSpawnPoint;

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

        for (int i = 0; i < bulletCooldowns.Length; i++)
        {
            if (bulletCooldowns[i] > 0f)
                bulletCooldowns[i] -= Time.deltaTime;
        }

        if (!isLocked && inputManager.baseAttackInput)
        {
            int index = GetInputDirectionIndex();
            if (index != -1)
            {
                HandleAttackLogic(index);
                inputManager.ResetBaseAttackInput();
            }
        }

        if (!isLocked && inputManager.strongAttackInput)
        {
            HandleAttackLogic(3);
            inputManager.ResetStrongAttackInput();
        }

        if (isLocked && inputManager.specialAttackInput && GetComponent<PlayerOrbs>().CanUseSpecialAttack())
        {
            GetComponent<PlayerOrbs>().ConsumeOrbs();
            HandleAttackLogic(4);
            inputManager.ResetSpecialAttackInput();
        }
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

        if (bulletExists)
        {
            TryShoot(index); //Si hay bala disparamos
            return;
        }
        if (attackExists)
        {
            attackManager.TryPerformAttack(index); //Si no hay bala pero si ataque, atacamos
            return;
        }
        else
        {
            Debug.LogWarning($"No hay ataque ni bala asignado en el índice {index}.");
        }
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
