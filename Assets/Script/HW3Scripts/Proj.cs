using UnityEngine;

public class Proj : MonoBehaviour
{
    public float damage = 10f; // Damage dealt by the projectile
    public float lifetime = 5f; // Time before the projectile gets destroyed
    private GameObject shooter; // The entity that fired the projectile

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after its lifetime expires
    }

    public void Initialize(GameObject shooter)
    {
        this.shooter = shooter; // Set the shooter that fired this projectile
    }

    private void OnTriggerEnter(Collider coll)
    {
        // Ignore the shooter itself
        if (coll.gameObject == shooter) return;

        // Check if the collided object has a Character component
        Character character = coll.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
            //Debug.Log($"{character.name} Took {damage} damage, Their health is now: {character.Health}");
        }

        // Destroy the projectile on collision
        Destroy(gameObject);
    }
}