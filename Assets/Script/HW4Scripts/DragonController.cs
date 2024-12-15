using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DragonController : MonoBehaviour
{
    public NeuralNetwork network;  // The neural network controlling the dragon
    public Transform target;       // The target the dragon will fly towards
    public float speed = 10f;
    public float obstacleAvoidanceDistance = 10f;  // Distance to start avoiding obstacles
    public float maxAltitude = 50f;                // Maximum allowed altitude before descending
    public float minAltitude = 5f;                 // Minimum allowed altitude before ascending

    private Rigidbody rb;
    private List<NeuralNetwork> topNetworks = new List<NeuralNetwork>();  // To store top 10 networks

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure the dragon spawns at world position (0, 0, 0)
        transform.position = Vector3.zero;

        // Initialize the neural network if not assigned
        if (network == null)
        {
            network = new NeuralNetwork();
            Debug.LogWarning("Neural network was not assigned, initializing a new one.");
        }

        // Validate if the target is assigned
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
    }

    public void FlyTowardsTarget()
    {
        if (network == null || target == null)
        {
            return; 
        }

        // Move the dragon towards the target
        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        float[] inputs = new float[]
        {
            transform.position.x, transform.position.y, transform.position.z,
            rb.velocity.x, rb.velocity.y, rb.velocity.z,
            target.position.x, target.position.y
        };

        // Get network output (assuming 8 inputs and 4 outputs)
        float[] output = network.FeedForward(inputs);

        // Simulate wing flaps and tail movements based on neural network output
        float leftWingFlap = output[0];
        float rightWingFlap = output[1];
        float horizontalTail = output[2];
        float verticalTail = output[3];

        // Apply forces and torques based on network output
        rb.AddForce(Vector3.up * (leftWingFlap + rightWingFlap) * 10);  // Wing flap control
        rb.AddTorque(Vector3.right * verticalTail);  // Pitch control
        rb.AddTorque(Vector3.up * horizontalTail);   // Yaw control
    }


    private void HandleObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance))
        {
            // If obstacle detected, steer around it
            Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up);  // Simple avoidance by changing direction
            rb.velocity = avoidDirection * speed;
            Debug.Log("Avoiding obstacle.");
        }
    }

    private void HandleAltitudeAdjustment()
    {
        if (transform.position.y > maxAltitude)
        {
            rb.velocity += Vector3.down * speed * 0.5f;  // Fly down if too high
            Debug.Log("Descending due to high altitude.");
        }
        else if (transform.position.y < minAltitude)
        {
            rb.velocity += Vector3.up * speed * 0.5f;  // Fly up if too low
            Debug.Log("Ascending due to low altitude.");
        }
    }

    public void SaveTopNetworks(string path)
    {
        EnsureSaveDirectoryExists();

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, topNetworks);  
        }

        Debug.Log("Top neural networks saved to " + path);
    }


    public void LoadTopNetworks(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                topNetworks = (List<NeuralNetwork>)formatter.Deserialize(stream);
            }

            Debug.Log("Top neural networks loaded from " + path);
        }
        else
        {
            Debug.LogError("No saved neural networks found at " + path);
        }
    }

    public void ConfigureEvolutionSettings(int populationSize, float mutationRate, int generations)
    {
        
        Debug.Log($"Configuring evolution with Population Size: {populationSize}, Mutation Rate: {mutationRate}, Generations: {generations}");
     
    }

    private void EnsureSaveDirectoryExists()
    {
        string savePath = "Assets/SaveData/";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }
}
