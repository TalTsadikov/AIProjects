using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public float healAmount = 30f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character player = other.GetComponent<Character>();
            if (player != null && player.health < player.maxHealth)
            {
                player.Heal(healAmount);
                Destroy(gameObject);  // Destroy health pack after use.
            }
        }
    }
}
