using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    public GameObject healthPackPrefab;
    public Vector3 spawnAreaSize;
    public float spawnInterval = 5f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnHealthPack), spawnInterval, spawnInterval);
    }

    private void SpawnHealthPack()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2));

        Instantiate(healthPackPrefab, randomPosition, Quaternion.identity);
    }
}