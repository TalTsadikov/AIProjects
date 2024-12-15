using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    public GameObject healthPackPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnHealthPack), spawnInterval, spawnInterval);
    }

    private void SpawnHealthPack()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(healthPackPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
    }
}
