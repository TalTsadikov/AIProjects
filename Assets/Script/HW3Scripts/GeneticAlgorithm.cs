using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject characterPrefab;
    public List<Transform> spawnPoints;
    public int populationSize = 20;
    public float mutationRate = 0.1f;
    public int maxGenerations = 100;

    private List<CharacterParameters> population = new List<CharacterParameters>();
    private List<Transform> patrolWaypoints;
    private FitnessFunction fitnessFunction;
    private ResultsOutput resultsOutput;

    // Class to store character records
    public class CharacterRecord
    {
        public Character Character; // Reference to the Character instance
        public CharacterParameters Parameters;
        public float SurvivalTime;
        public float Health;

        public CharacterRecord(Character character, CharacterParameters parameters)
        {
            Character = character;
            Parameters = parameters;
            SurvivalTime = 0f;
            Health = character.currentHealth;
        }
    }

    public List<CharacterRecord> characterRecords = new List<CharacterRecord>();

    private void Start()
    {
        resultsOutput = GetComponent<ResultsOutput>();
        fitnessFunction = GetComponent<FitnessFunction>();
        InitializePopulation();
        patrolWaypoints = new List<Transform>();

        foreach (var patrolPoint in GameObject.FindGameObjectsWithTag("Patrol"))
        {
            patrolWaypoints.Add(patrolPoint.transform);
        }

        SpawnCharacters();
        InvokeRepeating("RunGeneration", 30f, 30f);
    }

    private void InitializePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            CharacterParameters parameters = new CharacterParameters
            {
                dangerThreshold = Random.Range(10f, 50f),
                safeThreshold = Random.Range(50f, 100f),
                decisionThreshold = Random.Range(0.3f, 0.7f)
            };

            population.Add(parameters);
        }
    }

    private void SpawnCharacters()
    {
        characterRecords.Clear(); // Clear records for the new generation

        foreach (var parameters in population)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject characterObj = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);

            Character character = characterObj.GetComponent<Character>();
            if (character != null)
            {
                // Create and store a new CharacterRecord
                CharacterRecord record = new CharacterRecord(character, parameters);
                characterRecords.Add(record);

                // Attach a listener to update the record continuously
                character.OnStatsUpdated += (survivalTime, health) =>
                {
                    record.SurvivalTime = survivalTime;
                    record.Health = health;
                };
            }

            Enemy enemy = characterObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.InitializeFuzzyLogic(parameters);
                enemy.patrolWaypoints = patrolWaypoints;
            }
        }
    }

    private void RunGeneration()
    {
        List<float> fitnessScores = new List<float>();

        // Use CharacterRecord data for fitness calculation
        foreach (var record in characterRecords)
        {
            float fitness = fitnessFunction.CalculateFitness(record.SurvivalTime, record.Health);
            fitnessScores.Add(fitness);
        }

        List<CharacterParameters> nextGeneration = GenerateNextGeneration(fitnessScores);
        population = nextGeneration;

        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }

        resultsOutput.LogResultsForGeneration();
        SpawnCharacters();
    }

    private List<CharacterParameters> GenerateNextGeneration(List<float> fitnessScores)
    {
        List<CharacterParameters> nextGeneration = new List<CharacterParameters>();
        float totalFitness = 0f;

        foreach (float score in fitnessScores)
        {
            totalFitness += score;
        }

        for (int i = 0; i < populationSize; i++)
        {
            CharacterParameters parent1 = SelectParent(fitnessScores, totalFitness);
            CharacterParameters parent2 = SelectParent(fitnessScores, totalFitness);

            CharacterParameters offspring = Crossover(parent1, parent2);
            Mutate(offspring);
            nextGeneration.Add(offspring);
        }

        return nextGeneration;
    }

    private CharacterParameters SelectParent(List<float> fitnessScores, float totalFitness)
    {
        float randomValue = Random.value * totalFitness;
        float cumulativeFitness = 0f;

        for (int i = 0; i < fitnessScores.Count; i++)
        {
            cumulativeFitness += fitnessScores[i];
            if (cumulativeFitness >= randomValue)
            {
                return population[i];
            }
        }

        return population[population.Count - 1];
    }

    private CharacterParameters Crossover(CharacterParameters parent1, CharacterParameters parent2)
    {
        return new CharacterParameters
        {
            dangerThreshold = Random.value < 0.5f ? parent1.dangerThreshold : parent2.dangerThreshold,
            safeThreshold = Random.value < 0.5f ? parent1.safeThreshold : parent2.safeThreshold,
            decisionThreshold = Random.value < 0.5f ? parent1.decisionThreshold : parent2.decisionThreshold
        };
    }

    private void Mutate(CharacterParameters parameters)
    {
        if (Random.value < mutationRate)
        {
            parameters.dangerThreshold += Random.Range(-5f, 5f);
            parameters.safeThreshold += Random.Range(-5f, 5f);
            parameters.decisionThreshold += Random.Range(-0.05f, 0.05f);
        }
    }
}
