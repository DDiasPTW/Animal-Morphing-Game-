using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;



//TO DO:
//- Add check to see if it's already signed in in between levels
//-Be able to actually load and save data


public class CloudSaving : MonoBehaviour
{
    private GameManager gM;
    private bool isSigningIn = false;
    private Task signInTask = null;

    void Awake()
    {
        signInTask = SetUpAndSignIn();
    }

    void Start()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    async Task SetUpAndSignIn()
    {
        if (isSigningIn) return; // Prevent multiple sign-in attempts

        isSigningIn = true;
        await UnityServices.InitializeAsync();
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Successfully signed in anonymously");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"SignInAnonymouslyAsync failed: {ex.Message}");
        }
        finally
        {
            isSigningIn = false;
        }

    }

    async public void SaveData()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                if (signInTask != null) await signInTask;
            }
            else
            {
                Debug.LogError("User is not signed in. Attempting to sign in again.");
                return;
            }

        }

        var data = new Dictionary<string, object>
        {
             { SceneManager.GetActiveScene().name + "_bestTime", gM.currentPB }
        };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save data: {ex.Message}");
        }
    }

    async public void LoadData()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            if (signInTask != null) await signInTask; // Wait for the sign-in task if it's not null
            else
            {
                Debug.LogError("User is not signed in and no sign-in task available.");
                return;
            }
        }

        var keysToLoad = new HashSet<string>
    {
        SceneManager.GetActiveScene().name + "_bestTime"
    };

        try
        {
            var loadedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keysToLoad);
            Debug.Log($"Loaded time. Current best time for {SceneManager.GetActiveScene().name} is : {loadedData[SceneManager.GetActiveScene().name]}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load data: {ex.Message}");
        }
    }

}
