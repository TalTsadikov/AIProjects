using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public int populationSize = 10;
    public float mutationRate = 0.1f;
    public int generations = 50;

    public GameObject characterPrefab; // Assign a prefab for agents in the scene
    public Vector3 spawnAreaSize; // Define the area where agents will spawn

    private List<CharacterParameters> population = new List<CharacterParameters>();

    private void Start()
    {
        InitializePopulation();
        EvaluateFitness();
    }

    private void Update()
    {
        // Example: Evolve population every time the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EvolvePopulation();
            SpawnCharacters();
            EvaluateFitness();
        }
    }

    public void InitializePopulation()
    {
        population.Clear();

        for (int i = 0; i < populationSize; i++)
        {
            CharacterParameters parameters = new CharacterParameters
            {
                dangerThreshold = Random.Range(10f, 50f),
                safeThreshold = Random.Range(50f, 100f),
                decisionThreshold = Random.Range(0.1f, 1f)
            };
            population.Add(parameters);
        }
    }

    public void EvaluateFitness()
    {
        foreach (var parameters in population)
        {
            // Placeholder: Replace with actual fitness evaluation logic
            parameters.fitness = Random.Range(0, 100);
        }

        // Sort population by fitness in descending order
        population.Sort((a, b) => b.fitness.CompareTo(a.fitness));
    }

    public void EvolvePopulation()
    {
        List<CharacterParameters> newPopulation = new List<CharacterParameters>();

        for (int i = 0; i < populationSize; i += 2)
        {
            CharacterParameters parent1 = population[i];
            CharacterParameters parent2 = population[i + 1];

            CharacterParameters child = Crossover(parent1, parent2);
            Mutate(child);
            newPopulation.Add(child);
        }

        population = newPopulation;
    }

    private CharacterParameters Crossover(CharacterParameters parent1, CharacterParameters parent2)
    {
        return new CharacterParameters
        {
            dangerThreshold = (parent1.dangerThreshold + parent2.dangerThreshold) / 2,
            safeThreshold = (parent1.safeThreshold + parent2.safeThreshold) / 2,
            decisionThreshold = (parent1.decisionThreshold + parent2.decisionThreshold) / 2
        };
    }

    private void Mutate(CharacterParameters child)
    {
        if (Random.value < mutationRate)
        {
            child.dangerThreshold += Random.Range(-5f, 5f);
            child.safeThreshold += Random.Range(-5f, 5f);
            child.decisionThreshold += Random.Range(-0.1f, 0.1f);
        }
    }

    public void SpawnCharacters()
    {
        // Remove all previous characters in the scene
        foreach (var character in FindObjectsOfType<Character>())
        {
            Destroy(character.gameObject);
        }

        // Spawn new characters using the population parameters
        foreach (var parameters in population)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            GameObject characterObj = Instantiate(characterPrefab, randomPosition, Quaternion.identity);
            Character character = characterObj.GetComponent<Character>();

            // You can store or use parameters for each character here
            // Example: Assign dangerThreshold or other parameters if needed
        }
    }
}