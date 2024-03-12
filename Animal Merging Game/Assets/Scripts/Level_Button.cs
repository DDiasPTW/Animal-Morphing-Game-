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
    [SerializeField] private bool forceUnlock = false; //Will set to true in the editor for the first level only
    public string sceneToLoad;
    public List<GameObject> stars = new List<GameObject>();
    private const string _bestTime = "_bestTime";
    private const string _bestStars = "_bestStars";
    private const string _nextLevelUnlocked = "_nextLevelUnlocked";
    [SerializeField] private TMP_Text pbText;

    void Awake()
    {
        string unlocked = sceneToLoad + _nextLevelUnlocked;      
        
        if(PlayerPrefs.GetInt(unlocked) == 1 || forceUnlock) //if the level is unlocked
        {
            GetComponent<Button>().interactable = true;
            
            string key = sceneToLoad + _bestStars;
            for (int i = 0; i < PlayerPrefs.GetInt(key); i++)
            {
                stars[i].SetActive(true);
            }
        }else
        {
            GetComponent<Button>().interactable = false;
        }


        if (pbText != null && !forceUnlock)
        {
            string key = sceneToLoad + _bestTime;
            if (PlayerPrefs.HasKey(key)) // Check if the key exists
            {
                pbText.text = PlayerPrefs.GetFloat(key).ToString() + "s";
                pbText.gameObject.SetActive(true);
            }else
            {
                pbText.text = "";
            }
        }
        else if (forceUnlock)
        {
            pbText.text = "";
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
