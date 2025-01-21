using UnityEngine;

public class Character : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float survivalTime { get; private set; }
    public float maxSurvivalTime = 300f;
    public bool IsAlive => currentHealth > 0;

    // Delegate and event for continuous stats updates
    public delegate void StatsUpdatedHandler(float survivalTime, float health);
    public event StatsUpdatedHandler OnStatsUpdated;

    private void Start()
    {
        currentHealth = maxHealth;
        survivalTime = 0f;
    }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        TriggerStatsUpdated();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        TriggerStatsUpdated();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        TriggerStatsUpdated(); // Ensure the final stats are updated
        Destroy(gameObject);
    }

    private void Update()
    {
        if (IsAlive)
        {
            survivalTime += Time.deltaTime;
            TriggerStatsUpdated();
        }
    }

    // Method to trigger the stats update event
    private void TriggerStatsUpdated()
    {
        OnStatsUpdated?.Invoke(survivalTime, currentHealth);
    }
}
