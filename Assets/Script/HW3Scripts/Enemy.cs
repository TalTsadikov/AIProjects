using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackDamage = 20f;
    public Transform firePoint; // The position from where projectiles will spawn
    public GameObject projectilePrefab; // Assign this in the prefab
    public float projectileSpeed = 15f; // Match player projectile speed
    public List<Transform> patrolWaypoints; // Set patrol waypoints in the scene
    private int currentWaypointIndex = 0;

    public float patrolSpeed = 2f;
    private NavMeshAgent navMeshAgent;

    private GameObject targetPlayer;
    private FuzzyLogic fuzzyLogic;
    private CharacterParameters parameters;
    private float attackCooldown = 1.5f; // Cooldown between attacks
    private float attackTimer = 0f;

    private void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent != null)
        {
            navMeshAgent.speed = patrolSpeed; // Set patrol speed
        }
    }

    public void InitializeFuzzyLogic(CharacterParameters characterParameters)
    {
        parameters = characterParameters;
        fuzzyLogic = new FuzzyLogic();
        fuzzyLogic.UpdateThresholds(
            parameters.dangerThreshold / 100f,
            detectionRange,
            parameters.safeThreshold / 100f
        );
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            float healthPercentage = GetComponent<Character>().currentHealth / GetComponent<Character>().maxHealth;
            string decision = fuzzyLogic.DecideAction(distanceToPlayer, detectionRange, healthPercentage);

            if (decision == "Attack" && attackTimer >= attackCooldown)
            {
                AttackPlayer();
            }
            else if (decision == "Search Health")
            {
                SearchForHealthPack();
            }
            else if (decision == "Patrol")
            {
                Patrol();
            }
        }
    }

    private void Patrol()
    {
        if (patrolWaypoints == null || patrolWaypoints.Count == 0 || navMeshAgent == null) return;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
            navMeshAgent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        }

        navMeshAgent.isStopped = false; // Ensure the enemy moves during patrol
    }

    private void AttackPlayer()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true; // Stop movement when attacking
        }

        Debug.Log($"{this.name} attacking the player!");

        // Rotate instantly to face the player
        Vector3 lookDirection = targetPlayer.transform.position;
        lookDirection.y = transform.position.y; // Keep rotation flat on the Y-axis
        transform.LookAt(lookDirection);

        ShootProjectile();
        attackTimer = 0f; // Reset attack cooldown timer
    }

    private void SearchForHealthPack()
    {
        Debug.Log($"{this.name} searching for health pack!");

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false; // Allow movement when searching for health packs

            // Find the closest health pack and move toward it
            GameObject[] healthPacks = GameObject.FindGameObjectsWithTag("HealthPack");
            GameObject closestHealthPack = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject healthPack in healthPacks)
            {
                float distance = Vector3.Distance(transform.position, healthPack.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHealthPack = healthPack;
                }
            }

            if (closestHealthPack != null)
            {
                navMeshAgent.SetDestination(closestHealthPack.transform.position);
            }
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * projectileSpeed;
            }

            Proj proj = projectile.GetComponent<Proj>();
            if (proj != null)
            {
                proj.Initialize(gameObject);
            }
        }
    }
}
