using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResetJsonPlayerPrefs : MonoBehaviour
{
    private JsonPlayerPrefs jsonPlayerPrefs;
    string jsonFilePath;
    private const string resetFlagKey = "PlayerrPrefsReset";

    void Awake()
    {
        // Initialize JsonPlayerPrefs with the appropriate file path
#if UNITY_EDITOR
        jsonFilePath = Application.persistentDataPath + "/EditorPlayerStats.json";
#else
    jsonFilePath = Application.persistentDataPath + "/PlayerStats.json";
#endif

        jsonPlayerPrefs = new JsonPlayerPrefs(jsonFilePath);

        ResetPrefs();
    }


    void ResetPrefs()
    {
        // Check if preferences have been reset
        if (!jsonPlayerPrefs.HasKey(resetFlagKey))
        {
            // Reset all player prefs
            jsonPlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs Reset");
            // Set the flag indicating preferences have been reset
            jsonPlayerPrefs.SetInt(resetFlagKey, 1);
            jsonPlayerPrefs.Save();
        }
    }
}
