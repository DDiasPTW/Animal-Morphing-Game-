using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    public static BallPool SharedInstance;
    public List<GameObject> pooledBalls;
    public GameObject ballPrefab;
    public int amountToPool = 20;

    void Awake()
    {
        SharedInstance = this;
        pooledBalls = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(ballPrefab);
            tmp.SetActive(false);
            pooledBalls.Add(tmp);
        }
    }

    public GameObject GetPooledBall()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledBalls[i].activeInHierarchy)
            {
                return pooledBalls[i];
            }
        }
        return null;
    }
}
