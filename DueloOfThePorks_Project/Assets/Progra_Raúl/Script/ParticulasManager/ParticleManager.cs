using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }

        [System.Serializable]
        public class ParticlePool
        {
            public string nombreID;
            public GameObject prefab;
            public int poolSize = 10;
        }

        [Header("Pools de partículas disponibles")]
        [SerializeField] private List<ParticlePool> particlePools;

        private readonly Dictionary<string, Queue<GameObject>> poolDictionary = new();
        private readonly Dictionary<string, GameObject> prefabDictionary = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InicializarPools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InicializarPools()
        {
            foreach (var pool in particlePools)
            {
                Queue<GameObject> objectPool = new();

                for (int i = 0; i < pool.poolSize; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    objectPool.Enqueue(obj);
                }

                poolDictionary[pool.nombreID] = objectPool;
                prefabDictionary[pool.nombreID] = pool.prefab;
            }
        }

        public void Play(string nombreID, Vector3 posicion, Quaternion rotacion = default)
        {
            if (!poolDictionary.ContainsKey(nombreID))
            {
                Debug.LogWarning($"[ParticleManager] No se encontró un pool con el nombreID: {nombreID}");
                return;
            }

            GameObject obj = poolDictionary[nombreID].Dequeue();
            obj.transform.position = posicion;
            obj.transform.rotation = rotacion;
            obj.SetActive(true);

            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            float duracion = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 2f;
            StartCoroutine(VolverAlPool(nombreID, obj, duracion));
        }

        private IEnumerator VolverAlPool(string nombreID, GameObject obj, float tiempo)
        {
            yield return new WaitForSeconds(tiempo);
            obj.SetActive(false);
            poolDictionary[nombreID].Enqueue(obj);
        }
    }
}
