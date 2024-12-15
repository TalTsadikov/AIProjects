using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FitnessFunction 
{
    public static float EvaluateFitness(FuzzyLogic agent)
    {
        float survivalTime = Time.time;
        float enemiesDefeated = agent.GetComponent<Character>().health;  // Could represent how well the agent survives.
        return survivalTime + enemiesDefeated;  // Fitness based on survival and success.
    }
}
