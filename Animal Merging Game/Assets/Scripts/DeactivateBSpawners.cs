using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateBSpawners : MonoBehaviour
{
    public List<GameObject> spawnersToDeactivate = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < spawnersToDeactivate.Count; i++)
            {
                if (spawnersToDeactivate[i].GetComponent<MovingWallManager>())
                {
                    spawnersToDeactivate[i].GetComponent<MovingWallManager>().canSpawn = false;
                }
                else if (spawnersToDeactivate[i].GetComponent<LaserSpawner>())
                {

                    spawnersToDeactivate[i].GetComponent<LaserSpawner>().canAttack = false;
                }
            }
        }
    }
}
