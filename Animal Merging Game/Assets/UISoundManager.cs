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
        aS = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudio(){
        aS.PlayOneShot(clickSound);
    }
}
