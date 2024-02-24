using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lava : MonoBehaviour
{

    private GameManager gM;
    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RestartLevel());
        }
    }

    //Killable
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gM.ResetLevel();;
        }
    }

    IEnumerator RestartLevel(){
        yield return new WaitForSeconds(.2f);
        gM.ResetLevel();
    }
}
