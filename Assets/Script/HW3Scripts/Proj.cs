using UnityEngine;

public class Proj : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 5f;

    private GameObject shooter;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(GameObject shooter)
    {
        this.shooter = shooter;
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null && other.gameObject != shooter) // Avoid self-damage
        {
            character.TakeDamage(damage);
            Destroy(gameObject); // Destroy projectile on hit
        }
    }
}
