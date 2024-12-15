using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public GameObject dragonPrefab;
    public int populationSize = 20;
    public Transform target;  // The target for the dragons to fly towards
    public float spawnSpacing = 2.0f;  // Spacing to avoid dragon collision during spawn
    private List<DragonController> population = new List<DragonController>();

    private void Start()
    {
        StartCoroutine(SpawnDragons());
    }

    private IEnumerator SpawnDragons()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 spawnPosition = new Vector3(i * spawnSpacing, 0, 0);  // Spread spawn positions
            GameObject dragon = Instantiate(dragonPrefab, spawnPosition, Quaternion.identity);
            DragonController dragonController = dragon.GetComponent<DragonController>();

            // Ensure a neural network and target are assigned
            dragonController.network = new NeuralNetwork();
            dragonController.target = target;

            population.Add(dragonController);

            yield return null;  // Small delay to prevent simultaneous spawning
        }
    }

    public void EvaluateFitness()
    {
        foreach (DragonController dragon in population)
        {
            float distanceToTarget = Vector3.Distance(dragon.transform.position, dragon.target.position);
            dragon.network.fitness = 1.0f / (distanceToTarget + 1);  // Fitness increases as distance decreases
        }
    }

    public void EvolvePopulation()
    {
        population.Sort((a, b) => b.network.fitness.CompareTo(a.network.fitness));  // Sort by fitness

        List<NeuralNetwork> newGeneration = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize / 2; i++)
        {
            NeuralNetwork parentA = population[i].network;
            NeuralNetwork parentB = population[i + 1].network;
            NeuralNetwork offspring = parentA.Crossover(parentB);
            offspring.Mutate();
            newGeneration.Add(offspring);
        }

        for (int i = 0; i < populationSize / 2; i++)
        {
            population[i].network = newGeneration[i];
        }
    }
}
