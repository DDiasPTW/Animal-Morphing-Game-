using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevel : MonoBehaviour
{
    public string sceneToLoad;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
