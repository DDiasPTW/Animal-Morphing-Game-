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
    private int bestStars = 0;
    private const string _bestTime = "_bestTime";
    private const string _bestStars = "_bestStars";
    private const string _nextLevelUnlocked = "_nextLevelUnlocked";

    [Header("Other")]
    public bool canEndLevel = true;
    public bool levelFinished = false;
    public bool canStartTimer = false;
    [SerializeField] private bool isTut = false;

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
    private string outTransition = "Scene_Out_v2";
    private string outZeroTransition = "Scene_Out_0";
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text[] recordTexts;
    public int starsToActivate = 0;

    [Header("Pause Menu")]
    [SerializeField] private GameObject PauseMenu;
    public static bool isGamePause = false; //allow other scripts to check this
    [SerializeField] private bool canPause = true;
    [SerializeField] private string mainMenuScene;

    void Awake()
    {
        time = 0f;
        canPause = true;
        //timer reference
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        levelFinished = false;
        finalTimeText = GameObject.FindGameObjectWithTag("FinalTime").GetComponent<TMP_Text>();
        pbTimeText = GameObject.FindGameObjectWithTag("Pb_Time").GetComponent<TMP_Text>();

        //next level transition
        nextLevelTransition = GameObject.FindGameObjectWithTag("Trans");
        GetStars();
        GetTexts();
        //--
        GetControls();
        //
        PauseMenu.SetActive(false);
    }

    private void GetControls()
    {
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
        playerControls.Gameplay.Pause.performed += ctx => PauseGame();
    }

    private void GetTexts()
    {
        // Get the length of the gradeTimes list
        int gradeCount = gradeTimes.Count;

        // Loop through the recordTexts array in reverse order
        for (int i = recordTexts.Length - 1; i >= 0; i--)
        {
            // Calculate the index for accessing gradeTimes in reverse
            int gradeIndex = gradeCount - 1 - i;

            // Set the text of each recordText to the corresponding element of gradeTimes
            recordTexts[i].text = gradeTimes[gradeIndex].ToString() + "s";
        }
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
        LoadBestStars();
    }

    void Update()
    {
        if (!levelFinished && canStartTimer)
        {
            UpdateTimerDisplay();
        }

        if (levelFinished && timerRunning)
        {
            FinalizeLevel();
        }

        if(Input.GetKeyDown(KeyCode.L)){
            //PlayerPrefs.DeleteAll();
            string key = SceneManager.GetActiveScene().name + _bestTime;
            PlayerPrefs.DeleteKey(key);
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void PreviousLevel()
    {
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
        if (finalTime < currentPB)
        {
            currentPB = finalTime;
            SaveBestTime();
        }
        GetGrade();


        UnlockNextLevel();

        // Indicate the game is ready for a next level jump action
        readyForNextLevel = true;

        if (!isTut)
        {
            nextLevelTransition.GetComponent<Animator>().Play(outTransition);
            canPause = false;
        }
        else nextLevelTransition.GetComponent<Animator>().Play(outZeroTransition);

    }

    public void UnlockNextLevel()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = currentBuildIndex + 1;

        if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextLevelName = SceneUtility.GetScenePathByBuildIndex(nextBuildIndex);
            nextLevelName = System.IO.Path.GetFileNameWithoutExtension(nextLevelName);

            string key = nextLevelName + _nextLevelUnlocked;
            PlayerPrefs.SetInt(key, 1); // 1 = true, 0 = false
            //Debug.Log(nextLevelName + " unlocked");
        }
        else
        {
            Debug.LogWarning("No next level available to unlock.");
        }
    }


    #region Save system
    private void SaveBestTime()
    {
        // Construct a unique key for the current level's best time
        double roundedFinalPB = Math.Round(currentPB, 3, MidpointRounding.AwayFromZero);
        string key = SceneManager.GetActiveScene().name + _bestTime;
        PlayerPrefs.SetFloat(key, (float)roundedFinalPB);
        PlayerPrefs.Save();
    }

    private void LoadBestTime()
    {
        string key = SceneManager.GetActiveScene().name + _bestTime;
        if (PlayerPrefs.HasKey(key))
        {
            // If a best time is saved, load it. Otherwise, keep currentPB at its default value.
            currentPB = PlayerPrefs.GetFloat(key);
        }
    }

    private void SaveStars(int starsToSave)
    {
        string key = SceneManager.GetActiveScene().name + _bestStars;
        PlayerPrefs.SetInt(key, starsToSave);
        PlayerPrefs.Save();
    }
    private void LoadBestStars()
    {
        string key = SceneManager.GetActiveScene().name + _bestStars;
        if (PlayerPrefs.HasKey(key))
        {
            // If a best star is saved, load it.
            bestStars = PlayerPrefs.GetInt(key);
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
        if (roundedFinalTime <= gradeTimes[0]) starsToActivate = 6;
        else if (roundedFinalTime <= gradeTimes[1]) starsToActivate = 5;
        else if (roundedFinalTime <= gradeTimes[2]) starsToActivate = 4;
        else if (roundedFinalTime <= gradeTimes[3]) starsToActivate = 3;
        else if (roundedFinalTime <= gradeTimes[4]) starsToActivate = 2;
        else if (roundedFinalTime <= gradeTimes[5]) starsToActivate = 1;

        if (starsToActivate > bestStars) SaveStars(starsToActivate);


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

    #endregion

    private void PauseGame()
    {
        if (!isGamePause && canPause)
        {
            //Need to actually pause the game, for now just open the UI
            PauseMenu.SetActive(true);
            isGamePause = true;
            Time.timeScale = 0f;
        }
        else
        {
            isGamePause = false;
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }

    }

    //vv - For the Pause Menu UI - vv
    public void Continue()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Quit(){
        Application.Quit();
    }

}