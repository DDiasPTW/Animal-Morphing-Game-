using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> sectionsToEnable = new List<GameObject>();
    [SerializeField] private GameObject sectionToDisable;
    [SerializeField] private GameObject storySection;
    private JsonPlayerPrefs jsonPlayerPrefs;
    private const string storyKey = "Player_FirstTime";
    private bool isPlayingStory;

    public AmbientSoundManager ambientSound;

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
    }


    void Start()
    {
        for (int i = 0; i < sectionsToEnable.Count; i++)
        {
            sectionsToEnable[i].SetActive(false);
        }
    }

    void Update()
    {
        if(!isPlayingStory) return;
        else if (isPlayingStory){
            if(Input.GetKeyDown(KeyCode.Space)){
                storySection.SetActive(false);
                isPlayingStory = false;
                jsonPlayerPrefs.SetInt(storyKey,1);
                jsonPlayerPrefs.Save();
            }
        }
    }

    public void PlayButton(){        
        UISoundManager.Instance.PlayAudio();

        //if it's the first time the player is playing the game -> show "story"
        if(!jsonPlayerPrefs.HasKey(storyKey))
        {
            storySection.SetActive(true);
            isPlayingStory = true;
        }
        sectionsToEnable[0].SetActive(true);
        sectionToDisable.SetActive(false);
    }

    public void SettingsButton()
    {
        UISoundManager.Instance.PlayAudio();
        sectionsToEnable[1].SetActive(true);
        sectionToDisable.SetActive(false);
    }

    public void QuitButton(){
        Application.Quit();
    }
}
