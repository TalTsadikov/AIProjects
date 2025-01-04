using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject projectilePrefab; // Assign the projectile prefab in the Inspector
    public Transform shootPoint; // The point from which the projectile will be shot

    private CharacterController controller;
    private Camera mainCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        Move();
        RotatePlayerToMouse();
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Shoot();
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Use world coordinates for movement
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void RotatePlayerToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; // Keep the same height
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void Shoot()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 shootDirection = (hit.point - shootPoint.position).normalized;
            Quaternion shootRotation = Quaternion.LookRotation(shootDirection);
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootRotation);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.velocity = shootDirection * projectile.GetComponent<ProjectileHW3>().speed;
        }
    }
}
