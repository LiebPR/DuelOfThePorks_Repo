using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] BulletSettings bulletSettings;
    [SerializeField] Transform bulletSpawnPoint;

    private InputManager inputManager;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    private void OnEnable()
    {
        cooldownTimer = 0f;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (inputManager.baseAttackInput)
        {
            if (inputManager.isWPressed && !inputManager.isSPressed)
            {
                TryShoot();
                inputManager.ResetBaseAttackInput();
            }
        }
    }

    void TryShoot()
    {
        if (cooldownTimer > 0f) return;

        Debug.Log("BulletManager is Active");

        GameObject bulletObj = BulletPool.Instance.GetBullet();
        if (bulletObj == null)
        {
            Debug.LogWarning("No bullets available in pool!");
            return;
        }

        bulletObj.transform.position = bulletSpawnPoint.position;
        bulletObj.transform.rotation = Quaternion.identity;

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.settings = bulletSettings;
        bullet.owner = gameObject;
        bullet.direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        bulletObj.SetActive(true);
        cooldownTimer = bulletSettings.cooldownTime;

        Debug.Log("Bullet fired!");
    }
}
