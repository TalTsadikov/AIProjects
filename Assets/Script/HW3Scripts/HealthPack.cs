using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int minHealthAmount = 15; // Minimum amount of health restored
    public int maxHealthAmount = 30; // Maximum amount of health restored

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            var character = other.GetComponent<Character>();
            if (character != null && character.currentHealth < 100)
            {
                int healthAmount = Random.Range(minHealthAmount, maxHealthAmount);
                character.AddHealth(healthAmount);
                Debug.Log($"{other.name} picked up a health pack and restored {healthAmount} health!");
                Destroy(gameObject);
            }
        }
    }
}
