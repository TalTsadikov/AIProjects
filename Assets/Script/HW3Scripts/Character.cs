using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{

    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 5f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}
