using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionButton : MonoBehaviour
{
    [SerializeField] private GameObject sectionToDisable;
    [SerializeField] private GameObject sectionToEnable;

    public void ChangeSection(){
        UISoundManager.Instance.PlayAudio();
        sectionToEnable.SetActive(true);
        sectionToDisable.SetActive(false);
    }
}
