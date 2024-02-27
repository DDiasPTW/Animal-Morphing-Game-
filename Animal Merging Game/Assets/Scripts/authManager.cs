using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;

public class authManager : MonoBehaviour
{
    public bool IsAuthenticated { get; private set; } = false;

    // Define an event to notify when authentication is done
    public event Action OnAuthenticationComplete;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        await SignIn();
    }

    public async Task SignIn()
    {
        await signInAnonymous();
    }

    async Task signInAnonymous()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Player already signed in.");
            // Optionally, invoke any actions or events that depend on the player being signed in.
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in Success");
            Debug.Log("Player Id:" + AuthenticationService.Instance.PlayerId);
            // Invoke any post-sign-in actions or events here.
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("Sign in failed!!");
            Debug.LogException(ex);
        }
    }

}
