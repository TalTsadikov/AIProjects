using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NeuralNetwork
{
    public float[] weights;
    public float fitness;

    public NeuralNetwork()
    {
        weights = new float[8];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        if (inputs.Length != 8)
        {
            throw new IndexOutOfRangeException("Input size must match the number of neurons in the input layer.");
        }

        float[] output = new float[4];
        output[0] = weights[0] * inputs[0] + weights[1] * inputs[1];
        output[1] = weights[2] * inputs[2] + weights[3] * inputs[3];
        output[2] = weights[4] * inputs[4] + weights[5] * inputs[5];
        output[3] = weights[6] * inputs[6] + weights[7] * inputs[7];

        return output;
    }

    public NeuralNetwork Crossover(NeuralNetwork other)
    {
        NeuralNetwork offspring = new NeuralNetwork();
        for (int i = 0; i < weights.Length; i++)
        {
            offspring.weights[i] = UnityEngine.Random.value > 0.5f ? this.weights[i] : other.weights[i];
        }
        return offspring;
    }

    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            if (UnityEngine.Random.value < 0.1f)
            {
                weights[i] += UnityEngine.Random.Range(-0.5f, 0.5f);
            }
        }
    }

    public static void SaveNetworks(string path, NeuralNetwork[] networks)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (var network in networks)
            {
                writer.WriteLine(string.Join(",", network.weights.Select(w => w.ToString("F4"))));
            }
        }
    }

    public static NeuralNetwork[] LoadNetworks(string path)
    {
        var networks = new List<NeuralNetwork>();

        foreach (var line in File.ReadAllLines(path))
        {
            NeuralNetwork network = new NeuralNetwork();
            var weights = line.Split(',').Select(float.Parse).ToArray();

            if (weights.Length == network.weights.Length)
            {
                network.weights = weights;
                networks.Add(network);
            }
        }

        return networks.ToArray();
    }
}