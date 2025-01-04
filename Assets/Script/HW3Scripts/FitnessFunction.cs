using UnityEngine;

public class FitnessFunction : MonoBehaviour
{
    public float CalculateFitness(Character character)
    {
        float healthFactor = character.Health / 100.0f; // Normalized health
        float survivalBonus = character.IsAlive ? 1.0f : 0.0f;

        return healthFactor * 0.7f + survivalBonus * 0.3f; // Weighted fitness calculation
    }
}