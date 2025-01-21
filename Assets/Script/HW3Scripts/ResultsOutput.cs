using System.Collections.Generic;
using System.IO; // For writing logs
using TMPro;
using UnityEngine;

public class ResultsOutput : MonoBehaviour
{
    private FitnessFunction fitnessFunction;

    [SerializeField] private TextMeshProUGUI resultsText; // UI Text element to display results on the canvas
    [SerializeField] private string logFilePath = "GeneticAlgorithmResults.log";

    private int generationCount = 0; // Track the generation number
    private GeneticAlgorithm geneticAlgorithm; // Reference to access CharacterRecords

    private void Start()
    {
        fitnessFunction = GetComponent<FitnessFunction>();
        geneticAlgorithm = GetComponent<GeneticAlgorithm>();

        // Initialize log file
        File.WriteAllText(logFilePath, "Genetic Algorithm Results\n\n");
    }

    public void LogResultsForGeneration()
    {
        generationCount++;

        string generationLog = $"Generation {generationCount} Results:\n";
        foreach (var record in geneticAlgorithm.characterRecords) // Access CharacterRecords
        {
            float fitness = fitnessFunction.CalculateFitness(record.SurvivalTime, record.Health);
            generationLog += $"Character: Fitness = {fitness:F2}, Survival Time = {record.SurvivalTime:F2}s, Health = {record.Health:F2}\n";
        }

        // Write to log file
        File.AppendAllText(logFilePath, generationLog + "\n");

        // Display on canvas
        if (resultsText != null)
        {
            resultsText.text = generationLog;
        }

        Debug.Log(generationLog); // Optional, for debugging in the console
    }
}
