using UnityEngine;

[RequireComponent(typeof(Character))]
public class Enemy : MonoBehaviour
{
    private FuzzyLogic fuzzyLogic;
    private Character target; // Reference to the player (Character script)
    private Character character; // Reference to this enemy's Character script

    public float speed = 2.0f;
    public float detectionRange = 15.0f;

    public GameObject projectilePrefab; // Projectile to shoot
    public Transform shootingPoint; // Where the projectile spawns
    public float projectileSpeed = 10f;
    public float shootingCooldown = 2f;

    private float lastShotTime;

    private CharacterParameters parameters;

    private void Start()
    {
        character = GetComponent<Character>();
        if (character == null)
        {
            Debug.LogError("Enemy is missing the Character component.");
            return;
        }

        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.GetComponent<Character>();
        }

        if (target == null)
        {
            Debug.LogError("Player with 'Character' script not found. Make sure the player has the correct tag and script.");
            return;
        }

        fuzzyLogic = new FuzzyLogic();

        // Initialize parameters for this enemy
        parameters = new CharacterParameters
        {
            dangerThreshold = Random.Range(10f, 50f),
            safeThreshold = Random.Range(50f, 100f),
            decisionThreshold = Random.Range(0.3f, 0.7f)
        };

        // Pass dynamic thresholds to FuzzyLogic
        fuzzyLogic.UpdateThresholds(parameters.dangerThreshold / 100f, detectionRange, parameters.safeThreshold);
    }

    private void Update()
    {
        if (character == null || !character.IsAlive) return; // Ensure this enemy is alive
        if (target == null || !target.IsAlive) return; // Ensure the target exists and is alive

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // Normalized values for self
        float normalizedSelfHealth = Mathf.Clamp01(character.Health / character.MaxHealth);
        float normalizedDistance = Mathf.Clamp01(distanceToTarget / detectionRange);

        // Decide action using fuzzy logic
        string action = fuzzyLogic.DecideAction(normalizedSelfHealth, normalizedDistance);

        switch (action)
        {
            case "Attack":
                Attack();
                break;

            case "Retreat":
                Retreat();
                break;

            case "Idle":
                Idle();
                break;
        }
    }

    private void Attack()
    {
        Debug.Log($"{this.name} is attacking!");

        if (Time.time - lastShotTime > shootingCooldown)
        {
            ShootProjectile();
            lastShotTime = Time.time;
        }

        // Move slightly toward the target while shooting
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && shootingPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = (target.transform.position - shootingPoint.position).normalized * projectileSpeed;
            }

            // Initialize the projectile with reference to this enemy as the shooter
            Proj proj = projectile.GetComponent<Proj>();
            if (proj != null)
            {
                proj.Initialize(gameObject);
            }
        }
    }

    private void Retreat()
    {
        Debug.Log($"{this.name} is retreating!");
        Vector3 direction = transform.position - target.transform.position;
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    private void Idle()
    {
        Debug.Log($"{this.name} is idling.");
    }
}