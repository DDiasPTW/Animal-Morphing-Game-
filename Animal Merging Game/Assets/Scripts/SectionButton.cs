using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionButton : MonoBehaviour
{
    [SerializeField] private List<string> levels = new List<string>();
    public int currentLevelIndex = 0;
    [SerializeField] private Level_Button levelButton;
    [SerializeField] private SectionButton otherButton;

    void Start()
    {
        UpdateLevelButton();
        otherButton.currentLevelIndex = currentLevelIndex;
    }

    public void ChangeNextSection()
    {
        if (levels.Count == 0) return; // Ensure levels list is not empty

        


        currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
        otherButton.currentLevelIndex = currentLevelIndex;


        UpdateLevelButton();
        levelButton.UpdateButton();
        
        UISoundManager.Instance.PlayAudio();
    }
    public void ChangePreviousSection()
    {
        if (levels.Count == 0) return; // Ensure levels list is not empty

        // Decrement currentLevelIndex, wrapping around to the last index if it goes below 0
        currentLevelIndex = (currentLevelIndex == 0) ? levels.Count - 1 : currentLevelIndex - 1;
        otherButton.currentLevelIndex = currentLevelIndex;


        UpdateLevelButton();
        levelButton.UpdateButton();

        UISoundManager.Instance.PlayAudio();
    }


    private void UpdateLevelButton()
    {
        if (levelButton != null && levels.Count > 0)
        {
            levelButton.sceneToLoad = levels[currentLevelIndex];
            levelButton.levelText.text = (currentLevelIndex + 1).ToString();
        }
    }
}
