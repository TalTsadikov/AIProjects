using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float moveSpeed = 5f;

    public GameObject projectilePrefab; // Projectile to shoot
    public Transform shootingPoint; // Where the projectile spawns
    public float projectileSpeed = 15f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent unwanted rotation
    }

    private void Update()
    {
        MovePlayer();
        RotatePlayerToMouse();

        if (Input.GetMouseButtonDown(0)) // Left mouse button to shoot
        {
            ShootProjectile();
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void RotatePlayerToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 direction = (point - transform.position).normalized;
            direction.y = 0; // Keep the rotation only on the XZ plane
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && shootingPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = shootingPoint.forward * projectileSpeed; // Launch the projectile forward
            }

            // Initialize the projectile and assign the shooter (player)
            Proj proj = projectile.GetComponent<Proj>();
            if (proj != null)
            {
                proj.Initialize(gameObject); // Pass the player as the shooter
            }
        }
    }
}