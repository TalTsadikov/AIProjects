using System;


public class NeuralNetwork
{
    public float[] weights;
    public float fitness;

    public NeuralNetwork()
    {
        // Initialize weights randomly (size adjusted to match inputs and outputs)
        weights = new float[8];  // Assume 8 weights corresponding to 8 inputs
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        // Ensure the input size matches expected size
        if (inputs.Length != 8)
        {
            throw new IndexOutOfRangeException("Input size must match the number of neurons in the input layer.");
        }

        float[] output = new float[4];  // 4 outputs: 2 for wings, 2 for tail
        output[0] = weights[0] * inputs[0] + weights[1] * inputs[1];  // Left wing flap
        output[1] = weights[2] * inputs[2] + weights[3] * inputs[3];  // Right wing flap
        output[2] = weights[4] * inputs[4] + weights[5] * inputs[5];  // Horizontal tail
        output[3] = weights[6] * inputs[6] + weights[7] * inputs[7];  // Vertical tail

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
            if (UnityEngine.Random.value < 0.1f)  // 10% chance to mutate each weight
            {
                weights[i] += UnityEngine.Random.Range(-0.5f, 0.5f);
            }
        }
    }
}