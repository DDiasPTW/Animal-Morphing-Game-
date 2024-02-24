using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Other")]
    public bool canEndLevel = false;
    public bool levelFinished = false;
    private Vector3 playerStartPosition;
    private GameObject player;


    [Header("Visuals")]
    private bool readyForNextLevel = false;
    private TMP_Text timerText;
    private TMP_Text gradeText;
    private TMP_Text finalTimeText;
    private bool timerRunning = true;
    [SerializeField] private List<float> gradeTimes = new List<float>();
    [SerializeField] private float finalTime;
    [SerializeField] private float time;
    public Player playerControls;
    private GameObject nextLevelTransition;
    private string inTransition = "Scene_In";
    private string outTransition = "Scene_Out";

    void Awake()
    {
        time = 0f;

        //get the player reference
        player = GameObject.FindGameObjectWithTag("Player");
        playerStartPosition = player.transform.position;

        //timer reference
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        levelFinished = false;
        gradeText = GameObject.FindGameObjectWithTag("Grade").GetComponent<TMP_Text>();
        finalTimeText = GameObject.FindGameObjectWithTag("FinalTime").GetComponent<TMP_Text>();

        //next level transition
        nextLevelTransition = GameObject.FindGameObjectWithTag("Trans");

        //
        playerControls = new Player();
        playerControls.Gameplay.Jump.performed += ctx =>
        {
            if (levelFinished && readyForNextLevel)
            {
                LoadNextLevel();
            }
        };
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
    }


    void Update()
    {
        //----REMOVE IN FINAL BUILD----------------------
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            levelFinished = true;
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        //----------------------------------------------------------


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
        GetGrade();

        // Indicate the game is ready for a next level jump action
        readyForNextLevel = true;

        nextLevelTransition.GetComponent<Animator>().Play(outTransition);
    }
    private void GetGrade()
    {
        // Round finalTime to 1 decimal place to align with player's view
        double roundedFinalTime = Math.Round(finalTime, 3, MidpointRounding.AwayFromZero);


        finalTimeText.text = roundedFinalTime.ToString() + " s";

        if(roundedFinalTime <= gradeTimes[0]){
            gradeText.text = "6 stars"; //NEED TO CHANGE THIS
        }   
        else if (roundedFinalTime <= gradeTimes[1])
        {
            gradeText.text = "5 stars"; //NEED TO CHANGE THIS

        }
        else if (roundedFinalTime <= gradeTimes[2])
        {
            gradeText.text = "4 stars"; //NEED TO CHANGE THIS
        }
        else if (roundedFinalTime <= gradeTimes[3])
        {
            gradeText.text = "3 stars"; //NEED TO CHANGE THIS
        }
        else if (roundedFinalTime <= gradeTimes[4])
        {
            gradeText.text = "2 stars"; //NEED TO CHANGE THIS
        }
        else if (roundedFinalTime <= gradeTimes[5])
        {
            gradeText.text = "1 star"; //NEED TO CHANGE THIS
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
