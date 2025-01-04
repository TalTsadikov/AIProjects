using UnityEngine;

public class Character : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float Health;
    public bool IsAlive => Health > 0;

    private void Start()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth;
    }
}