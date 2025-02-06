using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public GameObject dragonPrefab;
    public int populationSize = 20;
    public Transform target;
    public Vector2 spawnArea = new Vector2(20f, 20f);
    public int generations = 50;
    public float mutationRate = 0.1f;

    private List<DragonController> population = new List<DragonController>();
    private const string SavePath = "neural_networks.txt";

    private void Start()
    {
        StartCoroutine(EvolutionRoutine());
    }

    private IEnumerator EvolutionRoutine()
    {
        for (int generation = 0; generation < generations; generation++)
        {
            Debug.Log($"Generation {generation + 1}/{generations}");
            yield return SpawnDragons();
            yield return SimulateGeneration();
            EvaluateFitness();
            SaveTopNetworks();
            EvolvePopulation();
            CleanupPopulation();
        }

        Debug.Log("Evolution complete. Loading top networks in new environment...");
        LoadAndDemonstrateTopNetworks();
    }

    private IEnumerator SpawnDragons()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                Random.Range(5f, 10f),
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2)
            );

            GameObject dragon = Instantiate(dragonPrefab, spawnPosition, Quaternion.identity);
            DragonController dragonController = dragon.GetComponent<DragonController>();

            dragonController.network = new NeuralNetwork();
            dragonController.target = target;

            population.Add(dragonController);

            yield return null;
        }
    }

    private IEnumerator SimulateGeneration()
    {
        yield return new WaitForSeconds(20.0f);
    }

    private void EvaluateFitness()
    {
        foreach (DragonController dragon in population)
        {
            float distanceToTarget = Vector3.Distance(dragon.transform.position, target.position);
            dragon.network.fitness = 1.0f / (distanceToTarget + 1);
            Debug.Log($"Dragon Fitness: {dragon.network.fitness}");
        }
    }

    private void SaveTopNetworks()
    {
        population.Sort((a, b) => b.network.fitness.CompareTo(a.network.fitness));
        NeuralNetwork[] topNetworks = population.Take(10).Select(d => d.network).ToArray();
        NeuralNetwork.SaveNetworks(SavePath, topNetworks);
        Debug.Log("Top networks saved.");
    }

    private void LoadAndDemonstrateTopNetworks()
    {
        NeuralNetwork[] topNetworks = NeuralNetwork.LoadNetworks(SavePath);
        foreach (var network in topNetworks)
        {
            Debug.Log("Loaded network with weights: " + string.Join(",", network.weights));
        }
    }

    private void EvolvePopulation()
    {
        population.Sort((a, b) => b.network.fitness.CompareTo(a.network.fitness));

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

    private void CleanupPopulation()
    {
        foreach (DragonController dragon in population)
        {
            Destroy(dragon.gameObject);
        }

        population.Clear();
    }
}