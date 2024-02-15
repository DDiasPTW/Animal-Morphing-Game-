using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevels : MonoBehaviour
{
    private GameManager gM;
    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    void LateUpdate()
    {
        if(gM != null) return;
        if(gM == null){
            gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
    }
    //NEED TO CHANGE THIS TO A NICER BEHAVIOUR
    //!Need to some some sort a transition between levels
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gM.canEndLevel)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        else if (other.CompareTag("Catch"))
        { //restart the level
            Debug.Log("Catch");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
