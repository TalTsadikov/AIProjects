using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;
    public float damage = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime); // Destroy projectile after its lifetime
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hits the enemy
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Debug.Log("Projectile hit: " + other.gameObject.name);
        Destroy(gameObject); // Destroy projectile on collision
    }
}
