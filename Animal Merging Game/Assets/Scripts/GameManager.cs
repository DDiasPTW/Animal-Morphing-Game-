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
    private JsonPlayerPrefs jsonPlayerPrefs;

    [Header("Other")]
    public bool canEndLevel = true;
    public bool levelFinished = false;
    public bool canStartTimer = false;
    public bool forceReset = false;

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
    [SerializeField] private GameObject[] stars;
    public int starsToActivate = 0;

    [Header("Pause Menu")]
    [SerializeField] private GameObject PauseMenu;
    public bool isGamePause = false; //allow other scripts to check this
    [SerializeField] private bool canPause = true;
    [SerializeField] private string mainMenuScene;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private TMP_Text previousLevelButtonText;
    [SerializeField] private GameObject settingsMenu;

    void Awake()
    {
        Time.timeScale = 1f;
        time = 0f;
        canPause = true;


        // Initialize JsonPlayerPrefs with the appropriate file path
        string jsonFilePath;
#if UNITY_EDITOR
        jsonFilePath = Application.persistentDataPath + "/EditorPlayerStats.json";
#else
    jsonFilePath = Application.persistentDataPath + "/PlayerStats.json";
#endif


        jsonPlayerPrefs = new JsonPlayerPrefs(jsonFilePath);



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
        playerControls.Gameplay.Pause.performed += ctx => PauseGame();
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


        //MUST REMOVE IN FINAL BUILD!!!!
        // if(Input.GetKeyDown(KeyCode.L)){
        //     jsonPlayerPrefs.DeleteAll();
        //     jsonPlayerPrefs.Save();
        // }
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

        nextLevelTransition.GetComponent<Animator>().Play(outTransition);
        canPause = false;
        
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
            jsonPlayerPrefs.SetInt(key, 1);
            jsonPlayerPrefs.Save();
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

        jsonPlayerPrefs.SetFloat(key, (float)roundedFinalPB);
        jsonPlayerPrefs.Save();
    }


    private void LoadBestTime()
    {
        string key = SceneManager.GetActiveScene().name + _bestTime;
        if (jsonPlayerPrefs.HasKey(key))
        {
            // If a best time is saved, load it. Otherwise, keep currentPB at its default value.
            currentPB = jsonPlayerPrefs.GetFloat(key);
        }
    }

    private void SaveStars(int starsToSave)
    {
        string key = SceneManager.GetActiveScene().name + _bestStars;
        jsonPlayerPrefs.SetInt(key, starsToSave);
        jsonPlayerPrefs.Save();
    }
    private void LoadBestStars()
    {
        string key = SceneManager.GetActiveScene().name + _bestStars;
        if (jsonPlayerPrefs.HasKey(key))
        {
            // If a best star is saved, load it.
            bestStars = jsonPlayerPrefs.GetInt(key);
        }
    }

    private void GetGrade()
    {
        // Round finalTime to 1 decimal place to align with player's view
        double roundedFinalTime = Math.Round(finalTime, 3, MidpointRounding.AwayFromZero);
        // Use string formatting to ensure three decimal places are always shown
        finalTimeText.text = finalTime.ToString("F3");
        pbTimeText.text = "PB: " + currentPB.ToString("F3");
        // Initially disable all stars
        DisableStar();

        // Determine the number of stars to activate based on the grade
        if (roundedFinalTime <= gradeTimes[0]) {starsToActivate = 6;}
        else if (roundedFinalTime <= gradeTimes[1]) {starsToActivate = 5;}
        else if (roundedFinalTime <= gradeTimes[2]) {starsToActivate = 4;}
        else if (roundedFinalTime <= gradeTimes[3]) {starsToActivate = 3;}
        else if (roundedFinalTime <= gradeTimes[4]) {starsToActivate = 2;}
        else if (roundedFinalTime <= gradeTimes[5]){starsToActivate = 1;}
        else {Narrator.Instance.TriggerEndLevelLines(0);}


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
    //vv - For the Pause Menu UI - vv

    #region Pause
    private void PauseGame()
    {
        if (!isGamePause && canPause)
        {
            PauseMenu.SetActive(true);
            isGamePause = true;
            Time.timeScale = 0f;

            // Check if there's no next scene
            if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                // There is no next scene
                nextLevelButton.SetActive(false);
            }

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                previousLevelButtonText.text = "Main Menu";
            }
            else { previousLevelButtonText.text = "previous level"; }
        }
        else
        {
            isGamePause = false;
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }

    }


    public void Continue()
    {
        PauseGame();
        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayAudio();
        }
    }

    public void MainMenu()
    {
        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayAudio();
        }
        StartCoroutine(LoadMainMenu());
    }


    public void SettingsOn(){
        settingsMenu.SetActive(true);
    }
    public void SettingsOff(){
        settingsMenu.SetActive(false);
    }

    IEnumerator LoadMainMenu()
    {
        Time.timeScale = 1f;
        AmbientSoundManager.instance.PlayMainMenuMusic();
        SceneManager.LoadScene(mainMenuScene);
        yield return new WaitForSeconds(.3f);
    }

    public void NextLevel()
    {
        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayAudio();
        }
        StartCoroutine(LoadNext());
    }
    public void PreviousLevel()
    {
        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayAudio();
        }
        StartCoroutine(LoadPrevious());
    }

    IEnumerator LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(.3f);
    }
    IEnumerator LoadPrevious()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        yield return new WaitForSeconds(.3f);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

}