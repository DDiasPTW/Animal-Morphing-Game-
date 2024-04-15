using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance {get;private set;}
    private AudioSource aS;
    [SerializeField] private AudioClip clickSound;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        aS = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudio(){
        Debug.Log("Click called");
        aS.PlayOneShot(clickSound);
    }
}
