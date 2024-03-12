using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public GameObject roomPrefab;
    public float roomSpacing = 20f; // Distance between rooms
    public Transform playerSpawnPoint;
    public List<string> allLevels = new List<string>();

    void Start()
    {
        // Instantiate rooms to the right
        for (int i = 0; i < allLevels.Count; i++)
        {
            float posX = i * roomSpacing; // Calculate x position
            GameObject room = Instantiate(roomPrefab, new Vector3(posX, 0f, 0f), Quaternion.identity);
            room.GetComponentInChildren<GoToLevel>().sceneToLoad = allLevels[i];
        }

    }
}
