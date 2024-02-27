using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Data")]
    public float currentPB = Mathf.Infinity;
    private CloudSaving cS;

    [Header("Other")]
    public bool canEndLevel = false;
    public bool levelFinished = false;
    private Vector3 playerStartPosition;
    private GameObject player;


    [Header("Visuals")]
    private bool readyForNextLevel = false;
    private TMP_Text timerText;
    private TMP_Text finalTimeText;
    private bool timerRunning = true;
    [SerializeField] private List<float> gradeTimes = new List<float>();
    [SerializeField] private float finalTime;
    [SerializeField] private float time;
    public Player playerControls;
    private GameObject nextLevelTransition;
    private string inTransition = "Scene_In";
    private string outTransition = "Scene_Out";
    [SerializeField] private GameObject[] stars;


    void Awake()
    {
        time = 0f;
        cS = GetComponent<CloudSaving>();
        //get the player reference
        player = GameObject.FindGameObjectWithTag("Player");
        playerStartPosition = player.transform.position;

        //timer reference
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        levelFinished = false;
        finalTimeText = GameObject.FindGameObjectWithTag("FinalTime").GetComponent<TMP_Text>();

        //next level transition
        nextLevelTransition = GameObject.FindGameObjectWithTag("Trans");
        GetStars();

        //--
        GetControls();
    }

    private void GetControls(){
        playerControls = new Player();
        playerControls.Gameplay.Jump.performed += ctx =>
        {
            if (levelFinished && readyForNextLevel)
            {
                LoadNextLevel();
            }
        };
        playerControls.Gameplay.Reset.performed += ctx => ResetLevel();
        playerControls.Gameplay.NextLevel.performed += ctx => NextLevel();
        playerControls.Gameplay.PreviousLevel.performed += ctx => PreviousLevel();
    }

    private void GetStars()
    {
        // Find the StarsContainer by tag or name
        GameObject starsContainer = GameObject.FindGameObjectWithTag("Stars");
        if (starsContainer != null)
        {
            // Get all children and sort them by name to ensure correct order
            stars = starsContainer.transform.Cast<Transform>().Select(tr => tr.gameObject).OrderBy(go => go.name).ToArray();
        }

        DisableStar();
    }

    private void DisableStar()
    {
        foreach (var star in stars)
        {
            star.SetActive(false);
        }
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Disable();
    }

    void Start()
    {
        nextLevelTransition.GetComponent<Animator>().Play(inTransition);
        cS.LoadData();
    }

    void Update()
    {
        UpdateTimerDisplay();

        if (!levelFinished)
        {
            UpdateTimerDisplay();
        }

        if (levelFinished && timerRunning)
        {
            FinalizeLevel();
        }
    }


//----REMOVE IN FINAL BUILD----------------------
    private void NextLevel(){
        levelFinished = true;
            LoadNextLevel();
    }
    private void PreviousLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void UpdateTimerDisplay()
    {
        if (timerRunning)
        {
            time += Time.deltaTime;
            timerText.text = time.ToString("F3") + "s";
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void FinalizeLevel()
    {
        timerRunning = false; // Stop the timer
        finalTime = time;
        if(finalTime < currentPB) 
        {
            currentPB = finalTime;
            cS.SaveData();
        }
        GetGrade();

        // Indicate the game is ready for a next level jump action
        readyForNextLevel = true;

        nextLevelTransition.GetComponent<Animator>().Play(outTransition);
    }



    private void GetGrade()
    {
        // Round finalTime to 1 decimal place to align with player's view
        double roundedFinalTime = Math.Round(finalTime, 3, MidpointRounding.AwayFromZero);
        // Use string formatting to ensure three decimal places are always shown
        finalTimeText.text = finalTime.ToString("F3") + " s";

        // Initially disable all stars
        DisableStar();

        // Determine the number of stars to activate based on the grade
        int starsToActivate = 0;
        if (roundedFinalTime <= gradeTimes[0]) starsToActivate = 6;
        else if (roundedFinalTime <= gradeTimes[1]) starsToActivate = 5;
        else if (roundedFinalTime <= gradeTimes[2]) starsToActivate = 4;
        else if (roundedFinalTime <= gradeTimes[3]) starsToActivate = 3;
        else if (roundedFinalTime <= gradeTimes[4]) starsToActivate = 2;
        else if (roundedFinalTime <= gradeTimes[5]) starsToActivate = 1;

        // Activate the appropriate number of stars
        for (int i = 0; i < starsToActivate; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        if (levelFinished)
        {
            readyForNextLevel = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}



