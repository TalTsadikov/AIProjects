using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GeneticAlgorithm : MonoBehaviour
{
    public List<FuzzyLogic> population;
    public int populationSize = 10;
    public int generations = 50;
    public float mutationRate = 0.05f;
    public float crossoverRate = 0.7f;

    private void Start()
    {
        InitializePopulation();
        for (int i = 0; i < generations; i++)
        {
            EvaluatePopulation();
            SelectAndBreed();
            MutatePopulation();
        }
    }

    private void InitializePopulation()
    {
        population = new List<FuzzyLogic>();
        for (int i = 0; i < populationSize; i++)
        {
            FuzzyLogic individual = Instantiate(Resources.Load<FuzzyLogic>("FuzzyLogicPrefab"));
            population.Add(individual);
        }
    }

    private void EvaluatePopulation()
    {
        foreach (FuzzyLogic agent in population)
        {
            float fitness = FitnessFunction.EvaluateFitness(agent);  // Reference the static method in FitnessFunction
            agent.fitness = fitness;  // Set the fitness value
        }
    }

    private void SelectAndBreed()
    {
        List<FuzzyLogic> newPopulation = new List<FuzzyLogic>();

        for (int i = 0; i < populationSize; i++)
        {
            FuzzyLogic parent1 = SelectParent();
            FuzzyLogic parent2 = SelectParent();

            FuzzyLogic offspring;
            if (Random.value < crossoverRate)
            {
                offspring = Crossover(parent1, parent2);
            }
            else
            {
                offspring = Instantiate(parent1);
            }

            newPopulation.Add(offspring);
        }

        population = newPopulation;
    }

    private FuzzyLogic SelectParent()
    {
        float totalFitness = 0;
        foreach (var individual in population)
        {
            totalFitness += individual.fitness;
        }

        float randomValue = Random.value * totalFitness;
        float cumulativeFitness = 0;
        foreach (var individual in population)
        {
            cumulativeFitness += individual.fitness;
            if (cumulativeFitness >= randomValue)
            {
                return individual;
            }
        }
        return population[0];
    }

    private FuzzyLogic Crossover(FuzzyLogic parent1, FuzzyLogic parent2)
    {
        FuzzyLogic offspring = Instantiate(parent1);
        offspring.healthThreshold = (parent1.healthThreshold + parent2.healthThreshold) / 2;
        offspring.dangerThreshold = (parent1.dangerThreshold + parent2.dangerThreshold) / 2;
        return offspring;
    }

    private void MutatePopulation()
    {
        foreach (var individual in population)
        {
            if (Random.value < mutationRate)
            {
                individual.healthThreshold += Random.Range(-10f, 10f);
                individual.dangerThreshold += Random.Range(-5f, 5f);
            }
        }
    }
}

