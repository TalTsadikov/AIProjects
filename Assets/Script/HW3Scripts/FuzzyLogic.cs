using UnityEngine;

public class FuzzyLogic
{
    private float dangerThreshold;
    private float safeThreshold;
    private float detectionRange;

    public void UpdateThresholds(float danger, float range, float safe)
    {
        dangerThreshold = danger;
        detectionRange = range;
        safeThreshold = safe;
    }

    public string DecideAction(float distance, float detectionRange,float healthPercentage)
    {
        if (healthPercentage <= 0.3f) // Low health
        {
            return "Search Health";
        }

        if (distance < detectionRange && healthPercentage > 0.3f) // Close to the player OR healthy enough
        {
            return "Attack";
        }

        if (distance > safeThreshold) // Player is far away
        {
            return "Patrol";
        }

        return "Hold Position"; // Default behavior if no other condition is met
    }
}
