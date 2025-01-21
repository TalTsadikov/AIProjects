using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float moveSpeed = 5f;

    public GameObject projectilePrefab; // Projectile to shoot
    public Transform shootingPoint; // Where the projectile spawns
    public float projectileSpeed = 15f;

    private Camera mainCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent unwanted rotation
        mainCamera = Camera.main;
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            Vector3 lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0; // Ignore Y-axis for rotation
            transform.rotation = Quaternion.LookRotation(lookDirection);
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
