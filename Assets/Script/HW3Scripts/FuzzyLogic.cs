using UnityEngine;

public class FuzzyLogic
{
    private float dangerThreshold;
    private float safeThreshold;
    private float decisionThreshold;

    public void UpdateThresholds(float danger, float detectionRange, float safe)
    {
        dangerThreshold = danger;
        safeThreshold = safe;
        decisionThreshold = 0.5f; // Default balanced decision threshold
    }

    public string DecideAction(float normalizedSelfHealth, float normalizedDistance)
    {
        // Calculate fuzzy values for self (health)
        float lowHealth = Mathf.Clamp01((dangerThreshold - normalizedSelfHealth) / dangerThreshold);
        float highHealth = 1.0f - lowHealth;

        // Calculate fuzzy values for distance
        float farFromTarget = Mathf.Clamp01(normalizedDistance);
        float nearToTarget = 1.0f - farFromTarget;

        // Adjust weights for actions
        float attackWeight = highHealth * nearToTarget; // Attack when strong and close
        float retreatWeight = lowHealth; // Retreat only based on low health, distance removed
        float idleWeight = (highHealth * farFromTarget); // Idle when healthy but far from target

        // Debugging for weights
        // Debug.Log($"Attack: {attackWeight}, Retreat: {retreatWeight}, Idle: {idleWeight}");

        // Determine action with the highest weight
        if (attackWeight >= retreatWeight && attackWeight >= idleWeight)
        {
            return "Attack";
        }
        else if (retreatWeight >= attackWeight && retreatWeight >= idleWeight)
        {
            return "Retreat";
        }
        else
        {
            return "Idle";
        }
    }
}