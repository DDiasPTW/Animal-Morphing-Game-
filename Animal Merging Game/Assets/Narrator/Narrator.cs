using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Narrator : MonoBehaviour
{
    [Header("Other properties")]
    public static Narrator Instance { get; private set; }
    [SerializeField] private TMP_Text narratorText;
    private Coroutine currentCoroutine;

    [Header("Lines")]
    [SerializeField] private NarratorLines[] endLevelLines;
    [SerializeField] private NarratorLines dyingLines;
    [SerializeField] private NarratorLines lavaLines;
    [SerializeField] private NarratorLines laserLines;
    [SerializeField] private NarratorLines enterCatchableLines;
    [SerializeField] private NarratorLines catchCatchableLines;
    [SerializeField] private NarratorLines endCatchableLines;


    void Awake()
    {
        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
            return;
        }

        narratorText.text = "";
    }

    private IEnumerator ClearNarratorTextAfterDelay(float delay)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(delay);

        // Clear the narrator text
        narratorText.text = "";

        currentCoroutine = null;
    }


    #region End Level
    public void TriggerEndLevelLines(int index)
    {
        if (index < 0 || index >= endLevelLines.Length)
        {
            Debug.LogError("Invalid end level line index.");
            return;
        }

        float chance = Random.Range(0, 100);
        if (chance <= endLevelLines[index].chanceOfSaying)
        {
            // Stop the current display coroutine if it's running
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }


            // Choose a random line from the list
            string randomLine = endLevelLines[index].lines[Random.Range(0, endLevelLines[index].lines.Count)];

            // Set the narrator text to the randomly chosen line
            narratorText.text = randomLine;

            // Start a new coroutine to clear the text after the specified duration
            currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(5f));
        }
        else
        {
            Debug.Log("Not saying it this time");
        }
    }

    #endregion

    #region Lose
    //Die to random objects (walls and such)
    public void TriggerDyingLines()
    {
        if (currentCoroutine == null)
        {
            float chance = Random.Range(0, 100);
            if (chance <= dyingLines.chanceOfSaying)
            {
                // Choose a random line from the list
                string randomLine = dyingLines.lines[Random.Range(0, dyingLines.lines.Count)];

                // Set the narrator text to the randomly chosen line
                narratorText.text = randomLine;

                // Start a new coroutine to clear the text after the specified duration
                currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(4f));
            }
            else Debug.Log("Not saying it this time");
        }

    }

    //Die to the lava
    public void TriggerLavaLines()
    {
        if (currentCoroutine == null) //if no other coroutine is playing
        {
            float chance = Random.Range(0, 100);
            if (chance <= lavaLines.chanceOfSaying)
            {
                // Choose a random line from the list
                string randomLine = lavaLines.lines[Random.Range(0, lavaLines.lines.Count)];

                // Set the narrator text to the randomly chosen line
                narratorText.text = randomLine;

                // Start a new coroutine to clear the text after the specified duration
                currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(4f));
            }
            else Debug.Log("Not saying it this time");
        }
    }


    //Die to the lasers
    public void TriggerLaserLines()
    {
        if (currentCoroutine == null)
        {
            float chance = Random.Range(0, 100);
            if (chance <= laserLines.chanceOfSaying)
            {

                // Choose a random line from the list
                string randomLine = laserLines.lines[Random.Range(0, laserLines.lines.Count)];

                // Set the narrator text to the randomly chosen line
                narratorText.text = randomLine;

                // Start a new coroutine to clear the text after the specified duration
                currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(4f));
            }
            else Debug.Log("Not saying it this time");
        }

    }
    #endregion

    #region Catchable
    public void TriggerEnterCatchable()
    {
        if (currentCoroutine == null)
        {
            float chance = Random.Range(0, 100);
            if (chance <= enterCatchableLines.chanceOfSaying)
            {
                // Choose a random line from the list
                string randomLine = enterCatchableLines.lines[Random.Range(0, enterCatchableLines.lines.Count)];

                // Set the narrator text to the randomly chosen line
                narratorText.text = randomLine;

                // Start a new coroutine to clear the text after the specified duration
                currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(4f));
            }
            else Debug.Log("Not saying it this time");
        }
    }

    public void TriggerCatchCatchable()
    {
        float chance = Random.Range(0, 100);
        if (chance <= catchCatchableLines.chanceOfSaying)
        {
            // Stop the current display coroutine if it's running
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            // Choose a random line from the list
            string randomLine = catchCatchableLines.lines[Random.Range(0, catchCatchableLines.lines.Count)];

            // Set the narrator text to the randomly chosen line
            narratorText.text = randomLine;

            // Start a new coroutine to clear the text after the specified duration
            currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(4f));
        }
        else Debug.Log("Not saying it this time");
    }

    public void TriggerEndCatchable()
    {
        float chance = Random.Range(0, 100);
        if (chance <= endCatchableLines.chanceOfSaying)
        {
            // Stop the current display coroutine if it's running
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            // Choose a random line from the list
            string randomLine = endCatchableLines.lines[Random.Range(0, endCatchableLines.lines.Count)];

            // Set the narrator text to the randomly chosen line
            narratorText.text = randomLine;

            // Start a new coroutine to clear the text after the specified duration
            currentCoroutine = StartCoroutine(ClearNarratorTextAfterDelay(4f));
        }
        else Debug.Log("Not saying it this time");
    }
    #endregion

}
