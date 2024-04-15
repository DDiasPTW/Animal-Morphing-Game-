using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level_Button : MonoBehaviour
{
    public string sceneToLoad;
    public List<GameObject> stars = new List<GameObject>();
    private const string _bestTime = "_bestTime";
    private const string _bestStars = "_bestStars";
    private const string _nextLevelUnlocked = "_nextLevelUnlocked";
    [SerializeField] private TMP_Text pbText;
    public TMP_Text levelText;
    private JsonPlayerPrefs jsonPlayerPrefs;

    void Awake()
    {
        // Initialize JsonPlayerPrefs with the appropriate file path
        string jsonFilePath;
#if UNITY_EDITOR
        jsonFilePath = Application.persistentDataPath + "/EditorPlayerStats.json";
#else
    jsonFilePath = Application.persistentDataPath + "/PlayerStats.json";
#endif

        jsonPlayerPrefs = new JsonPlayerPrefs(jsonFilePath);


        if(jsonPlayerPrefs.GetInt("Level - 1_nextLevelUnlocked") == 0)
        {
            jsonPlayerPrefs.SetInt("Level - 1_nextLevelUnlocked", 1); //Force unlock the first level
        }
        
        UpdateButton();
    }

    public void UpdateButton(){
        string unlocked = sceneToLoad + _nextLevelUnlocked;
        
        for (int i = 0; i < 6; i++)
        {
            stars[i].SetActive(false); //Reset all stars
        }

        if (jsonPlayerPrefs.GetInt(unlocked) == 1) //if the level is unlocked
        {
            GetComponent<Button>().interactable = true;
            
            string key = sceneToLoad + _bestStars;
            for (int i = 0; i < jsonPlayerPrefs.GetInt(key); i++)
            {
                stars[i].SetActive(true);
            }
        }else
        {
            GetComponent<Button>().interactable = false;
        }

        if (pbText != null)
        {
            string key = sceneToLoad + _bestTime;
            if (jsonPlayerPrefs.HasKey(key)) // Check if the key exists
            {
                pbText.text = jsonPlayerPrefs.GetFloat(key).ToString("F3") + "s";
                pbText.gameObject.SetActive(true);
            }else
            {
                pbText.text = "";
            }
        }
    }

    public void LoadScene()
    {
        UISoundManager.Instance.PlayAudio();
        StartCoroutine(LoadLevel());      
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(.3f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
