using UnityEngine;

[CreateAssetMenu(fileName = "NewBullet", menuName = "Bullet/BulletSettings", order = 1)]
public class BulletSettings : ScriptableObject
{
    public float speed = 10f;
    public float damage = 5f;
    public float knockbackForce = 10f;
    public float cooldownTime = 1f;
    public float maxTravelDistance = 20f;
    public GameObject bulletPrefab;
    public LayerMask targetLayer;
}
