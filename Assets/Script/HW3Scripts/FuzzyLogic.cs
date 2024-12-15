using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class FuzzyLogic : MonoBehaviour
{

    public float healthThreshold = 50f;  // Health level to trigger health pack search
    public float dangerThreshold = 10f;  // Distance to trigger running away from enemies
    public float fitness { get; set; }   // Fitness for genetic algorithm

    private Character character;  // Reference to the character's health and status
    private NavMeshAgent navMeshAgent;  // For navigation
    private Transform targetEnemy;  // Closest enemy reference
    private Transform targetHealthPack;  // Closest health pack reference
    private HealthPack[] healthPacks;  // Array to store health packs in the scene
    private Enemy[] enemies;  // Array to store enemies in the scene

    void Start()
    {
        character = GetComponent<Character>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        FindSceneObjects();
    }

    void Update()
    {
        // Make decisions based on fuzzy logic during each frame
        MakeDecision();
    }

    // Fuzzy logic decision-making process
    public void MakeDecision()
    {
        if (character.health < healthThreshold && IsEnemyNear())
        {
            // Prioritize finding a health pack if health is low and enemy is near
            FindHealthPack();
        }
        else if (IsEnemyNear())
        {
            // Run away if enemies are near but health isn't low
            RunAway();
        }
        else
        {
            // If no enemy is near, attack the nearest enemy
            AttackEnemy();
        }
    }

    // Check if enemies are near using proximity (dangerThreshold)
    private bool IsEnemyNear()
    {
        // Find nearest enemy and check distance
        targetEnemy = FindNearestEnemy();
        if (targetEnemy == null) return false;

        float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);
        return distanceToEnemy <= dangerThreshold;
    }

    // Logic to find the nearest health pack and move towards it
    private void FindHealthPack()
    {
        targetHealthPack = FindNearestHealthPack();

        if (targetHealthPack != null)
        {
            navMeshAgent.SetDestination(targetHealthPack.position);
            Debug.Log("Searching for Health Pack...");
        }
        else
        {
            // If no health pack is available, run away
            RunAway();
        }
    }

    // Logic to run away from the nearest enemy
    private void RunAway()
    {
        if (targetEnemy == null) return;

        // Calculate a direction away from the enemy
        Vector3 directionAwayFromEnemy = transform.position - targetEnemy.position;
        Vector3 runToPosition = transform.position + directionAwayFromEnemy.normalized * dangerThreshold;

        navMeshAgent.SetDestination(runToPosition);
        Debug.Log("Running Away from Enemy!");
    }

    // Logic to attack the nearest enemy
    private void AttackEnemy()
    {
        if (targetEnemy == null) return;

        navMeshAgent.SetDestination(targetEnemy.position);
        Debug.Log("Attacking Enemy!");
        // Here you can add attack logic, like reducing enemy health, etc.
    }

    // Find the nearest enemy in the scene
    private Transform FindNearestEnemy()
    {
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    // Find the nearest health pack in the scene
    private Transform FindNearestHealthPack()
    {
        Transform nearestHealthPack = null;
        float closestDistance = Mathf.Infinity;

        foreach (HealthPack healthPack in healthPacks)
        {
            float distanceToHealthPack = Vector3.Distance(transform.position, healthPack.transform.position);
            if (distanceToHealthPack < closestDistance)
            {
                closestDistance = distanceToHealthPack;
                nearestHealthPack = healthPack.transform;
            }
        }

        return nearestHealthPack;
    }

    // Populate arrays with health packs and enemies in the scene
    private void FindSceneObjects()
    {
        // Locate all health packs and enemies at the start
        healthPacks = FindObjectsOfType<HealthPack>();
        enemies = FindObjectsOfType<Enemy>();
    }
}

