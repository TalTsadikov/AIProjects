using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsOutput : MonoBehaviour
{

    public TMP_Text resultText;

    public void LogGenerationResults(int generation, List<GameObject> population)
    {
        string result = $"Generation {generation + 1} Results:\n";
        foreach (GameObject agent in population)
        {
            Character character = agent.GetComponent<Character>();
            result += $"Health: {character.health}, Health Threshold: {agent.GetComponent<FuzzyLogic>().healthThreshold}\n";
        }

        Debug.Log(result);
        resultText.text = result;
    }
}
