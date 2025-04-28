using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrbsSpawner : MonoBehaviour
{
    [Header("Orbs and Spawnpoint")]
    [SerializeField] GameObject orbPrefab;
    [SerializeField] Transform[] spawnPoints;

    [Header("AudioSpawnOrb")]
    [SerializeField] SceneAudioManager audioManagerOrbSpawn;
    [SerializeField] int orbSpawnSFXIndex; 

    [SerializeField] float initialDelay = 20f;
    [SerializeField] float spawnIntervalMin = 20f;
    [SerializeField] float spawnIntervalMax = 20f;
    int maxOrbs = 12;
    int spawnedOrbs = 0;

    private void Start()
    {
        StartCoroutine(SpawnOrbsRountine());
    }

    IEnumerator SpawnOrbsRountine()
    {
        yield return new WaitForSeconds(initialDelay);

        while(spawnedOrbs < maxOrbs)
        {
            SpawnOrb();
            spawnedOrbs++;
            float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnOrb()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(orbPrefab, spawnPoint.position, Quaternion.identity);

        //Reproducir sonido
        if(audioManagerOrbSpawn != null)
        {
            audioManagerOrbSpawn.PlaySFX(orbSpawnSFXIndex);
        }
    }
}
