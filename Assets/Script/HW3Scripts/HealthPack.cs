using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public float healAmount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            character.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}