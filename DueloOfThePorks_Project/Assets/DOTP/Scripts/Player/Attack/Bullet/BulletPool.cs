using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [System.Serializable]
    public class BulletType
    {
        public string name; // Nombre identificador
        public GameObject bulletPrefab; // Prefab de la bala
        public int initialPoolSize = 10; // Cantidad inicial
    }

    [SerializeField] private BulletType[] bulletTypes;

    private Dictionary<string, Queue<GameObject>> bulletPools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var bulletType in bulletTypes)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < bulletType.initialPoolSize; i++)
            {
                GameObject bullet = Instantiate(bulletType.bulletPrefab);
                bullet.SetActive(false);
                pool.Enqueue(bullet);
            }
            bulletPools.Add(bulletType.name, pool);
        }
    }

    public GameObject GetBullet(string bulletTypeName)
    {
        if (!bulletPools.ContainsKey(bulletTypeName))
        {
            Debug.LogError($"No bullet pool found for type {bulletTypeName}!");
            return null;
        }

        var pool = bulletPools[bulletTypeName];
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            var bulletType = System.Array.Find(bulletTypes, b => b.name == bulletTypeName);
            if (bulletType != null)
            {
                GameObject newBullet = Instantiate(bulletType.bulletPrefab);
                newBullet.SetActive(false);
                return newBullet;
            }
            else
            {
                Debug.LogError($"BulletType not found for name: {bulletTypeName}");
                return null;
            }
        }
    }

    public void ReturnBullet(string bulletTypeName, GameObject bullet)
    {
        bullet.SetActive(false);
        if (!bulletPools.ContainsKey(bulletTypeName))
        {
            bulletPools[bulletTypeName] = new Queue<GameObject>();
        }
        bulletPools[bulletTypeName].Enqueue(bullet);
    }
}
