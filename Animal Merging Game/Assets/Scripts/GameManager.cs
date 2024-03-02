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
    //private CloudSaving cS;

    [Header("Other")]
    public bool canEndLevel = false;
    public bool levelFinished = false;


    [Header("Visuals")]
    private bool readyForNextLevel = false;
    private TMP_Text timerText;
    private TMP_Text finalTimeText;
    private TMP_Text pbTimeText;
    private bool timerRunning = true;
    
    public List<float> gradeTimes = new List<float>();
    [SerializeField] private float finalTime;
    [SerializeField] private float time;
    public Player playerControls;
    private GameObject nextLevelTransition;
    private string inTransition = "Scene_In";
    private string outTransition = "Scene_Out";
    [SerializeField] private GameObject[] stars;
    public int starsToActivate = 0;

    void Awake()
    {
        time = 0f;
        
        //cS = GetComponent<CloudSaving>();

        //timer reference
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        levelFinished = false;
        finalTimeText = GameObject.FindGameObjectWithTag("FinalTime").GetComponent<TMP_Text>();
        pbTimeText = GameObject.FindGameObjectWithTag("Pb_Time").GetComponent<TMP_Text>();

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
        LoadBestTime();
        //cS.LoadData();
    }

    void Update()
    {
        //UpdateTimerDisplay();

        if (!levelFinished)
        {
            UpdateTimerDisplay();
        }

        if (levelFinished && timerRunning)
        {
            FinalizeLevel();
        }

        // if(Input.GetKeyDown(KeyCode.L)){
        //     PlayerPrefs.DeleteAll();
        // }
    }


//----REMOVE IN FINAL BUILD----------------------
    private void NextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
            SaveBestTime();
        }
        GetGrade();

        // Indicate the game is ready for a next level jump action
        readyForNextLevel = true;

        nextLevelTransition.GetComponent<Animator>().Play(outTransition);
    }

    public void SaveBestTime()
    {
        // Construct a unique key for the current level's best time
        double roundedFinalPB = Math.Round(currentPB, 3, MidpointRounding.AwayFromZero);
        string key = SceneManager.GetActiveScene().name + "_bestTime";
        PlayerPrefs.SetFloat(key, (float)roundedFinalPB);
        PlayerPrefs.Save();
    }

    public void LoadBestTime()
    {
        // Construct the key similar to how you save it
        string key = SceneManager.GetActiveScene().name + "_bestTime";
        if (PlayerPrefs.HasKey(key))
        {
            // If a best time is saved, load it. Otherwise, keep currentPB at its default value.
            currentPB = PlayerPrefs.GetFloat(key);
        }
    }


    private void GetGrade()
    {
        // Round finalTime to 1 decimal place to align with player's view
        double roundedFinalTime = Math.Round(finalTime, 3, MidpointRounding.AwayFromZero);
        // Use string formatting to ensure three decimal places are always shown
        finalTimeText.text = finalTime.ToString("F3") + "s";
        pbTimeText.text = "PB: " + currentPB.ToString("F3") + "s";
        // Initially disable all stars
        DisableStar();

        // Determine the number of stars to activate based on the grade
        //int starsToActivate = 0;
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



