using System.Collections;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public NeuralNetwork network;
    public Transform target;
    public float speed = 10f;
    public float obstacleAvoidanceDistance = 10f;
    public float maxAltitude = 50f;
    public float minAltitude = 5f;
    public float maxVelocity = 15f;
    public float maxAngularVelocity = 5f;
    public float angularDrag = 2f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDrag = angularDrag;

        if (network == null)
        {
            network = new NeuralNetwork();
            Debug.LogWarning("Neural network was not assigned, initializing a new one.");
        }

        if (target == null)
        {
            Debug.LogError("Target is not assigned.");
        }
    }

    private void FixedUpdate()
    {
        FlyTowardsTarget();
        HandleObstacleAvoidance();
        HandleAltitudeAdjustment();
        ClampVelocity();
    }

    private void FlyTowardsTarget()
    {
        if (network == null || target == null)
        {
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        float[] inputs = new float[]
        {
            transform.position.x, transform.position.y, transform.position.z,
            rb.velocity.x, rb.velocity.y, rb.velocity.z,
            target.position.x, target.position.y
        };

        float[] output = network.FeedForward(inputs);

        float leftWingFlap = Mathf.Clamp(output[0], -1f, 1f);
        float rightWingFlap = Mathf.Clamp(output[1], -1f, 1f);
        float horizontalTail = Mathf.Clamp(output[2], -1f, 1f);
        float verticalTail = Mathf.Clamp(output[3], -1f, 1f);

        rb.AddForce(Vector3.up * (leftWingFlap + rightWingFlap) * 10);
        rb.AddTorque(Vector3.right * verticalTail);
        rb.AddTorque(Vector3.up * horizontalTail);
    }

    private void HandleObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);

            Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
            Vector3 newDirection = Vector3.Lerp(rb.velocity.normalized, avoidDirection, 0.1f);
            rb.velocity = newDirection * speed;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * obstacleAvoidanceDistance, Color.green);
        }
    }

    private void HandleAltitudeAdjustment()
    {
        if (transform.position.y > maxAltitude)
        {
            rb.velocity += Vector3.down * speed * 0.5f;
        }
        else if (transform.position.y < minAltitude)
        {
            rb.velocity += Vector3.up * speed * 0.5f;
        }
    }

    private void ClampVelocity()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAngularVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }
}