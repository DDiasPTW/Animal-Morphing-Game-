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
                StartCoroutine(NextLevel());
            }
        }

        else if (other.CompareTag("Catch"))
        { //restart the level
            StartCoroutine(RestartLevel());
        }
    }

    IEnumerator NextLevel(){
        yield return new WaitForSeconds(.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator RestartLevel(){
        yield return new WaitForSeconds(.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
