using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBack : MonoBehaviour
{
    [SerializeField] private GameObject sectionToDisable;
    [SerializeField] private GameObject sectionToEnable;
    
    public void Back()
    {
        sectionToEnable.SetActive(true);
        sectionToDisable.SetActive(false);
        UISoundManager.Instance.PlayAudio();
    }
}
