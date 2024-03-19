using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> sectionsToEnable = new List<GameObject>();
    [SerializeField] private GameObject sectionToDisable;

    void Start()
    {
        for (int i = 0; i < sectionsToEnable.Count; i++)
        {
            sectionsToEnable[i].SetActive(false);
        }
    }

    public void PlayButton(){
        UISoundManager.Instance.PlayAudio();
        sectionsToEnable[0].SetActive(true);
        sectionToDisable.SetActive(false);
    }

    public void QuitButton(){
        Application.Quit();
    }
}
