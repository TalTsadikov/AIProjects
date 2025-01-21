using UnityEngine;

public class FitnessFunction : MonoBehaviour
{
    public float CalculateFitness(float survivalTime, float health)
    {
        float survivalTimeFactor = survivalTime / 30f; // Normalize by max survival time
        float healthFactor = Mathf.Clamp01(health / 100f); // Normalize by max health

        // Adjust weights if necessary
        return (survivalTimeFactor * 0.7f) + (healthFactor * 0.3f);
    }
}
