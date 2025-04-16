using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
            public AudioClip audioClip; // opcional
        }

        [Header("Pools de partículas disponibles")]
        [SerializeField] private List<ParticlePool> particlePools;

        private readonly Dictionary<string, Queue<GameObject>> poolDictionary = new();
        private readonly Dictionary<string, AudioClip> audioClips = new();

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
                if (string.IsNullOrEmpty(pool.nombreID) || pool.prefab == null)
                {
                    Debug.LogWarning("[ParticleManager] Pool inválido.");
                    continue;
                }

                var queue = new Queue<GameObject>();
                for (int i = 0; i < pool.poolSize; i++)
                {
                    var obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }

                poolDictionary[pool.nombreID] = queue;
                if (pool.audioClip != null)
                    audioClips[pool.nombreID] = pool.audioClip;
            }
        }

        // Sobrecargas Play: sin/rotación, con delay o con rotación+delay
        public void Play(string nombreID, Vector3 pos) =>
            Play(nombreID, pos, Quaternion.identity, 0f);

        public void Play(string nombreID, Vector3 pos, float delay) =>
            Play(nombreID, pos, Quaternion.identity, delay);

        public void Play(string nombreID, Vector3 pos, Quaternion rot) =>
            Play(nombreID, pos, rot, 0f);

        public void Play(string nombreID, Vector3 pos, Quaternion rot, float delay) =>
            StartCoroutine(PlayConDelay(nombreID, pos, rot, delay));

        private IEnumerator PlayConDelay(string id, Vector3 pos, Quaternion rot, float delay)
        {
            yield return new WaitForSeconds(delay);
            EjecutarEfecto(id, pos, rot);
        }

        private void EjecutarEfecto(string id, Vector3 pos, Quaternion rot)
        {
            if (!poolDictionary.TryGetValue(id, out var queue))
            {
                Debug.LogWarning($"[ParticleManager] No hay pool '{id}'");
                return;
            }

            var obj = queue.Dequeue();
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.SetActive(true);

            if (audioClips.TryGetValue(id, out var clip))
                AudioSource.PlayClipAtPoint(clip, pos);

            var ps = obj.GetComponent<ParticleSystem>();
            ps?.Play();

            StartCoroutine(VolverAlPool(id, obj, ObtenerDuracion(ps)));
        }

        private IEnumerator VolverAlPool(string id, GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            obj.SetActive(false);
            poolDictionary[id].Enqueue(obj);
        }

        private float ObtenerDuracion(ParticleSystem ps)
        {
            if (ps == null) return 2f;
            var m = ps.main;
            return m.duration + m.startLifetime.constantMax;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var ids = new HashSet<string>();
            foreach (var p in particlePools)
            {
                if (string.IsNullOrEmpty(p.nombreID))
                    Debug.LogWarning("[PM] nombreID vacío");
                if (p.prefab == null)
                    Debug.LogWarning($"[PM] Prefab nulo en '{p.nombreID}'");
                if (!ids.Add(p.nombreID))
                    Debug.LogWarning($"[PM] nombreID duplicado: '{p.nombreID}'");
            }
        }
#endif
    }
}
