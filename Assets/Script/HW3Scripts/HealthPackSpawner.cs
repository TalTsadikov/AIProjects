using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    public GameObject healthPackPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;

    private int maxActiveHealthPacks = 10; // Maximum number of health packs allowed in the scene
    private int activeHealthPacks = 0;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnHealthPack), 2f, spawnInterval);
    }

    void SpawnHealthPack()
    {
        if (activeHealthPacks >= maxActiveHealthPacks)
            return;

        int index = Random.Range(0, spawnPoints.Length);
        Instantiate(healthPackPrefab, spawnPoints[index].position, Quaternion.identity);
        activeHealthPacks++;
    }

    public void OnHealthPackPickedUp()
    {
        activeHealthPacks = Mathf.Max(activeHealthPacks - 1, 0);
    }
}
