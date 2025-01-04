using System.Collections.Generic;
using UnityEngine;

public class ResultsOutput : MonoBehaviour
{
    private List<Character> characters;
    private FitnessFunction fitnessFunction;

    private void Start()
    {
        fitnessFunction = GetComponent<FitnessFunction>();
        RefreshCharacters();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Example: Press R to output results
        {
            RefreshCharacters();
            OutputResults();
        }
    }

    private void RefreshCharacters()
    {
        characters = new List<Character>(FindObjectsOfType<Character>());
    }

    private void OutputResults()
    {
        foreach (Character character in characters)
        {
            float fitness = fitnessFunction.CalculateFitness(character);
            Debug.Log($"Character {character.name} Fitness: {fitness}");
        }
    }
}